
namespace Enemy
{
    /// <summary>
    /// Interface of enemy
    /// </summary>
    public interface IEnemy
    {
        /// <summary>
        /// Called when player hits enemy
        /// </summary>
        /// <param name="damage">amount of damage enemy takes</param>
        void Hit(int damage);
        /// <summary>
        /// Called to move the enemy around the map
        /// </summary>
        void Move(); // Might take direction parameter

        /// <summary>
        /// Checks if enemy detects player
        /// </summary>
        /// <param name="player">Instance of the player</param>
        /// <returns>True if enemy is detected, false if not</returns>
        bool IsDetected(PlayerController player);
    }
}
