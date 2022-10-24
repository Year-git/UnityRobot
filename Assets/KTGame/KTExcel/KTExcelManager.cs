#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KTExcelManager : KTSimpleSingleton<KTExcelManager>
{
    public static string kExcelRootDir = "";
    private Dictionary<string, KTExcelSheet> _sheetCache = new Dictionary<string, KTExcelSheet>();


    public string Get<IT>(string tableName, IT id, string colume)
    {
        var sheetInfo = GetSheet(tableName);
        return (sheetInfo != null) ? sheetInfo.Get(id.ToString(), colume) : "";
    }

    public string GetColume(string tableName,int row,string colume)
    {
        var sheetInfo = GetSheet(tableName);
        return (sheetInfo != null) ? sheetInfo.GetColume(row, colume) : "";
    }

    public int[] GetAllRows(string tableName)
    {
        var sheetInfo = GetSheet(tableName);
        return (sheetInfo != null) ? sheetInfo.GetAllRow(): null;
    }

    public KTExcelSheet GetSheet(string tableName)
    {
        var sheetPath = GetTablePath(tableName).ToLower();

        KTExcelSheet sheetInfo = null;
        if (_sheetCache.TryGetValue(sheetPath, out sheetInfo))
        {
            return sheetInfo;
        }

        try
        {
            sheetInfo = new KTExcelSheet(sheetPath);
            _sheetCache.Add(sheetPath, sheetInfo);
            return sheetInfo;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
    }

    private static string GetTablePath(string tableName)
    {
        return string.Format("{0}/{1}", kExcelRootDir, tableName);
    }
}

#endif
