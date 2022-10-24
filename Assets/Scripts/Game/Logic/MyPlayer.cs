using Framework;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Cinemachine;

public static class MyPlayer
{
    /// <summary>
    /// 主玩家对象
    /// </summary>
    public static PlayerNpc player;

    /// <summary>
    /// 主玩家ID
    /// </summary>
    public static string roleID { get; private set; } = "";
    
    /// <summary>
    /// 主玩家昵称
    /// </summary>
    public static string myName { get; private set; } = "";

    /// <summary>
    /// 主玩家已通关的关卡Id
    /// </summary>
    public static int passGameLevelId = -1;

    /// <summary>
    ///  玩家超控方式
    /// </summary>
    public static int cameraType = 1;

    /// <summary>
    ///  镜头灵敏度
    /// </summary>
    public static float cameraSensitive = 0.5f;

    /// <summary>
    ///  声音
    /// </summary>
    private static float Vocality = 0.5f;

    /// <summary>
    /// ??????
    /// </summary>
    public static Vector3 offset;


    public static void SetVocality( float vocality){
        Vocality = vocality;
        
        CinemachineBrain cCb = Camera.main.GetComponent<CinemachineBrain>();
        if(cCb.ActiveVirtualCamera.Name != "battle"){
            return;
        }
        CinemachineFreeLook ActiveVirtualCamera = cCb.ActiveVirtualCamera as CinemachineFreeLook;
        ActiveVirtualCamera.m_Orbits[0].m_Height = vocality * 100;
        ActiveVirtualCamera.m_Orbits[0].m_Radius = 60;
    }

    /// <summary>
    ///  背景音量
    /// </summary>
    private static float BGM = 0.5f;
    public static void SetBGM( float bgm){
        BGM = bgm;
        
        CinemachineBrain cCb = Camera.main.GetComponent<CinemachineBrain>();
        if(cCb.ActiveVirtualCamera.Name != "battle"){
            return;
        }
        CinemachineFreeLook ActiveVirtualCamera = cCb.ActiveVirtualCamera as CinemachineFreeLook;
        ActiveVirtualCamera.m_Orbits[0].m_Radius = bgm * 100;
    }

    public static int playerInstId
    {
        get 
        {
            if (player == null)
            {
                return 0;
            }
            return player.InstId;
        }
    }
    
    public static void Start()
    {
        GEvent.RegistEvent(GacEvent.Update, Update);
        GEvent.RegistEvent(GacEvent.LateUpdate, LateUpdate);
        GEvent.RegistEvent(GacEvent.FixedUpdate, FixedUpdate);
    }

    public static void OnDestroy()
    {
        GEvent.RemoveEvent(GacEvent.Update, Update);
        GEvent.RemoveEvent(GacEvent.LateUpdate, LateUpdate);
        GEvent.RemoveEvent(GacEvent.FixedUpdate, FixedUpdate);
    }

    public static void Update()
    {
        CameraFollow();
    }

    public static void LateUpdate()
    {

    }

    public static void FixedUpdate()
    {

    }

    private static float cameraTargetAngle = 0;

    /// <summary>
    /// 相机跟随
    /// </summary>
    public static void CameraFollow()
    {
        // if(cameraHeightType != 0){
        //     return;
        // }
        
        // if (Camera.main == null)
        // {
        //     return;
        // }
        // if (player == null)
        // {
        //     return;
        // }

        // if (MyPlayer.player.TargetMonster == null)
        // {
        //     return;
        // }
        
        // Vector3 dis = MyPlayer.player.TargetMonster.gameObject.transform.position - Camera.main.gameObject.transform.position;
        // float angle = Vector3.Angle(Vector3.forward, dis);
        // Vector3 c = Vector3.Cross(Vector3.forward, dis.normalized);
        // if (c.y < 0)
        // {
        //     angle = 360 - angle;
        // }
        
        // CinemachineBrain cCb = Camera.main.GetComponent<CinemachineBrain>();
        // if(cCb.ActiveVirtualCamera.Name != "battle"){
        //     return;
        // }
        // CinemachineFreeLook ActiveVirtualCamera = cCb.ActiveVirtualCamera as CinemachineFreeLook;
        // //ActiveVirtualCamera.m_XAxis.Value = angle;
        // cameraTargetAngle = angle;
        // float absVal = Mathf.Abs(ActiveVirtualCamera.m_XAxis.Value - cameraTargetAngle);
        // float cmaeraSpeech = absVal * absVal * Time.deltaTime;
        // cmaeraSpeech = Mathf.Max(cmaeraSpeech, 0.1f);
        // cmaeraSpeech = Mathf.Min(cmaeraSpeech, 3f);
        // cmaeraSpeech = Mathf.Min(cmaeraSpeech, absVal);
        
        // float xValue = ActiveVirtualCamera.m_XAxis.Value;
        // if(xValue != cameraTargetAngle){
        //     if( absVal > 180){
        //         if(xValue > cameraTargetAngle){
        //             xValue += cmaeraSpeech;
        //         }else{
        //             xValue -= cmaeraSpeech;
        //         }
        //     }else{
        //         if(xValue > cameraTargetAngle){
        //             xValue -= cmaeraSpeech;
        //         }else{
        //             xValue += cmaeraSpeech;
        //         }
        //     }
        // }
        // if(xValue > 360){
        //     xValue -= 360;
        // }else if(xValue < 0){
        //     xValue += 360;
        // }
        //ActiveVirtualCamera.m_XAxis.Value = xValue;
    }

