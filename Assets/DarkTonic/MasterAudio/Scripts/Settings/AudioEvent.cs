using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class AudioEvent {
	public string actionName = "Your action name";
	public bool isExpanded = true;
	public string soundType = string.Empty;
	public bool allPlaylistControllersForGroupCmd = false;
	public bool allSoundTypesForGroupCmd = false;
	public bool allSoundTypesForBusCmd = false;
	public float volume = 1.0f;
	public bool useFixedPitch = false;
	public float pitch = 1f;
	public bool emitParticles = false;
	public int particleCountToEmit = 1;
	public float delaySound = 0f;
	public MasterAudio.EventSoundFunctionType currentSoundFunctionType = MasterAudio.EventSoundFunctionType.PlaySound;
	public MasterAudio.PlaylistCommand currentPlaylistCommand = MasterAudio.PlaylistCommand.None;
	public MasterAudio.SoundGroupCommand currentSoundGroupCommand = MasterAudio.SoundGroupCommand.None;
	public MasterAudio.BusCommand currentBusCommand = MasterAudio.BusCommand.None;
	public MasterAudio.CustomEventCommand currentCustomEventCommand = MasterAudio.CustomEventCommand.None;
	public MasterAudio.GlobalCommand currentGlobalCommand = MasterAudio.GlobalCommand.None;
	public string busName = string.Empty;
	public string playlistName = string.Empty;
	public string playlistControllerName = string.Empty;
	public bool startPlaylist = true;
	public float fadeVolume = 0f;
	public float fadeTime = 1f;
	public string clipName = "[None]";
	public EventSounds.VariationType variationType = EventSounds.VariationType.PlayRandom;
	public string variationName = string.Empty;
	
	// custom event fields
	public string theCustomEventName = string.Empty;
}
