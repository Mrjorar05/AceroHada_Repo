using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidadCaminar = 5f;
    [SerializeField] private float fuerzaSalto = 10f;

    [Header("Vuelo")]
    [SerializeField] private float velocidadVuelo = 6f;
    [SerializeField] private bool puedeVolar = false;

    [Header("Ataque")]
    [SerializeField] private float rangoAtaque = 1f;
    [SerializeField] private Transform puntoAtaque;
    [SerializeField] private LayerMask capasEnemigos;

    [Header("Detección de Suelo")]
    [SerializeField] private Transform detectarSuelo;
    [SerializeField] private float radioDeteccion = 0.2f;
    [SerializeField] private LayerMask capaSuelo;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool enSuelo;
    private bool volando = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Detectar si está en el suelo
        enSuelo = Physics2D.OverlapCircle(detectarSuelo.position, radioDeteccion, capaSuelo);

        // Movimiento horizontal
        float movimientoH = 0f;
        if (Input.GetKey(KeyCode.A))
            movimientoH = -1f;
        else if (Input.GetKey(KeyCode.D))
            movimientoH = 1f;

        // Voltear sprite según dirección
        if (movimientoH < 0)
            spriteRenderer.flipX = true;
        else if (movimientoH > 0)
            spriteRenderer.flipX = false;

        // Aplicar movimiento
        if (volando)
        {
            rb.linearVelocity = new Vector2(movimientoH * velocidadVuelo, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(movimientoH * velocidadCaminar, rb.linearVelocity.y);
        }

        // Saltar
        if (Input.GetKeyDown(KeyCode.Space) && enSuelo && !volando)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }

        // Activar/Desactivar vuelo
        if (Input.GetKeyDown(KeyCode.V) && puedeVolar)
        {
            volando = !volando;

            if (volando)
            {
                rb.gravityScale = 0f;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            }
            else
            {
                rb.gravityScale = 1f;
            }
        }

        // Control de vuelo vertical
        if (volando)
        {
            float movimientoV = 0f;
            if (Input.GetKey(KeyCode.W))
                movimientoV = 1f;
            else if (Input.GetKey(KeyCode.S))
                movimientoV = -1f;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, movimientoV * velocidadVuelo);
        }

        // Atacar
        if (Input.GetMouseButtonDown(0))
        {
            Atacar();
        }

        // Animaciones (si tienes Animator configurado)
        if (animator != null)
        {
            animator.SetBool("Jump", !enSuelo);
            if (movimientoH != 0) animator.SetBool("Walk", true);
            else animator.SetBool("Walk", false);
        }
    }

    void Atacar()
    {
        // Ejecutar animación de ataque
        if (animator != null)
            animator.SetTrigger("Attack");

        // Detectar enemigos en el rango
        Collider2D[] enemigosGolpeados = Physics2D.OverlapCircleAll(puntoAtaque.position, rangoAtaque, capasEnemigos);

        foreach (Collider2D enemigo in enemigosGolpeados)
        {
            Debug.Log("Golpeaste a: " + enemigo.name);
            // Aquí puedes agregar daño al enemigo
            // enemigo.GetComponent<Enemigo>().RecibirDaño(dañoAtaque);
        }
    }

    // Visualizar el rango de ataque y detección de suelo en el editor
    private void OnDrawGizmosSelected()
    {
        if (puntoAtaque != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(puntoAtaque.position, rangoAtaque);
        }

        if (detectarSuelo != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(detectarSuelo.position, radioDeteccion);
        }
    }
}