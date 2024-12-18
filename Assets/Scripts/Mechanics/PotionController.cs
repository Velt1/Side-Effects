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

        public void ApplyPotionEffect(PotionEffectType effectType, float duration, PlayerController player)
        {
            int usageCount = potionUsageCounts.ContainsKey(effectType) ? potionUsageCounts[effectType] : 0;

            switch (effectType)
            {
                case PotionEffectType.BlauesWunder:
                    // Effekte: +2 Extra Leben, unendlich Projektile für begrenzte Zeit
                    player.health.Increment(); // +1 Leben
                    player.health.Increment(); // +1 weiteres Leben
                    player.StartCoroutine(EnableInfiniteProjectiles(player, 10f)); // 10 Sekunden Beispieldauer

                    Debug.Log("Blaues Wunder Effekt: +2 Leben, unendlich Projektile kurzzeitig!");
                    break;

                case PotionEffectType.FluessigerAether:
                    // Effekte: Höhere Geschwindigkeit und Sprungkraft
                    // Dauer wird jedes Mal ggf. reduziert, je mehr Tränke genommen wurden
                    float baseDuration = 10f;
                    float reducedDuration = Mathf.Max(3f, baseDuration - (usageCount * 1f));
                    // Effekt anwenden
                    player.maxSpeed *= 1.5f;
                    player.jumpTakeOffSpeed *= 1.5f;
                    // Nach Ablauf revertieren und ggf. langsamer machen
                    player.StartCoroutine(RevertAetherEffect(player, reducedDuration));
                    Debug.Log("Flüssiger Aether: Schneller + höher springen für " + reducedDuration + " Sekunden.");

                    break;

                case PotionEffectType.Phantomgebraeu:
                    // Effekte: Unverwundbar und kann durch Gegner gleiten
                    // Mache Spieler invulnerable und collider disabled:
                    //player.collider2d.enabled = false;
                    // Du könntest einen Status für Unverwundbarkeit setzen:
                    player.health.invulnerable = true;
                    // Nach Ablauf wieder normalisieren, außer du willst es permanent machen.
                    player.StartCoroutine(RevertPhantomEffect(player, 8f));
                    Debug.Log("Phantomgebräu: Unverwundbar und Phasing für 8 Sekunden!");
                    break;

                default:
                    Debug.LogWarning("Unknown potion effect type!");
                    break;
            }
        }

        public void ApplyPermanentSideEffect(PotionEffectType effectType, PlayerController player, int intensity)
        {
            // Hier werden die permanenten oder eskalierenden Nebenwirkungen verstärkt.
            // Der "intensity"-Parameter steigt mit jedem zusätzlichen Trank über der Schwelle.

            switch (effectType)
            {
                case PotionEffectType.BlauesWunder:
                    // Nebenwirkung: Extra-Leben verschwinden immer schneller, ab X Tränken Chance auf direkten Tod
                    // Beispiel: Bei jeder Intensitätsstufe reduzierst du die Health Caps oder fügst eine Tod-Chance hinzu:
                    // z.B. ab intensity 1 (also 6. Trank), 10% Chance auf sofortigen Tod beim Konsum
                    // ab intensity 2 (7. Trank), 20% Chance, usw.
                    float deathChance = 0.1f * intensity;
                    FindObjectOfType<ColorController>().AdjustColors(10.0f * intensity, 1.0f - (0.1f * intensity), 1.0f - (0.1f * intensity));
                    FindObjectOfType<VignetteController>().AdjustVignette(0.3f * intensity, 0.7f, 0.9f);
                    if (Random.value < deathChance)
                    {
                        Debug.Log("Nebenwirkung Blaues Wunder: Sofortiger Tod!");
                        player.health.Die();
                    }
                    else
                    {
                        Debug.Log("Nebenwirkung Blaues Wunder Intensität " + intensity + ": Höhere Todeschance!");
                    }
                    break;

                case PotionEffectType.FluessigerAether:
                    // Nebenwirkung: Verstärkte Effekte halten immer kürzer, Spieler wird nach Effekt langsamer
                    // Mit höherer Intensität könnte der Spieler nach Ablauf immer länger verlangsamt sein.
                    // Beispiel: pro Intensität +2 Sek. Extra-Langsame Phase nach Effektende
                    // Dies kann in RevertAetherEffect berücksichtigt werden, indem du intensity abfragst:
                    // Hier kannst du den globalen Zustand speichern, z.B. im Player selbst.
                    player.maxSpeed -= 0.01f * intensity; // wird immer langsamer insgesamt
                    FindObjectOfType<ColorController>().AdjustColors(10.0f * intensity, 1.0f - (0.1f * intensity), 1.0f - (0.1f * intensity));
                    FindObjectOfType<VignetteController>().AdjustVignette(0.3f * intensity, 0.7f, 0.9f);
                    Debug.Log("Nebenwirkung Flüssiger Aether Intensität " + intensity + ": Dauerhaft geringere Grundgeschwindigkeit!");
                    break;

                case PotionEffectType.Phantomgebraeu:
                    // Nebenwirkung: Spieler verliert teilweise die Kontrolle.
                    // Mit jeder Intensitätsstufe verschlechtert sich die Steuerung stärker.
                    // Beispiel: Input-Inversion oder zufälliges "Stottern".
                    // Hier nur ein Beispiel: Spieler-Input wird langsamer umgesetzt.
                    // Du könntest ein Flag im Player setzen, dass bei jeder Intensität den Input verzögert oder invertiert.
                    Debug.Log("Nebenwirkung Phantomgebraeu Intensität " + intensity + ": Steuerung wird schwieriger!");
                    FindObjectOfType<VignetteController>().AdjustVignette(0.3f * intensity, 0.7f, 0.9f);
                    FindObjectOfType<ColorController>().AdjustColors(10.0f * intensity, 1.0f - (0.1f * intensity), 1.0f - (0.1f * intensity));
                    //player.StartCoroutine(ApplyControlDistortion(player, intensity));
                    break;
            }
        }

        // Beispiel-Koroutinen für die Effekte

        private IEnumerator EnableInfiniteProjectiles(PlayerController player, float duration)
        {
            // Angenommen PlayerController hat eine Variable, die unendliche Projektile erlaubt:
            // Im PlayerController könntest du ein bool "infiniteAmmo" definieren.
            bool originalSetting = player.projectilePrefab != null;
            // Hier müsstest du anpassen, wie du infinite Projektile sicherstellst,
            // z.B. indem du im PlayerController eine Variable infiniteAmmo = true setzt.

            // Beispiel: Wir tun so, als hätte Player unendlich Projektile:
            // Vielleicht besitzt der Spieler normal ein Ammo-Limit. Hier einfach nur ein Flag:
            player.GetComponent<ProjectileManager>()?.EnableInfiniteAmmo(true);

            yield return new WaitForSeconds(duration);

            // Danach infinite Ammo wieder ausschalten
            player.GetComponent<ProjectileManager>()?.EnableInfiniteAmmo(false);
        }

        private IEnumerator RevertAetherEffect(PlayerController player, float duration)
        {
            // Warte bis die Zeit um ist.
            yield return new WaitForSeconds(duration);
            // Effekt zurücksetzen
            player.maxSpeed /= 1.5f;
            player.jumpTakeOffSpeed /= 1.2f;

            // Nach dem Effekt wird der Spieler langsamer für eine gewisse Zeit.
            float slowDuration = 3f; // Basisverlangsamung
                                     // Falls du die Intensität berücksichtigen willst, hol sie dir:
            int usageCount = potionUsageCounts[PotionEffectType.FluessigerAether];
            int intensity = Mathf.Max(0, usageCount - 5);
            slowDuration += intensity * 2f; // Für jedes Intensitätslevel +2s langsam

            float originalSpeed = player.maxSpeed;
            player.maxSpeed *= 0.5f; // Temporär langsamer
            yield return new WaitForSeconds(slowDuration);
            player.maxSpeed = originalSpeed;

            // Bei sehr vielen konsumierten Tränken kannst du z. B. random den Effekt vorzeitig abbrechen
            // (Das könntest du beim Aktivieren des Effekts schon berücksichtigen.)
        }

        private IEnumerator RevertPhantomEffect(PlayerController player, float duration)
        {
            yield return new WaitForSeconds(duration);
            // Effekt zurücksetzen
            //player.collider2d.enabled = true;
            player.health.invulnerable = false;
        }

        private IEnumerator ApplyControlDistortion(PlayerController player, int intensity)
        {
            // Beispiel: Input-Inversion für 5 Sekunden * Intensität
            float distortDuration = 5f * intensity;

            // PlayerController so anpassen, dass er eine Variable "controlDistortion" hat
            // oder du modifizierst direkt im Update den Input basierend auf einer Variable.
            player.controlEnabled = true; // normal an, aber wir invertieren Input in ComputeVelocity oder Update
            player.GetComponent<ControlDistortion>()?.EnableDistortion(intensity);

            yield return new WaitForSeconds(distortDuration);

            player.GetComponent<ControlDistortion>()?.DisableDistortion();
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