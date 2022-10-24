using UnityEngine;
using UnityEngine.AI;

public static class RobotFindPath
{
    /// <summary>
    /// 返回寻路集合
    /// </summary>
    /// <param name="startPoint">trasform.position</param>
    /// <param name="targetPoint">寻路目标点</param>
    /// <returns></returns>
    public static Vector3[] FindPath(Vector3 startPoint, Vector3 targetPoint)
    {
        NavMeshPath _navMeshPath = new NavMeshPath();
        float Heighto = 0f;
        if (targetPoint.y >= 2)
        {
            Heighto = targetPoint.y;
            targetPoint.y = 0f;
        }
        bool _isfind = NavMesh.CalculatePath(startPoint, targetPoint, 1, _navMeshPath);
        if (!_isfind)
        {
            return null;
        }
        else
        {
            int _length = _navMeshPath.corners.Length;
            Vector3[] _reslt = new Vector3[_length];
            for (int i = 0; i < _length; i++)
            {
                Vector3 _fixv3 = _navMeshPath.corners[i];
                _fixv3 = _fixv3 + new Vector3(0, Heighto, 0);
                _reslt[i] = _fixv3;
            }
            return _reslt;
        }
    }
}
