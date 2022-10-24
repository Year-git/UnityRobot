#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using OfficeOpenXml;

using ColumeToIndexMap = System.Collections.Generic.Dictionary<string, int>;
using IdToRowMap = System.Collections.Generic.Dictionary<string, int>;
using System.IO;
using System.Linq;

public class KTExcelSheet : IEnumerable<int>
{
    public const int kInvalidRow = -1;

    private ExcelWorksheet _sheet;
    private ColumeToIndexMap _columeToIndex;
    private IdToRowMap _idToRow;
    private int _startRow;

    public KTExcelSheet(string path)
    {
        _sheet = LoadSheet(path);
        _columeToIndex = CreateColumeToIndexMap(_sheet);
        _idToRow = CreateIdToRowMap(_sheet);
    }

    #region Load
    private static ExcelWorksheet LoadSheet(string sheetPath)
    {
        ExcelWorksheet sheet;
        FileInfo file = new FileInfo(sheetPath);
        ExcelPackage ep = new ExcelPackage(file);
        sheet = ep.Workbook.Worksheets.FirstOrDefault();
        return sheet;
    }
    private static ColumeToIndexMap CreateColumeToIndexMap(ExcelWorksheet sheet)
    {
        var columeToIndexMap = new ColumeToIndexMap();
        var rowNum = sheet.Dimension.Rows;
        var columeNum = sheet.Dimension.Columns;
        if (rowNum <= 0 || columeNum <= 0)
        {
            return columeToIndexMap;
        }

        for (int i = 1; i <= columeNum; ++i)
        {
            var titleValue = sheet.Cells[1, i, 1, i].Value;
            if (titleValue != null)
            {
                columeToIndexMap[titleValue.ToString()] = i;
            }
        }
        return columeToIndexMap;
    }

    private static IdToRowMap CreateIdToRowMap(ExcelWorksheet sheet)
    {
        var idToRowMap = new IdToRowMap();
        var rowNum = sheet.Dimension.Rows;
        var columeNum = sheet.Dimension.Columns;

        for (int i = 2; i <= rowNum; ++i)
        {
            var row = sheet.Cells[i, 1, i, columeNum];
            var idValue = row.First().Value;
            if (idValue == null)
            {
                continue;
            }
            idToRowMap[idValue.ToString()] = i;
        }
        return idToRowMap;
    }
    #endregion

    #region IEnumerable
    public IEnumerator<int> GetEnumerator()
    {
        var rows = _sheet.Dimension.Rows;
        for (int i = 2; i <= rows; ++i)
        {
            var idValue = _sheet.Cells[i, 1].Value;
            if (idValue == null)
            {
                continue;
            }
            yield return i;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion

    public string FormatRow(int row, string[] columes, string seperator)
    {
        var sb = KTStringBuilderCache.Acquire();
        foreach (var title in columes)
        {
            int columeIndex;
            if (_columeToIndex.TryGetValue(title, out columeIndex))
            {
                string valueString = AsStringValue(_sheet.Cells[row, columeIndex].Value);
                if (sb.Length > 0)
                {
                    sb.Append(seperator);
                }
                sb.Append(valueString);
            }
        }
        return KTStringBuilderCache.GetStringAndRelease(sb);
    }

    public string Get(string id, string colume)
    {
        var row = GetRow(id);
        if (row == kInvalidRow)
        {
            return "";
        }
        return GetColume(row, colume);
    }

    public int GetRow<T>(T id)
    {
        int row;
        if (_idToRow.TryGetValue(id.ToString(), out row))
        {
            return row;
        }
        return kInvalidRow;
    }

    public string GetColume(int row, string colume)
    {
        int idx;
        if (_columeToIndex.TryGetValue(colume, out idx))
        {
            if (_sheet.Cells[row, idx].Value != null)
            {
                return _sheet.Cells[row, idx].Value.ToString();
            }
        }
        return "";
    }

    private static string AsStringValue(object value)
    {
        return value != null ? value.ToString() : null;
    }
    public int[] GetAllRow()
    {
        if (_idToRow.Count == 0) return null;
        int[] rows = new int[_idToRow.Count];
        int tindex = 0;
        foreach (var v in _idToRow)
        {
            rows[tindex] = v.Value;
            tindex++;
        }
        return rows;
    }
}
#endif
