using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Heartbeat : MonoBehaviour
{
    public float timeBetweenBeats; //1 beat consists of 2 pulses
    public float timeBetweenPulse; //Pulse is 1 'heartbeat'
    public Light2D myLight;

    public GameObject heartbeatEffect;
    
    private bool _coroutineRunning;
    public float maxIntensity;
    

    private int _framesPerBeat; //How many frames it takes for the heartbeat to go from zero to max

    private float _stepsPerBeat; //How much you need to increment intensity by to reach max in _framesPerBeat many frames;

    private float _halfStepsPerBeat; //How much you need to increment/decrement to reach half intesity
    // Start is called before the first frame update
    void Start()
    {
        //Find all heartbeats in the level?
        _coroutineRunning = false;
        _framesPerBeat = 14;
        _stepsPerBeat = maxIntensity / _framesPerBeat;
        _halfStepsPerBeat = _stepsPerBeat / 2;


    }
    
    
    // Update is called once per frame
    void Update()
    {
        if (!_coroutineRunning)
        {
            _coroutineRunning = true;
            StartCoroutine(HeartPulse());
        }
    }

    public void StartBeats()
    {
        //Start the beats
        //Need to make condition so that this plays for as long as the player is 'sensing' and then stops
    }
    
    private IEnumerator HeartPulse()
    {
        //This should play 2 heartbeats and wait be
        yield return StartCoroutine( HeartBeatOne() );
        // Instantiate(heartbeatEffect, transform.position, Quaternion.identity);
        //Wait between the two beats slightly
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine( HeartBeatTwo() );
        // Instantiate(heartbeatEffect, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.4f);
        _coroutineRunning = false;
        myLight.intensity -= 0;
    }
    
    private IEnumerator HeartBeatOne()
    {

        //Goes from zero to max intensity in x many frames
        for (var i = 0; i < _framesPerBeat; i++)
        {
            myLight.intensity += _stepsPerBeat;
            yield return new WaitForSeconds(0.01f);
        }

        //Goes from max intensity to half intensity
        for (var i = 0; i < _framesPerBeat / 2; i++)
        {
            myLight.intensity -= _halfStepsPerBeat;
            yield return new WaitForSeconds(0.01f);
        }
        
    }
    
    IEnumerator HeartBeatTwo()
    {
        //Goes from half intensity to max
        for (var i = 0; i < _framesPerBeat / 2; i++)
        {
            myLight.intensity += _halfStepsPerBeat;
            yield return new WaitForSeconds(0.01f);
        }
        
        //Goes from max intensity to zero
        for (var i = 0; i < _framesPerBeat; i++)
        {
            myLight.intensity -= _stepsPerBeat;
            yield return new WaitForSeconds(0.01f);
        }
    }
    
}
