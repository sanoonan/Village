using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum MemoryType
{
    Cue,
    Item,
    Character,
    Location,
    Event
};

public class MemoryGraph
{
    Dictionary<string, MemoryGraphNode> MemoryHash;  //This dictionary (i.e. hashtable) lookup allows for a O(1) lookup of any "named" nodes. These are non-generic nodes that represent cues, places, people, objects etc. Faster than a HashTable because it doesn't need to box / unbox contents.
    Dictionary<string, bool> RecursiveHash;
    List<MemoryGraphNode> MemoryNodes;   //This contains all the nodes in the graph (e.g. named nodes + any generic context nodes that are attached to the named nodes).
    //Current method of adding only named nodes mean that generic child ndoes are not stored in the list... making it rather redundant. Look at addressing this, or just cutting it if a HashTable for named nodes is all that is needed!
    BetterList<string> deadNodes = new BetterList<string>();
    float lastUpdateTime;

    public float InfluenceRate = 0.5f;  //How much an Agent's opinion of a concept is influenced by the opinion of the person they're speaking with (when copying new concepts across).

    public MemoryGraph()
    {
        MemoryHash = new Dictionary<string, MemoryGraphNode>();
        RecursiveHash = new Dictionary<string, bool>();
        MemoryNodes = new List<MemoryGraphNode>();
    }

    public MemoryGraphNode AddNamedNodeToGraph(MemoryNode memNode, bool copyOverload = false)
    {
        MemoryGraphNode mgn = new MemoryGraphNode(memNode);
        MemoryNodes.Add(mgn);
        //Debug.Log(mgn.UID);
        MemoryHash.Add(mgn.UID, mgn);

        if (memNode.MemoryType == MemoryType.Cue)// || (copyOverload && memNode.MemoryType != MemoryType.Location))   //Cues cannot have parent nodes.
            return mgn;

        //We can access related memories from high-level cues (e.g. Hammers, Villagers, Family etc.)
        //Here we either add a reference from the related cue to the object.
        //Or if this is the first object to have this high-level cue, we create the cue.      
        for (int i = 0; i < memNode.RelatedCues.Length; i++)
        {
            MemoryGraphNode cueNode;
            //Debug.Log("Creating cue node: " + memNode.RelatedCues[i]);
            if (Contains(memNode.RelatedCues[i]) == false)
                cueNode = AddNamedNodeToGraph(new MemoryNode(memNode.RelatedCues[i], MemoryType.Cue));
            else
                cueNode = GetNamedNodeFromGraph(memNode.RelatedCues[i]);

            AddDirectedEdge(cueNode, mgn, 11.0f, mgn.MemoryNode.GetInitialOpinion());
        }

        return mgn;
    }

    public bool RemoveNamedNodeFromGraph(MemoryGraphNode memNode)
    {
        if (memNode.MemoryType != MemoryType.Cue)
        {
            //First we check the cues that this memory belongs to.
            //If the cue is only pointing to this memory, we will remove the cue too.
            for (int i = 0; i < memNode.RelatedCues.Length; i++)
            {
                MemoryGraphNode cueNode;
                if (Contains(memNode.RelatedCues[i]) == false)
                    cueNode = AddNamedNodeToGraph(new MemoryNode(memNode.RelatedCues[i], MemoryType.Cue));
                else
                    cueNode = GetNamedNodeFromGraph(memNode.RelatedCues[i]);

                if (cueNode.Neighbours.Count == 0)
                    RemoveNamedNodeFromGraph(cueNode);
            }

            //Next we must check any neighbours which are pointing to it, and remove those.
            for (int i = 0; i < memNode.Neighbours.Count; i++)
            {
                int index = memNode.Neighbours.IndexOf(memNode);
                if (index != -1)
                {
                    memNode.RemoveNeighbour(index);
                }
            }
        }

        //Finally, we can remove the node itself from our overall list as well as the hash table.
        bool retVal = true;

        retVal &= MemoryNodes.Remove(memNode);
        retVal &= MemoryHash.Remove(memNode.UID);

        return retVal;  //We return a value which will be true if successfully removed from both, or false otherwise.
    }

    public bool RemoveNamedNodeFromGraph(string uid)
    {
        MemoryGraphNode mgn = GetNamedNodeFromGraph(uid);

        if (mgn == null)
            return false;

        return RemoveNamedNodeFromGraph(mgn);
    }

