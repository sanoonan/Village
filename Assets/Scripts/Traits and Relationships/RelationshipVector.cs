using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum RelationshipTrait
{
    Friendship,      //platonic
    Attraction,    //romantic
};


public class RelationshipVector
{
    private const int _minValue = -10;
    private const int _maxValue = 10;
    private const int _neutralValue = 0;

    private int _numTraits;
    private float[] _traits;

    private const float _minFamiliarity = 0.0f;
    private const float _maxFamiliarity = 10.0f;
    private const float _startingFamiliarity = 1.0f;
    private float _familiarity;

    public RelationshipVector()
    {
        int[] valueEnums = (int[])System.Enum.GetValues(typeof(RelationshipTrait));
        _numTraits = valueEnums.Length;

        _traits = new float[_numTraits];

        for (int i = 0; i < _numTraits; i++)
            _traits[i] = _neutralValue;
            
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
        for( int i=0; i<_numTraits; i++ )
        {
            _traits[i] = Random.Range( _minValue, _maxValue );
        }
    }
    private void RandomiseFamiliarity()
    {
        _familiarity = Random.Range( _minFamiliarity, _maxFamiliarity );
    }

    public float GetTraitValue( RelationshipTrait trait )
    {
        return _traits[ (int) trait ];
    }
    public float GetFamiliarity()
    {
        return _familiarity;
    }

    

}