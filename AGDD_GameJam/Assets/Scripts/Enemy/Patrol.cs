using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    public float speed;
    public float waitTime;
    private float _startWaitTime;
    public Transform[] moveSpots;
    public float speedMultiplier;
    
    private EnemyController _controller;
    private PlayerController _playerController;
    private float _enemyWidth;
    
    private int _currentSpot;
    // Start is called before the first frame update
    void Start()
    {
        _enemyWidth = 2;
        _controller = GetComponent<EnemyController>();
        _currentSpot = 0;
        _startWaitTime = waitTime;
        _playerController = _playerController = GameManager.instance.player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_controller.isDead)
            speed = 0;

        //If player is dead, enemy should no longer follower normal update rules
        if (_playerController.GetIsDead())
        {
            //If enemy has reached played, go back to idle animation
            if (transform.position.x > _playerController.transform.position.x - _enemyWidth &&
                transform.position.x < _playerController.transform.position.x + _enemyWidth)
            {
                _controller.SetChaseBoolAnimation(false);
            }
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, moveSpots[_currentSpot].position, speed * Time.deltaTime);

        if (!(Vector2.Distance(transform.position, moveSpots[_currentSpot].position) < 0.2f)) return;
        
        if (waitTime <= 0)
        {
            Vector3 scale = transform.localScale;
            if (transform.localScale.x < 0)
            {
                scale.x = Mathf.Abs(scale.x);
            }
            else
            {
                scale.x = -Mathf.Abs(scale.x);
            }
            transform.localScale = scale;
            _currentSpot = (_currentSpot + 1) % 2;
            waitTime = _startWaitTime;
        }
        else
        {
            waitTime -= Time.deltaTime;
        }
        
    }

    public void MoveTowardsPlayer()
    {
        //TODO out which direction the player is and apply a small buffer so that the enemy does not rush on top of player but rather close to him
        Vector3 moveTo = _playerController.transform.position;
        //If enemies x position is smaller than player, then player is to the right
        //Create new position with by subtracting roughly the width of the enemy from x-axis

        var position = transform.position;
        moveTo.x = position.x < moveTo.x ? moveTo.x - 2 : moveTo.x + _enemyWidth;
        moveTo.y = position.y;
        
        
        position = Vector2.MoveTowards(position, moveTo, speed * speedMultiplier * Time.deltaTime);
        transform.position = position;
    }

}
