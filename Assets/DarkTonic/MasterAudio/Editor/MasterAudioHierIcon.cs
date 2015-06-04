using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public class MasterAudioHierIcon : MonoBehaviour {
    static Texture2D icon;

    static MasterAudioHierIcon() {
        icon = AssetDatabase.LoadAssetAtPath("Assets/Gizmos/MasterAudio Icon.png", typeof(Texture2D)) as Texture2D;
        if (icon == null) {
            return;
        }
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
        EditorApplication.RepaintHierarchyWindow();
    }

    static void HierarchyItemCB(int instanceID, Rect selectionRect) {
        GameObject masterAudioGameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (icon != null && masterAudioGameObject != null && masterAudioGameObject.GetComponent<MasterAudio>() != null) {
            Rect r = new Rect(selectionRect);
            r.x = r.width - 5;

            GUI.Label(r, icon);
        }
    }
}
