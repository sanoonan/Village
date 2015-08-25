using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FamilyTreeController : MonoBehaviour
{
    private AgentManager agentManager;
    private int numAgents;
    public static FamilyTreeController Instance = null;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        agentManager = AgentManager.Instance;
        numAgents = agentManager.GetAgentCount();

        StartCoroutine( DelayedRandomiseRelationships() );
    }

    void Update()
    {
        numAgents = agentManager.GetAgentCount();
    }

    void LateUpdate()
    {
    }

    private IEnumerator DelayedRandomiseRelationships()
    {
        yield return new WaitForSeconds( 5.0f );
        numAgents = agentManager.GetAgentCount();
        RandomiseRelationships();
    }

    private void RandomiseRelationships()
    {
        int maxRelationships = numAgents - 2;
        for ( int i = 0; i < numAgents; i++ )
        {
            RelationshipManager agent = GetAgentRelationshipManager( i );
            int numRelationships = Random.Range( 0, maxRelationships );
            for ( int j = 0; j < numRelationships; j++ )
            {
                int randomAgentId = Random.Range( 0, numAgents );
                if ( agent.IsMe( randomAgentId ) )
                    continue;

                if ( agent.IsAgentKnown( randomAgentId ) )
                    continue;

                agent.AddNewRandomRelationship( randomAgentId );


                int randomConnectionNum = Random.Range( 0, 10 );
                if ( randomConnectionNum > 3 )
                    continue;

                if ( randomConnectionNum == 0 )
                    AddParent( i, randomAgentId );
                else if ( randomConnectionNum == 1 )
                    AddParent( randomAgentId, i );
                else if ( randomConnectionNum == 2 )
                    AddSpouses( i, randomAgentId );
                else if ( randomConnectionNum == 3 )
                    AddFormerSpouses( i, randomAgentId );
            }
        }
    }

    private int GetNumUnconnectedAgents()
    {
        int numUnconnectedAgents = 0;

        for (int i = 0; i < numAgents; i++)
        {
            RelationshipManager agent = GetAgentRelationshipManager(i);
            if (agent.IsUnconnected())
                numUnconnectedAgents++;
        }

        return numUnconnectedAgents;
    }

    public RelationshipManager GetAgentRelationshipManager(int id)
    {
        CharacterDetails character = agentManager.GetAgent(id);

        if (character == null)
            Debug.LogError("ERROR: FamilyTreeController:getAgentRelationshipManager. There's no agent with this id.");

        GameObject characterObject = character.gameObject;
        RelationshipManager relationshipManager = characterObject.GetComponent<RelationshipManager>();
        if ( relationshipManager == null )
            Debug.LogError( "ERROR: " + characterObject.name + " does not have an attached RelationshipManager" );

        return relationshipManager;
    }

    private void AddFormerSpouses( int spouse1, int spouse2 )
    {
        RelationshipManager spouseRM1 = GetAgentRelationshipManager( spouse1 );
        RelationshipManager spouseRM2 = GetAgentRelationshipManager( spouse2 );

        spouseRM1.AddFormerSpouse( spouse2 );
        spouseRM2.AddFormerSpouse( spouse1 );
    }
    private void AddSpouses( int spouse1, int spouse2 )
    {
        RelationshipManager spouseRM1 = GetAgentRelationshipManager( spouse1 );
        RelationshipManager spouseRM2 = GetAgentRelationshipManager( spouse2 );

        if ( spouseRM1 == null || spouseRM2 == null )
            return;

        spouseRM1.AddSpouse( spouse2 );
        spouseRM2.AddSpouse( spouse1 );
    }
    private void AddParent( int child, int parent )
    {
        RelationshipManager parentRM = GetAgentRelationshipManager( parent );
        RelationshipManager childRM = GetAgentRelationshipManager( child );

        if ( childRM.HasEnoughParents() )
            return;

        parentRM.AddChild( child );
        childRM.AddParent( parent );
    }
}