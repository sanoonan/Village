using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum Restriction
{
    Positive = 0, 
    Negative,
    NonPositive,
    NonNegative,
    NoEffect,
}



public class StateVectorModifer : MonoBehaviour
{
    private Dictionary<State, Dictionary<Task, Restriction>> _restrictedStateTasks;
    private int _numTasks;

    private List<float[]> _stateModificationVectors;
    private int _numStates;

    private const float _maxModMagnitude = 1.0f;
    private const float _minModMagnitudeForForcedPosNeg = 0.5f;

    private const float _minValue = -10.0f;
    private const float _maxValue = 10.0f;
    private const float _minThresholdValue = 0.0f;


    void Awake()
    {
        _numTasks = System.Enum.GetNames( typeof( Task ) ).Length;
        _numStates = System.Enum.GetNames( typeof( State ) ).Length;

        InitRestrictedStateTasks();
        InitStateModificationVectors();
    }

    public float[] GetStateModificationVector( Task task )
    {
        return _stateModificationVectors[( int )task];
    }

    private void InitStateModificationVectors()
    {
        _stateModificationVectors = new List<float[]>();

        for (int i = 0; i < _numTasks; i++)
        {
            Task currTask = (Task)i;
            float[] currSmv = new float[_numStates];

            if ( currTask == Task.Idle )
            {
                for ( int j = 0; j < _numStates; j++ )
                    currSmv[j] = 0.0f;

                _stateModificationVectors.Add( currSmv );
                continue;
            }

            for ( int j = 0; j < _numStates; j++ )
            {
                float modValue = Random.Range( 0.0f, _maxModMagnitude );
                if ( Random.value > 0.5f )
                    modValue = 0 - modValue;

                Dictionary<Task, Restriction> stateRestrictions;
                bool isStateRestricted = _restrictedStateTasks.TryGetValue( ( State )j, out stateRestrictions );

                if ( isStateRestricted )
                {
                    Restriction restriction;
                    bool isTaskRestricted = stateRestrictions.TryGetValue( ( Task )i, out restriction );

                    if ( isTaskRestricted )
                    {
                        if ( restriction == Restriction.NoEffect )
                        {
                            modValue = 0.0f;
                        }

                        if ( ( restriction == Restriction.Positive ) || ( restriction == Restriction.Negative ) )
                        {
                            modValue = Random.Range( _minModMagnitudeForForcedPosNeg, _maxModMagnitude );
                        }

                        if ( ( restriction == Restriction.Positive ) || ( restriction == Restriction.NonNegative ) )
                        {
                            if ( modValue < 0.0f )
                                modValue = 0 - modValue;
                        }
                        if ( ( restriction == Restriction.Negative ) || ( restriction == Restriction.NonPositive ) )
                        {
                            if ( modValue > 0.0f )
                                modValue = 0 - modValue;
                        }
                    }
                }

                currSmv[j] = modValue;
            }

            _stateModificationVectors.Add( currSmv );
        }
    }

    private void InitRestrictedStateTasks()
    {
        _restrictedStateTasks = new Dictionary<State, Dictionary<Task, Restriction>>();

        //ENERGY
        Dictionary<Task, Restriction> energyTasks = new Dictionary<Task, Restriction>();
        for( int i=0; i<_numTasks; i++ )
        {
            energyTasks.Add( (Task)i, Restriction.NonPositive );
        }
        AddToStateRestrictions( energyTasks, Task.Sleep, Restriction.Positive );
        AddToStateRestrictions( energyTasks, Task.Eat, Restriction.Positive );

        _restrictedStateTasks.Add( State.Energy, energyTasks );


        //SATEDNESS
        Dictionary<Task, Restriction> satednessTasks = new Dictionary<Task, Restriction>();
        for( int i=0; i<_numTasks; i++ )
        {
            satednessTasks.Add( (Task)i, Restriction.NonPositive );
        }
        AddToStateRestrictions( satednessTasks, Task.Eat, Restriction.Positive );

        _restrictedStateTasks.Add( State.Satedness, satednessTasks );
    }

    private void AddToStateRestrictions( Dictionary<Task, Restriction> dictionary, Task task, Restriction restriction )
    {
        Restriction oldRestriction;
        bool hasExistingRestriction = dictionary.TryGetValue( task, out oldRestriction );
        if ( hasExistingRestriction )
        {
            dictionary.Remove( task );
        }

        dictionary.Add( task, restriction );
    }

    public float[] SetupThresholdVector()
    {
        float[] thresholdVector = new float[_numStates];

        for ( int i = 0; i < _numStates; i++ )
        {
            thresholdVector[i] = ( int )Random.Range( _minThresholdValue, _maxValue );
        }
        return thresholdVector;
    }


}
