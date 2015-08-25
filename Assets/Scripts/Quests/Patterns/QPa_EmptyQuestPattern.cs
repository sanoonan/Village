using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QPa_EmptyQuestPattern : QuestPattern
{
    public QPa_EmptyQuestPattern( int questGiverId)
        : base( questGiverId )
    {
    }


    public override void SetPatternDescription()
    {
        _description = "Empty quest pattern";
    }
}