using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelMove : MonoBehaviour
{
    public bool bTurn = false;
    public bool bBrive = true;
    public float speed = 3000;
    public float angle = 30;
    public float brake = 3000;
    public Vector3 localRotOffset;

    private WheelCollider wheelCollider;
    public GameObject wheelModel;
    // Start is called before the first frame update
    void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
        wheelCollider.brakeTorque = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            wheelCollider.brakeTorque = 0;
            wheelCollider.motorTorque = speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            wheelCollider.brakeTorque = 0;
            wheelCollider.motorTorque = -speed;
        }
        if (Input.GetKey(KeyCode.A) && bTurn)
        {
            wheelCollider.steerAngle = -angle;
        }
        if (Input.GetKey(KeyCode.D) && bTurn)
        {
            wheelCollider.steerAngle = angle;
        }
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A) && bTurn)
        {
            wheelCollider.steerAngle = 0;
        }
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            wheelCollider.motorTorque = 0;
            wheelCollider.brakeTorque = brake;
        }

        // 轮子模型参数适配
        if (wheelModel)
        {
            Vector3 pos = new Vector3(0, 0, 0);
            Quaternion quat = new Quaternion();
            wheelCollider.GetWorldPose(out pos, out quat);

            wheelModel.transform.rotation = quat;

            //wheelModel.transform.localRotation *= Quaternion.Euler(localRotOffset);
            wheelModel.transform.position = pos;

            WheelHit wheelHit;
            wheelCollider.GetGroundHit(out wheelHit);
        }
    }
}
