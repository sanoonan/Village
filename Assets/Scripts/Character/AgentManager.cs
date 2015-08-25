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

    BetterList<CharacterDetails> agents;

	void Awake()
	{
		if(instance != null && instance != this)
			Destroy(this);

		instance = this;

        agents = new BetterList<CharacterDetails>();
	}

    void OnDestroy()
    {
        instance = null;
    }

    public int AddAgent(CharacterDetails agent)
	{
        if ( agent.IsPlayer() )
            return -1;

		agents.Add(agent);
		return agents.size - 1;
	}

    public CharacterDetails GetAgent(int agentID)
	{
		return agents[agentID];
	}

    public string GetAgentNameById(int agentId)
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

            if (!agents[i]._isPlayer && agents[i]._cognitiveAgent.CharacterCue.UniqueNodeID == targetUID)
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

    public void UpdateQuestGiversPossibility()
    {
        for ( int i = 0; i < agents.size; i++ )
        {
            CharacterDetails currNpc = agents[i];
            if ( currNpc.IsPlayer() )
                return;

            GameObject questStar = currNpc.gameObject.transform.FindChild( "QuestStar" ).gameObject;
            QuestGiver questGiver = questStar.GetComponent<QuestGiver>();
            questGiver.UpdateQuestPossibility();
        }
    }

    public List<int> GetAgentIdsWithTraits( Trait trait, float value, bool greaterThan )
    {
        List<int> suitableAgents = new List<int>();

        int numAgents = GetAgentCount();
        for ( int i = 0; i < numAgents; i++ )
        {
            if ( ( agents[i].IsPlayer() ) || ( !agents[i].IsAlive() ) )
                continue;

            float currValue = agents[i].GetTraitsManager().GetTraitValue( trait );

            if ( greaterThan )
            {
                if ( currValue >= value )
                    suitableAgents.Add( i );
            }
            else
            {
                if ( currValue <= value )
                    suitableAgents.Add( i );
            }
        }
        return suitableAgents;
    }

    public void DeleteAllRelationshipsWithAgent( int agentId )
    {
        int numAgents = GetAgentCount();
        for ( int i = 0; i < numAgents; i++ )
        {
            agents[i].GetRelationshipManager().DeleteRelationshipWithAgent( agentId );
        }
    }
}