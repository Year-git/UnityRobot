using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KTFormatDisplayNameAttribute : System.Attribute
{
    public string format;
    public string[] args;

    public KTFormatDisplayNameAttribute(string format, params string[] args)
    {
        this.format = format;
        this.args = args;
    }

    public override string ToString()
    {
        var argStrs = args.Select(s => string.Format("\"{0}\"", s)).ToArray();
        return string.Format(
            "KTFormatDisplayName(\"{0}\", {1})",
            format,
            string.Join(",", argStrs));

    }
}
