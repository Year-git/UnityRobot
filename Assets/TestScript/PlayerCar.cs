using VehicleBehaviour;

public class PlayerCar
{
    public CameraPos cp;
    public GameApp Gapp;
    public WheelVehicle _WheelVehicle;

    public int Index = 0;
    
    public PlayerCar(GameApp app, CameraPos cpos,int index)
    {
        Gapp = app;
        cp = cpos;
        Index = index;
    }

    public void UserCar(WheelVehicle wv)
    {
        if (_WheelVehicle != null)
        {
            _WheelVehicle.IsPlayer = false;
        }
        _WheelVehicle = wv;
        wv.IsPlayer = true;
        if (Index == 0)
        {
            wv.throttleInput = "Vertical";
            wv.turnInput = "Horizontal";
            wv.brakeInput = "F";
        }
        else
        {
            wv.throttleInput = "Verticalkey";
            wv.turnInput = "Horizontalkey";
            wv.brakeInput = "M";
        }
    }

}
