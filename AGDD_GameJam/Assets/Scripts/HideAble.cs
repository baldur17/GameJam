using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAble : MonoBehaviour
{
    private PlayerController _playerController;
    void Start()
    {
        _playerController = GameManager.instance.player.GetComponent<PlayerController>();
        Debug.Log("hallo");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player entered hideable object");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Player left safety");
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("oncollision");
    }
}
