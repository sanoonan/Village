using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Searching for neighbour (i.e. child) nodes - Using the UniqueNodeID. What type of search is best?
//A HashTable lookup for Cues, which have a UniqueCueID

//PROCESS - Get Cue, check neighbours for a match. Pull out that match = Thought about the item, and associated neighbours provide more context (e.g. owner, location etc.)

public class MindsEye : MonoBehaviour 
{
    public MemoryGraph MemoryGraph = new MemoryGraph();
    float UpdateFrequency = 0.5f;    //Every tenth of a second.
    public bool IsPlayer = false;

    public Dictionary<string, MemoryCue> ActiveCues = new Dictionary<string, MemoryCue>();

    public 

	//MemGraph<CueNode> memoryMap = new MemGraph<CueNode>();
	//List<CueNode> workingMemoryNodes = new List<CueNode>();

    void OnTriggerEnter(Collider other)
    {
        //Step 1 - Find out if we have any prior memory of this item / person / place.

        MemoryCue memCue = other.GetComponent<MemoryCue>();
        if (memCue == null || ActiveCues.ContainsKey(memCue.UniqueNodeID)) return;

        //Debug.Log(gameObject.name + " - " + memCue.UniqueNodeID);
        ActiveCues.Add(memCue.UniqueNodeID, memCue);

        if (IsPlayer)
        {
            //Debug.Log("Player: " + memCue.UniqueNodeID);

            if(memCue.CueType == CueType.Item)
                ItemUIPooler.Instance.PlaceLabel(memCue);
        }

        MemoryGraphNode retainedMemory;
        if (MemoryGraph.Contains(memCue.UniqueNodeID) == false)
            retainedMemory = MemoryGraph.AddNamedNodeToGraph(CreateNewMemoryNode(memCue));  //This is a new memory! Add it to our memory graph.
        else
        {
            retainedMemory = MemoryGraph.GetNamedNodeFromGraph(memCue.UniqueNodeID);  //We remember it! Recall the memory.
            retainedMemory.MemoryNode.UpdateMemory(MemoryGraph, retainedMemory, memCue, 0.0f);
        }

        retainedMemory.ActivateMemory();
        //Debug.Log("I've came across something worth remembering! - " + memCue.UniqueNodeID);
    }

    void OnTriggerStay(Collider other)
    {
        MemoryCue memCue = other.GetComponent<MemoryCue>();
        if (memCue == null) return;

        MemoryGraphNode retainedMemory = MemoryGraph.GetNamedNodeFromGraph(memCue.UniqueNodeID);
        if(retainedMemory == null)
        {
            Debug.Log(memCue.UniqueNodeID);
        }

        if(Time.time - retainedMemory.LastUpdated >= UpdateFrequency)   //We don't want to update every frame, as items won't change *that* much in such a short period of time.
            retainedMemory.MemoryNode.UpdateMemory(MemoryGraph, retainedMemory, memCue, UpdateFrequency);
    }

    void OnTriggerExit(Collider other)
    {
        MemoryCue memCue = other.GetComponent<MemoryCue>();
        if (memCue == null) return;

        ActiveCues.Remove(memCue.UniqueNodeID);

        //Debug.Log("I can't see that memory any longer, so I'm storing it for later. - " + memCue.UniqueNodeID);
        MemoryGraphNode retainedMemory = MemoryGraph.GetNamedNodeFromGraph(memCue.UniqueNodeID);

        //Update the item, so the NPC always has the most up-to-date impression of the memory as they last observed it.
        retainedMemory.MemoryNode.UpdateMemory(MemoryGraph, retainedMemory, memCue, Time.time - retainedMemory.LastUpdated);
        retainedMemory.DeactivateMemory();

        float timeObserved = Time.time - retainedMemory.LastActive; //The length of time the memory was active (i.e. making an impression in the working memory).
        //Will need to revist this, as it's currently a bit too simplistic. How do we capture increasing the strength of the memory's child nodes (i.e. if you see a hammer being held by someone...)
        for (int i = 0; i < retainedMemory.RelatedCues.Length; i++)
        {
            MemoryGraphNode cueNode = MemoryGraph.GetNamedNodeFromGraph(retainedMemory.RelatedCues[i]);
            if(cueNode == null || cueNode.Neighbours == null)
            {
                Debug.Log(retainedMemory.RelatedCues[i]);
            }
            int index = cueNode.Neighbours.IndexOf(retainedMemory);
            if (index == -1)
            {
                //Somehow the cue relationship has been broken!?
                //Rebuild it!
                MemoryGraph.AddDirectedEdge(cueNode, retainedMemory, timeObserved, timeObserved);
            }
            else
            {
                //Strengthing the memory based on the length of time it was active in the working set.
                cueNode.StrengthenMemory(index, timeObserved);
                //Debug.Log(string.Format("NS: {0}, ES: {1}", cueNode.NodeStrengths[index], cueNode.EdgeStrengths[index]));
            }
        }
        //Debug.Log("----");
    }

