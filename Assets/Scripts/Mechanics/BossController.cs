using UnityEngine;
using Platformer.Mechanics;

public class BossController : EnemyController
{
    [Header("Boss-Spezifische Einstellungen")]
    public float phaseThreshold = 0.5f; // Prozentsatz der HP für Phasenwechsel
    public float specialAttackCooldown = 5f; // Zeit zwischen Spezialangriffen
    private float specialAttackTimer = 0f;

    private bool isPhaseTwo = false;


    void Update()
    {

        // Spezialangriff-Logik
        specialAttackTimer += Time.deltaTime;
        if (specialAttackTimer >= specialAttackCooldown)
        {
            PerformSpecialAttack();
            specialAttackTimer = 0f;
        }

        // Phasenwechsel-Logik
        if (!isPhaseTwo && health.currentHP / (float)health.maxHP <= phaseThreshold)
        {
            EnterPhaseTwo();
        }
    }

    private void PerformSpecialAttack()
    {
        Debug.Log("Boss führt einen Spezialangriff aus!");

        // Beispiel: Der Boss feuert Projektile in alle Richtungen
        for (int i = 0; i < 8; i++) // 8 Richtungen
        {
            float angle = i * 45f; // 360° gleichmäßig aufteilen
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            ShootProjectile(direction);
        }
    }

    private void EnterPhaseTwo()
    {
        isPhaseTwo = true;
        Debug.Log("Boss wechselt in Phase 2!");

        // Beispieldaten für Phase 2
        speed *= 1.5f; // Erhöht die Bewegungsgeschwindigkeit
        specialAttackCooldown /= 2; // Spezialangriffe werden schneller
        // Du kannst weitere Mechaniken hinzufügen (z. B. neue Angriffe)
    }

    private void ShootProjectile(Vector2 direction)
    {
        // Projektil spawnen (ähnlich wie bei deinem Spielercharakter)
        GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        var projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.direction = direction.x; // Setze die Richtung
        }
    }
}
