using UnityEngine;
using System;
using System.Collections.Generic;
using LuaInterface;
using UnityEditor;
using Framework;
using BindType = ToLuaMenu.BindType;
using FairyGUI;

public static class CustomSettings
{
    public static string saveDir = Application.dataPath + "/Scripts/Generate/";
    public static string toluaBaseType = Application.dataPath + "/ThirdParty/ToLua/BaseType/";
    public static string baseLuaDir = Application.dataPath + "/Tolua/Lua/";
    public static string injectionFilesPath = Application.dataPath + "/ToLua/Injection/";

    //导出时强制做为静态类的类型(注意customTypeList 还要添加这个类型才能导出)
    //unity 有些类作为sealed class, 其实完全等价于静态类
    public static List<Type> staticClassTypes = new List<Type>
    {

    };

    //附加导出委托类型(在导出委托时, customTypeList 中牵扯的委托类型都会导出， 无需写在这里)
    public static DelegateType[] customDelegateList =
    {

    };

    //在这里添加你要导出注册到lua的类型列表
    public static BindType[] customTypeList =
    {
        //FairyGUI
        _GT(typeof(EventContext)),
        _GT(typeof(EventDispatcher)),
        _GT(typeof(EventListener)),
        _GT(typeof(InputEvent)),
        _GT(typeof(DisplayObject)),
        _GT(typeof(Container)),
        _GT(typeof(Stage)),
        _GT(typeof(FairyGUI.Controller)),
        _GT(typeof(GObject)),
        _GT(typeof(GGraph)),
        _GT(typeof(GGroup)),
        _GT(typeof(GImage)),
        _GT(typeof(GLoader)),
        _GT(typeof(GMovieClip)),
        _GT(typeof(TextFormat)),
        _GT(typeof(GTextField)),
        _GT(typeof(GRichTextField)),
        _GT(typeof(GTextInput)),
        _GT(typeof(GComponent)),
        _GT(typeof(GList)),
        _GT(typeof(GRoot)),
        _GT(typeof(GLabel)),
        _GT(typeof(GButton)),
        _GT(typeof(GComboBox)),
        _GT(typeof(GProgressBar)),
        _GT(typeof(GSlider)),
        _GT(typeof(PopupMenu)),
        _GT(typeof(ScrollPane)),
        _GT(typeof(Transition)),
        _GT(typeof(UIPackage)),
        _GT(typeof(Window)),
        _GT(typeof(GObjectPool)),
        _GT(typeof(Relations)),
        _GT(typeof(RelationType)),
        _GT(typeof(Timers)),
        _GT(typeof(GTween)),
        _GT(typeof(GTweener)),
        _GT(typeof(EaseType)),
        _GT(typeof(TweenValue)),
        _GT(typeof(UIObjectFactory)),
        _GT(typeof(GoWrapper)),
        _GT(typeof(LongPressGesture)),
        _GT(typeof(DragDropManager)),

        //自定义
        _GT(typeof(Common)),
        _GT(typeof(LocalFrameSynServer)),
        _GT(typeof(GTime)),
        _GT(typeof(LuaTimer)),
        _GT(typeof(Network)),
        _GT(typeof(AudioManager)),
        _GT(typeof(ConfigManager)),
        _GT(typeof(LuaEventTriggerListener)),
        _GT(typeof(LuaExtend)),        
        _GT(typeof(MyPlayer)),
        _GT(typeof(DragAndDropManager)),
        
        //toLua
        _GT(typeof(LuaInjectionStation)),
        _GT(typeof(InjectType)),
        _GT(typeof(Debugger)),

        //unity
        _GT(typeof(Behaviour)),
        _GT(typeof(MonoBehaviour)),
        //_GT(typeof(UnityEngine.Object)),
        _GT(typeof(GameObject)),
        _GT(typeof(Application)),
        _GT(typeof(Input)),
        _GT(typeof(KeyCode)),
        _GT(typeof(Resources)),
        _GT(typeof(Transform)),
    };

    public static List<Type> dynamicList = new List<Type>()
    {

    };

    //重载函数，相同参数个数，相同位置out参数匹配出问题时, 需要强制匹配解决
    //使用方法参见例子14
    public static List<Type> outList = new List<Type>()
    {

    };

    //ngui优化，下面的类没有派生类，可以作为sealed class
    public static List<Type> sealedList = new List<Type>()
    {

    };

    public static BindType _GT(Type t)
    {
        return new BindType(t);
    }

    public static DelegateType _DT(Type t)
    {
        return new DelegateType(t);
    }

    [MenuItem("Lua/Attach Profiler", false, 151)]
    static void AttachProfiler()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("警告", "请在运行时执行此功能", "确定");
            return;
        }

        LuaClient.Instance.AttachProfiler();
    }

    [MenuItem("Lua/Detach Profiler", false, 152)]
    static void DetachProfiler()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        LuaClient.Instance.DetachProfiler();
    }
}