using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
   
    [SerializeField]
    private float baseValue;

    public void UpdateValue(float value)
    {
        baseValue = value;
    }

    public void AddToValue(float value)
    {
        baseValue += value;
    }

    public float GetValue()
    {
        return baseValue;
    }
}
