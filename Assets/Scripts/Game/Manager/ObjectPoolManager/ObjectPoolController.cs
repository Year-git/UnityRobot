using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolController
{

    // 缓存池父节点
    private Transform _transform;

    /// <summary>
    ///  预设缓存记录
    /// </summary>
    private Dictionary<string, GameObject> _prefabs;

    /// <summary>
    /// 场景中实例化对象缓存池
    /// </summary>
    /// <returns></returns>
    private Dictionary<int, Queue<GameObject>> _pools;

    /// <summary>
    ///  实例物体id映射
    /// </summary>
    private Dictionary<int, int> _instanceIDMapping;

    /// <summary>
    /// 异步锁队列
    /// </summary>
    /// <returns></returns>
    private Dictionary<string, List<GetPoolsCallBack>> _assignMappingList;


    /// <summary>
    /// 回调函数
    /// </summary>
    /// <param name="obj"></param>
    public delegate void GetPoolsCallBack(GameObject obj);

    public ObjectPoolController()
    {
    }

    /// <summary>
    /// 初始化对象池
    /// </summary>
    public void Start()
    {
        _prefabs = new Dictionary<string, GameObject>();
        _pools = new Dictionary<int, Queue<GameObject>>();
        _transform = new GameObject("ObjectPool").transform;
        _instanceIDMapping = new Dictionary<int, int>();
        _assignMappingList = new Dictionary<string, List<GetPoolsCallBack>>();
    }

    public void Assign(string path, GetPoolsCallBack callBack)
    {   
        // 判断是否上锁 如果上锁则排队
        if(_assignMappingList.ContainsKey(path)){
            List<GetPoolsCallBack> List = _assignMappingList[path];
            List.Add(callBack);
            return;
        }
        GameObject prefab;
        if (_prefabs.ContainsKey(path))
        {
            prefab = _prefabs[path];
            Assign(prefab, callBack);
        }
        else
        {
            // 添加异步锁
            _assignMappingList.Add(path, new List<GetPoolsCallBack>());
            ResourcesManager.Instance.LoadAsync(path, delegate (GameObject obj)
            {
                prefab = obj;
                _prefabs.Add(path, prefab);
                Assign(prefab, callBack);
                // 打开异步锁 并且放行
                if(_assignMappingList.ContainsKey(path)){
                    List<GetPoolsCallBack> callBackList = _assignMappingList[path];
                    _assignMappingList.Remove(path);
                    for(int i = 0; i < callBackList.Count; i++){
                        Assign(path, callBackList[i]);
                    }
                }
            });
        }
    }

    public void Assign(GameObject prefab, GetPoolsCallBack callBack)
    {
        int instanceID = prefab.GetInstanceID();
        Queue<GameObject> objQueue;
        if (_pools.ContainsKey(instanceID))
        {
            objQueue = _pools[instanceID];
        }
        else
        {
            objQueue = new Queue<GameObject>();
            _pools.Add(instanceID, objQueue);
        }

        GameObject result = null;

#if !GAME_VERSION_ENABLED 
        if(objQueue.Count > 100){
            Debug.LogError(prefab.name + "这个物体创建超过100个了, 请查看是否回收!");
        }
#endif

        for (int i = 0; i < objQueue.Count; i++)
        {
            result = objQueue.Dequeue();
            if (result.gameObject == null)
            { 
#if !GAME_VERSION_ENABLED   
                Debug.LogError(prefab.name + " : 缓存池物品被销毁!");
#endif
                continue;
            }
            break;
        }

        if (result == null)
        {
            result = Object.Instantiate(prefab);
            result.transform.SetParent(_transform);
            _instanceIDMapping.Add(result.GetInstanceID(), instanceID);
        }
        
        result.SetActive(true);
        callBack?.Invoke(result);
    }

    public void Recover(GameObject result)
    {
        int instanceID = result.GetInstanceID();
        
        // 检测不是这里生产的不负责回收
        if(!_instanceIDMapping.ContainsKey(instanceID)){
            return;
        }

        int prefabInstanceID = _instanceIDMapping[instanceID];

        Queue<GameObject> objQueue = _pools[prefabInstanceID];
        objQueue.Enqueue(result);
        result.SetActive(false);
    }

}