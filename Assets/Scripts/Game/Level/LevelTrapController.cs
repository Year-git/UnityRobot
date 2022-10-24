using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTrapController : MonoBehaviour
{
    // #if UNITY_EDITOR
    //     [HeaderAttribute("编辑模式_显示线框")]
    //     public bool gizmosShowLine = false;
    //     [HeaderAttribute("编辑模式_显示实体")]
    //     public bool gizmosShowEntity = false;

    //     private void OnDrawGizmos()
    //     {
    //         Gizmos.color = Color.red;

    //         Vector3 pPositon = gameObject.transform.position;
    //         Vector3 pScale = gameObject.transform.localScale;
    //         Mesh pMesh;

    //         MeshFilter pMeshFilter = gameObject.GetComponent<MeshFilter>();
    //         BoxCollider pBoxCollider = GetComponent<BoxCollider>();
    //         SphereCollider pSphereCollider = GetComponent<SphereCollider>();
    //         MeshCollider pMeshCollider = GetComponent<MeshCollider>();

    //         if (pBoxCollider != null)
    //         {
    //             pMesh = pMeshFilter.sharedMesh;
    //             pPositon = pPositon + pBoxCollider.center;
    //             pScale.x = pScale.x * pBoxCollider.size.x;
    //             pScale.y = pScale.y * pBoxCollider.size.y;
    //             pScale.z = pScale.z * pBoxCollider.size.z;
    //         }
    //         else if (pSphereCollider != null)
    //         {
    //             pMesh = pMeshFilter.sharedMesh;

    //             pPositon = pPositon + pSphereCollider.center;

    //             float nMaxScale = Mathf.Max(pScale.x, Mathf.Max(pScale.y, pScale.z));
    //             float nRadius = pSphereCollider.radius / 0.5f;
    //             if (nRadius < 0){ nRadius = 0; }

    //             pScale = new Vector3(nMaxScale * nRadius, nMaxScale * nRadius, nMaxScale * nRadius);
    //         }
    //         else if (pMeshCollider != null)
    //         {
    //             pMesh = pMeshCollider.sharedMesh;
    //         }
    //         else
    //         {
    //             Debug.LogError("LevelTrapScript->OnDrawGizmos->This Collider Is Not Support!");
    //             return;
    //         }

    //         if (gizmosShowLine)
    //         {
    //             Gizmos.DrawWireMesh(pMesh, -1, pPositon, gameObject.transform.rotation, pScale);
    //         }
    //         if (gizmosShowEntity)
    //         {
    //             Gizmos.DrawMesh(pMesh, -1, pPositon, gameObject.transform.rotation, pScale);
    //         }
    //     }
    // #endif

    [HeaderAttribute("是否开启")]
    public bool isEnable;
    [HeaderAttribute("行为树名称")]
    public string aiTreeName;

    [HideInInspector]
    public TrapBehaviacController myBehaviacController;

    [HideInInspector]
    public int myTrapIdx = -1;

    /// <summary>
    /// 进入到trap的npc
    /// </summary>
    /// <typeparam name="int">npc实例ID</typeparam>
    /// <returns></returns>
    public HashSet<int> joinTrapNpcController {get; private set;} = new HashSet<int>();

    /// <summary>
    /// 调用Trap的持续触发的队列
    /// </summary>
    /// <typeparam name="int"></typeparam>
    /// <returns></returns>
    private List<int> _joinTrapNpcStayList = new List<int>();

    private int myTrapIntName;

    public void Init()
    {
        // 初始化Trap的Ai行为树
        this.myBehaviacController = new TrapBehaviacController(this.gameObject, this);

        this.myTrapIntName = int.Parse(this.gameObject.name);

        // 给子物体中所有带有碰撞盒并且isTrigger为True的物体添加触发调用脚本
        foreach (var pChildTransform in this.transform.GetComponentsInChildren<Transform>(true))
        {
            GameObject pChildGameObj = pChildTransform.gameObject;
            Collider pCollider = pChildGameObj.GetComponent<Collider>();
            if (pCollider != null && pCollider.isTrigger == true)
            {
                LevelTrapScript pScript = pChildGameObj.AddComponent<LevelTrapScript>();
                pScript.myController = this;
            }
        }

        // 设置Trap是否开启
        if (this.isEnable)
        {
            this.SetEnableState(true);
        }
        else
        {
            this.SetEnableState(false);
        }
    }

    private bool _enableState = true;
    public bool IsEnableState() { return this._enableState; }
    public void SetEnableState(bool bEnableState)
    {
        if (this.IsEnableState() == bEnableState)
        {
            return;
        }

        this._enableState = bEnableState;
        this.gameObject.SetActive(bEnableState);

        // 清理Trap持续调用队列
        this.joinTrapNpcController.Clear();
        this._joinTrapNpcStayList.Clear();
    }

    public void OnQueueTrapFrameSynLogicUpdate()
    {
        // 排队刷帧
        if(this._joinTrapNpcStayList.Count > 0){
            // 是否循环查找
            for(int i = 0; i<this._joinTrapNpcStayList.Count; i++ )
            {
                int nNpcInstID = this._joinTrapNpcStayList[0];
                this._joinTrapNpcStayList.RemoveAt(0);
                if (!this.joinTrapNpcController.Contains(nNpcInstID)){
                    continue;
                }

                BaseNpc pNpc = MapManager.Instance.baseMap.GetNpc(nNpcInstID);
                if (pNpc == null || pNpc.IsEnableState() == false)
                {
                    this.joinTrapNpcController.Remove(nNpcInstID);
                    this.myBehaviacController.DispatchGameEvent(BehaviacGameEvent.Trap_OnExit, this.myTrapIntName, nNpcInstID);
                    continue;
                }

                this._joinTrapNpcStayList.Add(nNpcInstID);
                this.myBehaviacController.DispatchGameEvent(BehaviacGameEvent.Trap_OnStay, this.myTrapIntName, nNpcInstID);
                break;
            }
        }
    }

    public void OnTrapCall(BehaviacGameEvent eAiEvent, string sTrapName, BaseNpc pNpc, Collider pCollider)
    {
        if (this.myBehaviacController == null)
        {
            return;
        }

        int nNpcInstID = pNpc.InstId;
        if (eAiEvent == BehaviacGameEvent.Trap_OnEnter)
        {
            if (!this.joinTrapNpcController.Contains(nNpcInstID))
            {
                this.joinTrapNpcController.Add(nNpcInstID);
                this._joinTrapNpcStayList.Add(nNpcInstID);
                this.myBehaviacController.DispatchGameEvent(BehaviacGameEvent.Trap_OnEnter, this.myTrapIntName, nNpcInstID);
            }
        }
        else if(eAiEvent == BehaviacGameEvent.Trap_OnExit)
        {
            if (this.joinTrapNpcController.Contains(nNpcInstID))
            {
                this.joinTrapNpcController.Remove(nNpcInstID);
                this._joinTrapNpcStayList.Remove(nNpcInstID);
                this.myBehaviacController.DispatchGameEvent(BehaviacGameEvent.Trap_OnExit, this.myTrapIntName, nNpcInstID);
            }
        }
    }
}
