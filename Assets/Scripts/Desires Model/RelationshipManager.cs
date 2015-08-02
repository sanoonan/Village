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

    public void AssignMyId(int agentId)
    {
        myId = agentId;
    }

    public void AddNewRelationship(int agentId)
    {
        if (IsMe(agentId))
            return;

        if(IsAgentKnown(agentId))
            return;

        relationships.Add(agentId, new RelationshipVector());
    }

	public void AddParent( int agentId )
	{
		AddNewRelationship( agentId );
		parentsIds.Add( agentId );
	}
	public void AddChild( int agentId )
	{
		AddNewRelationship( agentId );
		childrenIds.Add( agentId );
	}
	public void AddSpouse( int agentId )
	{
		AddNewRelationship( agentId );
		spousesIds.Add( agentId );
	}

    private bool IsAgentKnown(int agentId)
    {
        return relationships.ContainsKey(agentId);
    }


    private bool IsMe(int agentId)
    {
        if (agentId == myId)
        {
            return true;
        }
        return false;
    }


    public bool IsConnectedTo(int agentId)
    {
        if ( ( IsParent( agentId ) ) || ( IsChild( agentId ) ) || ( IsSpouse( agentId ) ) )
            return true;

        return false;
    }

    private bool IsParent(int agentId)
    {
        return parentsIds.Contains(agentId);
    }
    private bool IsChild(int agentId)
    {
        return childrenIds.Contains(agentId);
    }
    private bool IsSpouse(int agentId)
    {
        return spousesIds.Contains(agentId);
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
		if( parentsIds.Count >= 2 )
			return true;

		return false;
	}

    private bool HasParent()
    {
        if (parentsIds.Count > 0)
            return true;
        
        return false;
    }

    private bool HasChild()
    {
        if (childrenIds.Count > 0)
            return true;

        return false;
    }

    private bool HasSpouse()
    {
        if (spousesIds.Count > 0)
            return true;

        return false;
    }


 

}