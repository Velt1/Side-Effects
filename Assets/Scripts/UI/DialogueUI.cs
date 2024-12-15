using UnityEngine;
using UnityEngine.UI;
using TMPro; // falls Sie TextMeshPro verwenden möchten

public class DialogueUI : MonoBehaviour
{
    [Tooltip("UI Panel containing dialogue text and possibly NPC name")]
    public GameObject dialoguePanel;

    [Tooltip("Text component that shows the NPC name (optional)")]
    public TextMeshProUGUI npcNameText;

    [Tooltip("Text component that shows the dialogue lines")]
    public TextMeshProUGUI dialogueText;

    private string[] currentLines;
    private int currentIndex;
    public bool IsDialogueActive { get; private set; } = false;

    void Start()
    {
        // Am Anfang sicherstellen, dass das Panel versteckt ist
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string[] lines, string npcName = "")
    {
        if (lines.Length == 0) return;

        currentLines = lines;
        currentIndex = 0;
        IsDialogueActive = true;
        dialoguePanel.SetActive(true);

        if (npcNameText != null)
            npcNameText.text = npcName;

        dialogueText.text = currentLines[currentIndex];
        Debug.Log("Dialogue started with " + npcName);
        Debug.Log("Line 1: " + currentLines[0]);
    }

    public void ShowNextLine()
    {
        if (!IsDialogueActive) return;

        currentIndex++;
        if (currentIndex < currentLines.Length)
        {
            dialogueText.text = currentLines[currentIndex];
        }
        else
        {
            // Keine weiteren Dialogzeilen vorhanden, Dialog beenden
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        IsDialogueActive = false;
        dialoguePanel.SetActive(false);
    }
}