using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasterAudioGroup : MonoBehaviour {
	public const string NO_BUS = "[NO BUS]";

	public int busIndex = -1;
	
	public bool isExpanded = true;
	public float groupMasterVolume = 1f;
	public int retriggerPercentage = 50;
	public VariationMode curVariationMode = VariationMode.Normal;
	
	public float chainLoopDelayMin;
	public float chainLoopDelayMax;
	public ChainedLoopLoopMode chainLoopMode = ChainedLoopLoopMode.Endless;
	public int chainLoopNumLoops = 0;
	public bool useDialogFadeOut = false;
	public float dialogFadeOutTime = .5f;
	
	public VariationSequence curVariationSequence = VariationSequence.Randomized;
	public bool useInactivePeriodPoolRefill = false;
	public float inactivePeriodSeconds = 5f;
	public List<SoundGroupVariation> groupVariations = new List<SoundGroupVariation>();
	public MasterAudio.AudioLocation bulkVariationMode = MasterAudio.AudioLocation.Clip;
	public bool logSound = false;
	
	public LimitMode limitMode = LimitMode.None;
	public int limitPerXFrames = 1;
	public float minimumTimeBetween = 0.1f;
	public bool useClipAgePriority = false;
	
	public bool limitPolyphony = false;
	public int voiceLimitCount = 1;
	
	public bool isSoloed = false;
	public bool isMuted = false;
	
	private List<int> activeAudioSourcesIds = new List<int>();
	private int chainLoopCount = 0;
	
	public enum VariationSequence {
		Randomized,
		TopToBottom
	}

	public enum VariationMode {
		Normal,
		LoopedChain,
		Dialog
	}
	
	public enum ChainedLoopLoopMode {
		Endless,
		NumberOfLoops
	}
	
	public enum LimitMode {
		None,
		FrameBased,
		TimeBased
	}
	
	public int ActiveVoices {
		get {
			return activeAudioSourcesIds.Count;
		}
	}
	
	public int TotalVoices {
		get {
			return this.transform.childCount;
		}
	}
	
	public void AddActiveAudioSourceId(int varInstanceId) {
        if (activeAudioSourcesIds.Contains(varInstanceId))
        {
			return;
		}

        activeAudioSourcesIds.Add(varInstanceId);
		
		var bus = BusForGroup;
		if (bus != null) {
            bus.AddActiveAudioSourceId(varInstanceId);	
		}
	}
	
	public void RemoveActiveAudioSourceId(int _varInstanceId) {
		activeAudioSourcesIds.Remove(_varInstanceId);
		
		var bus = BusForGroup;
		if (bus != null) {
			bus.RemoveActiveAudioSourceId(_varInstanceId);	
		}
	}
	
	public GroupBus BusForGroup {
		get {
			if (busIndex < MasterAudio.HARD_CODED_BUS_OPTIONS || !Application.isPlaying) {
				return null; // no bus, so no voice limit
			}
			
			var index = busIndex - MasterAudio.HARD_CODED_BUS_OPTIONS;

			if (index >= MasterAudio.GroupBuses.Count) { // this happens only with Dynamic SGC item removal
				return null;
			}
			
			return MasterAudio.GroupBuses[index];
		}
	}
	
	public int ChainLoopCount {
		get {
			return chainLoopCount;
		}
		set {
			chainLoopCount = value;
		}
	}
}
