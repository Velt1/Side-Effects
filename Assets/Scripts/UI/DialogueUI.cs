using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.03f;

    private DialogueEntry[] currentEntries;
    private int currentIndex;
    public bool IsDialogueActive { get; private set; } = false;

    private Coroutine typingCoroutine;

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(DialogueEntry[] entries)
    {
        if (entries == null || entries.Length == 0) return;

        currentEntries = entries;
        currentIndex = 0;
        IsDialogueActive = true;
        dialoguePanel.SetActive(true);

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (currentIndex < 0 || currentIndex >= currentEntries.Length) return;

        DialogueEntry entry = currentEntries[currentIndex];

        // Sprechername anzeigen, wenn vorhanden
        if (npcNameText != null)
            npcNameText.text = string.IsNullOrEmpty(entry.speakerName) ? "" : entry.speakerName;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeLine(entry.lineText));
    }

    public void ShowNextLine()
    {
        if (!IsDialogueActive) return;

        currentIndex++;
        if (currentIndex < currentEntries.Length)
        {
            ShowCurrentLine();
        }
        else
        {
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
        dialogueText.text = "";
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}