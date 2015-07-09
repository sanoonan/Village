using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RelationshipManager : MonoBehaviour
{
    Dictionary<int, RelationshipVector> relationships;

    List<int> parentsIds;
    List<int> childrenIds;

    //current spouse is the most recent, ie. the last element in the list
    List<int> spousesIds;

    private int myId = -1;

    void Awake()
    {
        relationships = new Dictionary<int, RelationshipVector>();
    }

    void Start()
    {
    }

    void Update()
    {
    }

    void LateUpdate()
    {
    }

    public void assignMyId(int agentId)
    {
        myId = agentId;
    }

    public void addNewRelationship(int agentId)
    {
        if (checkIfMe(agentId))
            return;

        if(checkIfAgentKnown(agentId))
            return;

        relationships.Add(agentId, new RelationshipVector());
    }


    private bool checkIfAgentKnown(int agentId)
    {
        return relationships.ContainsKey(agentId);
    }


    private bool checkIfMe(int agentId)
    {
        if (agentId == myId)
        {
            Debug.LogError("I cant have a relationship with myself");
            return true;
        }
        return false;
    }


    public bool isConnectedTo(int agentId)
    {
        if ((isParent(agentId)) || (isChild(agentId)) || (isSpouse(agentId)))
            return true;

        return false;
    }

    private bool isParent(int agentId)
    {
        return parentsIds.Contains(agentId);
    }
    private bool isChild(int agentId)
    {
        return childrenIds.Contains(agentId);
    }
    private bool isSpouse(int agentId)
    {
        return spousesIds.Contains(agentId);
    }


    public bool isUnconnected()
    {
        if (hasParent())
            return true;

        if (hasChild())
            return true;

        if (hasSpouse())
            return true;

        return false;
    }

    private bool hasParent()
    {
        if (parentsIds.Count > 0)
            return true;
        
        return false;
    }

    private bool hasChild()
    {
        if (childrenIds.Count > 0)
            return true;

        return false;
    }

    private bool hasSpouse()
    {
        if (spousesIds.Count > 0)
            return true;

        return false;
    }


 

}