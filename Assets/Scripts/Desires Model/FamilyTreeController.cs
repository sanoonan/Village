using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FamilyTreeController : MonoBehaviour
{
    private AgentManager agentManager;
    private int numAgents;

    void Awake()
    {
        agentManager = AgentManager.Instance;
    }

    void Start()
    {
        numAgents = agentManager.GetAgentCount();
    }

    void Update()
    {
        numAgents = agentManager.GetAgentCount();
    }

    void LateUpdate()
    {
    }


  

    private int getNumUnconnectedAgents()
    {
        int numUnconnectedAgents = 0;

        for (int i = 0; i < numAgents; i++)
        {
            RelationshipManager agent = getAgentRelationshipManager(i);
            if (agent.isUnconnected())
                numUnconnectedAgents++;
        }

        return numUnconnectedAgents;
    }


    private RelationshipManager getAgentRelationshipManager(int id)
    {
        CharacterDetails character = agentManager.GetAgent(id);

        if (character == null)
            Debug.LogError("ERROR: FamilyTreeController:getAgentRelationshipManager. There's no agent with this id.");

        GameObject characterObject = character.gameObject;

        return characterObject.GetComponent<RelationshipManager>();
    }

 

}