/// <summary>
/// 帧同步操作类型
/// </summary>
public enum FrameSynchronOperationType
{
    Click = 1,
    Joystic = 2,
}

public enum SenceId
{
    LoginSence = 1,
    MainSence = 2,
    BattleSence = 10000,
}

/// <summary>
/// 地图类型枚举
/// </summary>
public enum MapType
{
    Login = -1,
    Main = 0,
    Battle = 1,
}

/// <summary>
/// Npc对象枚举
/// </summary>
public enum NpcType
{
    PlayerNpc = 1,      // 玩家Npc
    BuildNpc = 2,       // 建筑Npc
    MonsterNpc = 3,     // 怪物Npc
    UINpc = 4,          // 界面Npc
}

/// <summary>
/// 怪物类型
/// </summary>
public enum MonsterType
{
    Common = 0,      // 普通怪物
    Elite = 1,      // 精英怪物
    Boss = 2,      // Boss
    
}

/// <summary>
/// 配件类型枚举【枚举的字符串对应着配件槽位的挂点名】
/// </summary>
public enum RobotPartType
{
    Body = 0,       //载体配件
    Weapon = 1,     //武器配件
    Move = 2,       //移动配件
    Assist = 3,     //辅助配件
    Ornament = 4,   //装饰配件
}

public enum AttributeType
{
    Hp = 1,         //生命
    Attack = 2,     //攻击
    Weight = 3,     //承载
    Speed = 4,      //移动速度
    CostWeight = 5, //耗载
    Trun = 6,       //转向
    Durability = 7, //耐久
}

public enum AttributeValueType
{
    Base = 1,       //固定值
    Percent = 2,    //百分比
}

/// <summary>
/// 模型损坏类型
/// </summary>
public enum ModelActiveEnum
{
    normal,
    damage,
    dead,
}

/// <summary>
/// Buff叠加类型
/// </summary>
public enum BuffOverlayType
{
    Queue = 1,
    Share = 2,
    Each = 3,
}

public enum RobotPartHoleType
{
    Normal = 0,     // 父子槽
    Point = 1,  // 挂点槽
}

public enum LoadProgressType
{
    Map,
    ServerNpc,
    LevelNpc,
    LevelPoint,
    LevelTrap,
}