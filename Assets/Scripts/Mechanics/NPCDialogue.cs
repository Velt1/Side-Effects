using UnityEngine;
using Platformer.Mechanics;

public class NPCDialogue : MonoBehaviour
{
    [Tooltip("The lines of dialogue the NPC will say in order.")]
    public DialogueEntry[] dialogueEntries;

    private bool playerInRange = false;
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
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!dialogueUI.IsDialogueActive)
            {
                dialogueUI.StartDialogue(dialogueEntries);
            }
            else
            {
                dialogueUI.ShowNextLine();
            }
        }
    }
}