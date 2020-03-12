using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour, IEnemy
    {
        // Animator to set animation variables
        private Animator _animator;
        // Variable to set the death animation
        private static readonly int IsDead = Animator.StringToHash("IsDead");

        // Start is called before the first frame update
        void Start()
        {
        
            _animator = gameObject.GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
        }

        /// <summary>
        /// Called when player hits enemy
        /// </summary>
        /// <param name="damage">amount of damage enemy takes</param>
        public void Hit(int damage)
        {
            // Set player as dead
            _animator.SetBool("IsDead", true);
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
