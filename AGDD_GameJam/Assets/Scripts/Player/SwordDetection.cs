using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class SwordDetection : MonoBehaviour
{

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag($"Enemy"))
        {
            Debug.Log($"Enemy");
            IEnemy enemy = other.gameObject.GetComponentInParent<IEnemy>();
            enemy?.Hit(1);
            enemy?.PlayAnimation();
        }
    }
}
