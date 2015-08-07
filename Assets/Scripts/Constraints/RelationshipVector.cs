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
    private const float _minValue = 0.0f;
    private const float _maxValue = 10.0f;
    private const float _startingFamiliarity = 1.0f;
    private const float _neutralValue = 5.0f;

    private int _numValues;
    private float[] _values;
    private float _familiarity;

    public RelationshipVector()
    {
        int[] valueEnums = (int[])System.Enum.GetValues(typeof(RelationshipValue));
        numValues = valueEnums.Length;

        values = new float[numValues];

        for (int i = 0; i < numValues; i++)
            values[i] = _neutralValue;
            
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
        _familiarity = Random.Range( _minValue, _maxValue );
    }

}