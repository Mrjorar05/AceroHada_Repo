using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float crouchSpeedMultiplier = 0.5f;
    private Animator animator;
    public float fuerzaRebote = 6;
    public int vida = 100;
    private bool isFacingRight = true;
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isCrouching;
    private bool canAttack = true;
    private bool Damage;
   public bool Dead;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
       
        AnimationHandler();
       
        if (movement.x > 0 && !isFacingRight)
            Flip();
        else if (movement.x < 0 && isFacingRight)
            Flip();
       
        
    }


    void FixedUpdate()
    {
        float speed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;
        rb.linearVelocity = new Vector2(movement.x * speed, rb.linearVelocity.y);
    }

    void AnimationHandler()
    {
        animator.SetBool("IsWalking", movement.x != 0);
        animator.SetBool("IsCrounching", isCrouching);
       
    }



   public void GetDamage(Vector2 direccion,int cantGetDamage)
    {
        if (!Damage)
        {
            Damage = true;
            vida -= cantGetDamage;
            animator.SetTrigger("Damage");
            if (vida < 0)
            {
                if (GameManager.instance != null)
                {
                    GameManager.instance.GameOver();
                }
                Dead = true;
                animator.SetTrigger("Dead");
            }
            Vector2 rebote = new Vector2(transform.position.x - direccion.x, 1).normalized;
            rb.AddForce(rebote* fuerzaRebote, ForceMode2D.Impulse);
           
        }
    }
    public void GetDamage(int cantGetDamage)
    {
        // Si el cohete no empuja, solo restamos vida
        vida -= cantGetDamage;
        animator.SetTrigger("Damage");

        if (vida <= 0)
        {
            if (GameManager.instance != null)
                GameManager.instance.GameOver();

            Dead = true;
            animator.SetTrigger("Dead");
        }
    }


    public void DesactiveDamage()
    {
        Damage = false;
    }
    public void OnMovement(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
        AudioManager.Instance.PlaySFX(2);

    }

    
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isCrouching = true;
        }
        else if (context.canceled)
        {
            isCrouching = false;
        }
        AudioManager.Instance.PlaySFX(5);
    }

    
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed || !canAttack) return;

        canAttack = false;
        animator.SetTrigger("IsAttacking");
        Debug.Log("Attack");

       
        Invoke(nameof(ResetAttack), 0.5f);
        AudioManager.Instance.PlaySFX(3);
    }

    void ResetAttack()
    {
        canAttack = true;
    }

   
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Debug.Log("Interact");

        
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
