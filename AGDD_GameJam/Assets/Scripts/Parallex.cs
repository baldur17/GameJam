using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallex : MonoBehaviour
{
    public float parallaxEffect;
    private Vector3 _lastCameraPosition;
    private float _textureUnitSizeX;
    private Transform _cameraTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        if (Camera.main != null) _cameraTransform = Camera.main.transform;
        _lastCameraPosition = _cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        _textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        
    }
    
    void LateUpdate()
    {
        Vector3 deltaMovement = _cameraTransform.position - _lastCameraPosition;
        
        transform.position += new Vector3(deltaMovement.x * parallaxEffect, deltaMovement.y, 0f);

        _lastCameraPosition = _cameraTransform.position;

        if (Mathf.Abs(_cameraTransform.position.x - transform.position.x) >= _textureUnitSizeX)
        {
            float offsetPositionX = (_cameraTransform.position.x - transform.position.x) % _textureUnitSizeX;
            transform.position = new Vector3(_cameraTransform.position.x + offsetPositionX, transform.position.y, 0.3f);
        }
    }
}
