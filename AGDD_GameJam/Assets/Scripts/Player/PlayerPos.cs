using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPos : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(GameManager.instance.lastCheckpoint.x, GameManager.instance.lastCheckpoint.y, 0);
        Camera.main.transform.position = new Vector3(GameManager.instance.lastCheckpoint.x, GameManager.instance.lastCheckpoint.y, Camera.main.transform.position.z);
    }
}
