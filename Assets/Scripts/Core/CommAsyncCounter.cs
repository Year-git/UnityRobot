using System;
using UnityEngine;

/// <summary>
/// 通用异步计数器
/// </summary>
class CommAsyncCounter
{
    public int totalNum {get; private set;}
    public int recordNum {get; private set;}
    private Action _doneCall;
    private CommAsyncCounter(){}

    /// <summary>
    /// 创建通用异步计数器[nTotal:总数、fDoneCall:达到总数时的回调]
    /// </summary>
    /// <param name="nTotal">总数</param>
    /// <param name="fDoneCall">达到总数时的回调</param>
    public CommAsyncCounter(int nTotal, Action fDoneCall)
    {
        if (nTotal <= 0)
        {
            Debug.LogError("CommAsyncCounter.CommAsyncCounter->nTotal Is Less Than Or Equal to 0!");
        }
        
        totalNum = nTotal;
        recordNum = 0;
        _doneCall = fDoneCall;
    }

    /// <summary>
    /// 增加计数
    /// </summary>
    public void Increase()
    {
        recordNum++;
        if (recordNum == totalNum)
        {
            _doneCall?.Invoke();
        }
        else if (recordNum > totalNum)
        {
            Debug.LogError("CommAsyncCounter -> Use Increase() Count Greater Than Total Count!");
        }
    }
}