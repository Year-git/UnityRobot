using System;
using System.Net.NetworkInformation;
using System.Xml;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Cinemachine;

using System.Runtime.InteropServices;  

public static class Common
{
    /// <summary>
    /// 获取平台类型
    /// </summary>
    /// <returns></returns>
    public static string GetOSType()
    {
        return LuaConst.osDir;
    }

    /// <summary>
    /// 获取mac地址
    /// </summary>
    /// <returns></returns>
    public static string GetMacAddress()
    {
        NetworkInterface[] data = NetworkInterface.GetAllNetworkInterfaces();
        if (data.Length > 0)
        {
            return data[0].GetPhysicalAddress().ToString();
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// 创建XML文件
    /// </summary>
    /// <returns></returns>
    public static XmlDocument CreateXML()
    {
        //新建xml对象  
        XmlDocument xml = new XmlDocument();
        //加入声明  
        xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
        return xml;
    }

    public static void AmendPrecision(int retain, Rigidbody rigidbody, WheelCollider[] wheels = null)
    {
        rigidbody.transform.position = new Vector3((float)System.Math.Round(rigidbody.transform.position.x, retain), (float)System.Math.Round(rigidbody.transform.position.y, retain), (float)System.Math.Round(rigidbody.transform.position.z, retain));
        rigidbody.transform.rotation = new Quaternion((float)System.Math.Round(rigidbody.transform.rotation.x, retain), (float)System.Math.Round(rigidbody.transform.rotation.y, retain), (float)System.Math.Round(rigidbody.transform.rotation.z, retain), (float)System.Math.Round(rigidbody.transform.rotation.w, retain));
        rigidbody.velocity = new Vector3((float)System.Math.Round(rigidbody.velocity.x, retain), (float)System.Math.Round(rigidbody.velocity.y, retain), (float)System.Math.Round(rigidbody.velocity.z, retain));
        rigidbody.angularVelocity = new Vector3((float)System.Math.Round(rigidbody.angularVelocity.x, retain), (float)System.Math.Round(rigidbody.angularVelocity.y, retain), (float)System.Math.Round(rigidbody.angularVelocity.z, retain));

        // if (wheels != null)
        // {
        //     foreach (WheelCollider wheel in wheels)
        //     {
        //         wheel.steerAngle = (float)System.Math.Round(wheel.steerAngle, retain);
        //         wheel.wheelDampingRate = (float)System.Math.Round(wheel.wheelDampingRate, retain);
        //         wheel.suspensionDistance = (float)System.Math.Round(wheel.suspensionDistance, retain);
        //     }
        // }
        
        if (wheels != null)
        {
            foreach (WheelCollider wheel in wheels)
            {
                Vector3 vz = new Vector3(0,0,0);
                Quaternion qz = new Quaternion(0,0,0,0);
                wheel.GetWorldPose(out vz, out qz);
            }
        }
    }
    
    
    //JObject转JArray （适用于有序JObject）
    public static JArray JObject2JArray(JObject o){
        JArray a = new JArray();

        foreach (var item in o)
        {
            var val = item.Value;
            if (val.GetType() == typeof(JObject)){
                val = JObject2JArray((JObject) val);
            }
            a.Add(val);
        }
        return a;
    }

    /// <summary>
    /// 拷贝List<object>
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static List<object> CopyObjectList(List<object> list)
    {
        List<object> ret = null;
        if (list != null)
        {
            ret = new List<object>();
            for (int i = 0; i < list.Count; i++)
            {
                ret.Add(list[i]);
            }
        }
        return ret;
    }
    
    /// <summary>
    /// 获取设备ID（临时）
    /// </summary>
    /// <returns></returns>
    public static string GetEquipmentID()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    public static string getMemory(object o) // 获取引用类型的内存地址方法
    {
        GCHandle h = GCHandle.Alloc(o, GCHandleType.Pinned);
        IntPtr addr = h.AddrOfPinnedObject();
        return addr.ToString("X");
    }
}

//--------------------------------------------------------------------------
//本地模拟帧同步

public static class LocalFrameSynServer
{
    private static JArray _nextFramePlayerOperationList;
    public static void Start()
    {
        _nextFramePlayerOperationList = new JArray();
    }

    public static void SynFrameData()
    {
        FrameSynchronManager pManager = FrameSynchronManager.Instance;
        int nGameLogicFrame = pManager.fsData.GameLogicFrame + 1;

        JArray jNew = new JArray()
        {
            new JArray()
            {
                nGameLogicFrame,
                0,
                JArray.Parse(_nextFramePlayerOperationList.ToString())
            }
        };
        _nextFramePlayerOperationList.Clear();
        pManager.SavePlayerOperation(jNew, nGameLogicFrame);
    }

    public static void SendPlayerOperation(FrameSynchronOperationType type, int pPlayerNpcInstId, string info)
    {
        if(_nextFramePlayerOperationList == null){
            return;
        }

        //如果该Npc是非启用和暂停状态，则阻止
        BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(pPlayerNpcInstId);
        if (!pNpc.IsEnableState() || pNpc.IsPauseState())
        {
            return;
        }

        //如果该Npc有Ai行为树控制，则阻止
        if (pNpc.myBehaviacController.aiName != null && pNpc.myBehaviacController.aiName != "")
        {
            return;
        }
        
        JArray oData = new JArray
        {
            type,
            pPlayerNpcInstId,
            info
        };
        _nextFramePlayerOperationList.Add(oData);
    }

    public static void LuaSendPlayerOperation(int nType, int nNpcInstId, string sInfo)
    {
        SendPlayerOperation((FrameSynchronOperationType)nType, nNpcInstId, sInfo);
    }

    /// <summary>
    /// 控制相机
    /// </summary>
    /// <param name="deltaX"></param>
    /// <param name="deltaY"></param>
    public static void LuaSendCameraTouch(float deltaX, float deltaY)
    {
        float multipleX = 0.24f;
        //float multipleY = 0.0007f;
        CinemachineBrain cCb = Camera.main.GetComponent<CinemachineBrain>();
        if(cCb.ActiveVirtualCamera.Name != "battle"){
            return;
        }
        CinemachineFreeLook ActiveVirtualCamera = cCb.ActiveVirtualCamera as CinemachineFreeLook;
        ActiveVirtualCamera.m_XAxis.Value -= deltaX * multipleX * MyPlayer.cameraSensitive;
        //ActiveVirtualCamera.m_YAxis.Value -= deltaY * multipleY;
        
        if(MyPlayer.cameraType == 0){
            ActiveVirtualCamera.m_YAxis.Value = 0;
        }else{
            ActiveVirtualCamera.m_YAxis.Value = 1;
        }
        
        // if(MyPlayer.cameraHeightType == 0){
        //     MyPlayer.player.TargetMonster = null;
        // }
    }


}