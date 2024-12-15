using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class ColorController : MonoBehaviour
{
    public Volume globalVolume;
    private ColorAdjustments colorAdjustments;

    void Start()
    {
        VolumeProfile profile = globalVolume.sharedProfile;
        if (!profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            colorAdjustments = profile.Add<ColorAdjustments>(false);
        }

        colorAdjustments.contrast.Override(0.0f);
        colorAdjustments.colorFilter.Override(new Color(1.0f, 1.0f, 1.0f));

    }

    public void AdjustColors(float intensity, float green = 0.5f, float blue = 1.0f)
    {
        colorAdjustments.contrast.Override(intensity);
        colorAdjustments.colorFilter.Override(new Color(1.0f, green, blue));
    }


}
