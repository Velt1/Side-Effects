using System;
using UnityEngine;

[Serializable]
public class DialogueEntry
{
    [Tooltip("Name des Sprechers, z. B. 'Cordis' oder 'Spielerchar'.")]
    public string speakerName;

    [TextArea(3, 10)]
    [Tooltip("Der gesprochene Text für diese Zeile.")]
    public string lineText;
}