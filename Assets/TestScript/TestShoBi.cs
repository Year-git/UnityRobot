using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShoBi : MonoBehaviour
{

    [HeaderAttribute("完整的预制体")] public GameObject GoodObject;
    [HeaderAttribute("破碎的预制体")] public GameObject DamageObject;
    [HeaderAttribute("破碎时间")] public float Times = 3;
    [HeaderAttribute("抛出延迟时间")] public float delayTimes = 0.5f;

    private float flagTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        flagTime = 0;
        GoodObject.SetActive(true);
        DamageObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        flagTime += Time.deltaTime;
        if(flagTime >= Times && GoodObject.activeSelf){
            GoodObject.SetActive(false);
            DamageObject.SetActive(true);
        }

         if((flagTime >= Times + delayTimes) && (DamageObject.transform.parent != null)){
            DamageObject.transform.SetParent(null);
        }
    }
}
