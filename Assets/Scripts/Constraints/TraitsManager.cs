using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Traits
{
    Aggressiveness,
    Niceness,
    Appearence,
    Promiscuity,
    Wealth,
    Intelligence
}


public class TraitsManager : MonoBehaviour
{
    private int _numTraits;
    private int[] _traitValues;
    
    private const int _minValue = -10;
    private const int _maxValue = 10;
    private const int _neutralValue = 0;
    
    void Start()
    {
        _numTraits = ( ( int[] )System.Enum.GetValues( typeof( Traits ) ) ).Length;
        _traitValues = new int[ _numTraits ];

        NeutraliseAllTraits();
    }
        

    private void AssignRandomTraits()
    {
        for(int i=0; i<_numTraits; i++ )
        {
            _traitValues[i] = Random.Range( _minValue, _maxValue + 1 );
        }
    }
    
    public int GetTraitValue( Traits trait )
    {
        return _traitValues[ (int) trait ];
    } 
    
   public void ModifyTraitValue( Traits trait, int modification )
   {
        _traitValues[ (int) trait ] += modification;
        _traitValues[ (int) trait ] = Mathf.Clamp( _traitValues[ (int)trait ], _minValue, _maxValue );
   }  
   
   public void SetTraitValue( Traits trait, int value )
   {
        _traitValues[ (int)trait ] = value;
   } 

    public void NeutraliseTraitValue( Traits trait )
    {
        _traitValues[ (int)trait ] = _neutralValue;
    }
    public void RandomiseTraitValue( Traits trait )
    {
        _traitValues[ (int) trait ] = Random.Range( _minValue, _maxValue + 1 );
    }

    public void NeutraliseAllTraits()
    {
        for( int i=0; i<_numTraits; i++ )
        {
            _traitValues[i] = _neutralValue;
        }
    }
}