using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Estadísticas")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Jugador recibió {damage} de daño. Vida restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("El jugador ha muerto");
        // Aquí puedes añadir lógica de muerte del jugador
        // Por ejemplo: reiniciar nivel, mostrar pantalla de Game Over, etc.
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
}
