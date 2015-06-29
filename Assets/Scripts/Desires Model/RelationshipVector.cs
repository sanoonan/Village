using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum RelationshipValue
{
    PosNeg,
    Closeness
};


public class RelationshipVector
{
  
    private int numValues;


    private float[] values;
    private float[] startingValues;


    public RelationshipVector()
    {
        int[] valueEnums = (int[])System.Enum.GetValues(typeof(RelationshipValue));
        numValues = valueEnums.Length;

        values = new float[numValues];
        startingValues = new float[numValues];

        for (int i = 0; i < numValues; i++)
            startingValues[i] = Random.Range(-0.5f, 0.5f);
    }

 

}