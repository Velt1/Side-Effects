using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class VignetteController : MonoBehaviour
{
    public Volume globalVolume;
    private Vignette vignette;

    void Start()
    {
        VolumeProfile profile = globalVolume.sharedProfile;
        if (!profile.TryGet<Vignette>(out vignette))
        {
            vignette = profile.Add<Vignette>(false);
        }

        vignette.intensity.Override(0.0f);
    }

    public void AdjustVignette(float intensity, float smoothness = 0.5f, float roundness = 1.0f)
    {
        vignette.intensity.Override(intensity);
        Debug.Log("Vignette intensity: " + intensity);
    }


}
