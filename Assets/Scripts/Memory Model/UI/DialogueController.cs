using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class DialogueController : MonoBehaviour
{
    public UILabel speakerLabel;
    public UILabel dialogueLabel;

    public AnimationClip idleAni;
    public AnimationClip talkAni;

    public Animation[] charAnis;
    public GameObject[] chars;

    string[] lines = new string[] 
    {
        "Good morning, #TARGET.",
        "Oh, good morning #TARGET. How are you?",
        "I'm doing good, just on my way to the [df6b37]{market|guild|item shop|weapon shop|observatory}[-] actually.",
        "There appears to be a new [df6b37]{merchant|mage|healer|adventurer}[-] in town. I noticed [df6b37]{him|her}[-] setting up earlier.",
        "{Is that so? I'll need to give them a visit then.|Yeah, I was talking to them this morning.}"
    };

    int lineID = 0;
    int charID = 0;
    int targetID = 0;

	void Start () 
    {
        for (int i = 0; i < charAnis.Length; i++)
        {
            charAnis[i][idleAni.name].wrapMode = WrapMode.Loop;
            charAnis[i][talkAni.name].wrapMode = WrapMode.Loop;

            charAnis[i].CrossFade(idleAni.name);
        }

        targetID = (charID + 1) % chars.Length;
        speakerLabel.text = chars[charID].name.ToUpper();
        dialogueLabel.text = ParseDialogue(lines[lineID]);
        charAnis[charID].CrossFade(talkAni.name);
	}

    static Regex regex = new Regex("\\[([a-zA-Z0-9_]*)\\]");
    static Regex regex2 = new Regex("\\{(?<content>[^\\{\\}|]*)(\\|(?<content>[^\\{\\}|]*))*\\}", System.Text.RegularExpressions.RegexOptions.ExplicitCapture);
    static Regex regex3 = new Regex(@"\B#\w+\b");
    
    private string ParseDialogue(string textToParse)
    {
        //Create optional text content by writing "... { optional text } ..." to have text that will be shown with a 50/50 chance.
        //Or write "... { Option A | Option B | Option C } ..." to have a 33% chance of Option A, Option B, or Option C being inserted.
        //You could also write "... { Option A | Option B | [VARIABLE NAME] } ..." to have a 33% chance of inserting the contents of the given variable name.
        //This is currently a VERY INEFFICENT OVERHEAD. SHOULD BE USED WITH CAUTION. 

        int index = 0;
        int length = 0;

        Match m = regex2.Match(textToParse);

        while (m.Success)
        {
            string content = string.Empty;
            int count = m.Groups["content"].Captures.Count;

            if (count > 1)
                content = m.Groups["content"].Captures[Random.Range(0, count)].Value;
            else if (Random.value > 0.5)
                content = m.Groups["content"].Captures[0].Value;

            index = m.Groups[0].Index;
            length = m.Groups[0].Length;

            //Potentially very inefficient! Look at STRINGBUILDER.
            textToParse = textToParse.Substring(0, index) + content + textToParse.Substring(index + length, textToParse.Length - index - length);

            m = regex2.Match(textToParse);
        }

        ////Replace variables in the text (within [...] ) with the variable contents.
        //m = regex.Match(textToParse);
        //while (m.Success)
        //{
        //    //This variable transfer needs looked at once the new Cinematic Windows are implemented!
        //    index = m.Groups[0].Index;
        //    length = m.Groups[0].Length;

        //    textToParse = textToParse.Substring(0, index) + GetVariableData(m.Groups[1].Value) + textToParse.Substring(index + length, textToParse.Length - index - length);

        //    m = regex.Match(textToParse, index);
        //}

        m = regex3.Match(textToParse);
        while(m.Success)
        {
            index = m.Groups[0].Index;
            length = m.Groups[0].Length;

            textToParse = textToParse.Substring(0, index) + GetVariableData(m.Groups[0].Value) + textToParse.Substring(index + length, textToParse.Length - index - length);
            m = regex3.Match(textToParse, index);
        }

        return "[6a6a6a]" + textToParse;
    }

    private string GetVariableData(string key)
    {
        if(key == "#TARGET")
            return chars[targetID].name;

        return "null";
    }

	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            targetID = charID;
            charAnis[charID].CrossFade(idleAni.name);

            charID = (charID + 1) % chars.Length;
            lineID = (lineID + 1) % lines.Length;

            speakerLabel.text = chars[charID].name.ToUpper();
            dialogueLabel.text = ParseDialogue(lines[lineID]);
            charAnis[charID].CrossFade(talkAni.name);
        }

	}
}
