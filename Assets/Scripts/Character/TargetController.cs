//Placed on the player (rolled into the Agent updates for NPCs) to have the player character automatically look towards the nearest NPC using the HeadLookController component.﻿
using UnityEngine;
using System.Collections;

public class TargetController : MonoBehaviour
{
    internal HeadLookController ViewController;

    internal CharacterDetails CharDetails;

    internal Transform Transform;
    internal GameObject Self;
    internal Animation Animation;

    void Awake()
    {
        Transform = GetComponent<Transform>();
        Self = gameObject;
        CharDetails = GetComponent<CharacterDetails>();

        Animation = GetComponent<Animation>();
        ViewController = GetComponent<HeadLookController>();
    }

    void LateUpdate()
    {
        int closestID = -1;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < AgentManager.Instance.GetAgentCount(); i++)
        {
            if (i == CharDetails.AgentID)
                continue;

            float dist = Vector2.Distance(Transform.position, AgentManager.Instance.GetAgent(i).HeadTarget.position);
            if (dist < closestDistance)
            {
                closestID = i;
                closestDistance = dist;
            }

        }

        if (closestID == -1)
            ViewController.enabled = false;
        else
            ViewController.UpdateTarget(AgentManager.Instance.GetAgent(closestID));
    }
}