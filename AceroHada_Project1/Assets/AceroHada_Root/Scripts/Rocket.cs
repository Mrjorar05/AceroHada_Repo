using UnityEngine;

public class Rocket : MonoBehaviour
{
    [Header("Rocket Settings")]
    public float speed = 8f;
    public int damage = 20;
    public float lifeTime = 5f;

    private Transform target;
    private Vector2 direction;

    void Start()
    {
        // Destruir el cohete si no impacta en un tiempo
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (target == null)
        {
            // Si el player desaparece, el cohete sigue recto
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            return;
        }

        // Calcular dirección hacia el objetivo
        direction = (target.position - transform.position).normalized;

        // Mover el cohete
        transform.Translate(direction * speed * Time.deltaTime);

        // Rotar el cohete hacia el objetivo (opcional)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInteractor playerHealth = collision.GetComponent<PlayerInteractor>();

            if (playerHealth != null)
            {
                playerHealth.GetDamage(damage);
            }

            Destroy(gameObject);
        }

        // Si choca con el suelo, paredes, etc.
        if (collision.CompareTag("Ground") || collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
