using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCarButton : MonoBehaviour
{
    public int id = 0;
    public GameApp app;
    public void ChangeCar_01()
    {
        app.ChangeCar(id, 0);
    }
    public void ChangeCar_02()
    {
        app.ChangeCar(id, 1);
    }
    public void ChangeCar_03()
    {
        app.ChangeCar(id, 2);
    }
    public void ChangeCar_04()
    {
        app.ChangeCar(id, 3);
    }
    public void ChangeCar_05()
    {
        app.ChangeCar(id, 4);
    }
    public void ChangeCar_06()
    {
        app.ChangeCar(id, 5);
    }
}
