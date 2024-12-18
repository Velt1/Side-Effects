using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public int maxAmmo = 1; // maximal mögliche Munition ohne Checkpoint
    private int currentAmmo;
    private bool infiniteAmmo = false;

    void Start()
    {
        // Zu Beginn hat der Spieler 1 Schuss
        currentAmmo = maxAmmo;
    }

    public bool CanShoot()
    {
        // Wenn unendlich Munition an ist, kann der Spieler immer schießen
        if (infiniteAmmo) return true;

        // Sonst prüfen wir, ob noch Munition vorhanden ist
        return currentAmmo > 0;
    }

    public void ConsumeAmmo()
    {
        if (!infiniteAmmo && currentAmmo > 0)
        {
            currentAmmo--;
        }
    }

    public void RefillAmmo()
    {
        // Auffüllen auf maxAmmo
        currentAmmo = maxAmmo;
    }

    public void EnableInfiniteAmmo(bool enable)
    {
        infiniteAmmo = enable;
    }
}