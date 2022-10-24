#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class KTUtils
{
    public static List<System.Type> GetAllSubclass(System.Type type)
    {
        return type.Assembly.GetTypes().Where(t => t.IsSubclassOf(type)).ToList();
    }

	/// <summary>
	/// 获取展示在界面的名字
	/// </summary>
    public static string GetDisplayName<T>(T t, string defaultName) where T : System.Reflection.ICustomAttributeProvider
    {
        var attr = t.GetCustomAttributes(typeof(KTDisplayNameAttribute), false).FirstOrDefault() as KTDisplayNameAttribute;
        return (attr != null) ? attr.displayName : defaultName;
    }

    public static string GetDisplayName<T>(T inst) where T : class
    {
        var t = typeof(T);
        var attr = t.GetCustomAttributes(typeof(KTFormatDisplayNameAttribute), false).FirstOrDefault() as KTFormatDisplayNameAttribute;
        if (attr == null)
        {
            return GetDisplayName(t, t.Name);
        }

        var values = attr.args
            .Select(arg => t.GetField(arg))
            .Select(f => f.GetValue(inst) != null ? f.GetValue(inst).ToString() : "")
            .ToArray();
        return string.Format(attr.format, values);
    }

    public static bool UseObject<T>(System.Action<T> fn) where T : Object
    {
        var obj = GameObject.FindObjectOfType<T>();
        if (obj != null)
        {

            fn(obj);
            return true;
        }
        return false;
    }

}
#endif

