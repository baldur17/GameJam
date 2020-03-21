using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TorchFlicker : MonoBehaviour
{
    public float maxReduction;
    public float maxIncrease;
    public float rateDamping;
    public float maxRangeReduction;
    public float maxRangeIncrease;
    public float strength;
    public bool stopFlickering;
 
    private Light2D _lightSource;
    private float _baseIntensity;
    private float _baseRange;
    private bool _flickering;
 
    public void Reset()
    {
        maxReduction = 0.5f;
        maxIncrease = 0.5f;
        rateDamping = 0.1f;
        strength = 300;
    }
 
    public void Start()
    {
        _lightSource = GetComponent<Light2D>();
        if (_lightSource == null)
        {
            Debug.LogError("Flicker script must have a Light Component on the same GameObject.");
            return;
        }
        _baseIntensity = _lightSource.intensity;
        _baseRange = _lightSource.pointLightOuterRadius;
        StartCoroutine(DoFlicker());
    }
 
    void Update()
    {
        if (!stopFlickering && !_flickering)
        {
            StartCoroutine(DoFlicker());
        }
    }
 
    private IEnumerator DoFlicker()
    {
        _flickering = true;
        while (!stopFlickering)
        {
            _lightSource.intensity = Mathf.Lerp(_lightSource.intensity, Random.Range(_baseIntensity - maxReduction, _baseIntensity + maxIncrease), strength * Time.deltaTime);
            _lightSource.pointLightOuterRadius = Mathf.Lerp(_lightSource.pointLightOuterRadius, Random.Range(_baseRange - maxRangeReduction, _baseRange + maxRangeIncrease), strength * Time.deltaTime);
            yield return new WaitForSeconds(rateDamping);
        }
        _flickering = false;
    }
}