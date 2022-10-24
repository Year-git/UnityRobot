using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleBehaviour;
public class AutoChe : MonoBehaviour
{
    WheelVehicle wheelvehicle;
    public float wvtime;
    private float curTime;

    private float dTime;
    private int state = 0;
    // Start is called before the first frame update
    void Start()
    {
        wheelvehicle = GetComponent<WheelVehicle>();
    }

    // Update is called once per frame
    void Update()
    {
        if(curTime <= wvtime)
        {
            curTime += Time.deltaTime;
            if (state == 0)
            {
                wheelvehicle.Throttle = 1;
            }
            else
            {
                wheelvehicle.Throttle = -1;
            }
        }
        else
        {
            state = state == 0 ? 1 : 0;
            curTime = 0;
        }
        
    }
}
