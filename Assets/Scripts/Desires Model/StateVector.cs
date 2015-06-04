using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class StateVector : MonoBehaviour
{
    private StateManager stateManager;
    private State[] states;

    void Awake()
    {
        GameObject stateManagerObject = GameObject.FindGameObjectWithTag("StateManager");
        stateManager = stateManagerObject.GetComponent<StateManager>();
    }

    void Start()
    {
        states = stateManager.getStates();
    }

    void Update()
    {
        
    }

    void LateUpdate()
    {
    }

    public State[] getStates()
    {
        return states;
    }

   

    
 
}