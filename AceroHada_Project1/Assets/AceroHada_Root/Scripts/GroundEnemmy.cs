using UnityEngine;

public class GroundEnemmy : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float chaseSpeed = 3.5f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackDamage = 10;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolDistance = 5f;
    [SerializeField] private float waitTimeAtPoint = 2f;

    private Animator animator;
    private Rigidbody2D rb;
    private Transform player;
    private SpriteRenderer spriteRenderer;

    private Vector2 startPosition;
    private Vector2 patrolTarget;
    private float waitTimer;
    private float attackTimer;
    private bool isChasing = false;
    private bool facingRight = true;

    private enum State { Idle, Patrol, Chase, Attack }
    private State currentState = State.Patrol;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        startPosition = transform.position;
        SetNewPatrolTarget();

        // Buscar al jugador en la escena
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        // Detectar al jugador
        float distanceToPlayer = player != null ? Vector2.Distance(transform.position, player.position) : float.MaxValue;

        // Máquina de estados
        switch (currentState)
        {
            case State.Idle:
                HandleIdleState();
                break;
            case State.Patrol:
                HandlePatrolState(distanceToPlayer);
                break;
            case State.Chase:
                HandleChaseState(distanceToPlayer);
                break;
            case State.Attack:
                HandleAttackState(distanceToPlayer);
                break;
        }

        UpdateAnimations();
    }

    void HandleIdleState()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0)
        {
            currentState = State.Patrol;
            SetNewPatrolTarget();
        }
    }

    void HandlePatrolState(float distanceToPlayer)
    {
        // Si detecta al jugador, cambiar a persecución
        if (distanceToPlayer <= detectionRange)
        {
            currentState = State.Chase;
            isChasing = true;
            return;
        }

        // Patrullar
        Vector2 direction = (patrolTarget - (Vector2)transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        FlipSprite(direction.x);

        // Si llegó al punto de patrulla
        if (Vector2.Distance(transform.position, patrolTarget) < 0.5f)
        {
            currentState = State.Idle;
            waitTimer = waitTimeAtPoint;
        }
    }

    void HandleChaseState(float distanceToPlayer)
    {
        // Si el jugador está en rango de ataque
        if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attack;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // Si el jugador se alejó mucho, volver a patrullar
        if (distanceToPlayer > detectionRange * 1.5f)
        {
            currentState = State.Patrol;
            isChasing = false;
            SetNewPatrolTarget();
            return;
        }

        // Perseguir al jugador
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);

        FlipSprite(direction.x);
    }

    void HandleAttackState(float distanceToPlayer)
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        // Mirar hacia el jugador
        Vector2 direction = (player.position - transform.position).normalized;
        FlipSprite(direction.x);

        // Atacar si el cooldown terminó
        if (attackTimer <= 0)
        {
            Attack();
            attackTimer = attackCooldown;
        }

        // Si el jugador se alejó, volver a perseguir
        if (distanceToPlayer > attackRange * 1.2f)
        {
            currentState = State.Chase;
        }
    }

    void Attack()
    {
        // Aquí puedes agregar la lógica de daño al jugador
        // Por ejemplo, usando un Collider2D o Raycast
        Debug.Log("Enemigo ataca!");

        // Ejemplo: detectar jugador con overlap circle
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        if (hitPlayer != null)
        {
            // Aquí aplicarías daño al jugador
            // hitPlayer.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
        }
    }

    void SetNewPatrolTarget()
    {
        float randomX = Random.Range(-patrolDistance, patrolDistance);
        patrolTarget = startPosition + new Vector2(randomX, 0);
    }

    void FlipSprite(float direction)
    {
        if (direction > 0 && !facingRight)
        {
            facingRight = true;
            spriteRenderer.flipX = false;
        }
        else if (direction < 0 && facingRight)
        {
            facingRight = false;
            spriteRenderer.flipX = true;
        }
    }

    void UpdateAnimations()
    {
        // Actualizar parámetros del Animator
        animator.SetBool("IsWalking", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
        animator.SetBool("IsAttacking", currentState == State.Attack);
        animator.SetBool("Idle", currentState == State.Idle);
    }

    void OnDrawGizmosSelected()
    {
        // Visualizar rangos en el editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
