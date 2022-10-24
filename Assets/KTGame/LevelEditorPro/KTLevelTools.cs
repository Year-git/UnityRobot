using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[System.Serializable]
public class KTLevelTools
{
    public static T GetAttribute<T>(ICustomAttributeProvider provider) where T : System.Attribute
    {
        return provider.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T;
    }

    public static void CopyFieldsValues<T>(T s,T d,List<FieldInfo> fields)
    {
        foreach (var field in fields)
        {
            var fieldValue = field.GetValue(s);
            field.SetValue(d, fieldValue);
        }
    }
}
