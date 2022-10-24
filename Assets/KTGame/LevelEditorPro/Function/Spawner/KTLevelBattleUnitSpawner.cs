#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;
using KTEditor.LevelEditor;

[KTDisplayName("战斗单位孵化器")]
public class KTLevelBattleUnitSpawner : KTLevelSpawnerSingle
{
    [System.Serializable]
    public class PatrolPathSettings
    {
        [KTLevelExport("__patrolPath"), KTLevelExport("__patrolPath_uuid")]
        [LabelText("路径")]
        public KTLevelPatrolPath path;

        [KTLevelExport("isGoAndBackPatrol")]
        [LabelText("往返巡逻")]
        public bool rewind;

        [KTLevelExport("isKeepPatrol")]
        [LabelText("使用巡逻次数")]
        public bool loopPatrol;

        [KTLevelExport("patrolGoBackNum")]
        [LabelText("巡逻次数"), ShowIf("loopPatrol")]
        public int loopPatrolNum;

        //[LabelText("跟随对象"), KTLevelExport("__followTarget"), KTLevelExport("__followTarget_uuid")]
        //public KTLevelBattleUnitSpawner followTarget;

        //[LabelText("跟随对象偏移"), KTLevelExport("followOffset")]
        //public Vector3 followOffset;
    }

    [System.Serializable]
    public class PatrolZoneSettings
    {
        [KTLevelExport("__patrolArea"), KTLevelExport("__patrolArea_uuid")]
        [LabelText("区域")]
        public KTLevelPatrolZone zone;
    }

    private readonly static ValueDropdownList<RoleType> kBattleUnitTypeNames = new ValueDropdownList<RoleType>()
    {
        { "小怪", RoleType.little_monster },
        { "精英怪", RoleType.elite_monster },
        { "小BOSS", RoleType.little_boss },
        { "大BOSS", RoleType.big_boss },
        { "战斗NPC", RoleType.other },
        { "不可选中物", RoleType.un_seletable },
        { "公共载具", RoleType.public_carrier},
    };
    
    private readonly static ValueDropdownList<PatrolType> kPatrolTypeNames = new ValueDropdownList<PatrolType>()
    {
        { "路径", PatrolType.appoint},
        { "区域", PatrolType.area },
    };

    [KTLevelExport("selectedRoleId")]
    [OnValueChanged("RefreshView")]
    [OnValueChanged("RefreshRoleType")]
    [KTUseDataPicker(KTExcels.kCreatures, "id", new string[] { "id", "名字" }, "KTLevelEditorWindowPro")]
    public int id;

    [HideInInspector, KTLevelExport("generatorType")]
    public GeneratorType generatorType = GeneratorType.battleUnit;

    //[KTLevelExport("selectedRoleType")]
    //[LabelText("角色类型"), ValueDropdown("kBattleUnitTypeNames")]
    //public RoleType roleType = RoleType.little_monster;

    //[KTLevelExport("isLocal")]
    //[LabelText("是否本地NPC")]
    //public bool isLocal = false;

    //[KTLevelExport("isLocal")]
    //[LabelText("是否是定制NPC")]
    //public bool isCustom = true;//false则为群演npc

    //[KTLevelExport("hatredSharing")]
    //[LabelText("仇恨共享")]
    //public bool hatredSharing = true;

    [KTLevelExport("selectedPatrolType")]
    [LabelText("巡逻类型"), ValueDropdown("kPatrolTypeNames")]
    public PatrolType patrolType;

    [HideLabel, ShowIf("IsZonePatrol")]
    public PatrolZoneSettings zoneSettings;
    [HideLabel, ShowIf("IsPathPatrol")]
    public PatrolPathSettings pathSettings;

    private bool IsZonePatrol() { return patrolType == PatrolType.area; }
    private bool IsPathPatrol() { return patrolType == PatrolType.appoint; }

    public override void RefreshView()
    {
        RefreshUnitView(KTExcels.kCreatures, id);
    }

	private void RefreshRoleType()
	{
		//var roleType = KTExcelManager.instance.Get(KTExcels.kCreatures, id, "单位类型");
		//if (string.IsNullOrEmpty(roleType))
		//{
		//	Debug.LogError("单位类型不存在");
		//	return;
		//}
		//this.roleType = (RoleType)int.Parse(roleType);
	}
}

#endif
