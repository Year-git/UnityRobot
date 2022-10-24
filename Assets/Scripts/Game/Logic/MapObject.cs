using UnityEngine;
using System;
public abstract class MapObject 
{
    private static int _defaultId = 100000000;
    
    public int InstId
    { 
        get; 
        protected set; 
    }

    protected MapObject()
    {
        InstId = (_defaultId += 1);
    }
}
