#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;


[KTDisplayName("物件孵化器")]
public class KTLevelItemSpawner : KTLevelSpawnerSingle
{
    [KTLevelExport("selectedRoleId")]
    [OnValueChanged("RefreshView")]
    [KTUseDataPicker(KTExcels.kItems, "物件ID", new string[] { "物件ID", "备注" }, "KTLevelEditorWindowPro")]
	[LabelText("物件ID")]
    public int id;

    [HideInInspector, KTLevelExport("generatorType")]
    public GeneratorType generatorType = GeneratorType.item;

    public override void RefreshView()
    {
        RefreshUnitView(KTExcels.kItems, id);
    }
}
#endif