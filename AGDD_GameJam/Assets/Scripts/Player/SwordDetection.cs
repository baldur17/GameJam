using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class SwordDetection : MonoBehaviour
{

    public GameObject bloodEffect;
    
    private void OnTriggerExit2D(Collider2D other)
    {
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
