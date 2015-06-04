using UnityEngine;
using System.Collections;

public enum TelegramType
{
	Greeting,
    MeetingRequest,
    MeetingComplete,
    QuestTrade,
    DiscussEvent
};

public enum TargetType
{
	Agent,
	Patient,
	WorldObject
};

public struct Telegram
{
	public float DispatchTime;
	public int Sender;
	public int Receiver;
	public TelegramType TelegramType;
    public Vector3? Location;
    public bool IsResponse;

    public Telegram(int s, int r, TelegramType tt, bool ir, Vector3? l, float dt)
	{
		DispatchTime = dt;
		Sender = s;
		Receiver = r;
		TelegramType = tt;
        IsResponse = ir;
		Location = l;
	}
};

public static class Message 
{
	public static BetterList<Telegram> telegramQueue = new BetterList<Telegram>();

	public static void DispatchMessage(int sender, int receiver, TelegramType telegramType, bool isResponse = false,  Vector3? location = null, float delay = 0.0f)
	{
		Telegram telegram = new Telegram (sender, receiver, telegramType, isResponse, location, delay);

		if (delay <= 0.0f) 
		{
			CognitiveAgent rAgent = AgentManager.Instance.GetAgent(receiver).CognitiveAgent;
			SendMessage(rAgent, telegram);
		}
		else
		{
			telegram.DispatchTime = Time.time + delay;
			telegramQueue.Add(telegram);
		}
	}

	public static void SendDelayedMessages()
	{
		for(int i = telegramQueue.size-1; i >= 0; i--)
		{
			if(telegramQueue[i].DispatchTime <= Time.time)
			{
				CognitiveAgent rAgent = AgentManager.Instance.GetAgent(telegramQueue[i].Receiver).CognitiveAgent;
				SendMessage(rAgent, telegramQueue[i]);
				telegramQueue.RemoveAt(i);
			}
		}
	}

	public static void SendMessage(CognitiveAgent agent, Telegram telegram)
	{
        agent.HandleMessage(telegram);
        //if(agent.HandleMessage(telegram))
        //    Debug.Log("A message " + (telegram.IsResponse ? "response" : "") + " ( " + telegram.TelegramType + " ) was handled by " + agent.name);
        //else
        //    Debug.Log("A message " + (telegram.IsResponse ? "response" : "") + " ( " + telegram.TelegramType + " ) was left unhandled by " + agent.name);
	}
}