    public void RemoveEdge(MemoryGraphNode fromNode, MemoryGraphNode toNode)
    {
        //Debug.Log("Removing an edge: " + fromNode.UID + " to " + toNode.UID);
        for (int i = 0; i < fromNode.Neighbours.Count; i++)
        {
            if (fromNode.Neighbours[i] == toNode)
            {
                fromNode.RemoveNeighbour(i);
                break;
            }
        }

        for (int i = 0; i < toNode.Neighbours.Count; i++)
        {
            if (toNode.Neighbours[i] == fromNode)
            {
                toNode.RemoveNeighbour(i);
                break;
            }
        }

        //This will need to be watched, to see if any loose memories leak into this system.
        //As is, I don't believe it is possible. We only need to outright remove a node if the cue reference breaks down.
        if(fromNode.MemoryType == MemoryType.Cue)
        {
            //Debug.Log("Removing the entire node: " + toNode.UID);
            deadNodes.Add(toNode.UID);  //We can't remove a node in the middle of the update, so we store it until the end of the frame.
        }
    }

    public bool Contains(string nodeUID)
    {
        return MemoryHash.ContainsKey(nodeUID);
    }

    public MemoryGraphNode GetNamedNodeFromGraph(string nodeUID)
    {
        if (MemoryHash.ContainsKey(nodeUID))
            return MemoryHash[nodeUID];

        return null;    //If it can't find a match, return null.
    }

    public void AddDirectedEdge(MemoryGraphNode fromNode, MemoryGraphNode toNode, float initialMemoryStrength = 1.0f, float initialOpinionStrength = 0.0f, float initialPrimaryStrength = 0.0f, float initialSecondaryStrength = 0.0f)
    {
        fromNode.AddNeighbour(toNode, initialMemoryStrength, initialOpinionStrength, initialPrimaryStrength, initialSecondaryStrength);
    }

    public void AddDirectedEdge(string fromUID, string toUID, float initialMemoryStrength = 1.0f, float initialOpinionStrength = 0.0f, float initialPrimaryStrength = 0.0f, float initialSecondaryStrength = 0.0f)
    {
        MemoryGraphNode fromNode = GetNamedNodeFromGraph(fromUID);
        MemoryGraphNode toNode = GetNamedNodeFromGraph(toUID);

        if (fromNode == null)
            throw new UnityException("fromNode should not be null - " + fromUID);
        if (toNode == null)
            throw new UnityException("toNode should not be null - " + toUID);

        AddDirectedEdge(fromNode, toNode, initialMemoryStrength, initialOpinionStrength, initialPrimaryStrength, initialSecondaryStrength);
    }

    public void AddUndirectedEdge(MemoryGraphNode fromNode, MemoryGraphNode toNode, float initialMemoryStrength = 1.0f, float initialOpinionStrength = 0.0f, float initialPrimaryStrength = 0.0f, float initialSecondaryStrength = 0.0f)
    {
        fromNode.AddNeighbour(toNode, initialMemoryStrength, initialOpinionStrength, initialPrimaryStrength, initialSecondaryStrength);
        toNode.AddNeighbour(fromNode, initialMemoryStrength, initialOpinionStrength, initialPrimaryStrength, initialSecondaryStrength);
    }

    public void AddUndirectedEdge(string fromUID, string toUID, float initialMemoryStrength = 1.0f, float initialOpinionStrength = 0.0f, float initialPrimaryStrength = 0.0f, float initialSecondaryStrength = 0.0f)
    {
        MemoryGraphNode fromNode = GetNamedNodeFromGraph(fromUID);
        MemoryGraphNode toNode = GetNamedNodeFromGraph(toUID);

        if (fromNode == null)
            throw new UnityException("fromNode should not be null - " + fromUID);
        if (toNode == null)
            throw new UnityException("toNode should not be null - " + toUID);

        AddUndirectedEdge(fromNode, toNode, initialMemoryStrength, initialOpinionStrength, initialPrimaryStrength, initialSecondaryStrength);
    }

    internal void Update()
    {
        float ForgetThreshold = 0.007f;  //As the exponential curve never reaches zero, we have a small non-zero value count as the point at which memories are forgotten.
        float UpdateFrequency = 0.1f;
        float RetentionRate = 20.0f;
        RetentionRate = 1.0f / RetentionRate;
        float timeSinceLastUpdate = Time.time - lastUpdateTime;
        timeSinceLastUpdate = UpdateFrequency + 1.0f;

        //We start to forget any inactive memories, at a rate decided by the retention rate.
        if (timeSinceLastUpdate >= UpdateFrequency)
        {
            lastUpdateTime = Time.time;

            for (int i = MemoryNodes.Count - 1; i >= 0; i--)
            {
                if (MemoryNodes[i].IsActive) //|| MemoryNodes[i].MemoryType == MemoryType.Cue)
                    continue;

                timeSinceLastUpdate = Mathf.Min(UpdateFrequency, Time.time - MemoryNodes[i].LastUpdated);    //It's possible a memory has only become inactive in the middle of this update cycle, so we scale the forget rate to match the length of inactive time.

                for (int j = MemoryNodes[i].Neighbours.Count - 1; j >= 0; j--)
                {
                    if (MemoryNodes[i].Neighbours[j].IsActive || MemoryNodes[i].MemStrengths[j] >= 11.0f)      //We won't decay a two-way branch if the other side is active. (This might be fine as a simplification, will need to watch how it evolves in a running program.) and anything greater than 11 is a permanently encoded concept.
                        continue;

                    float decayFactor = 1.0f; //A higher decay factor means the forgetting curve is much harsher.
                    float forgettingExp = Mathf.Exp(-5.0f + MemoryNodes[i].MemStrengths[j] * 0.5f * decayFactor);  //This is a simplification of the Ebbinghause forgetting curve.
                    float expVal = timeSinceLastUpdate * RetentionRate *forgettingExp;

                    MemoryNodes[i].DecayMemory(j, timeSinceLastUpdate * RetentionRate * forgettingExp);

                    if (MemoryNodes[i].MemStrengths[j] <= ForgetThreshold)
                        RemoveEdge(MemoryNodes[i], MemoryNodes[i].Neighbours[j]);
                }
            }

            //We can't remove nodes when we are in the middle of an update iteration, so we instead check if any were marked for death and remove them here.
            if(deadNodes.size > 0)
            {
                for(int i = 0; i < deadNodes.size; i++)
                    RemoveNamedNodeFromGraph(deadNodes[i]);

                deadNodes.Clear();
            }
        }
    }
    
