using System.Collections;
using UnityEngine;

/// <summary>
/// 协程管理类、主要给没有继承MonoBehaviour的类使用
/// </summary>
public class CoroutineManager
{
    /// <summary>
    /// 开启协程
    /// </summary>
    /// <param name="routine"></param>
    /// <returns></returns>
    public static Coroutine StartCoroutine(IEnumerator routine)
    {
        return Main.Instance.StartCoroutine(routine);
    }

    /// <summary>
    /// 开启协程
    /// </summary>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static Coroutine StartCoroutine(string methodName)
    {
        return Main.Instance.StartCoroutine(methodName);
    }

    /// <summary>
    /// 停止协程
    /// </summary>
    /// <param name="routine"></param>
    public static void StopCoroutine(Coroutine routine)
    {
        Main.Instance.StopCoroutine(routine);
    }

    /// <summary>
    /// 停止协程
    /// </summary>
    /// <param name="routine"></param>
    public static void StopCoroutine(IEnumerator routine)
    {
        Main.Instance.StopCoroutine(routine);
    }

    /// <summary>
    /// 停止协程
    /// </summary>
    /// <param name="methodName"></param>
    public static void StopCoroutine(string methodName)
    {
        Main.Instance.StopCoroutine(methodName);
    }

    /// <summary>
    /// 停止所有协程
    /// </summary>
    public static void StopAllCoroutines()
    {
        Main.Instance.StopAllCoroutines();
    }
}