using UnityEngine;
using System.Collections;

public class DialogUIManager : MonoBehaviour 
{
    public UISprite sSpeechBubble;
    public UILabel lDialogue;

    public void SetDialog(string dString)
    {
        lDialogue.text = dString;
        Bounds uiBounds = NGUIMath.CalculateRelativeWidgetBounds(lDialogue.cachedTransform);
        sSpeechBubble.width = Mathf.CeilToInt(uiBounds.size.x) + 40;
    }

    public string GetDialogue()
    {
        return lDialogue.text;
    }
}