    public void PrintGraph()
    {
        for (int x = 0; x < MemoryNodes.Count; x++)
        {
            MemoryGraphNode node = MemoryNodes[x];
            Debug.Log(string.Format("Node: {0}, Neighbours: {1}", node.UID, node.Neighbours.Count));

            for (int i = 0; i < node.Neighbours.Count; i++)
                Debug.Log(string.Format("\tNeighbour: {0}, NS: {1}, ES:{2}", node.Neighbours[i].UID, node.MemStrengths[i], node.OpinionStrengths[i]));
            Debug.Log("-----------");
        }
    }

    public void PrintGraph(string cueUID)
    {
        MemoryGraphNode mgm = GetNamedNodeFromGraph(cueUID);

        if (mgm == null)
            Debug.Log("Cue Node not found! - " + cueUID);
        else
            PrintGraph(mgm, 3);
    }

    public void PrintGraph(MemoryGraphNode node, int depth)
    {
        Debug.Log(string.Format("Node: {0}, Neighbours: {1}", node.UID, node.Neighbours.Count));

        for (int i = 0; i < node.Neighbours.Count; i++)
            Debug.Log(string.Format("\tNeighbour: {0}, NS: {1}, ES:{2}", node.Neighbours[i].UID, node.MemStrengths[i], node.OpinionStrengths[i]));

        Debug.Log("\t-----------");
        Debug.Log("-----------");

        if (depth > 0)
            for (int i = 0; i < node.Neighbours.Count; i++)
                PrintGraph(node.Neighbours[i], depth - 1);
    }

    internal bool CopyFromOtherGraph(CognitiveAgent otherAgent, string cueID)
    {
        MemoryGraphNode otherMGN = otherAgent.MindsEye.GetConceptNode(cueID);

        if (otherMGN == null || otherMGN.Neighbours.Count == 0)
            return false;       //Other agent has no idea about the cue, so has nothing to share.

        MemoryGraphNode ourMGN = GetNamedNodeFromGraph(cueID);
        if (ourMGN == null)
            ourMGN = AddNamedNodeToGraph(new MemoryNode(cueID, MemoryType.Cue));

        RecursiveHash.Clear();   
        CopyFromOtherGraph(ourMGN, otherMGN, 0);

        return true;
    }

    private void CopyFromOtherGraph(MemoryGraphNode ourMGN, MemoryGraphNode otherMGN, int loopCount)
    {
        //if(loopCount >= 30)
            //Debug.Log("We've hit an infinite loop!");

        RecursiveHash.Add(ourMGN.UID, true);
        if(ourMGN.MemoryType == MemoryType.Location)
            return; //We go no deeper than location children.

        for (int i = 0; i < otherMGN.Neighbours.Count; i++)
        {
            for (int j = 0; j < ourMGN.Neighbours.Count; j++)
            {
                if (otherMGN.Neighbours[i].MemoryNode == ourMGN.Neighbours[j].MemoryNode)
                {
                    //Debug.Log("Found a match for " + otherMGN.Neighbours[i].MemoryNode.UID);
                    if (!RecursiveHash.ContainsKey(otherMGN.Neighbours[i].MemoryNode.UID))
                        CopyFromOtherGraph(ourMGN.Neighbours[j], otherMGN.Neighbours[i], loopCount + 1);   //We will see if any of their children also need updated.
                    //else
                        //Debug.Log("Infinite Loop Prevented!");

                    continue;
                }
            }

            //Debug.Log("Found no match for " + otherMGN.Neighbours[i].MemoryNode.UID + " so I will create it.");
            //We need to create it as an entirely fresh node.
            MemoryGraphNode newNode;
            if(Contains(otherMGN.Neighbours[i].MemoryNode.UID))
                newNode = GetNamedNodeFromGraph(otherMGN.Neighbours[i].MemoryNode.UID);
            else
                newNode = AddNamedNodeToGraph(otherMGN.Neighbours[i].MemoryNode, true);

            if (ourMGN.MemoryType == MemoryType.Cue)
                AddDirectedEdge(ourMGN, newNode, otherMGN.MemStrengths[i], otherMGN.OpinionStrengths[i] * InfluenceRate);
            else
                AddUndirectedEdge(ourMGN, newNode, otherMGN.MemStrengths[i], otherMGN.OpinionStrengths[i] * InfluenceRate);       //For now, we'll even copy the strength of the other character's memory.

            newNode.LastLocation = otherMGN.Neighbours[i].LastLocation;
            newNode.LastPosition = otherMGN.Neighbours[i].LastPosition;

//            Debug.Log("Copying from " + otherMGN.UID + " to " + otherMGN.Neighbours[i].UID);
            if (!RecursiveHash.ContainsKey(otherMGN.Neighbours[i].MemoryNode.UID))
                CopyFromOtherGraph(newNode, otherMGN.Neighbours[i], loopCount + 1);
            //else
                //Debug.Log("Infinite Loop Prevented!");
        }
    }

