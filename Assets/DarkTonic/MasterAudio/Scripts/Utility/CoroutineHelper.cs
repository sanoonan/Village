using UnityEngine;
using System.Collections;

public static class CoroutineHelper {
	public static IEnumerator WaitForActualSeconds(float time) {
    	var start = Time.realtimeSinceStartup;
    
		while (Time.realtimeSinceStartup < start + time) {
			yield return MasterAudio.endOfFrameDelay;
    	}
	}
}
