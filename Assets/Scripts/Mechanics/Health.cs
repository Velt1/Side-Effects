using System.Collections;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Represents the current vital statistics of some game entity.
    /// </summary>
    public class Health : MonoBehaviour
    {
        /// <summary>
        /// The maximum hit points for the entity.
        /// </summary>
        public int maxHP = 1;

        /// <summary>
        /// Indicates if the entity should be considered 'alive'.
        /// </summary>
        public bool IsAlive => currentHP > 0;
        public bool isPlayer = false;

        public bool invulnerable = false; // Neu: Unverwundbarkeits-Flag

        public int currentHP;

        /// <summary>
        /// Increment the HP of the entity.
        /// </summary>
        public void Increment()
        {
            currentHP = Mathf.Clamp(currentHP + 1, 0, maxHP);
        }

        /// <summary>
        /// Decrement the HP of the entity. Will trigger a HealthIsZero event when
        /// current HP reaches 0.
        /// </summary>
        public void Decrement()
        {
            // Wenn invulnerable aktiv ist, wird Schaden ignoriert
            if (invulnerable) return;

            currentHP = Mathf.Clamp(currentHP - 1, 0, maxHP);
            if (currentHP == 0)
            {
                var ev = Schedule<HealthIsZero>();
                ev.health = this;
            }
        }

        /// <summary>
        /// Decrement the HP of the entity until HP reaches 0.
        /// </summary>
        public void Die()
        {
            // Wenn unverwundbar, dann nicht sterben.
            if (invulnerable) return;

            while (currentHP > 0) Decrement();
        }

        void Awake()
        {
            currentHP = maxHP;
        }

        /// <summary>
        /// Macht die Einheit für duration Sekunden unverwundbar.
        /// </summary>
        public void BecomeInvincible(float duration)
        {
            if (!invulnerable)
            {
                StartCoroutine(InvincibilityRoutine(duration));
            }
        }

        private IEnumerator InvincibilityRoutine(float duration)
        {
            invulnerable = true;
            yield return new WaitForSeconds(duration);
            invulnerable = false;
        }
    }
}