    internal void BindRelatedCues(MemoryCue memCue)
    {
        for (int i = 0; i < memCue.CueMarkers.Length; i++)
        {
            MemoryGraphNode cueNode;
            if (Contains(memCue.CueMarkers[i]) == false)
                cueNode = AddNamedNodeToGraph(new MemoryNode(memCue.CueMarkers[i], MemoryType.Cue));
            else
                cueNode = GetNamedNodeFromGraph(memCue.CueMarkers[i]);

            for (int j = 0; j < cueNode.Neighbours.Count; j++)
            {
                if(cueNode.Neighbours[j].UID == memCue.UniqueNodeID)
                {
                    cueNode.BindMemory(j);      //We bind the memory as long as we need to, by setting it to 11 it will never degrade.
                    break;
                }
            }
        }
    }

    internal void UnbindRelatedCues(MemoryCue memCue)
    {
        for (int i = 0; i < memCue.CueMarkers.Length; i++)
        {
            MemoryGraphNode cueNode;
            if (Contains(memCue.CueMarkers[i]) == false)
                cueNode = AddNamedNodeToGraph(new MemoryNode(memCue.CueMarkers[i], MemoryType.Cue));
            else
                cueNode = GetNamedNodeFromGraph(memCue.CueMarkers[i]);

            for (int j = 0; j < cueNode.Neighbours.Count; j++)
            {
                if (cueNode.Neighbours[j].UID == memCue.UniqueNodeID)
                {
                    cueNode.UnbindMemory(j);                //We unbind the memory, and set it to the max value so it can begin to degrade.
                    break;
                }
            }
        }
    }

    internal void UpdateCueOpinion(MemoryNode memNode, float strengthFactor)
    {
        for (int i = 0; i < memNode.RelatedCues.Length; i++)
        {
            MemoryGraphNode cueNode = GetNamedNodeFromGraph(memNode.RelatedCues[i]);
            for (int j = 0; j < cueNode.Neighbours.Count; j++)
            {
                if (cueNode.Neighbours[j].UID == memNode.UID)
                {
                    cueNode.StrengthenOpinion(j, strengthFactor);
                    break;
                }
            }
        }
    }

    internal void UpdateCueOpinion(MemoryCue memCue, float strengthFactor)
    {
        for (int i = 0; i < memCue.CueMarkers.Length; i++)
        {
            MemoryGraphNode cueNode = GetNamedNodeFromGraph(memCue.CueMarkers[i]);
            for (int j = 0; j < cueNode.Neighbours.Count; j++)
            {
                if (cueNode.Neighbours[j].UID == memCue.UniqueNodeID)
                {
                    cueNode.StrengthenOpinion(j, strengthFactor);
                    break;
                }
            }
        }
    }
    
    internal float GetCueOpinion(MemoryCue memCue)
    {
        if(memCue.CueMarkers == null)
            return 0.0f;

        MemoryGraphNode cueNode = GetNamedNodeFromGraph(memCue.CueMarkers[0]);

        if (cueNode == null)
            return 0.0f;

        for (int j = 0; j < cueNode.Neighbours.Count; j++)
            if (cueNode.Neighbours[j].UID == memCue.UniqueNodeID)
                return cueNode.OpinionStrengths[j];

        return 0.0f;
    }

    internal float GetCueOpinion(MemoryNode memNode)
    {
        if(memNode.RelatedCues == null)
            return 0.0f;

        MemoryGraphNode cueNode = GetNamedNodeFromGraph(memNode.RelatedCues[0]);

        if (cueNode == null)
            return 0.0f;

        for (int j = 0; j < cueNode.Neighbours.Count; j++)
            if (cueNode.Neighbours[j].UID == memNode.UID)
                return cueNode.OpinionStrengths[j];

        return 0.0f;
    }

