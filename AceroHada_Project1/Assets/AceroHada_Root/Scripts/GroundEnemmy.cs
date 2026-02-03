using UnityEngine;

public class GroundEnemmy : MonoBehaviour
{
    [Header("Estadísticas")]
    [SerializeField] private float maxHealth = 60f;
    private float currentHealth;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float stopDistance = 0.5f; // Distancia mínima al jugador antes de detenerse

    [Header("Ataque")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float lastAttackTime;

    [Header("Referencias")]
    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool isDead = false;
    private bool facingRight = true;

    void Start()
    {
        // Inicializar vida
        currentHealth = maxHealth;

        // Obtener componentes
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Determinar dirección inicial basada en la escala del sprite
        facingRight = transform.localScale.x > 0;

        // Configurar Rigidbody2D si existe
        if (rb != null)
        {
            rb.gravityScale = 1; // Gravedad normal para 2D
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Evitar rotación
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Mejor detección de colisiones
        }

        // Encontrar al jugador por tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con tag 'Player'");
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Si el jugador está en rango de detección
        if (distanceToPlayer <= detectionRange)
        {
            // Si está en rango de ataque
            if (distanceToPlayer <= attackRange)
            {
                AttackPlayer();
            }
            else
            {
                // Perseguir al jugador
                ChasePlayer();
            }
        }
        else
        {
            // Idle - no hacer nada
            SetAnimationState("Idle");
        }
    }

    void ChasePlayer()
    {
        // Calcular dirección hacia el jugador (solo en X para 2D)
        float direction = player.position.x - transform.position.x;
        float distance = Mathf.Abs(direction);

        // Si está muy cerca, detenerse
        if (distance <= stopDistance)
        {
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            SetAnimationState("Idle");
            return;
        }

        // Voltear el sprite según la dirección
        if (direction > 0 && !facingRight)
        {
            Flip();
        }
        else if (direction < 0 && facingRight)
        {
            Flip();
        }

        // Moverse hacia el jugador
        if (rb != null)
        {
            Vector2 movement = new Vector2(Mathf.Sign(direction) * moveSpeed, rb.linearVelocity.y);
            rb.linearVelocity = movement;
        }
        else
        {
            Vector2 movement = new Vector2(Mathf.Sign(direction) * moveSpeed * Time.deltaTime, 0);
            transform.position += (Vector3)movement;
        }

        // Activar animación de caminar
        SetAnimationState("Walk");
    }

    void AttackPlayer()
    {
        // Voltear hacia el jugador
        float direction = player.position.x - transform.position.x;
        if (direction > 0 && !facingRight)
        {
            Flip();
        }
        else if (direction < 0 && facingRight)
        {
            Flip();
        }

        // Detener movimiento
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        // Verificar si puede atacar (cooldown)
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;

            // Activar animación de ataque
            SetAnimationState("Attack");

            // Aplicar daño al jugador
            // Envía un mensaje al jugador para que reciba daño
            player.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            // Esperar en idle mientras está en cooldown
            SetAnimationState("Idle");
        }
    }

    public void TakeDamage(float damage)
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

        // Detener movimiento
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
        }

        // Activar animación de muerte
        SetAnimationState("Death");

        // Desactivar colisión
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Desactivar el enemigo después de la animación
        Destroy(gameObject, 2f);
    }

    void Flip()
    {
        facingRight = !facingRight;

        // Voltear el sprite usando la escala
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void SetAnimationState(string state)
    {
        if (animator == null)
        {
            Debug.LogWarning("No hay Animator en el enemigo");
            return;
        }

        // Debug para ver qué estado se está activando
        // Debug.Log($"Cambiando a estado: {state}");

        // Resetear todos los triggers/bools (solo si existen)
        if (HasParameter("isWalking"))
            animator.SetBool("isWalking", false);
        if (HasParameter("isAttacking"))
            animator.SetBool("isAttacking", false);
        if (HasParameter("isDead"))
            animator.SetBool("isDead", false);

        // Activar el estado correspondiente
        switch (state)
        {
            case "Walk":
                if (HasParameter("isWalking"))
                {
                    animator.SetBool("isWalking", true);
                }
                else
                {
                    Debug.LogWarning("El parámetro 'isWalking' no existe en el Animator");
                }
                break;
            case "Attack":
                if (HasParameter("isAttacking"))
                {
                    animator.SetBool("isAttacking", true);
                }
                else
                {
                    Debug.LogWarning("El parámetro 'isAttacking' no existe en el Animator");
                }
                break;
            case "Death":
                if (HasParameter("isDead"))
                {
                    animator.SetBool("isDead", true);
                }
                else
                {
                    Debug.LogWarning("El parámetro 'isDead' no existe en el Animator");
                }
                break;
            case "Idle":
                // Ya están todos en false
                break;
        }
    }

    // Verificar si un parámetro existe en el Animator
    bool HasParameter(string paramName)
    {
        if (animator == null) return false;

        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    // Visualizar rangos en el editor
    void OnDrawGizmosSelected()
    {
        // Rango de detección (amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Rango de ataque (rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
