using System;
using System.Net.NetworkInformation;
using System.Xml;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Cinemachine;

using System.Runtime.InteropServices;  

public class CommTimer
{
    private struct TimerData
    {
        public TimerData(string sName, int nTime, bool isLoop, Action<string> pTrigger, int nCurTime)
        {
            Name = sName;
            TriggerTime = nTime;
            IsLoop = isLoop;
            TriggerCall = pTrigger;
            NextTime = nCurTime + nTime;
        }

        public void SetNextTime(int nNextTime)
        {
            NextTime = nNextTime;
        }

        public string Name;
        public int TriggerTime;
        public int NextTime;
        public bool IsLoop;
        public Action<string> TriggerCall;
    }

    private Dictionary<string, TimerData> _timerContainer = new Dictionary<string, TimerData>();
    public void Update(int nCurTime)
    {
        if (this._timerContainer.Count == 0)
        {
            return;
        }

        List<string> removeList = new List<string>();
        
        foreach(var kvPair in this._timerContainer)
        {

            if (nCurTime >= kvPair.Value.NextTime)
            {
                if (kvPair.Value.IsLoop)
                {
                    kvPair.Value.TriggerCall?.Invoke(kvPair.Key);
                    kvPair.Value.SetNextTime(nCurTime + kvPair.Value.TriggerTime);
                }
                else
                {
                    kvPair.Value.TriggerCall?.Invoke(kvPair.Key);
                    removeList.Add(kvPair.Key);
                }
            }
        }

        if (removeList.Count > 0)
        {
            foreach(string sName in removeList)
            {
                this._timerContainer.Remove(sName);
            }
        }
    }

    public void Add(string sName, int nTriggerTime, bool isLoop, Action<string> pTrigger, int nCurTime)
    {
        if (this._timerContainer.ContainsKey(sName))
        {
            return;
        }
        this._timerContainer.Add(sName, new TimerData(sName, nTriggerTime, isLoop, pTrigger, nCurTime));
    }

    public void Del(string sName)
    {
        if (!this._timerContainer.ContainsKey(sName))
        {
            return;
        }
        this._timerContainer.Remove(sName);
    }
}