    internal void ForceDebugAllocation(MemoryNode memoryNode)
    {
        MemoryGraphNode mgn = new MemoryGraphNode(memoryNode);
        MemoryNodes.Add(mgn);
    }
}

//Memory Nodes are the actual "thoughts" that an NPC has. 
//This could be about an item in the world, or a person, or a place, or a thing!
//Multiple NPCs can have thoughts about these items, so shared info (e.g. its transform, or whatever) can be stored in one large Memory Bank and referenced by each NPC.
//The MemoryGraphNode is unique to each NPC, and tells us what they think about the item. The connections they've made, and the strength of the memories.
public class MemoryGraphNode
{
    public MemoryGraphNode(MemoryNode memNode)
    {
        this.MemoryNode = memNode;
        //LastUpdated = Time.time;
        //LastActive = Time.time;
        IsActive = false;

        Neighbours = new List<MemoryGraphNode>();
        MemStrengths = new List<float>();
        OpinionStrengths = new List<float>();

        PrimaryStrengths = new List<float>();
        SecondaryStrengths = new List<float>();
    }

    public bool IsActive;
    public float LastActive;
    public float LastUpdated;
    public float NodeThreshold = 5.0f;  //This is the threshold seperating the strong and weak stage memories. Weak stage memories can be mis-remembered, e.g. "Fuzzy". This will be an Agent specific value later on.
    public List<float> MemStrengths;    //This represents how strongly the memory is currently encoded, and is in the range of 0 to 10. 11 is used to represent a permanently encoded memory, for something we don't want them to forget.

    public List<float> OpinionStrengths;        
    //This represents a context specific thought about the item, in the -1 to +1 range. 
    //For example, affinity with a person, familiarity with an item, attractiveness of a location. 
    //The same items can even be represented by different opinons, for example an owned hammer is "familiar" while a new hammer is "desirable". 
    //If the new hamer is more desirable than the current one is familiar, the agent will try to buy it.

    public List<float> PrimaryStrengths;
    public List<float> SecondaryStrengths;  

    public List<MemoryGraphNode> Neighbours;        //Sorted based on node strength / edge strength. Pick the first one that matches the context (e.g. person, place, item search)

    public MemoryNode MemoryNode; //This is the actual memory node containing the contents of the memory, allowing for them to be shared across multiple NPC graphs.

    public Locations LastLocation;
    public Vector3 LastPosition;

    public string UID
    {
        get { return MemoryNode.UID; }
    }

    public MemoryType MemoryType
    {
        get { return MemoryNode.MemoryType;  }
    }

    public string[] RelatedCues
    {
        get { return MemoryNode.RelatedCues; }
    }

    public MemoryGraphNode FindMemoryAmongNeighbours(string uid)
    {
        for(int i = 0; i < Neighbours.Count; i++)
            if(Neighbours[i].UID == uid)
                return Neighbours[i];

        return null;
    }

    public void ActivateMemory()
    {
        LastActive = Time.time;  //Since the item is now active in memory, update the time it was last made active.
        IsActive = true;
    }

    public void DeactivateMemory()
    {
        IsActive = false;
    }

    internal void RemoveNeighbour(int index)
    {
        Neighbours.RemoveAt(index);
        MemStrengths.RemoveAt(index);
        OpinionStrengths.RemoveAt(index);
    }

    internal void AddNeighbour(MemoryGraphNode toNode, float initialMemoryStrength, float initialOpinionStrength, float initialPrimaryStrength, float initialSecondaryStrength)
    {
        Neighbours.Add(toNode);
        MemStrengths.Add(initialMemoryStrength);

        PrimaryStrengths.Add(initialPrimaryStrength);
        SecondaryStrengths.Add(initialSecondaryStrength);
        OpinionStrengths.Add(initialOpinionStrength);
        //OpinionStrengths.Add(initialPrimaryStrength + initialSecondaryStrength);

        AscensionSortNeighbours(OpinionStrengths.Count - 1);    //Sorts the Neighbours by affinity (e.g. opinion strength)
    }

    /// <summary>
    /// For use when a new neighbour is added at the end of the list, or the strength increases. Assuming all other elements are sorted (as they should be).
    /// </summary>
    /// <param name="sortingElement">The id of the element in the list, in this case it should be the last one.</param>
    private void AscensionSortNeighbours(int sortingElement)
    {
        float key = OpinionStrengths[sortingElement];
        float memKey = MemStrengths[sortingElement];
        MemoryGraphNode neighKey = Neighbours[sortingElement];

        int j = sortingElement - 1;
        while (j >= 0 && OpinionStrengths[j] < key)
        {
            OpinionStrengths[j + 1] = OpinionStrengths[j];
            MemStrengths[j + 1] = MemStrengths[j];
            Neighbours[j + 1] = Neighbours[j];
            --j;
        }

        OpinionStrengths[j + 1] = key;
        MemStrengths[j + 1] = memKey;
        Neighbours[j + 1] = neighKey;
    }

