using System.Collections;
using System.Net.Sockets;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float meleeRange = 3f;
    [SerializeField] private float rangedRange = 10f;

    [Header("Melee Attack")]
    [SerializeField] private int meleeDamage = 25;
    [SerializeField] private float meleeCooldown = 2f;
    [SerializeField] private float meleeAttackRadius = 2.5f;
    [SerializeField] private Transform meleeAttackPoint;

    [Header("Rocket Attack")]
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private Transform rocketSpawnPoint;
    [SerializeField] private float rocketCooldown = 3f;
    [SerializeField] private int rocketsPerSalvo = 3;
    [SerializeField] private float timeBetweenRockets = 0.3f;

    [Header("Combat")]
    [SerializeField] private int maxHealth = 500;

    private Transform player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private int currentHealth;
    private float lastMeleeTime;
    private float lastRocketTime;
    private bool isDead = false;
    private bool isAttacking = false;

    private enum BossState { Idle, Melee, Ranged }
    private BossState currentState = BossState.Idle;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Siempre mirar hacia el player
        FlipTowardsPlayer();

        // Determinar estado según distancia
        if (distanceToPlayer <= meleeRange)
        {
            currentState = BossState.Melee;
            TryMeleeAttack();
        }
        else if (distanceToPlayer <= rangedRange)
        {
            currentState = BossState.Ranged;
            TryRocketAttack();
        }
        else
        {
            currentState = BossState.Idle;
        }

        // Actualizar animaciones
        UpdateAnimations();
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        animator.SetBool("isIdle", currentState == BossState.Idle && !isAttacking);
        animator.SetBool("isWalking", false);
    }

    void FlipTowardsPlayer()
    {
        if (spriteRenderer != null && player != null)
        {
            spriteRenderer.flipX = player.position.x < transform.position.x;
        }
    }

    void TryMeleeAttack()
    {
        if (Time.time >= lastMeleeTime + meleeCooldown && !isAttacking)
        {
            StartCoroutine(PerformMeleeAttack());
        }
    }

    IEnumerator PerformMeleeAttack()
    {
        isAttacking = true;
        lastMeleeTime = Time.time;

        if (animator != null)
            animator.SetTrigger("meleeAttack");

        yield return new WaitForSeconds(0.3f);
        DealMeleeDamage();
        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }

    void DealMeleeDamage()
    {
        if (meleeAttackPoint == null)
            meleeAttackPoint = transform;

        Collider2D[] hits = Physics2D.OverlapCircleAll(meleeAttackPoint.position, meleeAttackRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(meleeDamage);
                    Debug.Log("Boss golpeo al player con " + meleeDamage + " de dano!");
                }
            }
        }
    }

    void TryRocketAttack()
    {
        if (Time.time >= lastRocketTime + rocketCooldown && !isAttacking)
        {
            StartCoroutine(FireRocketSalvo());
        }
    }

    IEnumerator FireRocketSalvo()
    {
        isAttacking = true;
        lastRocketTime = Time.time;

        for (int i = 0; i < rocketsPerSalvo; i++)
        {
            if (animator != null)
                animator.SetTrigger("shoot");

            FireRocket();
            yield return new WaitForSeconds(timeBetweenRockets);
        }

        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }

    void FireRocket()
    {
        if (rocketPrefab == null || rocketSpawnPoint == null)
        {
            Debug.LogWarning("Rocket Prefab o Rocket Spawn Point no asignados!");
            return;
        }

        GameObject rocket = Instantiate(rocketPrefab, rocketSpawnPoint.position, Quaternion.identity);

        Rocket rocketScript = rocket.GetComponent<Rocket>();
        if (rocketScript != null)
        {
            rocketScript.SetTarget(player);
        }

        Debug.Log("Boss disparo un cohete!");
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;

        if (animator != null)
            animator.SetTrigger("hurt");

        Debug.Log("Boss vida: " + currentHealth + "/" + maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // Desactivar el collider
        GetComponent<Collider2D>().enabled = false;

        Debug.Log("Boss derrotado!");

        // Destruir después de 2 segundos
        Destroy(gameObject, 2f);
    }

    // Detectar golpes del puñetazo del player
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Puñetazo"))
        {
            TakeDamage(10); // Daño del puñetazo
        }
    }

    void OnDrawGizmosSelected()
    {
        // Rango de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Rango melee
        Gizmos.color = Color.red;
        if (meleeAttackPoint != null)
            Gizmos.DrawWireSphere(meleeAttackPoint.position, meleeAttackRadius);
        else
            Gizmos.DrawWireSphere(transform.position, meleeAttackRadius);

        // Rango de cohetes
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rangedRange);

        // Rango melee interior
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
