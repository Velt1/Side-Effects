using UnityEngine;

public class ControlDistortion : MonoBehaviour
{
    private int intensity = 0;
    private bool distortionEnabled = false;

    // Diese Werte kannst du anpassen, um die Inputverzerrung zu steuern.
    // z.B. Inversion, randomOffset, usw.
    public bool invertHorizontal = false;
    public bool randomStutter = false;

    // Rufe diese Methode auf, um den Effekt zu aktivieren.
    // Je höher intensity, desto stärker die Verzerrung.
    public void EnableDistortion(int intensityLevel)
    {
        intensity = intensityLevel;
        distortionEnabled = true;
        ApplyIntensitySettings();
    }

    // Rufe diese Methode auf, um den Effekt zu deaktivieren.
    public void DisableDistortion()
    {
        intensity = 0;
        distortionEnabled = false;
        invertHorizontal = false;
        randomStutter = false;
    }

    void ApplyIntensitySettings()
    {
        // Ein Beispiel:
        // intensity 1: invertiere horizontalen Input
        // intensity 2: invertiere horizontal + random stutter
        // intensity >2: noch stärkere Effekte

        if (intensity >= 1) invertHorizontal = true;
        if (intensity >= 2) randomStutter = true;
    }

    public Vector2 DistortInput(Vector2 originalInput)
    {
        if (!distortionEnabled) return originalInput;

        Vector2 distorted = originalInput;

        // Inversion bei horizontaler Achse
        if (invertHorizontal)
        {
            distorted.x = -distorted.x;
        }

        // Random Stutter: gelegentliches "Aussetzen" oder Ruckeln im Input
        if (randomStutter)
        {
            // Mit einer gewissen Wahrscheinlichkeit den Input "stottern" lassen
            // z.B. kleine Wahrscheinlichkeitsberechnung:
            if (Random.value < 0.1f) // 10% Chance, dass Input frameweise genullt wird
            {
                distorted.x = 0f;
            }
        }

        return distorted;
    }
}