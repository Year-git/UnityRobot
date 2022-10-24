using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
         Object.DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(!Camera.main){
            this.transform.position = Vector3.zero;
            this.transform.eulerAngles = Vector3.zero;
            return;
        }
        this.transform.position = Camera.main.transform.position;
        this.transform.rotation = Camera.main.transform.rotation;
    }
}
