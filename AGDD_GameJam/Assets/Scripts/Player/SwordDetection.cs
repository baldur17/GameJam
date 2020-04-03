using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
using Random = UnityEngine.Random;

public class SwordDetection : MonoBehaviour
{
    public AudioSource hitSound;
    public GameObject bloodEffect;

    public float xShake;
    public float minYShake, maxYShake;
    
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
                GameObject cam = Camera.main.gameObject;

                float x = Random.Range(-xShake, xShake);
                float y = Random.Range(minYShake, maxYShake);
                
                cam.transform.position = cam.transform.position + new Vector3(x, y, 0);
            }
        }
    }
}
