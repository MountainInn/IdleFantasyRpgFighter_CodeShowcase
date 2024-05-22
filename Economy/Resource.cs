using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource
{
    public Resource(float value)
    {
        this.value = value;
    }
    public Action<float> onValueEnd;
    public Action<float> onValueChanged;
    public float Value
    {
        get { return value; }
        set
        {
            if (value > 0)
            {
                this.value = value;
                onValueChanged?.Invoke(value);
            }
            else
            {
                this.value = 0;
                onValueChanged?.Invoke(value);
                onValueEnd?.Invoke(value);
            }
        }
    }
    [SerializeField] float value;
}
