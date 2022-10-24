//using KTEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ExportContextUtils
{
    /// <summary>
    /// 刷新创建excel new string[] { "ID", "路径" } list
    /// by lijunfeng 2018/9/12
    /// </summary>
    /// <param name="_filepath"></param>
    /// <param name="_tableheadarray"></param>
    /// <param name="_parameterlist"></param>
    public static void RefreshExcel(string _filepath, string[] _tableheadarray, List<string[]> _parameterlist)
    {
        string exceldir = Path.GetFullPath("../tables");
        if (!Directory.Exists(exceldir))
        {
            Debug.LogErrorFormat("目录缺失,{0}", exceldir);
            return;
        }
        string excelfilepath = string.Format("{0}/{1}.xlsx", exceldir, _filepath);
        if (!File.Exists(excelfilepath))
        {
            ExcelHelper.NewExcel(excelfilepath, _tableheadarray);
            RefreshExcel(_filepath, _tableheadarray, _parameterlist);
            return;
        }
        Excel excel = ExcelHelper.LoadExcel(excelfilepath);
        if (excel.Tables.Count == 0)
        {
            File.Delete(excelfilepath);
            RefreshExcel(_filepath, _tableheadarray, _parameterlist);
            return;
        }
        ExcelTable table = excel.Tables[0];
        int numberofrows = table.NumberOfRows;
        if (numberofrows > 1)
        {
            for (int i = 2; i <= numberofrows; i++)
            {
                table.DeleteRow(i);
            }
        }
        for (int i = 0; i < _parameterlist.Count; i++)
        {
            ExcelRow newRow = table.NewRow();
            UnityEngine.Assertions.Assert.IsTrue(_parameterlist[i].Length == _tableheadarray.Length, "paramenters field count is not match head count");
            for (int j = 0; j < _parameterlist[i].Length; j++)
            {
                newRow[j + 1].Value = _parameterlist[i][j];
            }
            table.AddRow(newRow);
        }
        ExcelHelper.SaveExcel(excel, excelfilepath);
    }

    /// <summary>
    /// 刷新创建excel   new string[] { "ID", "路径" } list 方便使用
    /// </summary>
    /// <param name="_filepath"></param>
    /// <param name="_tableheadarray"></param>
    /// <param name="_parameterlist"></param>
    public static void RefreshExcel(string _filepath, string[] _tableheadarray, List<string> _parameterlist)
    {
        List<string[]> arraylist = new List<string[]>();
        for (int i = 0; i < _parameterlist.Count; i++)
        {
            string[] ta = new string[2] { i.ToString(), _parameterlist[i] };
            arraylist.Add(ta);
        }
        RefreshExcel(_filepath, _tableheadarray, arraylist);
    }
}
