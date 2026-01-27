using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor2D : MonoBehaviour
{
    public class PlayerController2D : MonoBehaviour
    {
        #region General Variables
        [Header("Movement & jump Configuration)")]
        [SerializeField] float speed = 8f;
        [SerializeField] bool isFacingRight = true;
        [SerializeField] float jumpForce = 5f;
        [SerializeField] bool isGrounded;
        [SerializeField] Transform groundCheck; //Posicion del detector del suelo
        [SerializeField] float groundChekRadius = 0.1f; //Radio del detector del suelo
        [SerializeField] LayerMask groundLayer; //Define la capa que puede tocar el detectoe del suelo 
    
        //Variables de referencia privadas
        Rigidbody2D playerRb; //Referencia del rigidbody del player
        Animator anim; //Referencia al controlador de animciones del player
        Vector2 moveInput; //Referencia al valor pulsado de las teclas de movimineto 
        bool canAttack; //Comprobador para determinar si se pude atacar 
        PlayerInput input; //Referencia al valor de inputs del player

        #endregion

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
            //Logica de la deteccion del suelo
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundChekRadius, groundLayer);
            // Ejecucion de la logica del Flip
            //Logica de ejecucion de animaciones
            AnimationManagement();
            if (moveInput.x > 0 && !isFacingRight) Flip();
            if (moveInput.x < 0 && isFacingRight) Flip();

        }

        private void FixedUpdate()
        {
            Movement();
        }

        void Movement()
        {
            playerRb.linearVelocity = new Vector2(moveInput.x * speed, playerRb.linearVelocity.y);
        }

        void Flip()
        {
            Vector3 currentScale = transform.localScale; //Almacenamos el valor scale actual
            currentScale.x *= -1; //Cambiamos el valor de scale x al contrario actual
            transform.localScale = currentScale; //A la scale acual le pasamos la nueva modificada
            isFacingRight = !isFacingRight;  //Cambiar el bool al contrario
        }

        void Jump()
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            
        }              

        void AnimationManagement()
        {
            //Gestion del cambio de animaciones
            anim.SetBool("Jumping", !isGrounded);
            if (moveInput.x != 0) anim.SetBool("Running", true);
            else anim.SetBool("Running", false);
        }
        #region Input Methods
        public void OnMovement(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && isGrounded) Jump();
        }
    
       }

        #endregion
    }


