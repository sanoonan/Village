using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum State
{
    Mood,
    Energy,
    Satedness
};


public class StateVector : MonoBehaviour
{
    private const float _minValue = -10.0f;
    private const float _maxValue = 10.0f;

    public bool _allowAllTasks;


    private int _numStates;

    private int _numTasks;
    private List<Task> _tasks;

    private float[] _currentStateVector;

    private bool _hasModVector = false;
    private float[] _currentModVector;

    private float[] _thresholdValues;

    public float _updateFrequency = 15.0f; //updates every 15 minutes
    public float _giveUpTime = 30.0f;

    private float _lastUpdateTime;

    private bool _readyForTaskChange = false;
    private bool _mustTaskChange = false;

    private StateVectorModification _stateVectorModification;

    void Awake()
    {
        _stateVectorModification = gameObject.GetComponent<StateVectorModification>();

        InitCurrentStateVector();
    }

    void Start()
    {
        _thresholdValues = _stateVectorModification.SetupThresholds();
    }

    void Update()
    {
        float timePassedSinceUpdate = GetTimePassed();

        if ( timePassedSinceUpdate >= _updateFrequency )
        {
            if ( ( _hasModVector ) || ( timePassedSinceUpdate >= _giveUpTime ) )
            {
                if ( timePassedSinceUpdate >= _giveUpTime )
                    _mustTaskChange = true;
                else
                    ApplyModification( timePassedSinceUpdate );


                _readyForTaskChange = true;

                _lastUpdateTime = DaylightScript.GetCurrentTime();
            }
        }
    }

    void LateUpdate()
    {
    }


    private void InitCurrentStateVector()
    {
        _numStates = System.Enum.GetNames( typeof( State ) ).Length;
        _currentStateVector = new float[_numStates];

        for ( int i = 0; i < _numStates; i++ )
        {
            _currentStateVector[i] = Random.Range( _minValue, _maxValue );
        }
    }

    public Task GetBestTask( Task currentTask )
    {
        if ( _readyForTaskChange )
        {
            _readyForTaskChange = false;

            float currentDeltaSum = GetCurrentDeltaSum();


            if ( ( currentDeltaSum > 0 ) || ( _mustTaskChange ) )
            {
                Task bestTask = GetBestTaskByDelta( currentDeltaSum, currentTask );
                _mustTaskChange = false;
                return bestTask;
            }
        }
        return currentTask;
    }

    private Task GetBestTaskByDelta( float currentDeltaSum, Task currentTask )
    {
        List<Task> perfectTasks = new List<Task>();
        List<Task> goodTasks = new List<Task>();
        List<float> goodTasksDeltaDiffs = new List<float>();

        for ( int i = 0; i < _numTasks; i++ )
        {
            if( ( _mustTaskChange ) && ( currentTask == (Task)i ) )
                continue;

            float[] taskSmv = _stateVectorModification.GetStateModificationVector( ( Task )i );
            float[] taskModifiedStateVector = GetModifiedStateVector( taskSmv );
            float taskDeltaSum = GetDeltaSum( taskModifiedStateVector );

            if ( taskDeltaSum <= 0.0f )
            {
                perfectTasks.Add( ( Task )i );
            }
            else if ( taskDeltaSum < currentDeltaSum )
            {
                goodTasks.Add( ( Task )i );
                float deltaDiff = currentDeltaSum - taskDeltaSum;
                goodTasksDeltaDiffs.Add( deltaDiff );
            }
        }

        int numPerfectTasks = perfectTasks.Count;
        if ( numPerfectTasks > 0 )
        {
            int randomNum = Random.Range( 0, numPerfectTasks );
            return perfectTasks[randomNum];
        }

        int numGoodTasks = goodTasks.Count;
        float totalDeltaDiffs = 0.0f;
        for ( int i = 0; i < numGoodTasks; i++ )
        {
            totalDeltaDiffs += goodTasksDeltaDiffs[i];
        }
        for( int i=0; i<numGoodTasks; i++ )
        {
            goodTasksDeltaDiffs[i] /= totalDeltaDiffs;
        }

        float currTotal = 0.0f;
        float randomFloat = Random.value;
        for ( int i = 0; i < numGoodTasks; i++ )
        {
            currTotal += goodTasksDeltaDiffs[i];
            if ( randomFloat <= currTotal )
            {
                return goodTasks[i];
            }
        }

        if ( _numTasks <= 1 )
            return currentTask;

        while ( true )
        {
            int randomNum = Random.Range( 0, _numTasks );
            Task randomTask = ( Task )randomNum;

            if ( ( _mustTaskChange ) && ( randomTask == currentTask ) )
                continue;

            return randomTask;
        }
    }

    public void StartModification( Task task )
    {
        float[] modVector = _stateVectorModification.GetStateModificationVector( task );

        _currentModVector = modVector;
        _hasModVector = true;

        _lastUpdateTime = DaylightScript.GetCurrentTime();
    }

    public void StopModification()
    {
        float timePassedSinceUpdate = GetTimePassed();
        ApplyModification( timePassedSinceUpdate );
        _hasModVector = false;
    }

    private void ApplyModification( float timeDiff )
    {
        if ( !_hasModVector )
            return;

        float proportion = timeDiff / _updateFrequency;


        for ( int i = 0; i < _numStates; i++ )
        {
            float propValue = proportion * _currentModVector[i];
            _currentStateVector[i] = ModifyValue( _currentStateVector[i], propValue );
        }
    }


    public void SetupTasks( Routine routine )
    {
        _tasks = new List<Task>();

        if ( _allowAllTasks )
        {
            int numAllTasks = System.Enum.GetNames( typeof( Task ) ).Length;

            for ( int i = 0; i < numAllTasks; i++ )
            {
                Task currentTask = ( Task )i;
                _tasks.Add( currentTask );
            }
        }
        else
        {
            Task[] allTasks = routine.GetTasks();

            int numAllTasks = allTasks.Length;

            for ( int i = 0; i < numAllTasks; i++ )
            {
                if ( !_tasks.Contains( allTasks[i] ) )
                    _tasks.Add( allTasks[i] );
            }
        }

        _numTasks = _tasks.Count;
    }

    private float[] GetModifiedStateVector( float[] modVector )
    {
        float[] newValues = new float[_numStates];

        for (int i = 0; i < _numStates; i++)
            newValues[i] = ModifyValue( _currentStateVector[i], modVector[i] );

        return newValues;
    }

    private float ModifyValue( float original, float change )
    {
        float newValue = original + change;

        if ( newValue < _minValue )
            return _minValue;

        if ( newValue > _maxValue )
            return _maxValue;

        return newValue;
    }



    private float GetCurrentDeltaSum()
    {
        return GetDeltaSum( _currentStateVector );
    }

    private float GetDeltaSum( float[] vector )
    {
        float deltaSum = 0.0f;

        for ( int i = 0; i < _numStates; i++ )
        {
            float delta = GetDelta( i, vector[i] );
            deltaSum += delta;
        }

        return deltaSum;
    }

    private float GetDelta( int stateNum, float value )
    {
        float threshold = _thresholdValues[stateNum];

        if ( value < threshold )
        {
            float delta = threshold - value;
            return delta;
        }
        return 0.0f;
    }



    private float GetTimePassed()
    {
        float currentTime = DaylightScript.GetCurrentTime();
        float diffTime = currentTime - _lastUpdateTime;

        if ( diffTime < 0 )
            diffTime += 24 * 60;

        return diffTime;
    }
}