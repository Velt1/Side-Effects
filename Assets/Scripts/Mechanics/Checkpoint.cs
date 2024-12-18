using UnityEngine;
using Platformer.Mechanics;

public class Checkpoint : MonoBehaviour
{
    public GameObject spawnPoint;
    public DialogueUI dialogueUI; // Referenz auf das DialogueUI

    [Tooltip("Die Dialogeinträge, jeweils mit Sprecher und Text.")]
    public DialogueEntry[] dialogueEntries;

    // Bool, um zu verfolgen, ob der Dialog bereits gestartet wurde
    private bool dialogueTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            // Setze den Spawnpoint an die Position dieses Checkpoints
            if (spawnPoint != null)
            {
                spawnPoint.transform.position = transform.position;
                Debug.Log("Checkpoint reached! Spawnpoint moved here.");
            }

            // Refill Ammo beim Erreichen des Checkpoints
            var projectileManager = player.GetComponent<ProjectileManager>();
            if (projectileManager != null)
            {
                projectileManager.RefillAmmo();
                Debug.Log("Ammo refilled at checkpoint!");
            }

            // Dialog nur starten, wenn er noch nicht gestartet wurde
            if (!dialogueTriggered && dialogueUI != null && dialogueEntries != null && dialogueEntries.Length > 0)
            {
                // Falls gerade ein anderer Dialog läuft, beende diesen zuerst
                if (dialogueUI.IsDialogueActive)
                {
                    dialogueUI.EndDialogue();
                }

                // Starte den neuen Dialog
                dialogueUI.StartDialogue(dialogueEntries);
                dialogueTriggered = true; // Dialog als "abgespielt" markieren
            }
        }
    }
}