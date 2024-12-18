using UnityEngine;
using Platformer.Mechanics;

[RequireComponent(typeof(Collider2D))]
public class MusicTriggerZone : MonoBehaviour
{
    [Tooltip("Die AudioSource, welche die Hintergrundmusik abspielt.")]
    public AudioSource musicSource;

    [Tooltip("Die normale Hintergrundmusik, die gespielt wird, wenn der Player nicht im Trigger ist.")]
    public AudioClip defaultMusic;

    [Tooltip("Die spezielle Musik, die gespielt wird, wenn der Player im Trigger ist.")]
    public AudioClip triggerMusic;

    // Flag um zu erkennen, ob wir bereits in der Trigger-Zone sind
    private bool inTriggerZone = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Prüfe, ob das Objekt ein PlayerController ist (oder ein anderer Tag, je nach Setup)
        if (other.GetComponent<PlayerController>() != null)
        {
            // Wenn noch nicht in der Zone, Musik wechseln
            if (!inTriggerZone && triggerMusic != null && musicSource != null)
            {
                inTriggerZone = true;
                musicSource.clip = triggerMusic;
                musicSource.Play();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Wenn der Player die Zone verlässt, zur Default-Musik zurückwechseln
        if (other.GetComponent<PlayerController>() != null)
        {
            if (inTriggerZone && defaultMusic != null && musicSource != null)
            {
                inTriggerZone = false;
                musicSource.clip = defaultMusic;
                musicSource.Play();
            }
        }
    }
}