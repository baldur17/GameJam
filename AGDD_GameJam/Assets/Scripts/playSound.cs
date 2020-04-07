using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object that plays an audio clip when the animation it is attached to plays
/// </summary>
public class playSound : StateMachineBehaviour
{
    public AudioClip sound;
    public float volume;
    private AudioSource _source;
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
        _source.clip = sound;
        _source.volume = FindObjectOfType<HeartbeatAudioEffect>().EffectOn() ? volume * 0.2f : volume;
        _source.Play();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
