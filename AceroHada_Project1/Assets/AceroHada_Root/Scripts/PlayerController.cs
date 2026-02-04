ï»¿using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float crouchSpeedMultiplier = 0.5f;
    private Animator animator;

    private bool isFacingRight = true;
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isCrouching;
    private bool canAttack = true;
    private bool Damage;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //GestiÃ³n de las animaciones
        AnimationHandler();
        //GestiÃ³n del flip
        if (movement.x > 0 && !isFacingRight)
            Flip();
        else if (movement.x < 0 && isFacingRight)
            Flip();
        animator.SetBool("Damage", Damage);
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
            Vector2 rebote = new Vector2(transform.position.x - direccion.x, 1).normalized;
            rb.AddForce(rebote, ForceMode2D.Impulse);
        }
    }

    public void DesactiveDamage()
    {
        Damage = false;
    }
    public void OnMovement(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
        
      
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
    }

    
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed || !canAttack) return;

        canAttack = false;
        animator.SetTrigger("IsAttacking");
        Debug.Log("Attack");

       
        Invoke(nameof(ResetAttack), 0.5f);
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
