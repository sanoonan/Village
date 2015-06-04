using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class State : MonoBehaviour
{
    public bool isIncluded;
    public float defaultValue;
    public float satisfactoryValue;

    private float value;

    [HideInInspector]
    public string name;


    void Awake()
    {
        value = defaultValue;
        name = gameObject.transform.name;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    void LateUpdate()
    {
    }

    public void setValue(float val)
    {
        value = val;
    }

    public void setToZero()
    {
        value = 0.0f;
    }





}