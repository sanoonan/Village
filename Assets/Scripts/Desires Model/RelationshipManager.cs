using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RelationshipManager : MonoBehaviour
{
    Dictionary<int, RelationshipVector> relationships;

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

    public void addRandomRelationship(int agentId)
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

}