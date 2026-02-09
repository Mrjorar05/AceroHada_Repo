using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Vuelo")]
    public float flySpeed = 5f;
    public float maxFlyEnergy = 5f;
    public float energyDrain = 1f;
    public float energyRecharge = 2f;

    [Header("Saltos")]
    public int maxJumps = 2;

    private Rigidbody2D rb;
    private int jumpCount;
    private bool isFlying;
    private float currentEnergy;

    // Input System
    private PlayerInput playerInput;
    private Vector2 moveInput;
    private Vector2 flyInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        currentEnergy = maxFlyEnergy;
    }

    void Update()
    {
        // Movimiento horizontal
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // Vuelo
        if (isFlying)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, flyInput.y * flySpeed);

            currentEnergy -= energyDrain * Time.deltaTime;
            if (currentEnergy <= 0)
            {
                StopFlying();
            }
        }
        else
        {
            currentEnergy += energyRecharge * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxFlyEnergy);
        }
    }

    // ===== INPUT CALLBACKS =====

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && jumpCount < maxJumps && !isFlying)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
        }
    }

    public void OnFly(InputAction.CallbackContext context)
    {
        if (context.performed && currentEnergy > 0)
        {
            isFlying = !isFlying;

            if (isFlying)
            {
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.zero;
            }
            else
            {
                rb.gravityScale = 3f;
            }
        }
    }

    public void OnFlyMove(InputAction.CallbackContext context)
    {
        flyInput = context.ReadValue<Vector2>();
    }

    void StopFlying()
    {
        isFlying = false;
        rb.gravityScale = 3f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
        }
    }
}

