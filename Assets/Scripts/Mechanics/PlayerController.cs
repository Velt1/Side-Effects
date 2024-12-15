using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customization.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;

        public Collider2D collider2d;
        public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;

        // WALLJUMP ADDITION:
        [Header("Wall Jump Settings")]
        public LayerMask wallLayer; // Assign the wall layer in the inspector
        public float wallCheckDistance = 0.5f; // Distance to check for wall in front
        public float wallJumpHorizontalVelocity = 7f; // Horizontal velocity after wall jump
        public float wallJumpVerticalVelocity = 7f;   // Vertical velocity after wall jump
        public GameObject projectilePrefab;
        public Transform projectileSpawnPoint;

        [Header("Wall Jump Settings")]
        public float wallJumpCooldown = 0.5f; // Zeit in Sekunden
        private float lastWallJumpTime = -Mathf.Infinity; // Zeitpunkt des letzten Walljumps


        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                move.x = Input.GetAxis("Horizontal");

                // Normal jump input
                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                {
                    jumpState = JumpState.PrepareToJump;
                }
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }

                if (Input.GetButtonDown("Fire1"))
                {
                    Debug.Log("Fire1 button pressed");
                    ShootProjectile();
                }

                // WALLJUMP ADDITION:
                // If not grounded, but touching wall, allow a wall jump if Jump is pressed
                if (!IsGrounded && IsTouchingWall() && Input.GetButtonDown("Jump"))
                {
                    PerformWallJump();
                }
            }
            else
            {
                move.x = 0;
            }

            UpdateJumpState();
            base.Update();
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (!IsTouchingWall()) // Nur neu berechnen, wenn kein Walljump aktiv ist
            {
                targetVelocity = move * maxSpeed;
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
        }


        // WALLJUMP ADDITION:
        bool IsTouchingWall()
        {
            // We check to the left or right depending on sprite facing direction
            float direction = spriteRenderer.flipX ? -1f : 1f;
            Vector2 origin = transform.position;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * direction, wallCheckDistance, wallLayer);

            return hit.collider != null;
        }

        void PerformWallJump()
        {
            if (Time.time < lastWallJumpTime + wallJumpCooldown)
            {
                Debug.Log("Walljump is on cooldown!");
                return; // Abbrechen, wenn die Abklingzeit noch aktiv ist
            }

            // Walljump durchführen
            float direction = spriteRenderer.flipX ? 1f : -1f;
            velocity.x = direction * wallJumpHorizontalVelocity;
            velocity.y = wallJumpVerticalVelocity;

            jumpState = JumpState.InFlight;

            // Zeitpunkt des Walljumps speichern
            lastWallJumpTime = Time.time;

            Debug.Log($"Walljump executed at time {Time.time}");
        }

        void ShootProjectile()
        {
            if (projectilePrefab != null && projectileSpawnPoint != null)
            {
                Debug.Log("Shooting projectile");
                // Determine direction based on sprite flipping or your facing variable
                float direction = spriteRenderer.flipX ? -1f : 1f;

                // Instantiate the projectile
                GameObject projectileObj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                // Get the Projectile component and set its direction
                var projectile = projectileObj.GetComponent<Projectile>();
                if (projectile != null)
                {
                    projectile.direction = direction;
                }
            }
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}