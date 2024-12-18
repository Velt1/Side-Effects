using Platformer.Core;
using Platformer.Mechanics;
using static Platformer.Core.Simulation;
using UnityEngine;
using System.Collections.Generic;

namespace Platformer.Gameplay
{
    public class HealthIsZero : Simulation.Event<HealthIsZero>
    {
        public Health health;

        // Liste der Prefab-Namen im Resources-Ordner
        private string[] potionNames = new string[] {
            "BlauesWunder",
            "FluessigerAether",
            "Phantomgebraeu"
        };

        public override void Execute()
        {
            if (health.isPlayer)
            {
                Schedule<PlayerDeath>();
            }
            else
            {
                // handle enemy death
                SpawnRandomPotion(health.transform.position);
            }
        }

        private void SpawnRandomPotion(Vector3 position)
        {
            // Zufälligen Index wählen
            int randomIndex = Random.Range(0, potionNames.Length);
            string chosenPotionName = potionNames[randomIndex];

            // Lade das Prefab aus Resources
            GameObject potionPrefab = Resources.Load<GameObject>(chosenPotionName);
            if (potionPrefab == null)
            {
                Debug.LogError("Failed to load potion from Resources: " + chosenPotionName);
                return;
            }

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
                        List<PotionInstance> potionList = new List<PotionInstance>(potionController.potions);
                        potionList.Add(newPotion);
                        potionController.potions = potionList.ToArray();

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
                    Debug.LogError("Spawned potion does not have a PotionInstance component.");
                }
            }
            else
            {
                Debug.LogError("Parent GameObject 'Tokens' not found in the scene.");
            }
        }
    }
}