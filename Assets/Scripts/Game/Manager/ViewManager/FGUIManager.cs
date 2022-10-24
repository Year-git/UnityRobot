using UnityEngine;
using FairyGUI;
public static class FGUIManager
{
    // 场景层
    public static Window ScreenLayer;
    // ui 层
    public static Window ViewLayer;
    // top 层
    public static Window TopLayer;
    public static void Start()
    {
        GameObject UICamera = GameObject.Find("Stage Camera");
        Object.DontDestroyOnLoad(UICamera);
        GRoot.inst.SetContentScaleFactor(1920,1080,UIContentScaler.ScreenMatchMode.MatchWidthOrHeight);
        // 初始化配置
        UICamera.GetComponent<UIConfig>().InitConfig();
    }
}