using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour, IEnemy
    {
        public AudioSource deathSound;
        // Animator to set animation variables
        private Animator _animator;
        public GameObject trailObject;
        private Patrol _patrol;
        private static readonly int ChaseDead = Animator.StringToHash("ChaseDead");
        
        // Variable to set the death animation
        private static readonly int IsDead = Animator.StringToHash("IsDead");

        // private ParticleSystem _slashEffect;

        public bool isDead;

        
        // Start is called before the first frame update
        void Start()
        {
            isDead = false;
            _animator = gameObject.GetComponent<Animator>();
            _patrol = gameObject.GetComponentInChildren<Patrol>();
            
            // _slashEffect = GetComponentInChildren<ParticleSystem>();
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
            isDead = true;
            // Set player as dead
            _animator.SetBool(IsDead, true);
            // After enemy dies, disable their trail
            trailObject.SetActive(false);
                  
            if(!deathSound.isPlaying) deathSound.Play();
        }

        public void Move()
        {
            throw new System.NotImplementedException();
        }

        public bool IsDetected(PlayerController player)
        {
            throw new System.NotImplementedException();
        }

        public void PlayAnimation()
        {
            throw new System.NotImplementedException();
        //     _slashEffect.Play();
        }

        public void SetChaseBoolAnimation(bool input)
        {
            _animator.SetBool(ChaseDead, input);
        }
        

    }
}
