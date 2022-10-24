/*
 * This code is part of Arcade Car Physics for Unity by Saarg (2018)
 * 
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if MULTIOSCONTROLS
    using MOSC;
#endif

namespace VehicleBehaviour
{
    [RequireComponent(typeof(Rigidbody))]
    public class WheelVehicle : MonoBehaviour
    {
        [Header("Inputs")]
#if MULTIOSCONTROLS
        [SerializeField] PlayerNumber playerId;
#endif
        // If isPlayer is false inputs are ignored
        [SerializeField] bool isPlayer = true;
        public bool IsPlayer { get { return isPlayer; } set { isPlayer = value; } }

        [SerializeField] bool bAuto = false;
        [SerializeField] bool isDey = false;
        public bool IsDey { get { return isDey; } set { isDey = value; } }
        // Input names to read using GetAxis
        [SerializeField] public string throttleInput = "Vertical";
        [SerializeField] public string brakeInput = "F";
        [SerializeField] public string turnInput = "Horizontal";
        [SerializeField] public string jumpInput = "Jump";
        [SerializeField] public string driftInput = "Fire1";
        [SerializeField] public string boostInput = "Fire2";

        /* 
         *  Turn input curve: x real input, y value used
         *  My advice (-1, -1) tangent x, (0, 0) tangent 0 and (1, 1) tangent x
         */
        [SerializeField] AnimationCurve turnInputCurve = AnimationCurve.Linear(-1.0f, -1.0f, 1.0f, 1.0f);

        [Header("Wheels")]
        [SerializeField] WheelCollider[] driveWheel;
        public WheelCollider[] DriveWheel { get { return driveWheel; } }
        [SerializeField] WheelCollider[] turnWheel;
        [SerializeField] WheelCollider[] backWheel;

        public WheelCollider[] TurnWheel { get { return turnWheel; } }

        // This code checks if the car is grounded only when needed and the data is old enough
        bool isGrounded = false;
        int lastGroundCheck = 0;
        public bool IsGrounded
        {
            get
            {
                if (lastGroundCheck == Time.frameCount)
                    return isGrounded;

                lastGroundCheck = Time.frameCount;
                isGrounded = true;
                foreach (WheelCollider wheel in wheels)
                {
                    if (!wheel.gameObject.activeSelf || !wheel.isGrounded)
                        isGrounded = false;
                }
                return isGrounded;
            }
        }

        [Header("Behaviour")]
        /*
         *  Motor torque represent the torque sent to the wheels by the motor with x: speed in km/h and y: torque
         *  The curve should start at x=0 and y>0 and should end with x>topspeed and y<0
         *  The higher the torque the faster it accelerate
         *  the longer the curve the faster it gets
         */
        [SerializeField] AnimationCurve motorTorque = new AnimationCurve(new Keyframe(0, 200), new Keyframe(50, 300), new Keyframe(200, 0));

        // Differential gearing ratio
        [Range(2, 16)]
        [SerializeField] float diffGearing = 4.0f;
        public float DiffGearing { get { return diffGearing; } set { diffGearing = value; } }

        // Basicaly how hard it brakes
        [SerializeField] float brakeForce = 1500.0f;
        public float BrakeForce { get { return brakeForce; } set { brakeForce = value; } }

        // Max steering hangle, usualy higher for drift car
        [Range(0f, 50.0f)]
        [SerializeField] float steerAngle = 30.0f;
        public float SteerAngle { get { return steerAngle; } set { steerAngle = Mathf.Clamp(value, 0.0f, 50.0f); } }

        // The value used in the steering Lerp, 1 is instant (Strong power steering), and 0 is not turning at all
        [Range(0.001f, 1.0f)]
        [SerializeField] float steerSpeed = 0.2f;
        public float SteerSpeed { get { return steerSpeed; } set { steerSpeed = Mathf.Clamp(value, 0.001f, 1.0f); } }

        // How hight do you want to jump?
        [Range(1f, 10f)]
        [SerializeField] float jumpVel = 1.3f;
        public float JumpVel { get { return jumpVel; } set { jumpVel = Mathf.Clamp(value, 1.0f, 1.5f); } }

        // How hard do you want to drift?
        [Range(0.0f, 2f)]
        [SerializeField] float driftIntensity = 1f;
        public float DriftIntensity { get { return driftIntensity; } set { driftIntensity = Mathf.Clamp(value, 0.0f, 2.0f); } }

        // Reset Values
        Vector3 spawnPosition;
        Quaternion spawnRotation;

        /*
         *  The center of mass is set at the start and changes the car behavior A LOT
         *  I recomment having it between the center of the wheels and the bottom of the car's body
         *  Move it a bit to the from or bottom according to where the engine is
         */
        [SerializeField] public Transform centerOfMass;

        // Force aplied downwards on the car, proportional to the car speed
        [Range(0.5f, 1000f)]
        [SerializeField] float downforce = 1.0f;
        public float Downforce { get { return downforce; } set { downforce = Mathf.Clamp(value, 0, 5); } }

        // When IsPlayer is false you can use this to control the steering
        float steering;
        public float Steering { get { return steering; } set { steering = Mathf.Clamp(value, -1f, 1f); } }

        // When IsPlayer is false you can use this to control the throttle
        float throttle;
        public float Throttle { get { return throttle; } set { throttle = Mathf.Clamp(value, -1f, 1f); } }

        // Like your own car handbrake, if it's true the car will not move
        [SerializeField] bool handbrake;
        public bool Handbrake { get { return handbrake; } set { handbrake = value; } }

        // Use this to disable drifting
        [HideInInspector] public bool allowDrift = true;
        bool drift;
        public bool Drift { get { return drift; } set { drift = value; } }

        // Use this to read the current car speed (you'll need this to make a speedometer)
        [SerializeField] float speed = 0.0f;
        public float Speed { get { return speed; } }

        // Use this to boost when IsPlayer is set to false
        public bool boosting = false;
        // Use this to jump when IsPlayer is set to false
        public bool jumping = false;

        public SplineTrailRenderer[] trai;
        private float traiTime = 0;
        // Private variables set at the start
        Rigidbody _rb;
        WheelCollider[] wheels;

        // Init rigidbody, center of mass, wheels and more
        void Start()
        {
#if MULTIOSCONTROLS
            // Debug.Log("[ACP] Using MultiOSControls");
#endif
            _rb = GetComponent<Rigidbody>();
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;

            if (_rb != null && centerOfMass != null)
            {
                _rb.centerOfMass = centerOfMass.localPosition;
            }

            wheels = GetComponentsInChildren<WheelCollider>();

            // Set the motor torque to a non null value because 0 means the wheels won't turn no matter what
            foreach (WheelCollider wheel in wheels)
            {
                wheel.motorTorque = 0.0001f;
            }
        }

        // Visual feedbacks and boost regen
        void Update()
        {
            // foreach (ParticleSystem gasParticle in gasParticles)
            // {
            //     gasParticle.Play();
            //     ParticleSystem.EmissionModule em = gasParticle.emission;
            //     em.rateOverTime = handbrake ? 0 : Mathf.Lerp(em.rateOverTime.constant, Mathf.Clamp(150.0f * throttle, 30.0f, 100.0f), 0.1f);
            // }

            // if (isPlayer && allowBoost)
            // {
            //     boost += Time.deltaTime * boostRegen;
            //     if (boost > maxBoost) { boost = maxBoost; }
            // }
        }

        // Update everything
        void FixedUpdate()
        {
            // Mesure current speed
            speed = transform.InverseTransformDirection(_rb.velocity).z * 3.6f;

            handbrake = GetInput(brakeInput) == 1;
            // Get all the inputs!
            if (isPlayer)
            {
                // Accelerate & brake
                if (throttleInput != "" && throttleInput != null)
                {
                    throttle = GetInput(throttleInput) - GetInput(brakeInput);
                }
                // Boost
                boosting = (GetInput(boostInput) > 0.5f);
                // Turn
                steering = turnInputCurve.Evaluate(GetInput(turnInput)) * steerAngle;
                // Dirft
                drift = GetInput(driftInput) > 0 && _rb.velocity.sqrMagnitude > 100;
                // Jump
                jumping = GetInput(jumpInput) != 0;
            }

            // Direction
            foreach (WheelCollider wheel in turnWheel)
            {
                wheel.steerAngle = Mathf.Lerp(wheel.steerAngle, steering, steerSpeed);
            }

            foreach (WheelCollider wheel in wheels)
            {
                wheel.brakeTorque = 0;
            }

            // Handbrake
            // Handbrake
            if (handbrake)
            {
                foreach (WheelCollider wheel in wheels)
                {
                    // Don't zero out this value or the wheel completly lock up
                    wheel.motorTorque = 0.0001f;
                    wheel.brakeTorque = brakeForce;
                }
            }//Mathf.Abs(speed) < 4 || Mathf.Sign(speed) == Mathf.Sign(throttle)
            else if (throttle == 0)
            {
                foreach (WheelCollider wheel in wheels)
                {
                    wheel.motorTorque = 0;
                    // if (Speed > 0.1 || Speed < -0.1)
                    // {
                    //     wheel.motorTorque = Speed * -100;
                    // }
                    // else
                    // {
                    //wheel.motorTorque = 0.0001f;
                    //wheel.brakeTorque = brakeForce;
                    //}
                }
            }
            else
            {
                foreach (WheelCollider wheel in driveWheel)
                {
                    if (throttle > 0)
                    {
                        if (Speed < -5)
                        {
                            wheel.motorTorque = 10000f;
                        }
                        else
                        {
                            wheel.motorTorque = throttle * motorTorque.Evaluate(speed) * diffGearing / driveWheel.Length;
                        }
                    }
                    else
                    {
                        if (Speed > 5)
                        {
                            wheel.motorTorque = -10000f;
                        }
                        else
                        {
                            wheel.motorTorque = throttle * motorTorque.Evaluate(-speed) * diffGearing / driveWheel.Length;
                        }
                    }
                }
            }

            // Jump
            if (jumping && isPlayer)
            {
                if (!IsGrounded)
                    return;

                _rb.velocity += transform.up * jumpVel;
            }

            // Drift
            if (drift && allowDrift && Mathf.Abs(steering) > 20)
            {
                Vector3 f = transform.right * steering / steerAngle + transform.forward;
                transform.Translate(f.normalized.x * 0.1f, 0, f.normalized.z * 0.1f, Space.Self);
                // transform.Rotate(0, steering / steerAngle, 0, Space.Self);
                // transform.Translate(-(float)GetInput(turnInput) * 0.2f, 0, 0, Space.Self);
                // transform.Rotate(0, GetInput(turnInput) * 2, 0, Space.Self);
                // Debug.DrawLine(transform.position, f.normalized * 1000, Color.yellow, 5f);
                //Debug.DrawLine(transform.position, new Vector3(f.normalized.x * 0.1f, 0, f.normalized.z * 0.1f).normalized * 1000, Color.yellow, 5f);
            }

            // 后轮胎印处理
            for (int i = 1; i <= backWheel.Length; i++)
            {
                WheelCollider wheel = backWheel[i - 1];
                if (i <= trai.Length)
                {
                    SplineTrailRenderer t = trai[i - 1];

                    if (wheel.isGrounded && Mathf.Abs(Speed) > 1)
                    {
                        traiTime += Time.deltaTime;
                        float max = Speed * traiTime / 5f > 20 ? 20 : Speed * traiTime / 5f;
                        t.maxLength = Mathf.Lerp(t.maxLength, max, 0.01f);
                    }
                    else
                    {
                        t.maxLength = Mathf.Lerp(t.maxLength, 0, 0.3f);
                        traiTime = 0;
                    }
                }
            }
            if (throttle != 0)
            {
                if (Mathf.Abs(speed) > 30)
                {
                    _rb.AddForce(-transform.up * _rb.mass * Mathf.Abs(speed) / downforce, ForceMode.Force);
                    // Debug.Log(_rb.name + " AddF = " + (-transform.up * _rb.mass * Mathf.Abs(speed) / downforce).ToString() + "....." + speed.ToString("F6"));
                }
                else
                {
                    _rb.AddForce(-transform.up * _rb.mass / 2, ForceMode.Force);
                }
                _rb.AddForce(-transform.up * speed * downforce * _rb.mass/100);
            }
        }

        // Reposition the car to the start position
        public void ResetPos()
        {
            transform.position = spawnPosition;
            transform.rotation = spawnRotation;
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        public void toogleHandbrake(bool h)
        {
            handbrake = h;
        }

        public void ToPoint(Vector3 point){

        }

        // MULTIOSCONTROLS is another package I'm working on ignore it I don't know if it will get a release.
#if MULTIOSCONTROLS
        private static MultiOSControls _controls;
#endif

        // Use this method if you want to use your own input manager
        private float GetInput(string input)
        {
#if MULTIOSCONTROLS
        return MultiOSControls.GetValue(input, playerId);
#else
            return Input.GetAxis(input);
#endif
        }
    }
}
