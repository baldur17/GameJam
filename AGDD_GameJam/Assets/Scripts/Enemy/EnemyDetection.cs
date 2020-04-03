using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
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
        
        #endregion
        
        // Start is called before the first frame update
        void Start()
        {
            _collider = GetComponent<PolygonCollider2D>();
            _collider.points = new Vector2[4];

            _patrol = gameObject.GetComponentInParent<Patrol>();
            _originalSpeed = _patrol.speed;

            _playerDetected = false;

            _playerController = GameManager.instance.player.GetComponent<PlayerController>();
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
                if (_timeSinceDetected <= 0)
                {
                    filledExclamation.color = Color.red;
                    _playerController.Detected();
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
            // Layermask for every layer except the 9th layer
            //int layerMask = ~(1 << 9);
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
                    Debug.Log(hit.collider.name);
                    if (hit.collider.name == "Player")
                    {
                        if (!_playerDetected)
                        {      
                            //_patrol.speed = 0;
                            _playerDetected = true;
                            _timeSinceDetected = detectionTime;
                            emptyExclamation.gameObject.SetActive(true);
                            filledExclamation.gameObject.SetActive(true);
                            setExclamationPercentage(_timeSinceDetected);
                        }
                        
                        // player.Detected();
                        // exclamationGameObject.GetComponent<SpriteRenderer>().enabled = true;

                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (!player) return;
            
            print("PLAYER LEFT TRIGGER");
            _playerDetected = false;
            emptyExclamation.gameObject.SetActive(false);
            filledExclamation.gameObject.SetActive(false);

            _patrol.speed = _originalSpeed;
        }
    }
}
