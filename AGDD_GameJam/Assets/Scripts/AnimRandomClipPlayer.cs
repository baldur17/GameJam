using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimRandomClipPlayer : StateMachineBehaviour
{
    [Header("Source")]
    public List<AudioClip> clips;

    [Header("Settings")] 
    public List<int> volume;
    
    private AudioSource _source;
    private System.Random _rand;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.gameObject.GetComponent<AudioSource>() == null)
        {
            _source = animator.gameObject.AddComponent<AudioSource>();
        }
        else
        {
            _source = animator.gameObject.GetComponent<AudioSource>();
        }
        _rand = new System.Random();
        
        var index = _rand.Next(clips.Count);
        _source.clip = clips[index];
        _source.volume = volume[index];
        _source.Play();
//        Debug.Log("djfhg");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var index = _rand.Next(clips.Count);
        _source.clip = clips[index];
        _source.volume = volume[index];
        if(!_source.isPlaying) _source.Play();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
    
}
