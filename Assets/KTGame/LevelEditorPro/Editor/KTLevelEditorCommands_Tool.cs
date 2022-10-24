using Sirenix.OdinInspector;
using UnityEditor;

[System.Serializable]
public class KTLevelEditorCommands_Tool
{
    [ButtonGroup("ToolGroup")]
    [Button("导出区域表", ButtonSizes.Medium)]
    [GUIColor(0.0f, 1.0f, 0.0f)]
    public bool ExportAreaExcel()
    {
        var path = EditorUtility.OpenFolderPanel("选择json目录", "", "");
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        KTLevelJsonTranslator.ExportAreaToExcel(path);
        return true;
    }

    [ButtonGroup("ToolGroup")]
    [Button("导出NPC位置表", ButtonSizes.Medium)]
    [GUIColor(0.0f, 1.0f, 0.0f)]
    public bool ExportSpawnerPosExcel()
    {
        var path = EditorUtility.OpenFolderPanel("选择json目录", "", "");
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        KTLevelJsonTranslator.ExportSpawnerPosToExcel(path);
        return true;
    }

    [ButtonGroup("ToolGroup")]
    [Button("导出交互物位置表", ButtonSizes.Medium)]
    [GUIColor(0.0f, 1.0f, 0.0f)]
    public bool ExportInteractItemSpawnerPosToExcel()
    {
        var path = EditorUtility.OpenFolderPanel("选择json目录", "", "");
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        KTLevelJsonTranslator.ExportInteractItemSpawnerPosToExcel(path);
        return true;
    }

    [ButtonGroup("ToolGroup")]
    [Button("导出NPC巡逻路径表", ButtonSizes.Medium)]
    [GUIColor(0.0f, 1.0f, 0.0f)]
    public bool ExportPatrolPathToExcel()
    {
        var path = EditorUtility.OpenFolderPanel("选择json目录", "", "");
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        KTLevelJsonTranslator.ExportPatrolPathToExcel(path);
        return true;
    }

    [ButtonGroup("ToolGroup")]
    [Button("检查CreatueID是否存在", ButtonSizes.Medium)]
    [GUIColor(0.0f, 1.0f, 0.0f)]
    public bool CheckIDExistInCreature()
    {
        var path = EditorUtility.OpenFilePanel("选择json目录", "Assets/levels", "json");
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        KTLevelJsonTranslator.CheckIDExistInCreature(path);
        return true;
    }

    [ButtonGroup("ToolGroup")]
    [Button("检查单位类型", ButtonSizes.Medium)]
    [GUIColor(0.0f, 1.0f, 0.0f)]
    public bool CheckUnitTypeInvalide()
    {
        var path = EditorUtility.OpenFolderPanel("选择json目录", "", "");
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        KTLevelJsonTranslator.CheckUnitTypeInvalide(path);
        return true;
    }

    [ButtonGroup("ToolGroup")]
    [Button("检查旋转错误", ButtonSizes.Medium)]
    [GUIColor(0.0f, 1.0f, 0.0f)]
    public bool CheckRotationInvalide()
    {
        var path = EditorUtility.OpenFolderPanel("选择json目录", "", "");
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        KTLevelJsonTranslator.CheckRotationInvalide(path);
        return true;
    }
}
