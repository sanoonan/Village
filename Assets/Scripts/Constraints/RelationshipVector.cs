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

    public RelationshipVector()
    {
        int[] valueEnums = (int[])System.Enum.GetValues(typeof(RelationshipValue));
        numValues = valueEnums.Length;

        values = new float[numValues];


        for (int i = 0; i < numValues; i++)
            values[i] = 0.0f;
    }
}