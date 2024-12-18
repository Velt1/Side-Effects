using UnityEngine;
using System.Collections.Generic;
using Platformer.Mechanics;
using TMPro;
public class PlayerInventory : MonoBehaviour
{
    private Dictionary<PotionEffectType, int> potionCounts = new Dictionary<PotionEffectType, int>();
    private int totalPotionsUsed = 0;
    public TextMeshProUGUI bluePotionCount;
    public TextMeshProUGUI aetherPotionCount;
    public TextMeshProUGUI phantomPotionCount;

    public void Update()
    {
        // Beispiel: Drückt Spieler "Alpha1", um "Blaues Wunder" zu konsumieren
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UsePotion(PotionEffectType.BlauesWunder, GetComponent<PlayerController>());
        }
        // Drückt Spieler "Alpha2", um "Flüssiger Aether" zu konsumieren
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UsePotion(PotionEffectType.FluessigerAether, GetComponent<PlayerController>());
        }
        // Drückt Spieler "Alpha3", um "Phantomgebräu" zu konsumieren
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UsePotion(PotionEffectType.Phantomgebraeu, GetComponent<PlayerController>());
        }
    }
    public void AddPotion(PotionInstance potion)
    {
        if (!potionCounts.ContainsKey(potion.effectType))
            potionCounts[potion.effectType] = 0;

        potionCounts[potion.effectType] += 1;
        // Increate the count of the potion in the inventory
        updateCountText(effectType: potion.effectType);
        Debug.Log("Potion added to inventory: " + potion.effectType);
    }

    public void updateCountText(PotionEffectType effectType)
    {
        switch (effectType)
        {
            case PotionEffectType.BlauesWunder:
                bluePotionCount.text = potionCounts[PotionEffectType.BlauesWunder].ToString();
                break;
            case PotionEffectType.FluessigerAether:
                aetherPotionCount.text = potionCounts[PotionEffectType.FluessigerAether].ToString();
                break;
            case PotionEffectType.Phantomgebraeu:
                phantomPotionCount.text = potionCounts[PotionEffectType.Phantomgebraeu].ToString();
                break;
        }
    }

    public void UsePotion(PotionEffectType effectType, PlayerController player)
    {
        if (potionCounts.ContainsKey(effectType) && potionCounts[effectType] > 0)
        {
            // Einen Trank weniger
            potionCounts[effectType]--;
            totalPotionsUsed++;
            var potionController = UnityEngine.Object.FindObjectOfType<PotionController>();
            if (potionController != null)
            {

                // Wende den Effekt an
                potionController.ApplyPotionEffect(effectType, 0f, player);
                // Duration = 0f, wenn Effekt permanent ist oder einfach ignorieren

                // Überprüfe, ob permanente Nebenwirkungen anfallen
                if (totalPotionsUsed > 3)
                {
                    // Intensität = usageCount - 5, d.h. bei 6. Trank Intensität 1, bei 7. Trank Intensität 2, etc.
                    int intensity = totalPotionsUsed - 3;
                    potionController.ApplyPermanentSideEffect(effectType, player, intensity);
                    Debug.Log("Permanent side effect increased to intensity level: " + intensity);
                }
            }
            updateCountText(effectType);
        }
        else
        {
            Debug.Log("No potions of type " + effectType + " available!");
        }
    }
}