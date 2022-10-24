using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Framework;
public class DrawTool : MonoBehaviour
{

    GameObject go;
    MeshFilter mf;
    MeshRenderer mr;
    Shader shader;

    /// <summary>
    /// 绘制实心长方形
    /// </summary>
    public void ToDrawRectangle(float z, float x1, float x2, float x3, float x4, float y1, float y2, float y3, float y4)
    {
        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(new Vector3(x1, z, y1));
        vertices.Add(new Vector3(x3, z, y3));
        vertices.Add(new Vector3(x4, z, y4));
        vertices.Add(new Vector3(x2, z, y2));

        CreateMesh(vertices);
    }

    //绘制实心圆    
    public void ToDrawCircleSolid(Transform t, Vector3 center, float radius)
    {
        int pointAmount = 50;//点的数目，值越大曲线越平滑   
        float eachAngle = 360f / pointAmount;
        Vector3 forward = t.forward;
        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i <= pointAmount; i++)
        {
            Vector3 pos = Quaternion.Euler(0f, eachAngle * i, 0f) * forward * radius + center;
            vertices.Add(pos);
        }
        CreateMesh(vertices);
    }

    //制作网格
    private GameObject CreateMesh(List<Vector3> vertices)
    {
        int[] triangles;
        Mesh mesh = new Mesh();

        int triangleAmount = vertices.Count - 2;
        triangles = new int[3 * triangleAmount];

        for (int i = 0; i < triangleAmount; i++)
        {
            triangles[3 * 1] = 0;
            triangles[3 * i + 1] = i + 1;
            triangles[3 * i + 2] = i + 2;
        }

        if (go == null)
        {
            go = new GameObject("Rectang");
            go.transform.position = new Vector3(0, 0.1f, 0);
            mf = go.AddComponent<MeshFilter>();
            mr = go.AddComponent<MeshRenderer>();

            shader = Shader.Find("Unlit/Color");
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles;
        mf.mesh = mesh;
        mr.material.shader = shader;
        mr.material.color = Color.red;

        Timer.CreateTimer(1000, false, delegate { GameObject.Destroy(go); });
        return go;
    }


}
