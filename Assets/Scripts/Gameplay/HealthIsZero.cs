using Platformer.Core;
using Platformer.Mechanics;
using static Platformer.Core.Simulation;
using UnityEngine;
using System.Collections.Generic;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player health reaches 0. This usually would result in a 
    /// PlayerDeath event.
    /// </summary>
    /// <typeparam name="HealthIsZero"></typeparam>
    public class HealthIsZero : Simulation.Event<HealthIsZero>
    {
        public Health health;

        public override void Execute()
        {
            if (health.isPlayer)
            {
                Schedule<PlayerDeath>();
            }
            else
            {
                // handle enemy death 
                SpawnPotion(health.transform.position);
            }
        }
        private void SpawnPotion(Vector3 position)
        {
            // Lade das Prefab
            GameObject potionPrefab = Resources.Load<GameObject>("Potion");
            if (potionPrefab != null)
            {
                // Erstelle die Potion-Instanz
                GameObject tokensParent = GameObject.Find("Tokens");
                if (tokensParent != null)
                {
                    GameObject potionObject = GameObject.Instantiate(potionPrefab, position, Quaternion.identity, tokensParent.transform);

                    // Füge die neue Potion dem Controller hinzu
                    PotionInstance newPotion = potionObject.GetComponent<PotionInstance>();
                    if (newPotion != null)
                    {
                        var potionController = UnityEngine.Object.FindObjectOfType<PotionController>();
                        if (potionController != null)
                        {
                            // Füge die neue Potion-Instanz dem Controller hinzu
                            List<PotionInstance> potionList = new List<PotionInstance>(potionController.potions);
                            potionList.Add(newPotion);
                            potionController.potions = potionList.ToArray();

                            // Registriere den Controller für die neue Potion
                            newPotion.controller = potionController;
                            newPotion.potionIndex = potionList.Count - 1; // Setze Index
                        }
                        else
                        {
                            Debug.LogError("PotionController not found in the scene.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Potion prefab does not have a PotionInstance component.");
                    }
                }
                else
                {
                    Debug.LogError("Parent GameObject 'Tokens' not found in the scene.");
                }
            }
            else
            {
                Debug.LogError("PotionPrefab not found in Resources folder.");
            }
        }
    }
}