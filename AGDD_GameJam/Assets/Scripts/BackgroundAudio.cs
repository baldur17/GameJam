using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper to play background audio
/// </summary>
public class BackgroundAudio : MonoBehaviour
{
    public List<AudioClip> clips;
    public List<float> volume;
    private List<AudioSource> _sources;
    void Awake()
    {
        foreach (var c in clips)
        {
            AudioSource s = transform.GetChild(0).gameObject.AddComponent<AudioSource>();
            s.clip = c;
            s.Play();
            s.volume = volume[clips.IndexOf(c)];
            s.loop = true;
        }
    }
}
