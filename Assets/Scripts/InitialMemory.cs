using UnityEngine;
using System.Collections;

public class InitialMemory : MonoBehaviour 
{
    public bool IsTest = false; 
    public MemoryCue[] initialCues;   

	void Start ()
    {
        CognitiveAgent cAgent = GetComponent<CognitiveAgent>();

        for (int i = 0; i < initialCues.Length; i++)
        {
            MemoryCue memCue = initialCues[i];
            if (memCue == null) return;

            MemoryGraphNode retainedMemory;
            if (cAgent.MindsEye.MemoryGraph.Contains(memCue.UniqueNodeID) == false)
            {
                retainedMemory = cAgent.MindsEye.MemoryGraph.AddNamedNodeToGraph(CreateNewMemoryNode(memCue));  //This is a new memory! Add it to our memory graph.
                retainedMemory.MemoryNode.UpdateMemory(cAgent.MindsEye.MemoryGraph, retainedMemory, memCue, 0.0f);
            }
            else
            {
                retainedMemory = cAgent.MindsEye.MemoryGraph.GetNamedNodeFromGraph(memCue.UniqueNodeID);  //We remember it! Recall the memory.
                retainedMemory.MemoryNode.UpdateMemory(cAgent.MindsEye.MemoryGraph, retainedMemory, memCue, 0.0f);
            }
        }

        if (!IsTest)
            return;

        for (int i = 0; i < initialCues.Length; i++)
        {
            for (int j = 0; j < 100; j++)
                cAgent.MindsEye.MemoryGraph.ForceDebugAllocation(CreateNewMemoryNode(initialCues[i]));
        }
	}

    private MemoryNode CreateNewMemoryNode(MemoryCue memCue)
    {
        switch (memCue.CueType)
        {
            case CueType.Character:
                return new CharacterNode((CharacterCue)memCue);

            case CueType.Item:
                return new ItemNode((ItemCue)memCue);

            case CueType.Location:
                return new LocationNode((LocationCue)memCue);

            case CueType.Event:
                return new EventNode((EventCue)memCue);

            default:
                throw new UnityException("MemoryNode is unhandled! The above are the only accepted types!");
        }
    }
}
