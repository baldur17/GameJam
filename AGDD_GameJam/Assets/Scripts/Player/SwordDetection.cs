using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class SwordDetection : MonoBehaviour
{
    public AudioSource hitSound;
    public GameObject bloodEffect;
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag($"Enemy"))
        {
            IEnemy enemy = other.gameObject.GetComponentInParent<IEnemy>();
            enemy?.Hit(1);
            if (enemy != null)
            {
                if(!hitSound.isPlaying) hitSound.Play();
                Instantiate(bloodEffect, other.transform.position, Quaternion.identity);
            }
        }
    }
}
