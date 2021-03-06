﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Enemy
{
    public class EnemyDetection : MonoBehaviour
    {

        #region Public Variables
        
        public float lookDistance;
        public float lookAngle;
        [FormerlySerializedAs("ExclamationGameObject")] public GameObject exclamationGameObject;

        public Image emptyExclamation;
        public Image filledExclamation;

        public Light2D exclamationLight; 

        // Time needed for enemy to fully detect player
        public float detectionTime;

        public LayerMask layerMasks;
        
        #endregion
        
        // Region where all the private variables are stored
        #region Private Variables

        private PolygonCollider2D _collider;
        private float _timeSinceDetected;
        private bool _playerDetected;

        private Patrol _patrol;
        private float _originalSpeed;

        private PlayerController _playerController;
        private EnemyController _controller;
        private bool _lightCoroutine;
        
        #endregion
        
        // Start is called before the first frame update
        void Start()
        {
            _lightCoroutine = true;
            _collider = GetComponent<PolygonCollider2D>();
            _collider.points = new Vector2[4];

            _patrol = gameObject.GetComponentInParent<Patrol>();
            _originalSpeed = _patrol.speed;

            _playerDetected = false;

            _playerController = GameManager.instance.player.GetComponent<PlayerController>();
            _controller = gameObject.GetComponentInParent<EnemyController>();
        }

        // Update is called once per frame
        void Update()
        {
            // Create a vector array with all points for triangular cone vision
            var points = _collider.points;
            points[0] = Vector2.zero;
            points[1] = new Vector2(lookDistance, lookAngle/2);
            points[2] = new Vector2(lookDistance, -(lookAngle / 2));
            _collider.points = points;

            UpdateDetection();
        }

        private void UpdateDetection()
        {
            if (_playerDetected)
            {
                if (filledExclamation.fillAmount > 0.7)
                {
                    filledExclamation.color = Color.red;
                }
                
                if (_timeSinceDetected <= 0)
                {
                    _playerController.Detected();
                    emptyExclamation.color = Color.black;
                    //Start coroutine of flashing ! possibly
                    if (_lightCoroutine)
                    {
                        _controller.SetChaseBoolAnimation(true);
                        
                        _lightCoroutine = false;
                        StartCoroutine(nameof(ExclamationLight));
                    }
                    //Move enemy to players position
                    _patrol.MoveTowardsPlayer();
                }
                else
                {
                    _timeSinceDetected -= Time.deltaTime;
                    setExclamationPercentage(_timeSinceDetected);
                }
            }
        }

        private void setExclamationPercentage(float timeSinceDetected)
        {
            float currentFillPercentage = Mathf.Lerp(1, 0, timeSinceDetected / detectionTime);

            filledExclamation.fillAmount = currentFillPercentage;
        }

        void OnDrawGizmosSelected()
        {
            // Draw a line from enemy to where top and bottom points of cone vision are.
            // transform.TransformPoint translates a point to local position
            Gizmos.DrawLine(transform.TransformPoint(Vector3.zero), transform.TransformPoint(new Vector2(lookDistance, lookAngle/2)));
            Gizmos.DrawLine(transform.TransformPoint(Vector3.zero), transform.TransformPoint(new Vector2(lookDistance, -(lookAngle/2))));

        }

        /// <summary>
        /// Trigger for detection collision
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerStay2D(Collider2D other)
        {
            PlayerController player = other.GetComponent<PlayerController>();

            //If player is not able to be detected, return

            if (player)
            {
                // Raycast to the position of player
                RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, Mathf.Infinity, layerMasks);
                
                //If player is not able to be detected, return
                if (!player.isDetectable)
                {
                    return;
                }
                if ( hit )
                {
                    Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.yellow);
                    // If the raycast collides with player
                    if (hit.collider.name == "Player")
                    {
                        if (!_playerDetected)
                        {
                            //TODO make detection range larger ? or increase angle
                            
                            _playerDetected = true;
                            _timeSinceDetected = detectionTime;
                            emptyExclamation.gameObject.SetActive(true);
                            filledExclamation.gameObject.SetActive(true);
                            setExclamationPercentage(_timeSinceDetected);
                        }
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {

            PlayerController player = other.GetComponent<PlayerController>();

            if (!player) return;
            
            _playerDetected = false;
            emptyExclamation.gameObject.SetActive(false);
            filledExclamation.gameObject.SetActive(false);
            filledExclamation.color = new Color32(192, 192 , 48, 255);

            _patrol.speed = _originalSpeed;
        }
        
        private IEnumerator ExclamationLight()
        {
            //Light appears around the exclamation point and gets more intense
            for (var i = 0; i < 6; i++)
            {
                exclamationLight.intensity += 0.5f;
                yield return new WaitForSeconds(0.1f);
            }

            //_lightCoroutine = true;
        }
    }
}
