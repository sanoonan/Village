using UnityEngine;
using System.Collections;
using UnityEditor;

public static class MasterAudioInspectorResources {
    public const string MasterAudioFolderPath = "MasterAudio";

    public static Texture logoTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/inspector_header_master_audio.png", MasterAudioFolderPath)) as Texture;
    public static Texture deleteTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/deleteIcon.png", MasterAudioFolderPath)) as Texture;
    public static Texture gearTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/gearIcon.png", MasterAudioFolderPath)) as Texture;
    public static Texture muteOffTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/muteOff.png", MasterAudioFolderPath)) as Texture;
    public static Texture muteOnTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/muteOn.png", MasterAudioFolderPath)) as Texture;
    public static Texture nextTrackTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/nextTrackIcon.png", MasterAudioFolderPath)) as Texture;
    public static Texture pauseTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/pauseIcon.png", MasterAudioFolderPath)) as Texture;
    public static Texture pauseOnTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/pauseIconOn.png", MasterAudioFolderPath)) as Texture;
    public static Texture playSongTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/playIcon.png", MasterAudioFolderPath)) as Texture;
    public static Texture previousTrackTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/prevTrackIcon.png", MasterAudioFolderPath)) as Texture;
    public static Texture randomTrackTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/randomIcon.png", MasterAudioFolderPath)) as Texture;
    public static Texture soloOffTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/soloOff.png", MasterAudioFolderPath)) as Texture;
    public static Texture soloOnTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/soloOn.png", MasterAudioFolderPath)) as Texture;
    public static Texture previewTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/speakerIcon.png", MasterAudioFolderPath)) as Texture;
    public static Texture stopTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/stopIcon.png", MasterAudioFolderPath)) as Texture;

    public static Texture[] ledTextures = new Texture[] {
		EditorGUIUtility.LoadRequired(string.Format("{0}/LED5.png", MasterAudioFolderPath)) as Texture,
		EditorGUIUtility.LoadRequired(string.Format("{0}/LED4.png", MasterAudioFolderPath)) as Texture,
		EditorGUIUtility.LoadRequired(string.Format("{0}/LED3.png", MasterAudioFolderPath)) as Texture,
		EditorGUIUtility.LoadRequired(string.Format("{0}/LED2.png", MasterAudioFolderPath)) as Texture,
		EditorGUIUtility.LoadRequired(string.Format("{0}/LED1.png", MasterAudioFolderPath)) as Texture,
		EditorGUIUtility.LoadRequired(string.Format("{0}/LED0.png", MasterAudioFolderPath)) as Texture
	};
}
