using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAble : MonoBehaviour
{
    private PlayerController _playerController;
    void Start()
    {
        _playerController = GameManager.instance.player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Set player attribute to not detectable
        _playerController.isDetectable = false;
        Debug.Log("PLayer can not be detected");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //Set player attribute to detectable
        _playerController.isDetectable = true;
    }
    
}
