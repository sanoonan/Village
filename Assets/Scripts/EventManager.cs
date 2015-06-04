using UnityEngine;
using System.Collections;

public enum Events
{
    FrogAttack,
};

public class EventManager : MonoBehaviour 
{
    public static EventManager Instance;

    public MemoryGraph SharedEventGraph;

    public GameObject tFrogRoot;
    public EventTag[] tLargeFrogs;
    public EventTag[] tSmallFrogs;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
            throw new UnityException("Location Manager has already been declared! It cannot exist twice!");
        }

        SharedEventGraph = new MemoryGraph();
    }
    void Start()
    {
        tFrogRoot.SetActive(false);
        TriggerEvent(Events.FrogAttack);
    }

    public void TriggerEvent(Events curEvent)
    {
        //In a real game, this would likely be loaded from a script. 
        //For now we only have one event, so we can simplify it.
        StartCoroutine(HandleEvent(tFrogRoot));
    }

    IEnumerator HandleEvent(GameObject goTarget)
    {
        //Debug.Log("Starting event!");
        goTarget.SetActive(true);               //We enable the frogs.

        yield return new WaitForSeconds(5.0f);  //We wait for a moment.

        int layerMask = 1 << 11;
        // This would cast rays only against colliders in layer 11, the Memory Layer.
        // But instead we want to collide against everything except layer 11. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        //Creating the event within the shared event graph.
        //1. We create the core event node.
        //2. We add all the large enemy frogs to the graph, and link them to the event.
        //3. We add the location of the event to the graph, and link it to the event.
        //4. We create the sub-event, and add it to the graph.
        //5. We link the sub-event to the location.
        //6. We add all the small enemy frogs to the graph, and link them to the sub event.
        //7. We then generate the personalised memories for all agents in the area.

        EventNode coreSharedEvent = new EventNode("FrogAttack", new int[] { 3 }, new string[] { "EVN_FrogAttack" });
        MemoryGraphNode coreSharedEventNode = EventManager.Instance.SharedEventGraph.AddNamedNodeToGraph(coreSharedEvent);

        //We populate the shared event graph with info about the event.
        for (int i = 0; i < tLargeFrogs.Length; i++)
        {
            CharacterNode frogNode = new CharacterNode(tLargeFrogs[i].Tag);
            MemoryGraphNode frogEventNode = EventManager.Instance.SharedEventGraph.AddNamedNodeToGraph(frogNode);

            EventManager.Instance.SharedEventGraph.AddUndirectedEdge(coreSharedEventNode, frogEventNode, 11.0f);
        }

        LocationNode lNode = new LocationNode(Locations.SouthernPinnusula);
        MemoryGraphNode locationGraphNode = EventManager.Instance.SharedEventGraph.AddNamedNodeToGraph(lNode);
        EventManager.Instance.SharedEventGraph.AddUndirectedEdge(coreSharedEventNode, locationGraphNode, 11.0f);

        EventNode coreSharedSubEvent = new EventNode("SmallFrogEvent", new int[] { 17 }, new string[] { "Monsters" });
        MemoryGraphNode coreSharedSubEventNode = EventManager.Instance.SharedEventGraph.AddNamedNodeToGraph(coreSharedSubEvent);
        EventManager.Instance.SharedEventGraph.AddUndirectedEdge(coreSharedEventNode, coreSharedSubEventNode, 11.0f);
        EventManager.Instance.SharedEventGraph.AddUndirectedEdge(coreSharedSubEventNode, locationGraphNode, 11.0f);

        for (int i = 0; i < tSmallFrogs.Length; i++)
        {
            CharacterNode frogNode = new CharacterNode(tSmallFrogs[i].Tag);
            MemoryGraphNode frogEventNode = EventManager.Instance.SharedEventGraph.AddNamedNodeToGraph(frogNode);

            EventManager.Instance.SharedEventGraph.AddUndirectedEdge(coreSharedSubEventNode, frogEventNode, 11.0f);
        }

        for (int i = 0; i < AgentManager.Instance.GetAgentCount(); i++)
        {
            CharacterDetails charDetails = AgentManager.Instance.GetAgent(i);
            MemoryGraph charGraph = charDetails.IsPlayer ? charDetails.PlayerAgent.MindsEye.MemoryGraph : charDetails.CognitiveAgent.MindsEye.MemoryGraph;

            EventNode coreEvent = new EventNode("FrogAttack", null, new string[] { "EVN_FrogAttack" });
            MemoryGraphNode coreEventNode = charGraph.AddNamedNodeToGraph(coreEvent);

            string eventLocation = Locations.SouthernPinnusula.ToString();

            bool sawEvent = false;
            int frogCount = 0;
            for (int x = 0; x < tLargeFrogs.Length; x++)
            {
                RaycastHit hit;
                Vector3 direction = tLargeFrogs[x].cTransform.position - charDetails.HeadTarget.position;
                if (Physics.Raycast(charDetails.HeadTarget.position, direction, out hit, Mathf.Infinity, layerMask))
                {
                    //Debug.Log(charDetails.CharCue.UniqueNodeID + " - " + hit.transform.name);
                    EventTag eTag = hit.transform.GetComponent<EventTag>();
                    if (eTag != null && eTag.Tag == tLargeFrogs[x].Tag)
                        ++frogCount;
                }
            }

            MemoryGraphNode largeFrogNode = null;
            if (frogCount > 0)
            {
                sawEvent = true;
                //Create Large Frogs sub-event!
                EventNode largeFrogEvent = new EventNode("LargeFrogEvent", new int[] { frogCount }, new string[] { "Monsters" });
                largeFrogNode = charGraph.AddNamedNodeToGraph(largeFrogEvent);
                charGraph.AddUndirectedEdge(coreEventNode, largeFrogNode, 11.0f, 1.0f);        //Creates a strong connection between the core event and the optional sub-event.  

            }

            //Debug.Log(charDetails.name + " - I saw " + frogCount + " huge frogs!");

            frogCount = 0;
            for (int x = 0; x < tSmallFrogs.Length; x++)
            {
                RaycastHit hit;
                Vector3 direction = tSmallFrogs[x].cTransform.position - charDetails.HeadTarget.position;
                if (Physics.Raycast(charDetails.HeadTarget.position, direction, out hit, direction.magnitude, layerMask))
                {
                    //Debug.Log(charDetails.CharCue.UniqueNodeID + " - " + hit.transform.name);
                    EventTag eTag = hit.transform.GetComponent<EventTag>();

                    //if(eTag != null)
                    //Debug.Log(charDetails.CharCue.UniqueNodeID + " : " + eTag.Tag);

                    if (eTag != null && eTag.Tag == tSmallFrogs[x].Tag)
                        ++frogCount;
                }
            }

            MemoryGraphNode smallFrogNode = null;
            if (frogCount > 0)
            {
                sawEvent = true;
                //Create small frogs sub-event!
                EventNode smallFrogEvent = new EventNode("SmallFrogEvent", new int[] { frogCount }, new string[] { "Monsters" });
                smallFrogNode = charGraph.AddNamedNodeToGraph(smallFrogEvent);
                charGraph.AddUndirectedEdge(coreEventNode, smallFrogNode, 11.0f, 1.0f);        //Creates a strong connection between the core event and the optional sub-event.

            }

            if (sawEvent)
            {
                MemoryGraphNode retainedMemory;
                if (charGraph.Contains(eventLocation) == false)
                    retainedMemory = charGraph.AddNamedNodeToGraph(new LocationNode(Locations.SouthernPinnusula));  //This is a new memory! Add it to our memory graph.
                else
                    retainedMemory = charGraph.GetNamedNodeFromGraph(eventLocation);

                charGraph.UpdateCueOpinion(retainedMemory.MemoryNode, -5.0f);               //The agent will have a strong negative opinion about the area where they attack.
                charGraph.AddUndirectedEdge(coreEventNode, retainedMemory, 11.0f, 1.0f);     //They will also make a strong connection between the area and the frogs.

                if (largeFrogNode != null)
                    charGraph.AddDirectedEdge(largeFrogNode, retainedMemory, 11.0f, 1.0f);          //Creates a strong (but one-way, since we already connect the core event) connection to the location.

                if (smallFrogNode != null)
                    charGraph.AddDirectedEdge(smallFrogNode, retainedMemory, 11.0f, 1.0f);          //Creates a strong (but one-way, since we already connect the core event) connection to the location.
            }
        }

        //We broadcast the Frog Sight Event and create the Shared Events and personalised Sub Events.
        //We also broadcast the start of the event (so people flee).

        yield return new WaitForSeconds(5.0f);

        //We broadcast the end of the event, and hide the frogs again.
        goTarget.SetActive(false);
        //Debug.Log("Ending event!");

        //EventManager.Instance.SharedEventGraph.PrintGraph();
    }
}
