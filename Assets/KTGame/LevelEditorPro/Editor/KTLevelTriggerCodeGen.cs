using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEditor;
using System.IO;

public static class KTLevelTriggerCodeGen
{
    const string kExportDir = "Assets/KTGame/LevelEditor/TriggerGen";

    private static readonly HashSet<string> kMethodAttrIgnoreSet = new HashSet<string>() { "KTDisplayNameAttribute" };

    private static readonly Dictionary<System.Type, string> kBaseTypeToCSName = new Dictionary<System.Type, string>()
    {
        { typeof(bool), "bool" },
        { typeof(byte), "byte" },
        { typeof(sbyte), "sbyte" },
        { typeof(char), "char" },
        { typeof(decimal), "decimal" },
        { typeof(double), "double" },
        { typeof(float), "float" },
        { typeof(int), "int" },
        { typeof(uint), "uint" },
        { typeof(long), "long" },
        { typeof(ulong), "ulong" },
        { typeof(short), "short" },
        { typeof(ushort), "ushort" },
        { typeof(string), "string" },
    };

    private static string kCodePartImport = @"//! GenerateBy KTLevelTriggerCodeGen.cs, Dont Modify
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;
";
    private static string kCodePartClassHeader = @"
public class {0}: {2}
{{
";
    private static string kCodePartType = @"
    [HideInInspector, KTLevelExport(""{0}"")]
    public {1} triggerNodeType = {1}.{2};
";

    private static string kCodePartClassMember = @"
    {3}
    [LabelText(""{2}""), OnValueChanged(""Refresh"")]
    public {0} {1};";

    private static string kCodePartRefresh = @"
    public override void Refresh()
    {{
        gameObject.name = KTUtils.GetDisplayName(this);
    }}
";

    private static string kCodePartClassTail = @"
}
#endif
";

    public static void GenAll()
    {
        GenMethods(GetMethodOfReturnType(typeof(KTLevelTriggerConditions), typeof(bool)), typeof(KTLevelTriggerCondition), "KTECondition", "conditionType");
        GenMethods(GetMethodOfReturnType(typeof(KTLevelTriggerActions), typeof(void)), typeof(KTLevelTriggerAction), "KTEAction", "resultType");
        AssetDatabase.Refresh();
    }

    private static IEnumerable<MethodInfo> GetMethodOfReturnType(System.Type defineClassType, System.Type returnType)
    {
        return defineClassType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.ReturnType == returnType);
    }

