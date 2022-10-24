using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownText : MonoBehaviour
{
    /// 破碎的物体
    //public GameObject _fraturing,_cube1;
    private List<GameObject> _fraturing, _cube1;
    private Dictionary<string,int> _buildHp;
    private Dictionary<string, GameObject> gameRelation; //场景内的建筑于破碎建筑关联
    private int childNum_wu, childNum_ti;
    private GameObject fangwu, fangti;
    private int _hp = 3;
    private Text _namelabel;
    void Start()
    {      
        this.GetGameObject("jianzu", 4, _cube1);
        this.GetGameObject("buld_house", 2, _fraturing);
        this.SetBuildHp(_cube1, _buildHp, gameRelation);
        int childNum = _fraturing.Count;
        for (int i = 0; i<childNum -1; i++)
        {
            _fraturing[i].SetActive(false);
            this.SetGameObjShow(_fraturing[i], childNum, true);
        }        
    }

    private void GetGameObject(string sName,int childNum,List<GameObject> childList)
    {
        GameObject buildPanrent = GameObject.Find(sName);
        for (int i = 0; i < childNum; i++)
        {
            GameObject OnceParent = buildPanrent.transform.GetChild(i).gameObject;
            int chilidNum_once = OnceParent.transform.childCount;
            this.AddGameObject(chilidNum_once, OnceParent, childList);
        }
    }

    /// <summary>
    /// 初始化 每个建筑物的血量
    /// </summary>
    /// <param name="buildList"></param>
    /// <param name="childList"></param>
    private void SetBuildHp(List<GameObject> buildList, Dictionary<string, int> childList,Dictionary<string ,GameObject> gameRelation)
    {        
        for (int nIndx=0; nIndx< buildList.Count-1; nIndx++)
        {
            int a = _fraturing.Count;
            string ItemName=buildList[nIndx].name;
            childList[ItemName] = 3;
            gameRelation[ItemName] = _fraturing[nIndx];
        }
    }

    private void AddGameObject(int childNum,GameObject gameParent, List<GameObject> childList)
    {
        for(int i =0; i< childNum-1; i++)
        {
            GameObject _child=gameParent.transform.GetChild(i).gameObject;
            childList.Add(_child);            
        }
    }

    /// <summary>
    /// 设置破碎的物体隐藏 且不发生碰撞
    /// </summary>
    /// <param name="build"></param>
    /// <param name="childNum"></param>
    /// <param name="_show"></param>
    private void SetGameObjShow(GameObject build,int childNum,bool _show){
        GameObject _child;
        for(int i = 0 ;i < childNum - 1;i++){
            _child= build.transform.GetChild(i).gameObject;
            _child.GetComponent<MeshCollider>().isTrigger =_show;
        }
    }

    private void ShowGameObject(string  ItemNmae)
    {
        GameObject item=GameObject.Find(ItemNmae);
        GameObject _buildCube=gameRelation[item.name];
        item.SetActive(false);
        int itemNum = _buildCube.transform.childCount;
        this.SetGameObjShow(_buildCube, itemNum, false);
    }

    void OnCollisionEnter(Collision collision){
        string sName= collision.gameObject.name;
        int _hp=_buildHp[sName]--;
        if (_hp<=0)
        {
            ShowGameObject(sName);
        }
    }
}
