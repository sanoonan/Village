using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentManager : MonoBehaviour 
{
	private static AgentManager instance;

	public static AgentManager Instance
	{
		get { return instance ?? (instance = new GameObject ("*AgentManager").AddComponent<AgentManager> ());}
	}

    BetterList<CharacterDetails> agents = new BetterList<CharacterDetails>();

	void Awake()
	{
		if(instance != null && instance != this)
			Destroy(this);

		instance = this;
	}

    void OnDestroy()
    {
        instance = null;
    }

    public int AddAgent(CharacterDetails agent)
	{
		agents.Add(agent);
		return agents.size - 1;
	}

    public CharacterDetails GetAgent(int agentID)
	{
		return agents[agentID];
	}

    public string GetAgentName(int agentId)
    {
        CharacterDetails agentCharDetails = GetAgent(agentId);
        GameObject agentObject = agentCharDetails.gameObject;
        string agentName = agentObject.name;
        return agentName;
    }

    public int GetAgentIdByName(string agentName)
    {
        int numAgents = GetAgentCount();

        for (int i = 0; i < numAgents; i++)
        {
            string currAgentName = agents[i].gameObject.name;

            if (agentName == currAgentName)
                return i;
        }


        Debug.LogError("AgentManager:GetAgentIdByName - can't find agent with name '" + agentName + "'");
        return -1;
    }



    public CharacterDetails GetAgent(string targetUID)
    {
        for (int i = 0; i < agents.size; i++)
        {
            //Debug.Log(targetUID + " - " + agents[i]);
            //if (!agents[i].IsPlayer)
            //{
            //    Debug.Log(agents[i].CognitiveAgent);
            //    Debug.Log(agents[i].CognitiveAgent.CharacterCue);
            //    Debug.Log(agents[i].CognitiveAgent.CharacterCue.UniqueNodeID);
            //}

            if (!agents[i].IsPlayer && agents[i].CognitiveAgent.CharacterCue.UniqueNodeID == targetUID)
                return agents[i];
        }

        return null;
    }

    public int GetAgentCount()
	{
		return agents.size;
	}

    public bool RemoveAgent(CharacterDetails charDetails)
    {
        throw new UnityException("Can't yet handle the removal of agents. If this is neccessary, we'll need to look at a way of making IDs more dynamic.");

        //targetTransforms.RemoveAt(charDetails.AgentID);
        //return agents.RemoveAt(charDetails.AgentID);
    }
}