using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KTSimpleSingleton<T> where T : new()
{
    static T _instance;
    public static T instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            _instance = new T();
            return _instance;
        }
    }
}
