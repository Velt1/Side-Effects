using UnityEngine;
using Platformer.Mechanics;

public class Checkpoint : MonoBehaviour
{
    public GameObject spawnPoint;
    public DialogueUI dialogueUI; // Referenz auf das DialogueUI
    [TextArea(3, 5)]
    public string[] dialogueLines; // Zeilen für den Dialog beim Erreichen des Checkpoints
    public string npcName; // Optional: Name des NPC/Charakters, von dem der Dialog stammt

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

            // Dialog abspielen, sofern vorhanden und DialogueUI referenziert ist
            if (dialogueUI != null && dialogueLines.Length > 0)
            {
                // Falls gerade ein anderer Dialog läuft, erst beenden
                if (dialogueUI.IsDialogueActive)
                {
                    dialogueUI.EndDialogue();
                }

                dialogueUI.StartDialogue(dialogueLines, npcName);
            }
        }
    }
}