using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Just a script to attatch to objects which will only live for a specific amount of time;
/// </summary>
public class Destroyer : MonoBehaviour
{

    public float time = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, time);        
    }

}
