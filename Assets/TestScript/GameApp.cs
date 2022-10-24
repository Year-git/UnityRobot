using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleBehaviour;

public class GameApp : MonoBehaviour
{
    [Space(7)] public CameraPos[] Camerp;
    [Space(7)] public List<WheelVehicle> Cars;

    public List<PlayerCar> PlayerCarList = new List<PlayerCar>();
    //public List<BulletBoom> BList = new List<BulletBoom>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 1; i++)
        {
            PlayerCarList.Add(new PlayerCar(this, Camerp[i], i));
            PlayerCarList[i].UserCar(Cars[i]);
        }
    }

    public void ChangeCar(int id, int carid)
    {
        if (Cars[carid].IsPlayer == true) return;
        PlayerCarList[id].UserCar(Cars[carid]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.M))
        {
            foreach (var che in Cars)
            {
                if (che.IsPlayer)
                {
                    Transform centerOfMass = che.centerOfMass;
                    Transform[] t = che.gameObject.GetComponentsInChildren<Transform>();
                    che.IsDey = true;
                    foreach (var item in t)
                    {
                        Vector3 f = (item.transform.position - centerOfMass.transform.position);
                        item.gameObject.AddComponent(typeof(Rigidbody));
                        Rigidbody rigidbody = item.gameObject.GetComponent<Rigidbody>();
                        rigidbody.AddForce(f,ForceMode.Impulse);
                    }
                }
            }
        }
    }
}
