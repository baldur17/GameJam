using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioController : MonoBehaviour
{

    public AudioSource clip;

    public void PlayAudio()
    {
        clip.Play();
    }
    
}