    /// <summary>
    /// For use when a new neighbour is added at the start of the list, or the strength decreases. Assuming all other elements are sorted (as they should be).
    /// </summary>
    /// <param name="sortingElement">The id of the element in the list, in this case it should be the last one.</param>
    private void DescensionSortNeighbours(int sortingElement)
    {
        float key = OpinionStrengths[sortingElement];
        float memKey = MemStrengths[sortingElement];
        MemoryGraphNode neighKey = Neighbours[sortingElement];

        int j = sortingElement + 1;
        int terminus = OpinionStrengths.Count - 1;
        while (j <= terminus && OpinionStrengths[j] > key)
        {
            OpinionStrengths[j - 1] = OpinionStrengths[j];
            MemStrengths[j - 1] = MemStrengths[j];
            Neighbours[j - 1] = Neighbours[j];
            ++j;
        }

        OpinionStrengths[j - 1] = key;
        MemStrengths[j - 1] = memKey;
        Neighbours[j - 1] = neighKey;
    }

    /// <summary>
    /// Used when the neighbours are unsorted, and so the entire list needs updated. Assuming elements are mostly sorted.
    /// </summary>
    private void SortNeighbours()
    {
        //Maybe only sort at the end of each frame...?
        //We only add elements (or change elements) one at a time, so we can simplify the insertion sort to a single iteration.
        //For now we shall stick to the basic sort, and optimise later.

        for (int i = 1; i < OpinionStrengths.Count; i++)
        {
            float key = OpinionStrengths[i];
            float memKey = MemStrengths[i];
            MemoryGraphNode neighKey = Neighbours[i];

            int j = i - 1;
            while(j >= 0 && OpinionStrengths[j] < key)
            {
                OpinionStrengths[j + 1] = OpinionStrengths[j];
                MemStrengths[j + 1] = MemStrengths[j];
                Neighbours[j + 1] = Neighbours[j];
                j--;
            }

            OpinionStrengths[j + 1] = key;
            MemStrengths[j + 1] = memKey;
            Neighbours[j + 1] = neighKey;
        }
    }

    internal void StrengthenMemory(int index, float strengthFactor)
    {
        MemStrengths[index] += strengthFactor;
        MemStrengths[index] = Mathf.Min(MemStrengths[index], 10.0f);    //The strongest memories are encoded at 10.0f;
    }

    internal void DecayMemory(int index, float decayFactor)
    {
        MemStrengths[index] -= decayFactor;
    }

    internal void StrengthenOpinion(int index, float strengthFactor)
    {
        OpinionStrengths[index] += strengthFactor;                            
        OpinionStrengths[index] = Mathf.Min(OpinionStrengths[index], 10.0f);

        if (strengthFactor < 0.0f)
            DescensionSortNeighbours(index);
        else
            AscensionSortNeighbours(index);
    }

    internal void StrengthenPrimaryOpinion(int index, float strengthFactor)
    {
        PrimaryStrengths[index] += strengthFactor;
        PrimaryStrengths[index] = Mathf.Min(PrimaryStrengths[index], 10.0f);

        OpinionStrengths[index] = PrimaryStrengths[index] + SecondaryStrengths[index];

        if (strengthFactor < 0.0f)
            DescensionSortNeighbours(index);
        else
            AscensionSortNeighbours(index);
    }

    internal void StrengthenSecondaryOpinion(int index, float strengthFactor)
    {
        SecondaryStrengths[index] += strengthFactor;
        SecondaryStrengths[index] = Mathf.Min(SecondaryStrengths[index], 10.0f);

        OpinionStrengths[index] = PrimaryStrengths[index] + SecondaryStrengths[index];

        if (strengthFactor < 0.0f)
            DescensionSortNeighbours(index);
        else
            AscensionSortNeighbours(index);
    }

    internal void SetPrimaryOpinion(int index, float strength)
    {
        PrimaryStrengths[index] = strength;
        PrimaryStrengths[index] = Mathf.Min(PrimaryStrengths[index], 10.0f);

        float prevStrength = OpinionStrengths[index];
        OpinionStrengths[index] = PrimaryStrengths[index] + SecondaryStrengths[index];

        if (prevStrength > OpinionStrengths[index])
            DescensionSortNeighbours(index);
        else
            AscensionSortNeighbours(index);
    }

