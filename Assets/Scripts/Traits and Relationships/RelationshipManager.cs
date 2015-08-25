using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RelationshipManager : MonoBehaviour
{
    Dictionary<int, RelationshipVector> _relationships;

    List<int> _parentsIds;
    List<int> _childrenIds;

    List<int> _formerSpouseIds;

    private bool _hasSpouse = false;
    private int _spouseId = -1;
    
    private int myId = -1;

    void Awake()
    {
        _relationships = new Dictionary<int, RelationshipVector>();
        _parentsIds = new List<int>();
        _childrenIds = new List<int>();
        _formerSpouseIds = new List<int>();
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

    public void AssignMyId(int agentId)
    {
        myId = agentId;
    }


    #region ADD RELATIONSHIPS
    public void AddNewRelationship( CharacterCue agent )
    {
        AddNewRelationship( agent.GetAgentId() );
    }
    public void AddNewRelationship(int agentId)
    {
        if (IsMe(agentId))
            return;

        if(IsAgentKnown(agentId))
            return;

        _relationships.Add(agentId, new RelationshipVector());
    }
    public void AddNewRandomRelationship( int agentId )
    {
        if ( IsMe( agentId ) )
            return;

        if ( IsAgentKnown( agentId ) )
            return;

        RelationshipVector relationshipVector = new RelationshipVector();
        relationshipVector.MakeRandomExistingRelationship();
        _relationships.Add( agentId, relationshipVector );
    }

    public void AddParent( CharacterCue agent )
    {
        AddParent( agent.GetAgentId() );
    }
	public void AddParent( int agentId )
	{
        if ( IsMe( agentId ) )
            return;
        if ( HasEnoughParents() )
            return;
        if ( IsConnectedTo( agentId ) )
            return;

		AddNewRelationship( agentId );
		_parentsIds.Add( agentId );
	}
    public void AddChild( CharacterCue agent )
    {
        AddChild( agent.GetAgentId() );
    }
	public void AddChild( int agentId )
	{
        if ( IsMe( agentId ) )
            return;
        if ( IsConnectedTo( agentId ) )
            return;

		AddNewRelationship( agentId );
		_childrenIds.Add( agentId );
	}
    public void AddFormerSpouse( CharacterCue agent )
    {
        AddFormerSpouse( agent.GetAgentId() );
    }
	public void AddFormerSpouse( int agentId )
	{
        if ( IsMe( agentId ) )
            return;
        if ( IsFamily( agentId ) )
            return;
        if( IsFormerSpouse( agentId ) )
            return;

		AddNewRelationship( agentId );
		_formerSpouseIds.Add( agentId );
	}
    public void AddSpouse( CharacterCue agent )
    {
        AddSpouse( agent.GetAgentId() );
    }
    public void AddSpouse( int agentId )
    {
        if ( IsMe( agentId ) )
            return;
        if ( IsFamily( agentId ) )
            return;
        if ( IsSpouse( agentId ) )
            return;

        AddNewRelationship( agentId );

        if ( HasSpouse() )
        {
            RelationshipManager spouseRelationshipManager = FamilyTreeController.Instance.GetAgentRelationshipManager( _spouseId );
            spouseRelationshipManager.EndSpouseRelationship();

            EndSpouseRelationship();
        }

        _hasSpouse = true;
        _spouseId = agentId;
    }
    public void EndSpouseRelationship()
    {
        if ( !HasSpouse() )
            return;

        if ( !_formerSpouseIds.Contains( _spouseId ) )
            _formerSpouseIds.Add( _spouseId );

        _hasSpouse = false;
    }
    #endregion


    public bool IsAgentKnown(int agentId)
    {
        return _relationships.ContainsKey(agentId);
    }


    public bool IsMe(int agentId)
    {
        if (agentId == myId)
            return true;
        
        return false;
    }


    public bool IsConnectedTo(int agentId)
    {
        if ( ( IsParent( agentId ) ) || ( IsChild( agentId ) ) || ( IsSpouse( agentId ) ) )
            return true;

        return false;
    }
    public bool IsFamily( int agentId )
    {
        if( ( IsParent( agentId ) ) || ( IsChild( agentId ) ) )
            return true;

        return false;
    }

    private bool IsParent(int agentId)
    {
        return _parentsIds.Contains(agentId);
    }
    private bool IsChild(int agentId)
    {
        return _childrenIds.Contains(agentId);
    }
    private bool IsFormerSpouse( int agentId )
    {
        return _formerSpouseIds.Contains( agentId );
    }
    private bool IsSpouse(int agentId)
    {
        if ( ( _hasSpouse ) && ( _spouseId == agentId ) )
            return true;

        return false;
    }


    public bool IsUnconnected()
    {
		if (HasParent())
            return true;

		if (HasChild())
            return true;

		if (HasSpouse())
            return true;

        return false;
    }

	public bool HasEnoughParents()
	{
		if( _parentsIds.Count >= 2 )
			return true;

		return false;
	}

    private bool HasParent()
    {
        if (_parentsIds.Count > 0)
            return true;
        
        return false;
    }

    private bool HasChild()
    {
        if (_childrenIds.Count > 0)
            return true;

        return false;
    }

    private bool HasFormerSpouse()
    {
        if ( _formerSpouseIds.Count > 0 )
            return true;

        return false;
    }

    public bool HasSpouse()
    {
        return _hasSpouse;
    }
    public int GetSpouseId()
    {
        if ( !HasSpouse() )
            return -1;

        return _spouseId;
    }

    public List<int> GetAgentIdsWithFamiliarity( float value, bool greaterThan )
    {
        List<int> suitableAgents = new List<int>();
        List<KeyValuePair<int, RelationshipVector>> relationshipList = _relationships.ToList();

        int numRelationships = relationshipList.Count;
        for ( int i = 0; i < numRelationships; i++ )
        {
            RelationshipVector currRelationship = relationshipList[i].Value;
            float currValue = currRelationship.GetFamiliarity();

            if ( greaterThan )
            {
                if ( currValue >= value )
                    suitableAgents.Add( relationshipList[i].Key );
            }
            else
            {
                if ( currValue <= value )
                    suitableAgents.Add( relationshipList[i].Key );
            }
        }
        return suitableAgents;
    }

    public List<int> GetAgentIdsWithRelationshipTraits( RelationshipTrait relationshipTrait, float value, bool greaterThan )
    {
        List<int> suitableAgents = new List<int>();
        List<KeyValuePair<int, RelationshipVector>> relationshipList = _relationships.ToList();

        int numRelationships = relationshipList.Count;
        for ( int i = 0; i < numRelationships; i++ )
        {
            RelationshipVector currRelationship = relationshipList[i].Value;
            float currValue = currRelationship.GetTraitValue( relationshipTrait );

            if ( greaterThan )
            {
                if ( currValue >= value )
                    suitableAgents.Add( relationshipList[i].Key );
            }
            else
            {
                if ( currValue <= value )
                    suitableAgents.Add( relationshipList[i].Key );
            }
        }
        return suitableAgents;
    }

    public RelationshipVector GetRelationshipById( int id )
    {
        RelationshipVector vector;
        bool hasRelationship = _relationships.TryGetValue( id, out vector );
        if ( hasRelationship )
            return vector;

        return null;
    }

    public void DeleteRelationshipWithAgent( int agentId )
    {
        if ( IsMe( agentId ) )
            return;

        RelationshipVector vector = GetRelationshipById( agentId );
        if ( vector == null )
            return;

        if ( IsSpouse( agentId ) )
            EndSpouseRelationship();

        if ( IsFormerSpouse( agentId ) )
            _formerSpouseIds.Remove( agentId );

        if( IsParent( agentId ) )
            _parentsIds.Remove( agentId );

        if ( IsChild( agentId ) )
            _childrenIds.Remove( agentId );

        _relationships.Remove( agentId );
    }
}