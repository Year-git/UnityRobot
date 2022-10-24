using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 碰撞开始
     void OnCollisionEnter(Collision collision) {
         // 销毁当前游戏物体
         Destroy(this.gameObject);
    }
 
     // 碰撞结束
     void OnCollisionExit(Collision collision) {
 
     }
 
     // 碰撞持续中
     void OnCollisionStay(Collision collision) {
 
     }
     void TransRelieve(GameObject gameobject)
    {
        if (gameobject.transform.childCount != 0)
        {
            for (int i = 0; i < gameobject.transform.childCount; i++)
            {
                TransRelieve(gameobject.transform.GetChild(i).gameObject);//递归，优先分离子级的子级
                gameobject.transform.GetChild(i).DetachChildren();
            }
        }
    }
}