    ///改装界面的摄像机拉近
    public static void RefitCameraShow(bool show)
    {
        if(player!=null)
        {
            player.SetRefitCameraShow(show);
        }
    }
    
    /// 接收玩家上线同步数据
    /// <summary>
    /// </summary>
    public static void SyncPlayerInfoMsg(JArray data)
    {
        roleID = (string)data[0];
        myName = (string)data[1];
    }

    public static void UpdateFrameSynRender(Fix64 interpolation)
    {
        KeyCodMove();
    }

    public static void KeyCodMove()
    {
        CollectInputCommand();
    }

    //收集按键输入
    public static void CollectInputCommand()
    {
        if (player == null)
        {
            return;
        }

        JObject joInput = new JObject();

        if (Input.GetKeyDown(KeyCode.W))
        {
            joInput["W"] = 1;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            joInput["W"] = 0;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            joInput["S"] = 1;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            joInput["S"] = 0;
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            joInput["A"] = 1;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            joInput["A"] = 0;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            joInput["D"] = 1;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            joInput["D"] = 0;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            joInput["Space"] = 1;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            joInput["Space"] = 0;
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            MyPlayer.player.mySkillController.SkillDo(40501);
        }
        // joInput["A"] = 0;
        // joInput["D"] = 0;
        // float cameraRY = Camera.main.transform.eulerAngles.y;
        // int bS = joInput["S"] == null ? 0 : (int)joInput["S"];
        // float carRY = player.gameObject.transform.eulerAngles.y;
        // // 求出相对坐标
        // float relativeRY = carRY - cameraRY;
        // bool bLeft = relativeRY > 0;
        // if (Math.Abs(relativeRY) > 180){
        //     bLeft = !bLeft;
        //     relativeRY = 360 - Math.Abs(relativeRY);
        // }
        // if(Math.Abs(relativeRY) > 15){
        //     if(bLeft){
        //         joInput["A"] = 1;
        //     }else{
        //         joInput["D"] = 1;
        //     }
        // }

        if (joInput.HasValues)
        {
            // FrameSynchronManager.Instance.SendPlayerOperation(FrameSynchronOperationType.Joystic, player.InstId, joInput.ToString());
            LocalFrameSynServer.SendPlayerOperation(FrameSynchronOperationType.Joystic, player.InstId, joInput.ToString());
        }
    }

    public static float GetPlayerSpeed()
    {
        if (player != null)
        {
            float speed =  Mathf.Floor(player.gameObject.transform.InverseTransformDirection(player.myModel.rigidbody.velocity).z);
            return speed;
        }
        return 0;
    }

    public static int cameraHeightType = 2;
    public static void ChangeCameraHight()
    {
        CinemachineBrain cCb = Camera.main.GetComponent<CinemachineBrain>();
        if(cCb.ActiveVirtualCamera.Name != "battle"){
            return;
        }
        CinemachineFreeLook ActiveVirtualCamera = cCb.ActiveVirtualCamera as CinemachineFreeLook;
        cameraHeightType ++;
        if(cameraHeightType > 2){
            cameraHeightType = 0;
        }
        if(cameraHeightType == 0){
            ActiveVirtualCamera.m_Orbits[0].m_Height = 10;
            ActiveVirtualCamera.m_Orbits[0].m_Radius = 20;
        }else if(cameraHeightType == 1){
            ActiveVirtualCamera.m_Orbits[0].m_Height = 30;
            ActiveVirtualCamera.m_Orbits[0].m_Radius = 30;
        }else if(cameraHeightType == 2){
            ActiveVirtualCamera.m_Orbits[0].m_Height = 40;
            ActiveVirtualCamera.m_Orbits[0].m_Radius = 55;
        }
    }

    
}