    void LateUpdate()
    {
        MemoryGraph.Update();

        if (IsPlayer && Input.GetKeyDown(KeyCode.Space))
        {
            //MemoryGraph.PrintGraph();

            //MemoryGraphNode mgn = MemoryGraph.GetNamedNodeFromGraph("Hammers");
            //if (mgn != null)
            //{
            //    for (int i = 0; i < mgn.Neighbours.Count; i++)
            //        Debug.Log(string.Format("{0}. {1} - {2}", i + 1, mgn.Neighbours[i].UID, mgn.OpinionStrengths[i]));
            //}
            //else
            //    Debug.Log("I've no idea what a hammer is.");

            Dictionary<string, bool> memHash = new Dictionary<string, bool>();
            MemoryGraphNode mgn = MemoryGraph.GetNamedNodeFromGraph("EVN_FrogAttack");
            PrintNodeAndNeighbours(mgn, 0, memHash);
        }        
    }

    void PrintNodeAndNeighbours(MemoryGraphNode mgn, int loopCount, Dictionary<string, bool> memHash)
    {
        if (loopCount > 5)
            return;

        if (mgn != null && memHash.ContainsKey(mgn.UID) == false)
        {
            Debug.Log(mgn.UID);
            memHash.Add(mgn.UID, true);
            for (int i = 0; i < mgn.Neighbours.Count; i++)
                Debug.Log(string.Format("{0}. {1} - {2}", i + 1, mgn.Neighbours[i].UID, mgn.OpinionStrengths[i]));

            for (int i = 0; i < mgn.Neighbours.Count; i++)
                PrintNodeAndNeighbours(mgn.Neighbours[i], loopCount + 1, memHash);
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

    public bool GetConceptLocation(string cueUID, out MemoryGraphNode strongestMemoryConcept)
    {
        strongestMemoryConcept = null;

       // Debug.Log("Looking for: " + cueUID);

        if(!MemoryGraph.Contains(cueUID))
        {
            //Debug.Log("Memory Graph does not contain - " + cueUID);
            return false;
        }

        MemoryGraphNode memNode = MemoryGraph.GetNamedNodeFromGraph(cueUID);
        //Debug.Log("This node has neighbours - " + memNode.Neighbours.Count);

        if (memNode.Neighbours == null || memNode.Neighbours.Count == 0)
            return false;

        for (int i = 0; i < memNode.Neighbours.Count; i++)
        {
            if (memNode.Neighbours[i].MemoryType == MemoryType.Item && memNode.Neighbours[i].LastLocation != Locations.Unknown)
            {
                strongestMemoryConcept = memNode.Neighbours[PerformFuzzyCheck(memNode, i, cueUID)];     //We know the list is sorted, so the first valid memory is the one with the greatest affinity. We then fuzzify it, and return the "remembered" concept (either real or fuzzied)
                return true;
            }
        }
        
        return false;
    }

    //A quick check to see if we have a possible location, for goals to know if they should complete or continue.
    internal bool GetConceptLocation(string cueUID)
    {
        if (!MemoryGraph.Contains(cueUID))
            return false;

        MemoryGraphNode memNode = MemoryGraph.GetNamedNodeFromGraph(cueUID);

        if (memNode.Neighbours == null || memNode.Neighbours.Count == 0)
            return false;

        for (int i = 0; i < memNode.Neighbours.Count; i++)
        {
            if (memNode.Neighbours[i].MemoryType == MemoryType.Item && memNode.Neighbours[i].LastLocation != Locations.Unknown)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Takes a memory and checks if it is remembered correctly or not.
    /// </summary>
    /// <returns>Returns the memory, the agent will have no idea if it is genuine or fuzzy.</returns>
    private int PerformFuzzyCheck(MemoryGraphNode memNode, int strongestMem, string cueUID)
    {
        if (memNode.MemStrengths[strongestMem] >= memNode.NodeThreshold)
            return strongestMem;    //It's a strong stage memory, so is remembered correctly!

        float fuzzyProbability = 1.0f - (memNode.MemStrengths[strongestMem] / memNode.NodeThreshold);

        if (Random.Range(0.0f, 1.0f) > fuzzyProbability)
            return strongestMem;    //Although a weak stage memory, the probability roll has rememebred it correctly in this instance!

        MemoryType fuzzyType = memNode.Neighbours[strongestMem].MemoryType;
        int fuzzyNeighbourCount = GetNeighbourCount(memNode, fuzzyType);

        if (fuzzyNeighbourCount <= 1)
            return strongestMem;    //Although we are meant to return a false memory, we've only one related memory so it can't really be fuzzified!

        int fuzzyMem = Random.Range(0, fuzzyNeighbourCount - 2);        //We then get the element that will be return in its place.
        for(int i = 0; i < memNode.Neighbours.Count; i++)
        {
            if(memNode.Neighbours[i].MemoryType == fuzzyType && i != strongestMem)
            {
                --fuzzyMem;
                if (fuzzyMem <= 0)
                {
                    //Debug.Log(string.Format("Agent wanted " + strongestMem + " but it was fuzzied into " + i));
                    return i;
                }
            }
        }

        return strongestMem;    //We should never get here, but if we do we'll just return the original memory to ensure no crashes.
    }

    private int GetNeighbourCount(MemoryGraphNode memNode, MemoryType fuzzyType)
    {
        int count = 0;
        for (int i = 0; i < memNode.Neighbours.Count; i++)
        {
            if (memNode.Neighbours[i].MemoryType == fuzzyType)
                ++count;
        }

        return count;
    }

    internal bool GetConceptNeighbours(string cueUID, ref List<string> targetUIDs)
    {
        targetUIDs.Clear();
        if (!MemoryGraph.Contains(cueUID))
            return false;

        MemoryGraphNode memNode = MemoryGraph.GetNamedNodeFromGraph(cueUID);

        //Should I just return the mem node references?
        targetUIDs.Capacity = memNode.Neighbours.Count;
        for (int i = 0; i < memNode.Neighbours.Count; i++)
            targetUIDs.Add(memNode.Neighbours[i].UID);

        return true;
    }

    internal bool GetConceptNeighbours(string cueUID, MemoryType memType, ref List<string> targetUIDs)
    {
        targetUIDs.Clear();
        if (!MemoryGraph.Contains(cueUID))
            return false;

        MemoryGraphNode memNode = MemoryGraph.GetNamedNodeFromGraph(cueUID);
 
        //Should I just return the mem node references?
        for (int i = 0; i < memNode.Neighbours.Count; i++)
        {
            //Debug.Log("Neighbour " + i + " - " + memNode.Neighbours[i].UID);
            if (memNode.Neighbours[i].MemoryType == memType)
                targetUIDs.Add(memNode.Neighbours[i].UID);
        }

        //Debug.Log("GetConceptTargetUIDs - " + targetUIDs.Count);

        if (targetUIDs.Count > 0)
            return true;

        //Can see Asbel, and that he has a hammer, but has no memory of a location! Is the location being forgotten, or is it not being stored at all!?
        return false;
    }

    internal MemoryGraphNode GetConceptNode(string cueUID)
    {
        if (!MemoryGraph.Contains(cueUID))
            return null;

        return MemoryGraph.GetNamedNodeFromGraph(cueUID);
    }

    internal void GainMemoriesFromSource(List<int> participants, int ownerID, string targetUID)
    {
        for (int i = 0; i < participants.Count; i++)
        {
            if(participants[i] == ownerID)
                continue;
            
            MemoryGraph.CopyFromOtherGraph(AgentManager.Instance.GetAgent(participants[i])._cognitiveAgent, targetUID);
        }
    }
}
