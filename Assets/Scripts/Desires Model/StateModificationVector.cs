using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class StateModificationVector
{
    private State[] states;
    private int numStates;

    public StateModificationVector()
    {
        GameObject stateManagerObject = GameObject.FindGameObjectWithTag("StateManager");
        StateManager stateManager = stateManagerObject.GetComponent<StateManager>();
        states = stateManager.getStates();
        numStates = states.Length;
    }

    public StateModificationVector(StateVector stateVector)
    {
        states = stateVector.getStates();
        numStates = states.Length;
    }




    public void setStateModVecValues(float[] values)
    {
        if (values.Length != numStates)
            Debug.LogError("Number of state mod vec values don't match number of states");

        for (int i = 0; i < numStates; i++)
            states[i].setValue(values[i]);    
    }


    public void setToZero()
    {
        for (int i = 0; i < numStates; i++)
            states[i].setToZero();
    }

   
}