using UnityEngine;

public class AttrbuteValue
{
    public int Base { get; set; }
    public int Percent { get; set; }

    public int GetValue()
    {
        return Mathf.FloorToInt(Base * (1 + Percent / 10000));
    }

    public AttrbuteValue()
    {
        Base = 0;
        Percent = 0;
    }
}
