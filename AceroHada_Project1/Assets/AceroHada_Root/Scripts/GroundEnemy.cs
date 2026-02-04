using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class GroundEnemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 60;
    [SerializeField] private int attackDamage = 25;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Vector2 moveDirection = Vector2.left;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;

    [Header("References")]
    [SerializeField] private Transform player;

    private int currentHealth;
    private Rigidbody2D rb;
    private Animator animator;

    private bool isDead = false;
    private bool isAttacking = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }
    }

    void FixedUpdate()
    {
        if (isDead || player == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            StartAttack();
        }
        else
        {
            Move();
        }
    }

    void Move()
    {
        isAttacking = false;

        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);

        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);
    }

    void StartAttack()
    {
        rb.linearVelocity = Vector2.zero;

        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetBool("isAttacking", true);
        }

        animator.SetBool("isWalking", false);
    }

    // LLAMAR DESDE UN ANIMATION EVENT
    public void DealDamage()
    {
        if (player == null || isDead) return;

        // Intentamos obtener un script de vida del player
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    // CUANDO EL PLAYER ATACA AL ENEMIGO
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;

        animator.SetBool("isDead", true);
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);

        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
