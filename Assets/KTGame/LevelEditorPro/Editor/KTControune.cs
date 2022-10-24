using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class KTControune
{
    private static Dictionary<IEnumerator, IEnumerator> dic = new Dictionary<IEnumerator, IEnumerator>();

    public static void StartControune(IEnumerator func)
    {
        dic.Add(func, func);
    }

    public static void StopControune(IEnumerator func)
    {
        dic.Remove(func);
    }

    public static void Update()
    {
        var keys = dic.Keys.ToArray();
        for (int i = 0; i < keys.Length; i++)
        {
            if (dic.ContainsKey(keys[i]))
                dic[keys[i]].MoveNext();
        }
    }
}