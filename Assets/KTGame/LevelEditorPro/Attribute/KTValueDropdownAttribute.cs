#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KTValueDropdownAttribute : System.Attribute
{
    public string member;
    public KTValueDropdownAttribute(string member)
    {
        this.member = member;
    }

    public override string ToString()
    {
        return string.Format("ValueDropdown(\"{0}\")", this.member);
    }
}

#endif
