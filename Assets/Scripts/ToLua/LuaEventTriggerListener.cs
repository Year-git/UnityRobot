using UnityEngine;
using UnityEngine.UI;
using LuaInterface;

public static class LuaEventTriggerListener
{
    public static void OnClick(GameObject obj, LuaFunction luaClickFun, LuaTable luaTable = null)
    {
        if (obj == null) return;
        Button btn = obj.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(delegate ()
            {
                CallLuaFunction(luaClickFun, luaTable);
            });
        }
        else
        {
            EventTriggerListener.Get(obj).onClick = delegate
            {
                CallLuaFunction(luaClickFun, luaTable);
            };
        }
    }

    public  static Vector3 MousePosInView(){
        Vector3 mouse =Input.mousePosition;
        Canvas canvas=GameObject.Find("Canvas").GetComponent<Canvas>();
        GameObject View = canvas.gameObject;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(View.transform as RectTransform,mouse
        , canvas.worldCamera, out Vector2 move_end);
        return move_end;
    }

    public static Vector3 MousePosInView(int fingerId)
    {
        Vector3 mouse = Vector3.zero;
#if UNITY_ANDROID && !UNITY_EDITOR
        for( int f = 0; f< UnityEngine.Input.touches.Length; f++ ){
            Touch m = UnityEngine.Input.touches[f];
            if( m.fingerId ==  fingerId){
                    mouse = m.position;
                    break;
            }
        }
#elif UNITY_IOS && !UNITY_EDITOR
        for( int f = 0; f< UnityEngine.Input.touches.Length; f++ ){
            Touch m = UnityEngine.Input.touches[f];
            if( m.fingerId ==  fingerId){
                    mouse = m.position;
                    break;
            }
        }
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
        mouse = Input.mousePosition;
#endif
        // 曾经的代码废弃 不知道写的什么 遇到问题再写吧
        return new Vector2();
    }

    public static void OnDown(GameObject obj, LuaFunction luaClickFun, LuaTable luaTable = null)
    {
        if (obj == null) return;
        EventTriggerListener.Get(obj).onDown = delegate
        {
            CallLuaFunction(luaClickFun, luaTable);
        };
    }

    public static void OnUp(GameObject obj, LuaFunction luaClickFun, LuaTable luaTable = null)
    {
        if (obj == null) return;
        EventTriggerListener.Get(obj).onUp = delegate
        {
            CallLuaFunction(luaClickFun, luaTable);
        };
    }

    public static void OnEnter(GameObject obj, LuaFunction luaClickFun, LuaTable luaTable = null)
    {
        if (obj == null) return;
        EventTriggerListener.Get(obj).onEnter = delegate
        {
            CallLuaFunction(luaClickFun, luaTable);
        };
    }

    public static void OnExit(GameObject obj, LuaFunction luaClickFun, LuaTable luaTable = null)
    {
        if (obj == null) return;
        EventTriggerListener.Get(obj).onExit = delegate
        {
            CallLuaFunction(luaClickFun, luaTable);
        };
    }

    public static void OnSelect(GameObject obj, LuaFunction luaClickFun, LuaTable luaTable = null)
    {
        if (obj == null) return;
        EventTriggerListener.Get(obj).onSelect = delegate
        {
            CallLuaFunction(luaClickFun, luaTable);
        };
    }

    public static void OnUpdateSelect(GameObject obj, LuaFunction luaClickFun, LuaTable luaTable = null)
    {
        if (obj == null) return;
        EventTriggerListener.Get(obj).onUpdateSelect = delegate
        {
            CallLuaFunction(luaClickFun, luaTable);
        };
    }

    private static void CallLuaFunction(LuaFunction luaFun, LuaTable luaTable)
    {
        if (luaFun != null)
        {
            luaFun.BeginPCall();
            if (luaTable != null)
            {
                object[] ret = luaTable.ToArray();
                for (int i = 0; i < ret.Length; i++)
                {
                    luaFun.Push(ret[i]);
                }
            }
            luaFun.PCall();
            luaFun.EndPCall();
        }
    }
}