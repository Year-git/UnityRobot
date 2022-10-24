using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController
{
    private static int AnimationMaxNum = 2;
    public Animation myAnimation {get; private set;}
    private Dictionary<int, string> _animationContainer = new Dictionary<int, string>();
    private int curPlayIdx = -1;
    private int curEndTime = 0;

    private AnimationController(){}
    
    public AnimationController(int nModelCfgId, GameObject pGameObj)
    {
        this.myAnimation = pGameObj.AddComponent<Animation>();
        this.myAnimation.animatePhysics = true;
        this.myAnimation.playAutomatically = false;

        CommAsyncCounter pCounter = new CommAsyncCounter(AnimationMaxNum, delegate()
            {
                this.Play();
            }
        );

        for(int i = 0; i < AnimationMaxNum; i++)
        {
            int nIdx = i;
            string sAnimationName = Model.GetModelAnimation(nModelCfgId, nIdx);
            if (sAnimationName != "")
            {
                ResourcesManager.Instance.LoadAsync(sAnimationName, delegate (AnimationClip pClip)
                    {
                        this._animationContainer.Add(nIdx, sAnimationName);
                        this.myAnimation.AddClip(pClip, sAnimationName);
                        pCounter.Increase();
                    }
                );
            }
            else
            {
                pCounter.Increase();
            }
        }
    }

    public void Update()
    {
        // 如果没有播放任何动作，则返回
        if (this.curPlayIdx == -1)
        {
            return;
        }

        // 如果结束时间是-1，代表播放的是待机动作,则返回
        if (this.curEndTime == -1)
        {
            return;
        }

        if (FrameSynchronManager.Instance.fsData.FrameRunningTime < this.curEndTime)
        {
            return;
        }

        this.Play();
    }
    
    /// <summary>
    /// 动画播放
    /// </summary>
    /// <param name="nAnimationIdx"></param>
    public void Play(int nAnimationIdx = 0)
    {
        if (nAnimationIdx < 0)
        {
            return;
        }

        if (!this._animationContainer.ContainsKey(nAnimationIdx))
        {
            return;
        }

        if (this.curPlayIdx != -1)
        {
            this.myAnimation.Stop();
        }

        // 记录要播放的动作的索引
        this.curPlayIdx = nAnimationIdx;

        // 记录要播放的动作的结束时间
        if (nAnimationIdx == 0)
        {
            this.curEndTime = -1;
        }
        else
        {
            this.curEndTime = FrameSynchronManager.Instance.fsData.FrameRunningTime + ((int)Mathf.Round(this.myAnimation[this._animationContainer[nAnimationIdx]].length * 1000));
        }

        this.myAnimation[this._animationContainer[nAnimationIdx]].speed = 1f;
        this.myAnimation.Play(this._animationContainer[nAnimationIdx]);
    }

    /// <summary>
    /// 动画停止
    /// </summary>
    public void Stop()
    {
        if (this.curPlayIdx == -1 && this.curPlayIdx == 0)
        {
            return;
        }

        this.Play();
    }

    /// <summary>
    /// 动画暂停
    /// </summary>
    public void Pause()
    {
        if (this.curPlayIdx == -1)
        {
            return;
        }

        this.myAnimation[this._animationContainer[curPlayIdx]].speed = 0f;
    }

    /// <summary>
    /// 动画恢复
    /// </summary>
    public void Resume()
    {
        if (this.curPlayIdx == -1)
        {
            return;
        }

        this.myAnimation[this._animationContainer[curPlayIdx]].speed = 1f;
    }
}
