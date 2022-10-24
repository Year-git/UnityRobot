#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;

[KTDisplayName("交互物孵化器")]
public class KTLevelInteractionItemSpawner : KTLevelSpawnerSingle
{
    public readonly static ValueDropdownList<ShowType> kShowTypeNames = new ValueDropdownList<ShowType>()
        {
            { "仅自己可见", ShowType.only_self},
            { "所有人可见", ShowType.for_all},
        };

    [KTLevelExport("selectedRoleId")]
    [OnValueChanged("RefreshView")]
    [KTUseDataPicker(KTExcels.kInteractionItems, "宝物ID", new string[] { "宝物ID", "宝物名称" }, "KTLevelEditorWindowPro")]
    public int id;

    [KTLevelExport("isEnabled")]
    [LabelText("默认开启交互状态")]
    public bool enableOnDefault;

    //[KTLevelExport("selectedShowType")]
    //[LabelText("可见类型"), ValueDropdown("kShowTypeNames")]
    //public ShowType showType = ShowType.for_all;

    [HideInInspector, KTLevelExport("generatorType")]
    public GeneratorType generatorType = GeneratorType.interactionItem;

    [HideInInspector, KTLevelExport("selectedRoleType")]
    public RoleType roleType = RoleType.reactor;

    public override void RefreshView()
    {
        RefreshUnitView(KTExcels.kInteractionItems, id);
    }
}
#endif

