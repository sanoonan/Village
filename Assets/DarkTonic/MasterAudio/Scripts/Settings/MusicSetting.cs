using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class MusicSetting {
    public MasterAudio.AudioLocation audLocation = MasterAudio.AudioLocation.Clip;
	public AudioClip clip;
	public string songName = string.Empty;
	public string resourceFileName = string.Empty;
	public float volume = 1f;
	public float pitch = 1f;
	public bool isExpanded = true;
	public bool isLoop = false;
	public float customStartTime = 0f;
	public int lastKnownTimePoint = 0;
	public int songIndex = 0;
}
