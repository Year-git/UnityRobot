using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;

public class KTLevelEditorWindowPro : OdinEditorWindow
{
    private const string kTablePathKey = "KTLevelEditorWindowPro.tablePath";

    [MenuItem("Tools/CheckPointEditorPro")]
    public static void ShowLevelEditor()
    {
        EditorWindow.GetWindow<KTLevelEditorWindowPro>("关卡编辑器").autoRepaintOnSceneChange = true;
    }

	private new void OnEnable()
	{
		Selection.selectionChanged += OnSelectionChanged;
		tablePath = EditorPrefs.GetString(kTablePathKey, "");
		KTExcelManager.kExcelRootDir = tablePath;
		EditorApplication.update += this.EditorUpdate;
	}

    private void EditorUpdate()
    {
        KTControune.Update();
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= OnSelectionChanged;
        EditorApplication.update -= this.EditorUpdate;
    }

    private void OnSelectionChanged()
    {
        var selTrans = Selection.activeTransform;
        selectEntity = (selTrans != null) ? selTrans.GetComponent<KTLevelEntity>() : null;
        Repaint();
    }

	[BoxGroup("关卡"), HideLabel, GUIColor(0.5f, 0.8f, 0.9f)]
	public KTLevelEditorCommands_Level _levelCommands;
	[BoxGroup("对象"), HideLabel, GUIColor(0.0f, 1.0f, 0.0f)]
    public KTLevelEditorCommands_Object _entityCommands;
    [BoxGroup("工具"), HideLabel, GUIColor(0.5f, 0.8f, 0.9f)]
    public KTLevelEditorCommands_Tool _toolCommands;

    [BoxGroup("选中"), HideLabel, InlineEditor(Expanded = true), GUIColor(0.5f, 0.8f, 0.9f), ShowIf("HasSelectEntity")]
    public KTLevelEntity selectEntity;
    private bool HasSelectEntity() { return selectEntity != null; }

    [FoldoutGroup("高级", false), LabelText("数据表目录"), OnValueChanged("OnTablePathChanged")]
    public string tablePath;

    private void OnTablePathChanged()
    {
        KTExcelManager.kExcelRootDir = tablePath;
        EditorPrefs.SetString(kTablePathKey, tablePath);
    }

    [FoldoutGroup("高级", false), Button("生成触发器编辑器代码", ButtonSizes.Medium), GUIColor(1.0f, 0.4f, 0.1f)]
    public void GenTriggerCode()
    {
        KTLevelTriggerCodeGen.GenAll();   
    }
}
