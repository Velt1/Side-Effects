using UnityEngine;
using Platformer.Mechanics;

public class NPCDialogue : MonoBehaviour
{
    [Tooltip("The lines of dialogue the NPC will say in order.")]
    public string[] dialogueLines;

    [Tooltip("Optional NPC name to display before the dialogue lines.")]
    public string npcName;

    // Flag to indicate if player is in range
    private bool playerInRange = false;

    // A reference to a DialogueUI script that handles showing/hiding the UI
    // Assign this in the inspector or find it at runtime.
    public DialogueUI dialogueUI;

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            playerInRange = false;
            dialogueUI.EndDialogue();
        }
    }

    void Update()
    {
        // Check if player is in range and presses a key to start or advance dialog
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {

            if (!dialogueUI.IsDialogueActive)
            {
                Debug.Log("Player pressed E key to interact with NPC");
                // Start the dialogue
                dialogueUI.StartDialogue(dialogueLines, npcName);
            }
            else
            {
                // Advance to next line
                dialogueUI.ShowNextLine();
            }
        }
    }
}