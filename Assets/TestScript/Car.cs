using UnityEngine;
using Newtonsoft.Json.Linq;

public class Car : MonoBehaviour
{
    private Fix64 _motorTorque = 50000;
    private Fix64 _steerAngle = 45.0f;
    private Fix64 _steerSpeed = 1.00f;
    
    WheelCollider[] wheels;

    Rigidbody _rigidbody;

    void Start()
    {
        this.GetComponent<Rigidbody>().centerOfMass = transform.Find("centerOfMass").transform.localPosition;
        wheels = GetComponentsInChildren<WheelCollider>();
        foreach (WheelCollider wheel in wheels)
        {
            wheel.motorTorque = (float)(Fix64)0.0001;
        }
        _rigidbody = GetComponent<Rigidbody>();
    }

    private JObject joInput = new JObject
    {
        ["W"] = 0,
        ["A"] = 0,
        ["S"] = 0,
        ["D"] = 0,
    };

    public void NpcFrameSynLogicUpdate(JObject jInfo)
    {
        joInput["W"] = jInfo["W"] != null ? jInfo["W"] : joInput["W"];
        joInput["A"] = jInfo["A"] != null ? jInfo["A"] : joInput["A"];
        joInput["S"] = jInfo["S"] != null ? jInfo["S"] : joInput["S"];
        joInput["D"] = jInfo["D"] != null ? jInfo["D"] : joInput["D"];
    }

    private int lasta = -1;
    public void ExecuteInputCommand()
    {
        int a = FrameSynchronManager.Instance.GetGameLogicFrame();
        if( a != (lasta + 1)){
            Debug.Log("****************************bug: " + a + ":" + (lasta + 1));
            lasta = a;
        }
        else{
            lasta ++ ;
        }
        
        if ((int)joInput["W"] == 1)
        {
            SetWheelMotorTorque(_motorTorque);
            SetWheelBrakeTorque(false);
        }
        else if ((int)joInput["S"] == 1)
        {
            SetWheelMotorTorque(_motorTorque * -1);
            SetWheelBrakeTorque(false);
        }
        else
        {
            if ((int)joInput["W"] == 0 || (int)joInput["S"] == 0)
            {
                SetWheelMotorTorque((Fix64)0);
                SetWheelBrakeTorque(true);
            }
        }

        if ((int)joInput["A"] == 1)
        {
            SetWheelSteerAngle(_steerAngle * -1);
        }
        else if ((int)joInput["D"] == 1)
        {
            SetWheelSteerAngle(_steerAngle);
        }
        else
        {
            SetWheelSteerAngle((Fix64)0);
        }
        Common.AmendPrecision(2, _rigidbody, wheels);
    }

    private void SetWheelMotorTorque(Fix64 MotorTorque)
    {
        foreach (WheelCollider wc in wheels)
        {
            wc.motorTorque = (float)MotorTorque;
        }
    }

    private void SetWheelBrakeTorque(bool bBreak)
    {
        foreach (WheelCollider wc in wheels)
        {
            if (bBreak)
            {
                wc.brakeTorque = (float)(Fix64)10000;
            }
            else
            {
                wc.brakeTorque = (float)(Fix64)0;
            }
        }
    }

    private void SetWheelSteerAngle(Fix64 SteerAngle)
    {
        WheelCollider wc_FL = transform.Find("Wheel_FL").GetComponent<WheelCollider>();
        wc_FL.steerAngle = Mathf.Lerp(wc_FL.steerAngle, (float)SteerAngle, (float)_steerSpeed);
        WheelCollider wc_FR = transform.Find("Wheel_FR").GetComponent<WheelCollider>();
        wc_FR.steerAngle = Mathf.Lerp(wc_FR.steerAngle, (float)SteerAngle, (float)_steerSpeed);
    }

    // void FixedUpdate()
    // {
    //     CollectInputCommand();
    //     ExecuteInputCommand();
    //     CameraFollow(Time.fixedDeltaTime);
    // }

    // public void CameraFollow(Fix64 interpolation)
    // {
    //     if (Camera.main != null)
    //     {
    //         float x = 20;
    //         float y = 20;
    //         float z = 20;
    //         float lerpPositionMultiplier = 4f;

    //         Vector3 tPos = new Vector3(transform.position.x + x, y, transform.position.z - z);
    //         Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, tPos, (float)interpolation * lerpPositionMultiplier);
    //         Camera.main.transform.LookAt(transform);
    //     }
    // }

    // public void CollectInputCommand()
    // {
    //     JObject joInput = new JObject();
    //     if (Input.GetKeyDown(KeyCode.W))
    //     {
    //         joInput["W"] = 1;
    //     }
    //     if (Input.GetKeyUp(KeyCode.W))
    //     {
    //         joInput["W"] = 0;
    //     }
    //     if (Input.GetKeyDown(KeyCode.A))
    //     {
    //         joInput["A"] = 1;
    //     }
    //     if (Input.GetKeyUp(KeyCode.A))
    //     {
    //         joInput["A"] = 0;
    //     }
    //     if (Input.GetKeyDown(KeyCode.S))
    //     {
    //         joInput["S"] = 1;
    //     }
    //     if (Input.GetKeyUp(KeyCode.S))
    //     {
    //         joInput["S"] = 0;
    //     }
    //     if (Input.GetKeyDown(KeyCode.D))
    //     {
    //         joInput["D"] = 1;
    //     }
    //     if (Input.GetKeyUp(KeyCode.D))
    //     {
    //         joInput["D"] = 0;
    //     }
    //     if (joInput.HasValues)
    //     {
    //         NpcFrameSynLogicUpdate(joInput);
    //     }
    // }
}