    private static void GenMethods(IEnumerable<MethodInfo> methods, System.Type parentType, string prefix, string typeExportField)
    {
        KTUtils.EnsureDir(kExportDir);

		bool isAction = true;
		string switchText = "switch(type){";

		foreach (var method in methods)
		{
			var sb = KTStringBuilderCache.Acquire();
			sb.Append(kCodePartImport);

			var className = prefix + method.Name;
			var classDisplayName = KTUtils.GetDisplayName(method, className);

			sb.AppendLine();
			var clsAttrStr = GetKTAttributesAsString(method);
			if (!string.IsNullOrEmpty(clsAttrStr))
			{
				sb.AppendFormat("[{0}]", clsAttrStr);
			}
			sb.AppendFormat(kCodePartClassHeader, className, classDisplayName, parentType.FullName);

			var typeAttr = method.GetCustomAttributes(typeof(KTLevelClassTypeAttribute), false).FirstOrDefault() as KTLevelClassTypeAttribute;
			sb.AppendFormat(
				kCodePartType,
				typeExportField,
				typeAttr.type.GetType().Name,
				typeAttr.type.ToString());

			var args = method.GetParameters();
			foreach (var arg in args)
			{
				var argAttrStr = GetKTAttributesAsString(arg, kMethodAttrIgnoreSet);
				if (!string.IsNullOrEmpty(argAttrStr))
				{
					argAttrStr = string.Format("[{0}]", argAttrStr);
				}

				sb.AppendFormat(
					kCodePartClassMember,
					GetTypeCSName(arg.ParameterType),
					arg.Name,
					KTUtils.GetDisplayName(arg, arg.Name),
					argAttrStr);

				sb.AppendLine();//by lijunfeng 2018/6/27
			}

			sb.AppendFormat(kCodePartRefresh, classDisplayName);

			sb.Append(kCodePartClassTail);
			var scriptPath = string.Format("{0}/{1}.cs", kExportDir, className);
			File.WriteAllText(scriptPath, KTStringBuilderCache.GetStringAndRelease(sb));

			//根据typeAttr 生成对应的处理类和switch函数
			isAction = typeAttr.type.GetType() == typeof(KTEditor.LevelEditor.TriggerResultType);
			string[] nameArr = typeAttr.type.ToString().Split('_');
			string clsName = isAction ? "TriggerAction_" : "TriggerCondition_";
			for (int i = 0; i < nameArr.Count(); i++)
			{
				clsName += nameArr[i].First().ToString().ToUpper() + nameArr[i].Substring(1);
			}

			KTUtils.EnsureDir("Assets/.TriggerGen.");
			scriptPath = string.Format("{0}/{1}.cs", "Assets/.TriggerGen.", clsName);

			string result = "";
			if (isAction)
			{
				result = "using System.Collections;\r\n" +
					"using System.Collections.Generic;\r\n" +
					"using UnityEngine;\r\n" + "\r\n" +
					string.Format("public class {0} : ATriggerAction\r\n",   	clsName)   +	"{\r\n" +
					"\tpublic override void PlayAction()\r\n" +
					"\t{\r\n" + "\r\n" + "\t}\r\n" + "}";

				switchText += "\r\ncase TriggerResultType." + typeAttr.type.ToString() + string.Format(":\r\nresult = new {0}(); \r\nbreak;", clsName);
			}
			else
			{
				result = "using System.Collections;\r\n" +
					"using System.Collections.Generic;\r\n" +
					"using UnityEngine;\r\n" + "\r\n" +
					string.Format("public class {0} : ATriggerCondition\r\n", clsName) + "{\r\n" +
					"\tpublic override bool CheckCondition()\r\n" +
					"\t{\r\n" + "\t\treturn false;\r\n" + "\t}\r\n" + "}";
				switchText += "\r\ncase TriggerConditionType." + typeAttr.type.ToString() + string.Format(":\r\nresult = new {0}(); \r\nbreak;", clsName);
			}
			File.WriteAllText(scriptPath, result);
		}

		switchText += "}";

		if (!isAction)
		{
			File.WriteAllText(string.Format("{0}/{1}.text", "Assets/.TriggerGen.", "condition_switch"), switchText);
		}
		else
		{
			File.WriteAllText(string.Format("{0}/{1}.text", "Assets/.TriggerGen.", "action_switch"), switchText);
		}
	}

    /// <summary>
    /// by lijunfeng 2018/6/25
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool CheckIsIListIterface(System.Type type)
    {
        foreach (var iface in type.GetInterfaces())
        {
            if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// by lijunfeng 2018/6/25
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static string GetTypeCSName(System.Type type)
    {
        string name;
        if (kBaseTypeToCSName.TryGetValue(type, out name))
        {
            return name;
        }

        if (CheckIsIListIterface(type))
        {
            var dstItemType = type.GetGenericArguments()[0];
            if (kBaseTypeToCSName.TryGetValue(dstItemType, out name))
            {
                return name;
            }

            return string.Format("List<{0}>", dstItemType.FullName);
        }
        
        return type.FullName.Replace('+', '.');
    }

    private static string GetKTAttributesAsString(ICustomAttributeProvider provider, HashSet<string> ignore = null)
    {
        var attrStrs = provider.GetCustomAttributes(false)
            .Where(attr =>
            {
                var attrTypeName = attr.GetType().Name;
                if (!attrTypeName.StartsWith("KT"))
                {
                    return false;
                }

                if (ignore != null && ignore.Contains(attrTypeName))
                {
                    return false;
                }

                return true;
            })
            .Select(attr => attr.ToString())
            .ToArray();
        return string.Join(", ", attrStrs);
    }
}
