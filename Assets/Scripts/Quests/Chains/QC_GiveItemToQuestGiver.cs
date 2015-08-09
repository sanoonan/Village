using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QC_GiveItemToQuestGiver : QuestChain
{
    public string targetItemMarker;

    public QC_GiveItemToQuestGiver(int _questGiverId, string itemMarker)
        : base(_questGiverId)
    {
        targetItemMarker = itemMarker;

        AddActiveQuestFragment(new QF_AcquireItem(targetItemMarker));
        AddLinearQuestFragment(new QF_GiveItemToNPC(_questGiverId, targetItemMarker));
    }

    public QC_GiveItemToQuestGiver(string _questGiverName, string itemMarker)
        : base(_questGiverName)
    {
        targetItemMarker = itemMarker;

        AddActiveQuestFragment(new QF_AcquireItem(targetItemMarker));
        AddLinearQuestFragment(new QF_GiveItemToNPC(_questGiverId, targetItemMarker));
    }

   




    public override void SetChainDescription()
    {
        description = "Give a " + targetItemMarker + " to " + AgentManager.Instance.GetAgentNameById(_questGiverId);
    }
}