using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RobotPartFragment : MapObject
{
    /// <summary>
    /// 模型实例
    /// </summary>
    /// <value></value>
    public Model myModel{ get; private set;}
    private static GameObject _fragmentListGameObj;

    /// <summary>
    /// 配件死亡时可抛出的物体模型名称表
    /// </summary>
    /// <typeparam name="int"></typeparam>
    /// <returns></returns>
    public static List<int> throwOut{ get; private set;} = new List<int>()
    {
        1001,
        1002,
        1003,
        1004,
    };

    public RobotPartFragment(Vector3 pPosition, Vector3 pEulerAngles, Action<RobotPartFragment> fLoaded = null)
    {
        if (_fragmentListGameObj == null)
        {
            _fragmentListGameObj = new GameObject("RobotPartFragmentList");
            _fragmentListGameObj.transform.position = Vector3.zero;
            _fragmentListGameObj.transform.rotation = Quaternion.identity;
        }

        Load(throwOut[SeedRandom.instance.Next(0,throwOut.Count)], pPosition, pEulerAngles, fLoaded);
    }

    /// <summary>
    /// 加载配件模型
    /// </summary>
    private void Load(int nModelCfgId, Vector3 pPosition, Vector3 pEulerAngles, Action<RobotPartFragment> fLoaded = null)
    {
        string sModelName = Model.GetModelName(nModelCfgId);
        myModel = new Model(this, sModelName, pPosition, pEulerAngles, 0f, delegate (Model model)
            {
                myModel = model;
                myModel.transform.parent = _fragmentListGameObj.transform;
                fLoaded?.Invoke(this);
            }
        );
    }

    /// <summary>
    /// 随机抛出
    /// </summary>
    /// <param name="fv3CenterPos"></param>
    public void RandomThrowOut(){
        Vector3 fv3Random;
        fv3Random.x = SeedRandom.instance.Next(-500, 501) / 1000f;
        fv3Random.y = SeedRandom.instance.Next(500, 1001) / 1000f;
        fv3Random.z = SeedRandom.instance.Next(-500, 501) / 1000f;
        fv3Random = fv3Random * SeedRandom.instance.Next(13, 21);
        myModel.rigidbody.AddForce(fv3Random,ForceMode.Impulse);
    }
}
