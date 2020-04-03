using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Scale down volume of all AudioSource components except the 'hearbeat' if player is crouching
/// </summary>
public class HeartbeatAudioEffect : MonoBehaviour
{
    public GameObject player;
    public AudioLowPassFilter filter;
    private List<float> _originalVolumes;
    private List<AudioSource> _sources;
    private PlayerController _playerController;
    private bool _lastState;

    void Start()
    {
        _playerController = player.GetComponent<PlayerController>();
        _sources = FindObjectsOfType<AudioSource>().ToList();
        _originalVolumes = new List<float>();
        foreach (var s in _sources) _originalVolumes.Add(s.volume);
        filter.enabled = false;
        _lastState = false;
    }

    // Update is called once per frame
    void Update()
    {
        var currentCrouch = _playerController.GetCrouch();
        if (_lastState != currentCrouch)
        {
            HeartBeatMode(currentCrouch);
            _lastState = currentCrouch;
        }
        
    }

    void HeartBeatMode(bool crouch)
    {
        foreach (var s in _sources)
        {
            if (s.clip.name == "heartbeat") continue;
            filter.enabled = crouch;
            s.volume = crouch ? s.volume *= 0.6f : _originalVolumes[_sources.IndexOf(s)];
        }
    }

    public bool EffectOn()
    {
        return _playerController.GetCrouch();
    }

}
