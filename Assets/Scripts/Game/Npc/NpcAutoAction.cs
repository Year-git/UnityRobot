using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class NpcAutoAction
{   
    private enum NpcActionMove {None, Forward, Backward}
    private enum NpcActionTurn {None, Left, Right}
    private enum NpcActionSkill {None, Skill_0, Skill_1, Skill_2, Skill_3}
    private BaseNpc _myNpc;
    private float _nextActionTime = 0f;
    public bool isRun {get; private set;} = false;

    private NpcAutoAction(){}

    public NpcAutoAction(BaseNpc pNpc)
    {
        _myNpc = pNpc;
    }

    public void Init()
    {
        _nextActionTime = 0f;
        Run();
        RunAutoAction(true);
    }

    public void Run()
    {
        isRun = true;
    }

    public void Stop()
    {
        isRun = false;
    }

    public void UpdateAction()
    {
        if (isRun)
        {
            return;
        }
        if (!isRun)
        {
            return;
        }
        
        float nCurTime = Framework.GTime.RealtimeSinceStartup;
        if (nCurTime < _nextActionTime)
        {
            return;
        }
        _nextActionTime = nCurTime + (SeedRandom.instance.Next(2500,5001) / 1000f);
        RunAutoAction();
    }

    private void RunAutoAction(bool bInit = false)
    {
        JObject joInput = new JObject();

        if (bInit)
        {
            joInput["W"] = 0;
            joInput["S"] = 0;
            joInput["A"] = 0;
            joInput["D"] = 0;
        }
        else
        {
            NpcActionMove eMove = GetRandomActionMove();
            switch(eMove)
            {
                case NpcActionMove.None:
                    joInput["W"] = 0;
                    joInput["S"] = 0;
                    break;
                case NpcActionMove.Forward:
                    joInput["W"] = 1;
                    joInput["S"] = 0;
                    break;
                case NpcActionMove.Backward:
                    joInput["W"] = 0;
                    joInput["S"] = 1;
                    break;
                default:
                    break;
            }

            NpcActionTurn eTurn = GetRandomActionTurn();
            switch(eTurn)
            {
                case NpcActionTurn.None:
                    joInput["A"] = 0;
                    joInput["D"] = 0;
                    break;
                case NpcActionTurn.Left:
                    joInput["A"] = 1;
                    joInput["D"] = 0;
                    break;
                case NpcActionTurn.Right:
                    joInput["A"] = 0;
                    joInput["D"] = 1;
                    break;
                default:
                    break;
            }

            NpcActionSkill eSkill = GetRandomActionSkill();
            switch(eSkill)
            {
                case NpcActionSkill.None:
                    break;
                case NpcActionSkill.Skill_0:
                    joInput["Skill"] = 0;
                    break;
                case NpcActionSkill.Skill_1:
                    joInput["Skill"] = 1;
                    break;
                case NpcActionSkill.Skill_2:
                    joInput["Skill"] = 2;
                    break;
                case NpcActionSkill.Skill_3:
                    joInput["Skill"] = 3;
                    break;
                default:
                    break;
            }
        }

        if (joInput.HasValues)
        {
            // FrameSynchronManager.Instance.SendPlayerOperation(FrameSynchronOperationType.Joystic, _myNpc.InstId, joInput.ToString());
            LocalFrameSynServer.SendPlayerOperation(FrameSynchronOperationType.Joystic, _myNpc.InstId, joInput.ToString());
        }
    }

    private NpcActionMove GetRandomActionMove()
    {
        return (NpcActionMove)SeedRandom.instance.Next(0,Enum.GetNames(typeof(NpcActionMove)).Length);
    }

    private NpcActionTurn GetRandomActionTurn()
    {
        return (NpcActionTurn)SeedRandom.instance.Next(0,Enum.GetNames(typeof(NpcActionTurn)).Length);
    }

    private NpcActionSkill GetRandomActionSkill()
    {
        return (NpcActionSkill)SeedRandom.instance.Next(0,Enum.GetNames(typeof(NpcActionSkill)).Length);
    }
}
