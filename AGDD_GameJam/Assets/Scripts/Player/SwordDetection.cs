using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class SwordDetection : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        IEnemy enemy = other.gameObject.GetComponentInParent<IEnemy>();
        Debug.Log(enemy);
        enemy?.Hit(1);
    }
}
