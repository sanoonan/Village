using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    // Static singleton property
    public static MusicManager Instance { get; private set; }

    public AudioSource activeAudio;
    public AudioSource inactiveAudio;

    //Combine the SFX stuff in here too
    //Add ability to fade music in / out.
    //Add ability to instantly stop music (with a SFX playing to cut it off...?)
    //Add ability to crossfade from one track into another.

    void Awake()
    {
        // First we check if there are any other instances conflicting
        if (Instance != null && Instance != this)
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }

        // Here we save our singleton instance
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(string musicName)
    {
        if (activeAudio.clip == null || activeAudio.clip.name != musicName)
        {
            AudioClip newBGM = Resources.Load(musicName, typeof(AudioClip)) as AudioClip;
            activeAudio.enabled = true;

            if (!activeAudio.isPlaying)
            {
                activeAudio.clip = newBGM;
                activeAudio.Play();
            }
            else
            {
                //We need to crossfade, but for now we will just stop and start a new track.
                activeAudio.Stop();
                activeAudio.clip = newBGM;
                activeAudio.Play();
            }

            
        }
    }
}
