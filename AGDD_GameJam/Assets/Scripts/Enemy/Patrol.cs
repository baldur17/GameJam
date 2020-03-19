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

    private EnemyController _controller;

    private int _currentSpot;
    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<EnemyController>();
        _currentSpot = 0;
        _startWaitTime = waitTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (_controller.isDead)
            speed = 0;
        
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
}
