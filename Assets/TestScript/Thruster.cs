using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour
{
    public float force = 5000;
    public float CD = 5;
    private float curCD = 0;
    public Rigidbody _rigidbody;

    public ParticleSystem[] juqis;
    public ParticleSystem[] baoqis;
    public bool IsCD()
    {
        return curCD > 0;
    }

    void Start()
    {

    }

    void Update()
    {
        if (curCD > 0)
        {
            curCD -= Time.deltaTime;
            curCD = curCD < 0 ? 0 : curCD;
        }
        if (Input.GetKey(KeyCode.V) && _rigidbody && !IsCD())
        {
            curCD = CD;
            // 播放特效
            if (juqis != null)
            {
                foreach (ParticleSystem effect in juqis)
                {
                    effect.Play();
                }
            }

            if (baoqis != null)
            {
                foreach (ParticleSystem effect in baoqis)
                {
                    effect.Play();
                }
            }
            _rigidbody.AddForceAtPosition(transform.forward * force, transform.position, ForceMode.Impulse);
        }
    }
}
