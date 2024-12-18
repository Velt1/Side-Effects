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

        internal Collider2D collider2d;
        internal AudioSource audioSource;
        internal Health health;
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
        public float wallCheckDistance = 0.1f; // Distance to check for wall in front
        public float wallJumpHorizontalVelocity = 7f; // Horizontal velocity after wall jump
        public float wallJumpVerticalVelocity = 7f;   // Vertical velocity after wall jump
        public GameObject projectilePrefab;
        public Transform projectileSpawnPoint;

        [Header("Wall Jump Settings")]
        public float wallJumpCooldown = 0.5f; // Zeit in Sekunden
        private float lastWallJumpTime = -Mathf.Infinity; // Zeitpunkt des letzten Walljumps
        public PlayerInventory inventory;
        public ProjectileManager projectileManager;
        public DialogueUI dialogueUI;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            inventory = GetComponent<PlayerInventory>();
            projectileManager = GetComponent<ProjectileManager>();
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                move.x = Input.GetAxis("Horizontal");

                //press e or enter to end dialogue
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
                {
                    if (dialogueUI.IsDialogueActive)
                    {
                        dialogueUI.EndDialogue();
                    }
                }
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
                //Use Keycode Q
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    Debug.Log("Q button pressed");
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
            if (projectileManager != null && projectileManager.CanShoot())
            {
                Debug.Log("Shooting projectile");
                float direction = spriteRenderer.flipX ? -1f : 1f;
                GameObject projectileObj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                var projectile = projectileObj.GetComponent<Projectile>();
                if (projectile != null)
                {
                    projectile.direction = direction;
                    projectile.owner = ProjectileOwner.Player;
                }

                // Verbrauch Munition
                projectileManager.ConsumeAmmo();
            }
            else
            {
                Debug.Log("No ammo left! Reach a checkpoint or enable infinite ammo.");
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