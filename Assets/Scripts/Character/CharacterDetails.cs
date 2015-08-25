using UnityEngine;
using System.Collections;

public class CharacterDetails : MonoBehaviour 
{
    //Allows us to emulate some of the Agent behaviour neccessary to allow the player to neatly be part of their discovery and animation systems.
    internal int AgentID;                   //This is the unique ID used by the messaging system, and is assigned when the Agent hooks up to recieve messages.

    public Transform GripTarget;         //The parent transform where items the agent pick up will be added to. This should be their hand transform.
    public Transform HeadTarget;         //The transform in the body which should be used for targeting when seeing if two agents can look / see each other.
    public PlayerController PlayerAgent;
    public CognitiveAgent _cognitiveAgent;
    public CharacterCue CharCue;
    public bool _isPlayer;
    private bool _isAlive = true;
    private TraitsManager _traitsManager;
    private RelationshipManager _relationshipsManager;
    private StateVector _stateVector;

    void Awake()
    {
        _traitsManager = gameObject.GetComponent<TraitsManager>();
        _relationshipsManager = gameObject.GetComponent<RelationshipManager>();
        _stateVector = gameObject.GetComponentInChildren<StateVector>();
    }

	// Use this for initialization
	void Start () 
    {
        AgentID = AgentManager.Instance.AddAgent(this);     //Adds the agent to the manager so it can recieve messages, and gains its unique agent id.

        RelationshipManager relationshipManager = gameObject.GetComponent<RelationshipManager>();
        
        if ( _isPlayer )
        {
            PlayerAgent = GetComponent<PlayerController>();
        }
        else
        {
            if ( relationshipManager != null )
                relationshipManager.AssignMyId( AgentID );

            _cognitiveAgent = GetComponent<CognitiveAgent>();
        }
	}

    public void Die()
    {
        _isAlive = false;

        QuestGiver questGiver = gameObject.GetComponentInChildren<QuestGiver>();
        questGiver.UpdateQuestPossibility();

        gameObject.SetActive( false );
    }

    public bool IsAlive()
    {
        return _isAlive;
    }
    public bool IsPlayer()
    {
        return _isPlayer;
    }
    public RelationshipManager GetRelationshipManager()
    {
        return _relationshipsManager;
    }
    public StateVector GetStateVector()
    {
        return _stateVector;
    }
    public TraitsManager GetTraitsManager()
    {
        return _traitsManager;
    }
}