    internal void SetSecondaryOpinion(int index, float strength)
    {
        SecondaryStrengths[index] = strength;
        SecondaryStrengths[index] = Mathf.Min(SecondaryStrengths[index], 10.0f);

        float prevStrength = OpinionStrengths[index];
        OpinionStrengths[index] = PrimaryStrengths[index] + SecondaryStrengths[index];

        if (prevStrength > OpinionStrengths[index])
            DescensionSortNeighbours(index);
        else
            AscensionSortNeighbours(index);
    }

    internal void BindMemory(int index)
    {
        MemStrengths[index] = 11.0f;    //This stops the memory degrading.
    }

    internal void UnbindMemory(int index)
    {
        MemStrengths[index] = 10.0f;    //This starts the memory degrading.
    }
}

public class MemoryNode
{
    public MemoryNode(string uid, MemoryType mt)
    {
        UID = uid;
        MemoryType = mt;
    }

    public MemoryNode(MemoryCue memInfo, MemoryType mt)
    {
        MemoryCue = memInfo;
        UID = memInfo.UniqueNodeID;
        RelatedCues = memInfo.CueMarkers;
        MemoryType = mt;
    }

    public MemoryCue MemoryCue;
    public string[] RelatedCues;
    public string UID;              //This is the unique identifier for the node.
    //public Locations LastLocation;
    //public Vector3 LastPosition;

    public MemoryType MemoryType;   //This tells us what type of memory this is about.

    public virtual void UpdateMemory(MemoryGraph memoryGraph, MemoryGraphNode retainedMemory, MemoryCue memCue, float UpdateFrequency)
    {
        //LastLocation = memCue.CurrentLocation;

        //if(memCue.CachedTransform != null)
        //    LastPosition = memCue.CachedTransform.position;

        retainedMemory.LastLocation = memCue.CurrentLocation;

        if (memCue.CachedTransform != null)
            retainedMemory.LastPosition = memCue.CachedTransform.position;

        retainedMemory.LastUpdated = Time.time;

        //Debug.Log("Making a memory connection between " + memCue.UniqueNodeID + " and " + LastLocation.ToString() + " - " + UpdateFrequency);

        //if (UpdateFrequency <= 0.0f)    //We want to strengthen the memory between the specific item and the place it has been observed.
        //    return;

        MemoryGraphNode locNode;
        string locString = retainedMemory.LastLocation.ToString();

        if (!memoryGraph.Contains(locString))
        {
            locNode = memoryGraph.AddNamedNodeToGraph(new LocationNode(retainedMemory.LastLocation));
            memoryGraph.AddUndirectedEdge(locNode, retainedMemory);
        }

        if (UpdateFrequency <= 0.0f)    //We want to strengthen the memory between the specific item and the place it has been observed.
            return;

        if (memoryGraph.Contains(locString))
            locNode = memoryGraph.GetNamedNodeFromGraph(locString);
        else
            locNode = memoryGraph.AddNamedNodeToGraph(new LocationNode(retainedMemory.LastLocation));

        //First make a connection between the place and the item.
        int index = locNode.Neighbours.IndexOf(retainedMemory);
        if (index == -1)
        {
            //Somehow the cue relationship has been broken!?
            //Rebuild it!
            memoryGraph.AddDirectedEdge(locNode, retainedMemory, UpdateFrequency, UpdateFrequency);
        }
        else
        {
            //Strengthing the memory based on the length of time it was active in the working set.
            locNode.StrengthenMemory(index, UpdateFrequency);
            //locNode.StrengthenOpinion(index, UpdateFrequency);   //We keep opinion strength updating here because it represents how strongly the agent believes the item / character etc. is connected to this location.
            //Debug.Log(string.Format("Node: {0}, NS: {1}, ES: {2}", retainedMemory.UID, locNode.NodeStrengths[index], locNode.EdgeStrengths[index]));
            //Debug.Log(string.Format("NS: {0}, ES: {1}", locNode.NodeStrengths[index], cueNode.EdgeStrengths[index]));
        }

        //Then between the item and the place.
        index = retainedMemory.Neighbours.IndexOf(locNode);
        if (index == -1)
        {
            //Somehow the cue relationship has been broken!?
            //Rebuild it!
            memoryGraph.AddDirectedEdge(retainedMemory, locNode, UpdateFrequency, UpdateFrequency);
        }
        else
        {
            //Strengthing the memory based on the length of time it was active in the working set.
            retainedMemory.StrengthenMemory(index, UpdateFrequency);
            //retainedMemory.StrengthenOpinion(index, UpdateFrequency);   //We keep opinion strength updating here because it represents how strongly the agent believes the item / character etc. is connected to this location.
            //Debug.Log(string.Format("Node: {0}, NS: {1}, ES: {2}", locNode.UID, retainedMemory.NodeStrengths[index], retainedMemory.EdgeStrengths[index]));
            //Debug.Log(string.Format("NS: {0}, ES: {1}", locNode.NodeStrengths[index], cueNode.EdgeStrengths[index]));
        }
    }

