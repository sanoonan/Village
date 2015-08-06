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

    private int[] _traitValues;
    
    private const int minValue = 0;
    private const int maxValue = 10;
    
    void Start()
    {
        
    }
        
        
    private void AssignRandomTraits()
    {
        int numTraits = ( ( int[] )System.Enum.GetValues( typeof( Traits ) ) ).Length;
        _traitValues = new int[ numTraits ];
        
        for(int i=0; i<numTraits; i++ )
        {
            _traitValues[i] = Random.Range( minValue, maxValue + 1 );
        }
    }
    
    public int GetTraitValue( Traits trait )
    {
        return _traitValues[ (int) trait ];
    } 
    
   public void ModifyTraitValue( Traits trait, int modification )
   {
        _traitValues[ (int) trait ] += modification;
        _traitValues[ (int) trait ] = Mathf.Clamp( _traitValues[ (int)trait ], minValue, maxValue );
   }  
   
   public void SetTraitValue( Traits trait, int value )
   {
        _traitValues[ (int)trait ] = value;
   }     
}