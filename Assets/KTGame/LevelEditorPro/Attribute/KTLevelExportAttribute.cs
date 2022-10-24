#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.All, AllowMultiple =true)]
public class KTLevelExportAttribute : System.Attribute
{
    public string field;
    
    public KTLevelExportAttribute(string field)
    {
        this.field = field;
    }

    public override string ToString()
    {
        return string.Format("KTLevelExport(\"{0}\")", field);
    }
}

#endif
