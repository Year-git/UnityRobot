using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerNpc : BaseNpc
{
    public string roleId;
    CinemachineVirtualCamera vcam;
    CinemachineFreeLook freelook;
    CinemachineBrain cCb;
    private float blendtime = 2;//转化需要的时间
    private bool camerachange = false;

    /// <summary>
    /// 指引特效
    /// </summary>
    private GameObject Effect_zhiyin;

    public MonsterNpc TargetMonster;

    private GameObject TargetMonEffect;

    public PlayerNpc(int nCfgId) : base(NpcType.PlayerNpc, nCfgId){}

    public override void OnNpcStart()
    {
    }

    public override void FollowLoad(Action<BaseNpc> fLoaded = null)
    {
        if (MapManager.Instance.baseMap.mapType == MapType.Battle)
        {
            ResourcesManager.Instance.LoadAsync("Assets/Res/Prefabs/Effect/Scenes/Effect_zhiyin.prefab",
            delegate (GameObject gobj)
            {
                GameObject obj = UnityEngine.Object.Instantiate<GameObject>(gobj);
                obj.transform.parent = gameObject.transform;
                obj.transform.localPosition =new Vector3(0,0.5f,0);
                Effect_zhiyin = obj;
                fLoaded?.Invoke(this);
            });
        }
        else
        {
            fLoaded?.Invoke(this);
        }
    }
    
    public override void CreateTargetMonEffect()
    { 
        if (MapManager.Instance.baseMap.mapType == MapType.Battle)
        {
            ResourcesManager.Instance.LoadAsync("Assets/Res/Prefabs/Effect/Scenes/Effect_zhuizhong.prefab",
                delegate (GameObject gobj)
                {
                    GameObject obj = UnityEngine.Object.Instantiate<GameObject>(gobj);
                    obj.gameObject.SetActive(false);
                    obj.transform.localPosition = new Vector3(0,0.5f,0);                  
                    TargetMonEffect = obj;
                });  
        }
    }
    public override void OnNpcUpdate()
    {
        base.OnNpcUpdate();
        if (camerachange)
        {
            blendtime -= Time.deltaTime;
            if (blendtime <= 0)
            {
                LuaGEvent.DispatchEventToLua(GacEvent.UICreate);
                camerachange = false;
                blendtime = 2;
            }
        }

        if (Effect_zhiyin != null)
        {
            if (TargetMonster != null)
            {
                if (TargetMonster.isDead)
                {
                    TargetMonster = null;
                }
                else if (!TargetMonster.IsEnableState())
                {
                    TargetMonster = null;
                }
                else if (!TargetMonster.IsCombatState())
                {
                    TargetMonster = null;
                }
            }

            if (TargetMonster == null)
            {
                foreach (BaseNpc pNpc in MapManager.Instance.baseMap.GetNpcContainer().Values)
                {
                    if (pNpc.type == NpcType.MonsterNpc)
                    {
                        if (pNpc.isDead)
                        {
                            continue;
                        }
                        else if (!pNpc.IsEnableState())
                        {
                            continue;
                        }
                        else if (!pNpc.IsCombatState())
                        {
                            continue;
                        }
                        if (TargetMonster == null)
                        {
                            TargetMonster = pNpc as MonsterNpc; 
                            ChangeMonster(TargetMonster.gameObject);                  
                        }
                        else
                        {
                            float distance = (TargetMonster.gameObject.transform.position - this.gameObject.transform.position).sqrMagnitude;
                            float nowDistance = (pNpc.gameObject.transform.position - this.gameObject.transform.position).sqrMagnitude;
                            if (nowDistance < distance)
                            {
                                TargetMonster = pNpc as MonsterNpc;
                                ChangeMonster(TargetMonster.gameObject);
                            }
                        }
                       
                    }
                }
            }

            if (TargetMonster != null)
            {
                Effect_zhiyin.SetActive(true);
                Effect_zhiyin.transform.LookAt(TargetMonster.gameObject.transform, Vector3.up);
            }
            else
            {
                Effect_zhiyin.SetActive(false);
            }
        }
    }

    public void ChangeMonster(GameObject Mon)
    {
        if(TargetMonEffect!=null)
        {
            TargetMonEffect.gameObject.SetActive(true);
            TargetMonEffect.transform.parent=Mon.gameObject.transform;
            TargetMonEffect.transform.localPosition = new Vector3(0,0.5f,0);
        }
    }
    public override void UnPackServer(JArray jNpcInfo)
    {
        roleId = (string)jNpcInfo[3];
        roleName = (string)jNpcInfo[4];
        SetMasterPlayer();
    }

    public void SetMasterPlayer()
    {
        if (MyPlayer.roleID == roleId)
        {
            MyPlayer.player = this;
            gameObject.tag = "MasterPlayer";
            isMaster = true;
            GameObject MoFaS = GameObject.Find("MobileFastShadow");
            if (MoFaS != null)
            {
                MoFaS.GetComponent<taecg.tools.mobileFastShadow.MobileFastShadow>().FollowTarget = gameObject;
            }

            // 初始化相机跟随
            cCb = Camera.main.GetComponent<CinemachineBrain>();
            //cCb.ActiveVirtualCamera.Follow = gameObject.transform;
            GameObject cameraLookAt = new GameObject("CameraLookAt");
            cameraLookAt.transform.parent = gameObject.transform;
            cameraLookAt.transform.localPosition = new Vector3(0, 5f, 0);
            //cCb.ActiveVirtualCamera.LookAt = cameraLookAt.transform;

            freelook = GameObject.Find("Camera/battle").GetComponent<CinemachineFreeLook>();
            freelook.m_LookAt = cameraLookAt.transform;
            freelook.m_Follow = gameObject.transform;
            freelook.m_Priority = 10;

            freelook.m_XAxis.Value = this.myModel.transform.rotation.eulerAngles.y;

            vcam = GameObject.Find("Camera/assemble").GetComponent<CinemachineVirtualCamera>();
            vcam.m_LookAt = cameraLookAt.transform;
            vcam.m_Follow = gameObject.transform;
            vcam.m_Priority = 9;
            vcam.gameObject.transform.localPosition = new Vector3(49.42f, -4.45f, -39f);
            vcam.gameObject.transform.eulerAngles = new Vector3(8.911f, -99f, 5.6f);
            var composer = vcam.AddCinemachineComponent<CinemachineComposer>();
            composer.m_ScreenX = 0.58f; //0.30f;
            composer.m_ScreenY = 0.1f; //0.35f;

        }
    }

    public void SetRefitCameraShow(bool show)
    {
        Rigidbody rigidbody = gameObject.transform.GetComponent<Rigidbody>();
        if (freelook != null)
        {
            freelook.enabled = !show;
        }
        //摄像机拉近时 m_Follow为空 避免旋转模型时摄像机一同旋转
        if (show)
        {
            vcam.m_Follow = gameObject.transform;
            rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        }
        else
        {
            vcam.m_Follow = gameObject.transform;
            rigidbody.constraints = RigidbodyConstraints.None;
        }

        camerachange = show;
    }

    public override void NpcDestroy()
    {
        base.NpcDestroy();
    }

    public override void NpcLayerSet()
    {
        this.NpcLayerNameOccupy();
    }

    public override void NpcLayerClear()
    {
        this.NpcLayerNameRelease();
    }

    // --------------------------------------------------------------
    public bool isPlayerMoving = false;
    public bool isLookAtTargetMonster = false;

    public override bool IsAutoSkill(){
        
        if(!base.IsAutoSkill()){
            return false;
        }

        if (isPlayerMoving)
        {
            return false;
        }

        if (this.TargetMonster == null)
        {
            return false;
        }
        
        if(!isLookAtTargetMonster){
            return false;
        }
        
        return true;
    }
    public override void SetNpcIsMove()
    {
        isPlayerMoving = true;
        isLookAtTargetMonster = false;
    }

    public override void SetNpcIsStand()
    {
        if(isPlayerMoving){
            isPlayerMoving = false;
            // if(MyPlayer.cameraHeightType != 0){
            this.TargetMonster = null;
            // }
        }
    }

    /// <summary>
    /// 检测是否瞄准目标怪物 如有瞄准则准备开炮
    /// </summary>
    public void CheckLookAtTargetMonster(){
        if(this.TargetMonster == null){
            return;
        }
        if(isLookAtTargetMonster){
            return;
        }
        
        if (isPlayerMoving)
        {
            return;
        }
        
        Vector3 dis = this.TargetMonster.gameObject.transform.position - gameObject.transform.position;
        float angle = Vector3.Angle(Vector3.forward, dis);
        Vector3 c = Vector3.Cross(Vector3.forward, dis.normalized);
        if (c.y < 0)
        {
            angle = 360 - angle;
        }
        isLookAtTargetMonster = Mathf.Abs(gameObject.transform.eulerAngles.y - angle) < 7;
    }

    public override void NpcFrameSynLogicUpdate()
    {
        base.NpcFrameSynLogicUpdate();
        CheckLookAtTargetMonster();
    }
    //----------------------------------------------------------------------------------------
    // Npc行为相关代码

    /// <summary>
    /// 获取敌对Npc
    /// </summary>
    /// <returns></returns>
    public override int GetEnemyNpcInView()
    {                               
        //临时代码
        if (this.TargetMonster == null)
        {
            return 0;
        }
        return this.TargetMonster.InstId;
    }
}
