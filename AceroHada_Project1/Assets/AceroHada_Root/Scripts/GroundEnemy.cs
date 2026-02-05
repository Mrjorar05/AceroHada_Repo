using System.Collections;
using UnityEditor.Build;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class GroundEnemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 60;
    [SerializeField] private int attackDamage = 25;
    public int vida = 5;
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Vector2 moveDirection = Vector2.left;
    private bool OnMovement;
    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;

    [Header("References")]
    [SerializeField] private Transform player;
    private bool gettingDamage;
    private int currentHealth;
    private Rigidbody2D rb;
    private Animator animator;
    private bool Dead;
    private bool isDead = false;
    private bool isAttacking = false;
    public float fuerzaRebote = 6f;
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
        animator.SetBool("Isdead", isDead);
    }
   
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direccionDamage = new Vector2(transform.position.x, 0);
           collision.gameObject.GetComponent<PlayerInteractor>().GetDamage(direccionDamage,1);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Puñetazo"))
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x, 0);

            RecibeDanio(direccionDanio, 1);
        }
    }
    
    public void RecibeDanio(Vector2 direccion, int cantDamage)
    {
        if (!gettingDamage)
        {
            vida -= cantDamage;
            gettingDamage = true;
            if (vida <= 0)
            {
                isDead = true;
               OnMovement= false;
                animator.SetTrigger("IsDead");
                rb.linearVelocity = Vector2.zero;
                Destroy(gameObject, 2f);
            }
            else
            {
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.2f).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
                StartCoroutine(DesactivaDanio());
            }
        }
    }
    IEnumerator DesactivaDanio()
    {
        yield return new WaitForSeconds(0.4f);
        gettingDamage = false;
        rb.linearVelocity = Vector2.zero;
    }
    void Move()
    {
        isAttacking = false;

        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);

        animator.SetBool("IsWalking", true);
        animator.SetBool("IsAttacking", false);
    }

    void StartAttack()
    {
        Debug.Log("Ataque entra");
        rb.linearVelocity = Vector2.zero;

        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetBool("IsAttacking", true);
        }

        animator.SetBool("IsWalking", false);
    }

    // LLAMAR DESDE UN ANIMATION EVENT    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
