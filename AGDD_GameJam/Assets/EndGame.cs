using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    public PauseMenu menuScript;

    private void OnTriggerEnter2D(Collider2D other)
    {
        menuScript.FinishGame();
    }
}
