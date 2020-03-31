using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TrailMaker : MonoBehaviour
{

    [FormerlySerializedAs("FromTransform")] public Transform fromTransform;
    [FormerlySerializedAs("ToTransform")] public Transform toTransform;

    public float xScale;
    public float yScale;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = Vector3.Lerp(toTransform.localPosition, fromTransform.localPosition, 0.5f);
        // float x = fromTransform.position.x + trailXOffset;
        // float y = fromTransform.position.y;
        // while (x < toTransform.position.x)
        // {
        //     var prefab = Instantiate(trailMaterial, new Vector3(x, y, 0), Quaternion.identity);
        //     prefab.transform.parent = gameObject.transform;
        //     x += trailXOffset;
        // }
    }

    private void Update()
    {
        transform.localScale = new Vector3(xScale, yScale, 1);
    }
}
