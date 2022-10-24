using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTrapScript : MonoBehaviour
{
    public LevelTrapController myController;

    private void OnTriggerCall(BehaviacGameEvent eAiEvent, Collider other)
    {
        if (this.myController == null)
        {
            return;
        }

        NpcScript pNpcScript = other.gameObject.GetComponent<NpcScript>();
        if (pNpcScript == null || pNpcScript.myNpc == null)
        {
            return;
        }

        myController.OnTrapCall(eAiEvent, this.name, pNpcScript.myNpc, other);
    }

    public void OnTriggerEnter(Collider other)
    {
        this.OnTriggerCall(BehaviacGameEvent.Trap_OnEnter, other);
    }

    // public void OnTriggerStay(Collider other)
    // {
    //     this.OnTriggerCall(BehaviacGameEvent.Trap_OnStay, other);
    // }

    public void OnTriggerExit(Collider other)
    {
        this.OnTriggerCall(BehaviacGameEvent.Trap_OnExit, other);
    }
}
