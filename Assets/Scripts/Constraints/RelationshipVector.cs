using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum RelationshipValue
{
    Friendship,      //platonic
    Attraction,    //romantic
};


public class RelationshipVector
{
    private const int _minValue = -10;
    private const int _maxValue = 10;
    private const int _neutralValue = 0;

    private int _numValues;
    private float[] _values;

    private const float _minFamiliarity = 0.0f;
    private const float _maxFamiliarity = 10.0f;
    private const float _startingFamiliarity = 1.0f;
    private float _familiarity;

    public RelationshipVector()
    {
        int[] valueEnums = (int[])System.Enum.GetValues(typeof(RelationshipValue));
        _numValues = valueEnums.Length;

        _values = new float[_numValues];

        for (int i = 0; i < _numValues; i++)
            _values[i] = _neutralValue;
            
        _familiarity = _startingFamiliarity;
    }

    public void MakeRandomExistingRelationship()
    {
        RandomiseFamiliarity();
        RandomiseValues();
    }
    public void MakeRandomNewRelationship()
    {
        RandomiseValues();
    }
    
    private void RandomiseValues()
    {
        for( int i=0; i<_numValues; i++ )
        {
            _values[i] = Random.Range( _minValue, _maxValue );
        }
    }
    private void RandomiseFamiliarity()
    {
        _familiarity = Random.Range( _minFamiliarity, _maxFamiliarity );
    }

}