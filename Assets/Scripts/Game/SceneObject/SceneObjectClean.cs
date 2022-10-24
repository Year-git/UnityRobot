using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CleanObject{
    public GameObject gameObject;
    public Vector3 startPos;

    public CleanObject(GameObject gObject){
        gameObject = gObject;
        startPos = gObject.transform.position;
    }
}

public class SceneObjectClean : MonoBehaviour
{   
    public Queue<CleanObject> CleanObjectList = new Queue<CleanObject>();
    // Start is called before the first frame update
    void Start()
    {
        Transform[] allChild = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChild)
        {
            if(child.gameObject.GetComponent<Rigidbody>()){
                if(child.gameObject.GetComponent<SceneObjectExplosionScript>() == null){
                    CleanObject cO = new CleanObject(child.gameObject);
                    CleanObjectList.Enqueue(cO);
                }
            }
        }
    }

    private void Update()
    {
        for(int i = 0; i < 5; i++){
            if(CleanObjectList.Count > 0){
                bool bClear = false;
                CleanObject cO = CleanObjectList.Dequeue();

                float distance = (cO.gameObject.transform.position - cO.startPos).magnitude;
                if(distance > 1){
                    bClear = true;
                }

                if(bClear){
                    Destroy(cO.gameObject, 20f);
                }
                else
                {
                    CleanObjectList.Enqueue(cO);
                }
            }
        }
    }
    
}