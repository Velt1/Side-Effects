using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This class represents a potion instance that the player can collect.
    /// Potions provide temporary boosts or abilities, but can also cause side effects
    /// if overused.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class PotionInstance : MonoBehaviour
    {
        public AudioClip potionCollectAudio;

        [Tooltip("If true, animation will start at a random position in the sequence.")]
        public bool randomAnimationStartTime = false;

        [Tooltip("List of frames that make up the idle animation.")]
        public Sprite[] idleAnimation;

        [Tooltip("List of frames that make up the collected animation.")]
        public Sprite[] collectedAnimation;

        internal Sprite[] sprites = new Sprite[0];
        internal SpriteRenderer _renderer;

        // Unique index assigned by the PotionController in a scene.
        internal int potionIndex = -1;
        internal PotionController controller;
        // Active frame in animation, updated by the controller.
        internal int frame = 0;
        internal bool collected = false;

        [Header("Potion Effect Settings")]
        [Tooltip("Type of effect this potion grants.")]
        public PotionEffectType effectType;

        [Tooltip("Duration in seconds of the temporary effect.")]
        public float effectDuration = 5f;

        [Tooltip("Chance (0 to 1) that a side effect occurs with repeated usage.")]
        public float sideEffectChance = 0.2f;

        // How many times the player can safely use this type of potion before side effects occur.
        [Tooltip("Max safe usage count before side effects start to appear.")]
        public int safeUsageCount = 0;

        void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            // Set initial sprite set to idle
            sprites = idleAnimation;
            if (randomAnimationStartTime)
                frame = Random.Range(0, sprites.Length);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            // Only execute OnPlayerEnter if the player collides with this potion.
            var player = other.gameObject.GetComponent<PlayerController>();
            if (player != null) OnPlayerEnter(player);
        }

        void OnPlayerEnter(PlayerController player)
        {
            if (collected) return;
            // Potion is now considered collected
            frame = 0;
            sprites = collectedAnimation;
            if (controller != null)
                collected = true;

            // Schedule an event to handle potion collection
            var ev = Schedule<PlayerPotionCollision>();
            ev.potion = this;
            ev.player = player;

            // Apply effects to the player immediately (or via the event).
            ApplyPotionEffect(player);
        }

        void ApplyPotionEffect(PlayerController player)
        {
            if (controller != null)
            {
                int usageCount = controller.IncrementPotionUsage(effectType);
                controller.ApplyPotionEffect(effectType, effectDuration, player);
                // Wenn Nutzung die Sicherheitsgrenze überschreitet, triggere Nebeneffekt
                if (usageCount > safeUsageCount && Random.value < sideEffectChance)
                {
                    Debug.Log("Side effect triggered!");
                    controller.ApplyPotionSideEffect(effectType, player);
                }
            }
        }


    }

    /// <summary>
    /// Enum representing different types of potion effects.
    /// Extend this as needed (extra life, higher jump, invincibility, etc.)
    /// </summary>
    public enum PotionEffectType
    {
        ExtraLife,
        HighJump,
        PhaseThroughEnemies,
        // Add more effects as needed
    }
}