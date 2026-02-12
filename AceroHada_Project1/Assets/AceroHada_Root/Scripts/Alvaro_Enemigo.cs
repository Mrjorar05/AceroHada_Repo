using UnityEngine;

public class Alvaro_Enemigo : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player; 
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;

    [Header("Detección")]
    [SerializeField] private float detectionRange = 10f; 
    [SerializeField] private float attackRange = 1.5f; 
    [SerializeField] private LayerMask playerLayer; 

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 3f; 
    [SerializeField] private bool flipTowardsPlayer = true; 

    [Header("Combate")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.5f; 
    [SerializeField] private int maxHealth = 100;

    [Header("Sonidos")]
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float walkSoundInterval = 0.5f; 

    
    private enum EnemyState
    {
        Idle,
        Chasing,
        Attacking,
        Dead
    }

    private EnemyState currentState = EnemyState.Idle;
    private int currentHealth;
    private float attackTimer = 0f;
    private float walkSoundTimer = 0f;
    private bool isDead = false;
    private bool facingRight = true;

    
    private readonly string ANIM_RUN = "isRunning";
    private readonly string ANIM_ATTACK = "Attack";
    private readonly string ANIM_DIE = "Die";

    private void Start()
    {
        
        if (animator == null)
            animator = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

       
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        currentHealth = maxHealth;
        audioSource.loop = false;
    }

    private void Update()
    {
        if (isDead || player == null) return;

        attackTimer += Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        
        switch (currentState)
        {
            case EnemyState.Idle:
                CheckForPlayer(distanceToPlayer);
                break;

            case EnemyState.Chasing:
                ChasePlayer(distanceToPlayer);
                break;

            case EnemyState.Attacking:
                AttackPlayer(distanceToPlayer);
                break;
        }
    }

    private void CheckForPlayer(float distance)
    {
        animator.SetBool(ANIM_RUN, false);

        if (distance <= detectionRange)
        {
            currentState = EnemyState.Chasing;
        }
    }

    private void ChasePlayer(float distance)
    {
        animator.SetBool(ANIM_RUN, true);

        
        PlayWalkSound();

        if (distance <= attackRange)
        {
            
            currentState = EnemyState.Attacking;
            animator.SetBool(ANIM_RUN, false);
        }
        else if (distance > detectionRange)
        {
           
            currentState = EnemyState.Idle;
        }
        else
        {
            
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );

            
            if (flipTowardsPlayer)
            {
                FlipTowards(direction.x);
            }
        }
    }

    private void AttackPlayer(float distance)
    {
        
        if (distance > attackRange)
        {
            currentState = EnemyState.Chasing;
            return;
        }

        
        if (attackTimer >= attackCooldown)
        {
            Attack();
            attackTimer = 0f;
        }

        
        if (flipTowardsPlayer)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            FlipTowards(direction.x);
        }
    }

    private void Attack()
    {
        animator.SetTrigger(ANIM_ATTACK);

        
        if (attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        
    }

    
    public void DealDamageToPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
           
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    private void PlayWalkSound()
    {
        if (walkSound == null) return;

        walkSoundTimer += Time.deltaTime;

        if (walkSoundTimer >= walkSoundInterval)
        {
            audioSource.PlayOneShot(walkSound);
            walkSoundTimer = 0f;
        }
    }

    private void FlipTowards(float directionX)
    {
        if (directionX > 0 && !facingRight)
        {
            Flip();
        }
        else if (directionX < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

   
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        currentState = EnemyState.Dead;

        
        animator.SetTrigger(ANIM_DIE);
        animator.SetBool(ANIM_RUN, false);

        
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        
        Destroy(gameObject, 2f);
    }

    
    private void OnDrawGizmosSelected()
    {
       
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

       
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
