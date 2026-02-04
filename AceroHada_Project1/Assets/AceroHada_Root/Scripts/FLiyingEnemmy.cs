using UnityEngine;

public class FLiyingEnemmy : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float hoverHeight = 3f;
    [SerializeField] private float hoverSpeed = 1f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int attackDamage = 15;
    [SerializeField] private float diveSpeed = 6f;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 6f;
    [SerializeField] private float waitTimeAtPoint = 1.5f;

    [Header("Hover Settings")]
    [SerializeField] private float hoverAmplitude = 0.5f;
    [SerializeField] private float hoverFrequency = 1f;

    private Animator animator;
    private Rigidbody2D rb;
    private Transform player;
    private SpriteRenderer spriteRenderer;

    private Vector2 startPosition;
    private Vector2 patrolTarget;
    private float waitTimer;
    private float attackTimer;
    private float hoverTimer;
    private bool isChasing = false;
    private bool facingRight = true;

    private enum State { Idle, Patrol, Chase, Attack, Dive }
    private State currentState = State.Patrol;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Los enemigos voladores no usan gravedad
        rb.gravityScale = 0;

        startPosition = transform.position;
        SetNewPatrolTarget();

        // Buscar al jugador
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
        hoverTimer += Time.deltaTime;

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
            case State.Dive:
                HandleDiveState(distanceToPlayer);
                break;
        }

        UpdateAnimations();
    }

    void HandleIdleState()
    {
        // Flotar en el lugar con movimiento sutil
        Vector2 hoverOffset = new Vector2(0, Mathf.Sin(hoverTimer * hoverFrequency) * hoverAmplitude);
        rb.linearVelocity = hoverOffset;

        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0)
        {
            currentState = State.Patrol;
            SetNewPatrolTarget();
        }
    }

    void HandlePatrolState(float distanceToPlayer)
    {
        // Detectar jugador
        if (distanceToPlayer <= detectionRange)
        {
            currentState = State.Chase;
            isChasing = true;
            return;
        }

        // Patrullar en 2D con efecto de flotación
        Vector2 targetPos = patrolTarget;
        targetPos.y += Mathf.Sin(hoverTimer * hoverFrequency) * hoverAmplitude;

        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        FlipSprite(direction.x);

        // Si llegó al punto de patrulla
        if (Vector2.Distance(transform.position, patrolTarget) < 0.8f)
        {
            currentState = State.Idle;
            waitTimer = waitTimeAtPoint;
        }
    }

    void HandleChaseState(float distanceToPlayer)
    {
        // Si está en rango de ataque
        if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attack;
            return;
        }

        // Si el jugador se alejó mucho
        if (distanceToPlayer > detectionRange * 1.8f)
        {
            currentState = State.Patrol;
            isChasing = false;
            SetNewPatrolTarget();
            return;
        }

        // Perseguir al jugador manteniendo cierta altura
        Vector2 targetPosition = player.position;
        targetPosition.y += hoverHeight;

        // Añadir efecto de flotación durante la persecución
        targetPosition.y += Mathf.Sin(hoverTimer * hoverFrequency * 2) * (hoverAmplitude * 0.5f);

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;

        FlipSprite(direction.x);
    }

    void HandleAttackState(float distanceToPlayer)
    {
        // Flotar sobre el jugador
        Vector2 hoverOffset = new Vector2(0, Mathf.Sin(hoverTimer * hoverFrequency * 1.5f) * hoverAmplitude);
        rb.linearVelocity = hoverOffset;

        // Mirar hacia el jugador
        Vector2 direction = (player.position - transform.position).normalized;
        FlipSprite(direction.x);

        // Atacar (puede ser un ataque en picada)
        if (attackTimer <= 0)
        {
            currentState = State.Dive;
            attackTimer = attackCooldown;
        }

        // Si el jugador se alejó
        if (distanceToPlayer > attackRange * 1.5f)
        {
            currentState = State.Chase;
        }
    }

    void HandleDiveState(float distanceToPlayer)
    {
        // Ataque en picada hacia el jugador
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * diveSpeed;

        FlipSprite(direction.x);

        // Si golpeó o pasó al jugador, volver a perseguir
        if (transform.position.y <= player.position.y - 0.5f || distanceToPlayer < 0.5f)
        {
            Attack();
            currentState = State.Chase;
        }
    }

    void Attack()
    {
        Debug.Log("Enemigo volador ataca!");

        // Detectar jugador en área de ataque
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        if (hitPlayer != null)
        {
            // Aplicar daño
            // hitPlayer.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
        }
    }

    void SetNewPatrolTarget()
    {
        // Generar punto de patrulla aleatorio en círculo alrededor del punto inicial
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(patrolRadius * 0.5f, patrolRadius);

        patrolTarget = startPosition + randomDirection * randomDistance;
        patrolTarget.y += Random.Range(-1f, 2f); // Variación en altura
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
        bool isMoving = rb.linearVelocity.magnitude > 0.1f;
        animator.SetBool("IsWalking", isMoving && currentState != State.Dive);
        animator.SetBool("IsAttacking", currentState == State.Dive || currentState == State.Attack);
        animator.SetBool("Idle", currentState == State.Idle);
    }

    void OnDrawGizmosSelected()
    {
        // Visualizar rangos
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Visualizar área de patrulla
        Gizmos.color = Color.cyan;
        Vector3 startPos = Application.isPlaying ? (Vector3)startPosition : transform.position;
        Gizmos.DrawWireSphere(startPos, patrolRadius);
    }
}
