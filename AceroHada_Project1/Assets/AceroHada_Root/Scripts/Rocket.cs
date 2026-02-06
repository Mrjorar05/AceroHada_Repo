using UnityEngine;

public class Rocket : MonoBehaviour
{
    [Header("Rocket Settings")]
    public float speed = 8f;
    public int damage = 3;
    public float lifeTime = 5f;

    private Vector2 direction; // Dirección fija del cohete

    void Start()
    {
        // Destruir el cohete si no impacta en un tiempo
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Mover el cohete en línea recta
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    // El Boss llama a esto para indicar hacia dónde disparar
    public void SetDirection(bool facingLeft)
    {
        direction = facingLeft ? Vector2.left : Vector2.right;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInteractor player = collision.GetComponent<PlayerInteractor>();

            if (player != null)
            {
                // Usamos la posición del cohete para el knockback
                player.GetDamage(transform.position, damage);
            }

            Destroy(gameObject);
        }

        // Si choca con el suelo o paredes
        if (collision.CompareTag("Ground") || collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
