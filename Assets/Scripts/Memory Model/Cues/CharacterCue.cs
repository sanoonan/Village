using UnityEngine;
using System.Collections;

public class CharacterCue : MemoryCue
{
    public AgentInventory Inventory;
    public CharacterDetails CharDetails;

    public int GetAgentId()
    {
        return CharDetails.AgentID;
    }
}
