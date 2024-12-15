using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This class animates all potion instances in a scene, similar to how token instances
    /// were animated. If the potions property is empty, it will automatically find and 
    /// load all potion instances in the scene at runtime.
    /// </summary>
    public class PotionController : MonoBehaviour
    {
        [Tooltip("Frames per second at which potions are animated.")]
        public float frameRate = 12;

        [Tooltip("Instances of potions which are animated. If empty, potion instances are found and loaded at runtime.")]
        public PotionInstance[] potions;

        float nextFrameTime = 0;

        [ContextMenu("Find All Potions")]
        void FindAllPotionsInScene()
        {
            potions = UnityEngine.Object.FindObjectsOfType<PotionInstance>();
        }
        private Dictionary<PotionEffectType, int> potionUsageCounts = new Dictionary<PotionEffectType, int>();


        void Awake()
        {
            // If potions are empty, find all instances.
            // If not empty, they have been added at editor time.
            if (potions.Length == 0)
                FindAllPotionsInScene();

            // Register all potions so they can work with this controller.
            for (var i = 0; i < potions.Length; i++)
            {
                potions[i].potionIndex = i;
                potions[i].controller = this;
            }
        }

        void Update()
        {
            // If it's time for the next frame...
            if (Time.time - nextFrameTime > (1f / frameRate))
            {
                // Update all potions with the next animation frame.
                for (var i = 0; i < potions.Length; i++)
                {
                    var potion = potions[i];
                    // If potion is null, it has been collected/disabled and is no longer animated.
                    if (potion != null)
                    {
                        potion._renderer.sprite = potion.sprites[potion.frame];
                        if (potion.collected && potion.frame == potion.sprites.Length - 1)
                        {
                            potion.gameObject.SetActive(false);
                            potions[i] = null;
                        }
                        else
                        {
                            potion.frame = (potion.frame + 1) % potion.sprites.Length;
                        }
                    }
                }
                // Calculate the time of the next frame.
                nextFrameTime += 1f / frameRate;
            }
        }
        public int IncrementPotionUsage(PotionEffectType effectType)
        {
            if (!potionUsageCounts.ContainsKey(effectType))
                potionUsageCounts[effectType] = 0;

            potionUsageCounts[effectType]++;
            return potionUsageCounts[effectType];
        }

        public void ApplyPotionSideEffect(PotionEffectType effectType, PlayerController player)
        {
            switch (effectType)
            {
                case PotionEffectType.HighJump:
                    // Beispiel: Spieler sieht verschwommen
                    StartCoroutine(ApplyBlurEffect(player, 5f));
                    Debug.Log("Side effect: Blurred vision!");
                    break;

                case PotionEffectType.ExtraLife:
                    // Beispiel: Spieler verliert Bewegungsgeschwindigkeit
                    player.maxSpeed *= 0.8f;
                    StartCoroutine(RemoveEffectAfterDuration(() => player.maxSpeed /= 0.8f, 5f));
                    Debug.Log("Side effect: Reduced movement speed!");
                    break;

                case PotionEffectType.PhaseThroughEnemies:
                    // Beispiel: Spieler kann nicht springen
                    StartCoroutine(ApplyColorEffect(player, 5f));
                    Debug.Log("Side effect: Cannot jump!");
                    break;

                default:
                    Debug.LogWarning("Unknown potion side effect type!");
                    break;
            }
        }

        public void ApplyPotionEffect(PotionEffectType effectType, float duration, PlayerController player)
        {
            switch (effectType)
            {
                case PotionEffectType.ExtraLife:
                    player.health.Increment();
                    Debug.Log("Extra Life granted!");
                    break;
                case PotionEffectType.HighJump:
                    player.jumpTakeOffSpeed *= 1.5f; // Beispiel: Erhöht Sprungkraft um 50 %
                    StartCoroutine(RemoveEffectAfterDuration(() => player.jumpTakeOffSpeed /= 1.5f, duration));
                    Debug.Log("High Jump enabled!");
                    break;
                case PotionEffectType.PhaseThroughEnemies:
                    player.collider2d.enabled = false; // Beispiel: Spieler kollidiert nicht mit Feinden
                    StartCoroutine(RemoveEffectAfterDuration(() => player.collider2d.enabled = true, 0.2f));
                    Debug.Log("Phase Through Enemies activated!");
                    break;
                default:
                    Debug.LogWarning("Unknown potion effect type!");
                    break;
            }
        }

        private IEnumerator ApplyBlurEffect(PlayerController player, float duration)
        {
            // Beispiel für verschwommene Sicht(Post-Processing o.ä.)
            FindObjectOfType<VignetteController>().AdjustVignette(0.9f, 0.7f, 0.9f);
            yield return new WaitForSeconds(duration);
            FindObjectOfType<VignetteController>().AdjustVignette(0.0f, 0.7f, 0.9f);
        }

        private IEnumerator ApplyColorEffect(PlayerController player, float duration)
        {
            // Beispiel für verschwommene Sicht(Post-Processing o.ä.)
            FindObjectOfType<ColorController>().AdjustColors(100.0f, 0.2f, 0.2f);
            yield return new WaitForSeconds(duration);
            FindObjectOfType<ColorController>().AdjustColors(0.0f, 1.0f, 1.0f);
        }


        private IEnumerator RemoveEffectAfterDuration(System.Action removeEffect, float duration)
        {
            yield return new WaitForSeconds(duration);
            removeEffect.Invoke();
            Debug.Log("Potion effect removed.");
        }

        public void ApplyPotionSideEffect(PotionEffectType effectType)
        {
            // Apply negative side effects or even permadeath if conditions are met.
            // For example, reduce health, slow movement, or trigger a permadeath event.
        }
    }
}