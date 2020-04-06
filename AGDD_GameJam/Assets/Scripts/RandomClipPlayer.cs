using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomClipPlayer : MonoBehaviour
{
    [Header("Source")]
    public List<AudioClip> clips;

    [Header("Settings")] 
    public List<int> volume;
    
    private AudioSource _source;
    private System.Random _rand;
    
    // Start is called before the first frame update
    void Start()
    {
        _source = new AudioSource();
        _rand = new System.Random();
        
    }

    public void Play()
    {
        var index = _rand.Next(clips.Count);
        _source.clip = clips[index];
        _source.volume = volume[index];
        _source.Play();
    }
}
