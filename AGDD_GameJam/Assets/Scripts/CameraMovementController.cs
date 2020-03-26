using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementController : MonoBehaviour
{

    // The object the camera follows
    public GameObject followObject;
    public float followTime;

    // Update is called once per frame
    void Update()
    {
        // Follow 20% of the way to the player
        transform.position += (followObject.transform.position - new Vector3(transform.position.x, transform.position.y, followObject.transform.position.z)) * (Time.deltaTime * followTime);
    }
}
