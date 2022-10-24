#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KTLevelClassTypeAttribute : System.Attribute
{
    public object type;
    public KTLevelClassTypeAttribute(object type)
    {
        this.type = type;
    }

    public override string ToString()
    {
        return string.Format("KTLevelClassType({0}.{1})", type.GetType().Name, type);
    }
}

#endif
