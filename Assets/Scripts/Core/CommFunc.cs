using UnityEngine;
using System.Collections.Generic;
public static class CommFunc
{
    /// <summary>
    /// 根据传入数组的权重获取随机索引
    /// </summary>
    /// <param name="pArrayInt"></param>
    /// <returns></returns>
    public static int GetRandomIdxByWeight(List<int> pArrayInt)
    {
        int nCount = pArrayInt.Count;
        if (nCount == 0)
        {
            CommFunc.LogError("CommFunc.GetRandomIdxByWeight->Not Value In pArrayInt");
            return -1;
        }

        if (nCount == 1)
        {
            return 0;
        }

        int nTotal = 0;

        int[] pStageArray = new int[nCount];
        for(int i = 0; i < nCount; i++)
        {
            if (pArrayInt[i] <= 0)
            {
                CommFunc.LogError("CommFunc.GetRandomIdxByWeight->The Value Is 0 In pArrayInt" + "#i = " + i);
                return -1;
            }
            nTotal += pArrayInt[i];
            pStageArray[i] = nTotal;
        }

        int nStage = SeedRandom.instance.Next(1, nTotal + 1);

        for(int i = 0; i < nCount; i++)
        {
            if (nStage <= pStageArray[i])
            {
                return i;
            }
        }
        return -1;
    }

    public static void LogError(object pObject)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.LogError(pObject);
#endif
    }

    public static void Log(object pObject)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Log(pObject);
#endif
    }

    /// <summary>
    /// 获取两点之间的角度
    /// </summary>
    /// <param name="pFrom"></param>
    /// <param name="pTo"></param>
    /// <returns></returns>
    public static float GetPosAngle(Vector3 pFrom, Vector3 pTo)
    {
        Vector3 v3 = Vector3.Cross(pFrom, pTo);
        if (v3.z > 0)
        {
            return Vector3.Angle(pFrom, pTo);
        }
        else
        {
            return 360 - Vector3.Angle(pFrom, pTo);
        }
    }

    /// <summary>
    /// 获取朝向某坐标的旋转角度
    /// </summary>
    /// <param name="originalObj">自身的Transform</param>
    /// <param name="pPos">目标坐标</param>
    /// <returns></returns>
    public static Vector3 GetTurnEuler(Transform originalObj, Vector3 pPos)
    {
        //计算物体在朝向某个向量后的正前方
        Vector3 forwardDir = pPos - originalObj.position;
        //计算朝向这个正前方时的物体四元数值
        Quaternion lookAtRot = Quaternion.LookRotation(forwardDir);
        //把四元数值转换成角度
        Vector3 resultEuler = lookAtRot.eulerAngles;

        return resultEuler;
    }
}
