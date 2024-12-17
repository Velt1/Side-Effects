using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Tooltip("UI Panel containing dialogue text and possibly NPC name")]
    public GameObject dialoguePanel;

    [Tooltip("Text component that shows the NPC name (optional)")]
    public TextMeshProUGUI npcNameText;

    [Tooltip("Text component that shows the dialogue lines")]
    public TextMeshProUGUI dialogueText;

    [Tooltip("Time delay between each character appearing")]
    public float typingSpeed = 0.03f; // Zeit in Sekunden zwischen Zeichen

    private string[] currentLines;
    private int currentIndex;
    public bool IsDialogueActive { get; private set; } = false;

    private Coroutine typingCoroutine;

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

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeLine(currentLines[currentIndex]));
    }

    public void ShowNextLine()
    {
        if (!IsDialogueActive) return;

        currentIndex++;
        if (currentIndex < currentLines.Length)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeLine(currentLines[currentIndex]));
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

    private IEnumerator TypeLine(string line)
    {
        dialogueText.text = ""; // Textfeld leeren
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed); // Wartezeit zwischen den Zeichen
        }
    }
}
