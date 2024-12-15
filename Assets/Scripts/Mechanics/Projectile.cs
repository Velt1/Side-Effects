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
            // If the projectile hits an enemy, decrement its health.
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

                // Destroy the projectile after hitting an enemy.
                Destroy(gameObject);
            }

            // You could also decide what happens if the projectile hits walls or other objects.
            // For simplicity, we only handle enemy hit here.
        }
    }
}