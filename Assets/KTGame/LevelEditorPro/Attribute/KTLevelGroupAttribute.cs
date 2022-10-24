#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false)]
public class KTLevelGroupAttribute : System.Attribute
{
    public string[] fields;

    public KTLevelGroupAttribute(params string[] fields)
    {
        this.fields = fields;
    }

    public override string ToString()
    {
        return string.Format("KTLevelGroup");
    }
}

#endif