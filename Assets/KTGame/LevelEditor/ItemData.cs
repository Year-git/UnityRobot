using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KTEditor.LevelEditor {
    [System.Serializable]
    public class JsonData {
        public int ver = 0;//by lijunfeng 2018/6/28
        public float x = 0;
        public float y = 0;
        public float width = 0;
        public float height = 0;
        public Root root = new Root();
        public List<BornLocation> bornLocations = new List<BornLocation>();
        public List<FightStage> fightStages = new List<FightStage>();
        public List<MonsterPoint> monsters = new List<MonsterPoint>();
        public List<MonsterGenerator> monsterGenerators = new List<MonsterGenerator>();
        public List<Trigger> triggers = new List<Trigger>();
        public List<Trigger> clientTriggers = new List<Trigger>();
        public List<PatrolArea> patrolAreas = new List<PatrolArea>();
        public List<PatrolPath> patrolPaths = new List<PatrolPath>();
        public List<PatrolPoint> patrolPoints = new List<PatrolPoint>();
    }

    [System.Serializable]
    public class NodeData {
        [System.NonSerialized]
        public GameObject item = null;

        public ItemNodeType nodeType = ItemNodeType.none;
        public string addr = string.Empty;
        public string name = string.Empty;
        public Vector3 localPosition = Vector3.zero;
        public Vector3 localRotation = Vector3.zero;
        public Vector3 localScale = Vector3.zero;
        public Vector3 worldPosition = Vector3.zero;
        public Vector3 worldRotation = Vector3.zero;
        public Vector3 worldScale = Vector3.zero;

        public List<string> __children = new List<string>();
        public string addr_uuid;
        public List<string> __children_uuid = new List<string>();
    }

    [System.Serializable]
    public class Root : NodeData {

    }

    [System.Serializable]
    public class BornLocation : NodeData {

    }

    [System.Serializable]
    public class FightStage : NodeData {
        public int id = -1;
        public string __bornLocation = string.Empty;
        public string __bornLocation_uuid = string.Empty;
        public string __area = string.Empty;
        public string __area_uuid = string.Empty;
        public string __monsterGenerator = string.Empty;
        public string __monsterGenerator_uuid = string.Empty;
    }

    [System.Serializable]
    public class MonsterPoint : NodeData {
        public int generateType = -1;
        public int roleId = -1;
        public int roleType = -1;
    }

    [System.Serializable]
    public class MonsterGenerator : NodeData {
        public GeneratorType generatorType = GeneratorType.item;
        public int selectedRoleType = -1;
        public int selectedRoleId = -1;
        public bool hatredSharing = true; //仇恨共享 by lijunfeng 2018/7/9
        public bool isLocal = false; //是否本地 by lijunfeng 2018/9/11
        public bool isCustom = true;//是否为定制npc,false则为群演npc by lijunfeng 2018/10/18
        public string selectedNormalAction = string.Empty;
        public string selectedBornAction = string.Empty;
        public float bornShotRange = 0;
        public int generateNum = 0;
        public int level = 0;
        public LevelType selectedLevelType = LevelType.manual;
        public ShowType selectedShowType = ShowType.for_all;
        public bool isEnabled = true;
        public MobType selectedMobType = MobType.interval;
        public bool isRandomInterval = false;
        public int randomMode = 0;
        public float intervalLow = 0;
        public float intervalHigh = 0;
        public float intervalTime = 0;
        public float appendTime = 0;
        public int appendNum = 0;
        public PatrolType selectedPatrolType = PatrolType.appoint;
        public bool isGoAndBackPatrol = false;
        public bool isKeepPatrol = false;
        public int patrolGoBackNum = 0;
        public string __patrolPath = string.Empty;
        public string __patrolPath_uuid = string.Empty;
        public string __patrolArea = string.Empty;
        public string __patrolArea_uuid = string.Empty;

        public string __followTarget = string.Empty;
        public string __followTarget_uuid = string.Empty;
        public Vector3 followOffset = Vector3.zero;
    }

    [System.Serializable]
    public class PatrolArea : NodeData {
        public Color lineColor = Color.green;
        public float radius = 0;
        public int id = 0;
    }

    [System.Serializable]
    public class PatrolPath : NodeData {
        public int pointID1= 0;//by lijunfeng 2018/6/28 保存两个端点id
        public int pointID2 = 0;//by lijunfeng 2018/6/28 保存两个端点id
        public Color lineColor = Color.green;
    }

    [System.Serializable]
    public class PatrolPoint : NodeData {
        public int id=0; //by lijunfeng 2018/6/28
        public bool isPathPoint = false;//by lijunfeng 2018/7/13 是否是网格寻路点
        public float stayRangeLow = 0;
        public float stayRangeHigh = 0;
        public float moveSpeed = 0;
    }

    [System.Serializable]
    public class Trigger : NodeData {
        public int executeNum = 0;
        public bool isActive = true;
        public bool keepRunning = true;
        public bool runResultOnBorn = false;
        public TriggerConditionRelationType conditionRelationType = TriggerConditionRelationType.all;
        public int specNum = 0;
        public bool randomResult = false;
        public List<TriggerCondition> triggerConditions = new List<TriggerCondition>();
        public List<TriggerResult> triggerResults = new List<TriggerResult>();
    }

    [System.Serializable]
    public class ClientTrigger : Trigger
    {
    }

    [System.Serializable]
    public class TriggerCondition {
        public TriggerConditionType conditionType = TriggerConditionType.player_enter_area;
        public string __area = string.Empty;
        public string __area_uuid = string.Empty;
        public int taskID = 0;
        public int taskCdnIndex = -1;
        public float length = 0;
        public float width = 0;
        public float height = 0;
        public float angle = 0;
        public float radius = 0;
        public List<string> __condition = new List<string>(); //by lijunfeng 2018/6/26
        public List<string> __condition_uuid = new List<string>(); //by lijunfeng 2018/6/26
        public List<string> __targetGenerator = new List<string>(); //by lijunfeng 2018/10/16
        public List<string> __targetGenerator_uuid = new List<string>(); //by lijunfeng 2018/10/16
        public float bloodPercent = 0;

        public TriggerConditionPlayType playType = TriggerConditionPlayType.bubble_dialog;
        public int bubbleID = 0;
        public int skillID = 0;

        public DialogType dialogType = DialogType.dialog;
        public int dialogID = 0;

        public int times = 0; //by lijunfeng 2018/6/26 播放次数
        public int specifiedUnitType = 0;//by lijunfeng 2018/6/26 指定单位类型
        public int targetUnitType = 0;//by lijunfeng 2018/10/16 目标单位类型 
        //public List<string> __targetGenerator = new List<string>(); //by lijunfeng 2018/6/26
        //public List<string> __targetGenerator_uuid = new List<string>(); //by lijunfeng 2018/6/26
        public int areaRadius = 0;//by lijunfeng 2018/6/26 指定单位区域半径
        public int count = 0;//by lijunfeng 2018/6/26 目标单位数量,对应数量
        public int specifiedTimeType = 0;//by lijunfeng 2018/6/26 时间类型
        public int specifiedTimeFormat = 0;//by lijunfeng 2018/8/13 时间格式
        public Vector3 ymd = Vector3.zero;//by lijunfeng 2018/7/9 时间内容
        public int week = 0;//by lijunfeng 2018/7/9  时间内容
        public Vector3 hms = Vector3.zero;//by lijunfeng 2018/7/9  时间内容
        public int timeStamp = 0;//by lijunfeng 2018/8/13 时间戳
        public bool isRelativeToServer = false;//by lijunfeng 2018/8/13 是否相对服务器启动时间
        public int relationType = 0;//by lijunfeng 2018/10/16数值关系类型
        public int quizResult = 0;//by lijunfeng 2018/6/26 答题结果类型 
        public int eventID = 0;//by lijunfeng 2018/8/1 世界事件发生
        public int actID = 0;//by lijunfeng 2018/10/16 交互id
        public int weatherID = 0;//by lijunfeng 2018/10/17 天气id

        public string __patrolPath = string.Empty;//by lijunfeng 2018/9/12
        public string __patrolPath_uuid = string.Empty;//by lijunfeng 2018/9/12
        public string __patrolPoint = string.Empty;//by lijunfeng 2018/9/12
        public string __patrolPoint_uuid = string.Empty;//by lijunfeng 2018/9/12

    }

    [System.Serializable]
    public class TriggerResult {
        public TriggerResultType resultType = TriggerResultType.active_spawner;
        public List<string> __result = new List<string>(); //by lijunfeng 2018/6/26
        public List<string> __result_uuid = new List<string>(); //by lijunfeng 2018/6/26
        public List<string> __targetGenerator = new List<string>(); //by lijunfeng 2018/6/28
        public List<string> __targetGenerator_uuid = new List<string>(); //by lijunfeng 2018/6/28

        public float delay = 0;
        public int aiId = -1;

        public PatrolType selectedPatrolType = PatrolType.appoint;
        public bool isGoAndBackPatrol = false;
        public bool isKeepPatrol = false;
        public int patrolGoBackNum = 0;
        public string __patrolPath = string.Empty;
        public string __patrolPath_uuid = string.Empty;
        public string __patrolArea = string.Empty;
        public string __patrolArea_uuid = string.Empty;

        public string __followTarget = string.Empty;
        public string __followTarget_uuid = string.Empty;
        public Vector3 followOffset = Vector3.zero;

        public TriggerResultAnimType selectedAnimType = 0;
        public string animName = string.Empty;
        public int creatureId = -1;
        public bool isHasShotAnim = false;
        public bool isLoop = false;

        public int specifiedBubblePlayType = 0;//by lijunfeng 2018/6/26
        public int bubbleTargetType = 1;//by lijunfeng 2018/8/17 
        public string windowName = string.Empty;//by lijunfeng 2018/7/16
        public int transmissionID = 0;//by lijunfeng 2018/6/26
        public int tranportID = 0;//by lijunfeng 2018/7/16
        public int viewRange = 0;//by lijunfeng 2018/8/24
        public float emissiveScale = 0;//自发光强度 by lijunfeng 2018/9/26
        public bool isCustom = true;//是否为定制npc,false则为群演npc by lijunfeng 2018/10/18
        public int timeStampStart = 0;//开始时间戳 by lijunfeng 2018/10/18
        public int timeStampEnd = 0;//结束时间戳 by lijunfeng 2018/10/18
        public bool isRandomPos = false;//是否随机位置 by lijunfeng 2018/10/18
        public int taskID = 0;//任务id by lijunfeng 2018/11/22
        public int bubbleID = 0;
        public int skillID = 0;
        public bool isSkillTargeting = false;
        public string __area = string.Empty;
        public string __area_uuid = string.Empty;
        public int statuID = 0;
        public TriggerResultCampTagOpType campTagOpType = TriggerResultCampTagOpType.op_add;
        public int campTag = 0;

        public float innerRadius = 0;
        public float outerRadius = 0;

        public bool isInvincible = false;
        public int campId = 0;

        public DialogType dialogType = DialogType.dialog;
        public int dialogID = 0;

        public ToggleType toggle = ToggleType.on;

        public bool isReact = false;

        public int fsId = -1;
        public int progressId = -1;

        public int targetMode = (int)TriggerResultTargetMode.dps_cure_tank;
        public bool executeNow = false;
        public int executeCount = 0;
        public float executeInterval = 0;
        public float executeRandomRange = 0;

        public int buffId1 = 0;
        public int buffId2 = 0;
        public int buffId3 = 0;
        public int buffId4 = 0;
        public int shakeEffectId = 0;
        public bool toggleOn = false;
        public int camId = 0;

        public int musicId = 0;
        public int weatherId = 0;
        public int id = 0;
    }
}
