using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineLeg : MonoBehaviour
{
    [SerializeField] Transform _centerOfMass;
    // Start is called before the first frame update
    private Rigidbody _rb;
    void Start()
    {

        _rb = GetComponent<Rigidbody>();
        if (_rb != null && _centerOfMass != null)
        {
            _rb.centerOfMass = _centerOfMass.localPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
