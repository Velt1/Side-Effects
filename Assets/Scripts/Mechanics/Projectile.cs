using UnityEngine;
using Platformer.Mechanics;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// A simple projectile that moves horizontally and damages enemies on contact.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [Tooltip("Horizontal speed of the projectile.")]
        public float speed = 10f;

        [Tooltip("Time (in seconds) before the projectile is destroyed automatically.")]
        public float lifetime = 2f;

        [Tooltip("The horizontal direction of the projectile: 1 = right, -1 = left.")]
        public float direction = 1f;

        Rigidbody2D rb;
        float timer = 0f;

        [Tooltip("The owner of the projectile: Player or Enemy.")]
        public ProjectileOwner owner;


        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            // Move the projectile horizontally each frame.
            rb.velocity = new Vector2(speed * direction, rb.velocity.y);

            // Destroy the projectile after its lifetime expires.
            timer += Time.deltaTime;
            if (timer > lifetime)
            {
                Destroy(gameObject);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (owner == ProjectileOwner.Player)
            {
                // Wenn der Spieler schießt, soll das Projektil Gegner treffen
                var enemy = other.gameObject.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    var enemyHealth = enemy.GetComponent<Health>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.Decrement();
                        if (!enemyHealth.IsAlive)
                        {
                            Schedule<EnemyDeath>().enemy = enemy;
                        }
                    }

                    // Zerstöre das Projektil nach dem Treffer
                    Destroy(gameObject);
                }
            }
            else if (owner == ProjectileOwner.Enemy)
            {
                // Wenn ein Gegner schießt, soll das Projektil den Spieler treffen
                var player = other.gameObject.GetComponent<PlayerController>();
                if (player != null)
                {
                    var playerHealth = player.GetComponent<Health>();
                    if (playerHealth != null)
                    {
                        playerHealth.Decrement();
                        if (!playerHealth.IsAlive)
                        {
                            Schedule<HealthIsZero>().health = playerHealth;
                        }
                    }

                    // Zerstöre das Projektil nach dem Treffer
                    Destroy(gameObject);
                }
            }
        }

    }
}