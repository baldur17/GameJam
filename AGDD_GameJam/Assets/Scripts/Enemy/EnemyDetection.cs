using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Enemy
{
    public class EnemyDetection : MonoBehaviour
    {

        public float lookDistance;
        public float lookAngle;

        private PolygonCollider2D _collider;
        
        // Start is called before the first frame update
        void Start()
        {
            _collider = GetComponent<PolygonCollider2D>();
            _collider.points = new Vector2[4];
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
        }
        
        void OnDrawGizmosSelected()
        {
            // Draw a line from enemy to where top and bottom points of cone vision are.
            // transform.TransformPoint translates a point to local position
            Gizmos.DrawLine(transform.TransformPoint(Vector3.zero), transform.TransformPoint(new Vector2(lookDistance, lookAngle/2)));
            Gizmos.DrawLine(transform.TransformPoint(Vector3.zero), transform.TransformPoint(new Vector2(lookDistance, -(lookAngle/2))));

        }
        
        
        
    }
}
