using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class StateManager : MonoBehaviour
{

    private State[] states;

    void Awake()
    {
        State[] allStates = GetComponentsInChildren<State>();
        int numAllStates = allStates.Length;

        List<State> includedStates = new List<State>();

        for (int i = 0; i < numAllStates; i++)
            if (allStates[i].isIncluded)
                includedStates.Add(allStates[i]);

        int numIncludedStates = includedStates.Count;

        states = new State[numIncludedStates];

        for(int i=0; i<numIncludedStates; i++)
            states[i] = includedStates[i];
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

    public State[] getStates()
    {
        return states;
    }





}