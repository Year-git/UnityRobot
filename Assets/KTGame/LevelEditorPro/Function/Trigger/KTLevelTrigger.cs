#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using System.Linq;
using KTEditor.LevelEditor;
using System.Reflection;

[KTDisplayName("触发器"), ExecuteInEditMode]
public class KTLevelTrigger : KTLevelEntity
{
    protected override string uuidPrefix
    {
        get
        {
            return "triggers";
        }
    }

    public readonly static ValueDropdownList<TriggerConditionRelationType> kConditionModeNames = new ValueDropdownList<TriggerConditionRelationType>()
        {
            { "全部满足", TriggerConditionRelationType.all },
            { "任一满足", TriggerConditionRelationType.one },
            { "达到数量", TriggerConditionRelationType.spec },
        };

    [KTLevelExport("runResultOnBorn")]
    [LabelText("初始执行")]
    public bool runOnMapInit;

    [KTLevelExport("isActive")]
    [LabelText("激活")]
    public bool enable;

    [KTLevelExport("executeNum")]
    [LabelText("触发次数上限")]
    public int runMax;

    [KTLevelExport("keepRunning")]
    [LabelText("循环结果")]
    public bool loop;

    [KTLevelExport("conditionRelationType")]
    [LabelText("条件关系"), ValueDropdown("kConditionModeNames")]
    public TriggerConditionRelationType conditionMode;

    [KTLevelExport("randomResult")]
    [LabelText("随机触发一条结果")]
    public bool randomResult;

    [KTLevelExport("specNum")]
    [LabelText("达到数量"), ShowIf("IsSumMode")]
    public int passConditionMinNum;
    private bool IsSumMode() { return conditionMode == TriggerConditionRelationType.spec; }

    [KTLevelExport("triggerConditions")]
    [HorizontalGroup("Group"), LabelText("触发条件"), InlineEditor, ListDrawerSettings(HideAddButton = true, OnTitleBarGUI = "DrawAddCondition")]
    public List<KTLevelTriggerCondition> conditions = new List<KTLevelTriggerCondition>();

    [KTLevelExport("triggerResults")]
    [HorizontalGroup("Group"), LabelText("触发结果"), InlineEditor, ListDrawerSettings(HideAddButton = true, OnTitleBarGUI = "DrawAddAction")]
    public List<KTLevelTriggerAction> actions = new List<KTLevelTriggerAction>();

    class SubTypeInfos
    {
        public System.Type[] subTypes;
        public string[] subTypeNames;
        public System.Type baseType;

        private static int GetLevelClassType(System.Type type)
        {
            var levelClassTypeAttr = KTLevelTools.GetAttribute<KTLevelClassTypeAttribute>(type);
            if (levelClassTypeAttr != null)
            {
                return (int)levelClassTypeAttr.type;
            }
            return int.MaxValue;
        }

        //by lijunfeng 2018/6/27 菜单根据客户端或服务器平台分类显示
        public SubTypeInfos(System.Type type,System.Type triggerType)
        {
            baseType = type;
            subTypes = baseType.Assembly.GetTypes()
                .Where(t=>
                {
                    var platform =KTLevelTools.GetAttribute<KTLevelPlatformAttribute>(t);
                    if (triggerType == typeof(KTLevelTrigger))
                    {
                        return t.IsSubclassOf(baseType) && (platform == null || platform.platformType != PlatformType.Client);
                    }
                    else if (triggerType == typeof(KTLevelTriggerClient))
                    {
                        return t.IsSubclassOf(baseType) && (platform == null || platform.platformType != PlatformType.Server); ;
                    }
                    else
                    {
                        return t.IsSubclassOf(baseType);
                    }
                })
                .OrderBy(t => GetLevelClassType(t))
                .ToArray();
            subTypeNames = subTypes
                .Select(t => KTUtils.GetDisplayName(t, t.Name))
                .ToArray();
        }
    }

    private SubTypeInfos _conditionSubTypeInfos;
    private SubTypeInfos _actionSubTypeInfos;

    //运行中动态实例化，为了识别trigger类型
    private void Awake()
    {
        _conditionSubTypeInfos = new SubTypeInfos(typeof(KTLevelTriggerCondition), this.GetType());
        _actionSubTypeInfos = new SubTypeInfos(typeof(KTLevelTriggerAction), this.GetType());
    }

    [HideInInspector]
    public int _addConditionIndex;
    private void DrawAddCondition()
    {
        _addConditionIndex = DrawAdd(_addConditionIndex, _conditionSubTypeInfos, conditions);
    }

    [HideInInspector]
    public int _addActionIndex;
    private void DrawAddAction()
    {
        _addActionIndex = DrawAdd(_addActionIndex, _actionSubTypeInfos, actions);
    }

    private int DrawAdd<T>(int idx, SubTypeInfos subTypeInfos, List<T> list) where T : KTLevelTriggerNode
    {
        idx = EditorGUILayout.Popup(idx, subTypeInfos.subTypeNames);
        if (SirenixEditorGUI.ToolbarButton(EditorIcons.Plus))
        {
            var subType = subTypeInfos.subTypes[idx];
            AddTriggerNode(subType, list);
        }
        return idx;
    }

    public KTLevelTriggerNode CreateTriggerNode(System.Type type)
    {
        if (type.IsSubclassOf(typeof(KTLevelTriggerCondition)))
        {
            return AddTriggerNode(type, conditions);
        }
        else if(type.IsSubclassOf(typeof(KTLevelTriggerAction)))
        {
            return AddTriggerNode(type, actions);
        }
        return null;
    }

    private T AddTriggerNode<T>(System.Type subType, List<T> list) where T : KTLevelTriggerNode
    {
        var go = new GameObject();
        go.hideFlags = HideFlags.HideInHierarchy;
        GameObjectUtility.SetParentAndAlign(go, gameObject);
        var comp = (T)go.AddComponent(subType);
        comp.Refresh();
        list.Add(comp);
        return comp;
    }

    HashSet<KTLevelTriggerCondition> _conditionSet = new HashSet<KTLevelTriggerCondition>();
    HashSet<KTLevelTriggerAction> _actionSet = new HashSet<KTLevelTriggerAction>();
    private void Update()
    {
        RemoveUnusedTriggerNodes(_conditionSet, conditions);
        RemoveUnusedTriggerNodes(_actionSet, actions);
    }

    private void RemoveUnusedTriggerNodes<T>(HashSet<T> set, List<T> list) where T : KTLevelTriggerNode
    {
        set.Clear();
        set.UnionWith(Enumerable.Range(0, transform.childCount)
            .Select(idx => transform.GetChild(idx).GetComponent<T>())
            .Where(c => c != null));
        set.ExceptWith(list);
        foreach(var node in set)
        {
            GameObject.DestroyImmediate(node.gameObject);
        }
    }
}
#endif
