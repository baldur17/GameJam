using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour, IEnemy
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log("VAR");
        }

        public void Hit(int damage)
        {
            throw new System.NotImplementedException();
        }

        public void Move()
        {
            throw new System.NotImplementedException();
        }

        public bool IsDetected(PlayerController player)
        {
            throw new System.NotImplementedException();
        }
    }
}
