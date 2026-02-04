using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement & Jump Configuration")]
    [SerializeField] float speed = 8f;
    [SerializeField] bool isFacingRight = true;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] bool isGrounded;
    [SerializeField] Transform groundCheck; //Posicion del detector del suelo
    [SerializeField] float groundCheckRadius = 0.1f; //Radio del detector del suelo
    [SerializeField] LayerMask groundLayer; //Define la capa que puede tocar el detector del suelo


    [Header("Shoot Configuration")]
    [SerializeField] Transform shootPosition;
    [SerializeField] GameObject projectile;

    Rigidbody2D playerRb;
    Animator anim;
    PlayerInput input;
    Vector2 moveImput;
    bool canAttack; //comprobador para determiar si se puede atacar

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();
        canAttack = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {

        //Lógica detección suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        //logica ejecucion animaciones
        AnimationManagement();

        //Ejecución de la lódica del Flip
        if (moveImput.x > 0 && !isFacingRight) Flip();
        if (moveImput.x < 0 && isFacingRight) Flip();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        playerRb.linearVelocity = new Vector2(moveImput.x * speed, playerRb.linearVelocity.y);
    }

    void Flip()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }

    void Jump()
    {
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        AudioManager.Instance.PlaySFX(3);
    }

    IEnumerator Attack()
    {
        anim.SetTrigger("Attacking");
        canAttack = false;
        float actualSpeed = speed;
        speed = 0;
        yield return new WaitForSeconds(0.8f);
        speed = actualSpeed;
        canAttack = true;
        yield return null;
    }


    void AnimationManagement()
    {
        anim.SetBool("Jumping", !isGrounded);
        if (moveImput.x != 0) anim.SetBool("Running", true);
        else anim.SetBool("Running", false);
    }

    void ShootMagic()
    {
        //llamar a un instantiate de prefab de proyectil
        GameObject actualProjectile = Instantiate(projectile, shootPosition.position, Quaternion.identity);
        Bullet bulletScript = actualProjectile.GetComponent<Bullet>();
        bulletScript.isfacingRight = isFacingRight;
    }

    #region Input Methods
    public void OnMovement(InputAction.CallbackContext context)
    {
        moveImput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded) Jump();
    }
    public void OnAttack(InputAction.CallbackContext context)
    {

        if (context.performed && isGrounded && canAttack) StartCoroutine(Attack());
    }
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed) ShootMagic();
    }
    #endregion

}