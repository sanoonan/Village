//Updates the material used by an NPC so their face displays different emotions.
using UnityEngine;
using System.Collections;

public enum Emotion
{
    Neutral,
    Happy,
    Angry,
    Sad,
    Shy
};

public class EmotionDisplayTest : MonoBehaviour 
{
    public Material[] emotionMaterials;
    Renderer npcRenderer;
    int index;

	void Start () 
    {
        npcRenderer = gameObject.GetComponent<Renderer>();
	}

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width / 2 - 100, Screen.height - 70, 200, 50));
        GUI.Box(new Rect(10, 10, 190, 40), "");

        GUI.Label(new Rect(62, 20, 100, 20), ((Emotion)index).ToString());

        if (GUI.Button(new Rect(15, 15, 30, 30), "<<"))
        {
            index--;
            if (index < 0)
            {
                index = emotionMaterials.Length - 1;
            }
            npcRenderer.material = emotionMaterials[index];
        }

        if (GUI.Button(new Rect(165, 15, 30, 30), ">>"))
        {
            index++;
            if (index > emotionMaterials.Length - 1)
            {
                index = 0;
            }
            npcRenderer.material = emotionMaterials[index];
        }
        GUILayout.EndArea();
    }
}