    public virtual float GetInitialOpinion()
    {
        return 0.5f;
    }
}

public class ItemNode : MemoryNode
{
    ItemStatus status;
    CharacterCue owner;
    float durability;

    public ItemNode(ItemCue itemInfo)
        : base(itemInfo, MemoryType.Item)
    {
        status = itemInfo.Status;
        owner = itemInfo.Owner;
        durability = itemInfo.Durability;
    }

    public override float GetInitialOpinion()
    {
        return durability;
    }

    public override void UpdateMemory(MemoryGraph memoryGraph, MemoryGraphNode retainedMemory, MemoryCue memCue, float UpdateFrequency)
    {
        ItemCue itemInfo = (ItemCue)memCue;

        status = itemInfo.Status;
        owner = itemInfo.Owner;
        durability = itemInfo.Durability;

        if (durability < 0.0f)
        {
            //Once durability starts to fall, a agent's affinity with an item will also drop.
            memoryGraph.UpdateCueOpinion(retainedMemory.MemoryNode, -UpdateFrequency);
        }

        base.UpdateMemory(memoryGraph, retainedMemory, memCue, UpdateFrequency);

        if (UpdateFrequency <= 0.0f || itemInfo.Owner == null)    //We want to strengthen the memory between the specific item and the place it has been observed.
            return;

        MemoryGraphNode charNode;
        if (memoryGraph.Contains(itemInfo.Owner.UniqueNodeID))
            charNode = memoryGraph.GetNamedNodeFromGraph(itemInfo.Owner.UniqueNodeID);
        else
            charNode = memoryGraph.AddNamedNodeToGraph(new CharacterNode(itemInfo.Owner));

        //First make a connection between the character and the item.
        int index = charNode.Neighbours.IndexOf(retainedMemory);
        if (index == -1)
            memoryGraph.AddDirectedEdge(charNode, retainedMemory, UpdateFrequency, UpdateFrequency);
        else
        {
            //Strengthing the memory based on the length of time it was active in the working set.
            charNode.StrengthenMemory(index, UpdateFrequency);
            //charNode.OpinionStrengths[index] += UpdateFrequency;
        }

        //Then between the item and the character.
        index = retainedMemory.Neighbours.IndexOf(charNode);
        if (index == -1)
            memoryGraph.AddDirectedEdge(retainedMemory, charNode, UpdateFrequency, UpdateFrequency);
        else
        {
            //Strengthing the memory based on the length of time it was active in the working set.
            retainedMemory.StrengthenMemory(index, UpdateFrequency);
            //retainedMemory.OpinionStrengths[index] += UpdateFrequency;
        }
    }
}

public class CharacterNode : MemoryNode
{
    public bool IsMonster;
    public CharacterNode(CharacterCue charInfo)
        : base(charInfo, MemoryType.Character)
    {}

    public CharacterNode(string monsterUID)
        : base(monsterUID, MemoryType.Character)
    {
        IsMonster = true;
        RelatedCues = new string[0];
    }

    public override void UpdateMemory(MemoryGraph memoryGraph, MemoryGraphNode retainedMemory, MemoryCue memCue, float UpdateFrequency)
    {
        if (IsMonster)
            return;         //Current, we've no need to update monsters.

        CharacterCue charInfo = (CharacterCue)memCue;
        base.UpdateMemory(memoryGraph, retainedMemory, memCue, UpdateFrequency);        
    }

}

public class LocationNode : MemoryNode
{
    public LocationNode(Locations wl)
        : base(wl.ToString(), MemoryType.Location)
    {
        RelatedCues = new string[] { string.Format("LOC_{0}", UID) };
    }

    public LocationNode(LocationCue locInfo)
        : base(locInfo, MemoryType.Location)
    {
        RelatedCues = locInfo.CueMarkers;    
    }

    public override void UpdateMemory(MemoryGraph memoryGraph, MemoryGraphNode retainedMemory, MemoryCue memCue, float UpdateFrequency)
    {
        LocationCue locInfo = (LocationCue)memCue; 
        
        base.UpdateMemory(memoryGraph, retainedMemory, memCue, UpdateFrequency);        
    }

}

public class EventNode : MemoryNode
{
    public int[] eventParams;

    public EventNode(EventCue evnCue)
        : base(evnCue, MemoryType.Event)
    {
        //Event Cues could also contain the neccessary information... but possibly not the unique params?
    }

    public EventNode(string uid, int[] eParams, string[] rCues)
        : base(uid, MemoryType.Event)
    {
        RelatedCues = rCues;
        eventParams = eParams;
    }

    public override void UpdateMemory(MemoryGraph memoryGraph, MemoryGraphNode retainedMemory, MemoryCue memCue, float UpdateFrequency)
    {
        EventCue eventInfo = (EventCue)memCue;

        base.UpdateMemory(memoryGraph, retainedMemory, memCue, UpdateFrequency);
    }
}