using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
using Random = UnityEngine.Random;

public class SwordDetection : MonoBehaviour
{

    public GameObject bloodEffect;

    public float minXshake, maxXshake;
    public float minYshake, maxYshake;
    
    private void OnTriggerExit2D(Collider2D other)
    {
        GameObject cam = Camera.main.gameObject;

        float x = Random.Range(-maxXshake, maxXshake);
        float y = Random.Range(minYshake, maxYshake);
                
        cam.transform.position = cam.transform.position + new Vector3(x, y, 0);
        
        if (other.CompareTag($"Enemy"))
        {
            IEnemy enemy = other.gameObject.GetComponentInParent<IEnemy>();
            enemy?.Hit(1);
            if (enemy != null)
            {
                Instantiate(bloodEffect, other.transform.position, Quaternion.identity);
                
            }
        }
    }
}
