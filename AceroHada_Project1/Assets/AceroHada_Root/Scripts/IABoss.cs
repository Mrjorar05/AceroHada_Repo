using System.Net.Sockets;
using UnityEngine;

public class IABoss : MonoBehaviour
{
    [Header("Player & Movement")]
    public Transform player;
    public float moveSpeed = 3f;

    [Header("Attack Settings")]
    public float meleeRange = 2f;
    public float meleeCooldown = 2f;
    public float rocketCooldown = 5f;
    public GameObject rocketPrefab;
    public Transform rocketSpawnPoint;

    [Header("Vulnerability Window")]
    public float vulnerableTime = 1f;
    private bool isVulnerable = true;

    [Header("Animator")]
    public Animator animator;

    private float lastMeleeTime;
    private float lastRocketTime;

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Moverse hacia el jugador si está fuera del rango de melee
        if (distance > meleeRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }

        // Ataque melee
        if (distance <= meleeRange && Time.time - lastMeleeTime >= meleeCooldown)
        {
            StartCoroutine(MeleeAttack());
        }

        // Ataque con cohete
        if (Time.time - lastRocketTime >= rocketCooldown)
        {
            StartCoroutine(RocketAttack());
        }

        // Animación Idle si no se mueve ni ataca
        if (!animator.GetBool("IsWalking") && !animator.GetBool("IsAttacking") && !animator.GetBool("Rocket"))
        {
            animator.SetBool("Idle", true);
        }
        else
        {
            animator.SetBool("Idle", false);
        }
    }

    void MoveTowardsPlayer()
    {
        animator.SetBool("IsWalking", true);
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    System.Collections.IEnumerator MeleeAttack()
    {
        isVulnerable = false; // no se puede dañar mientras ataca
        lastMeleeTime = Time.time;
        animator.SetBool("IsAttacking", true);
        Debug.Log("Boss ataca melee!");
        yield return new WaitForSeconds(0.5f); // duración del ataque
        animator.SetBool("IsAttacking", false);
        isVulnerable = true;
    }

    System.Collections.IEnumerator RocketAttack()
    {
        isVulnerable = false;
        lastRocketTime = Time.time;
        Debug.Log("Boss dispara cohete!");
        // Instancia el cohete
        if (rocketPrefab != null && rocketSpawnPoint != null)
        {
            Instantiate(rocketPrefab, rocketSpawnPoint.position, Quaternion.identity);
        }
       
        yield return new WaitForSeconds(1f); // duración del ataque
        animator.SetBool("Rocket", false);
        isVulnerable = true;
    }

    // Método para que otros scripts puedan consultar si el boss es vulnerable
    public bool CanTakeDamage()
    {
        return isVulnerable;
    }
}
