using UnityEngine;
using UnityEngine.UI;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("UI Vida")]
    public Image healthBarFill;

    [Header("Respawn")]
    public Transform respawnPoint;
    public float voidYLevel = -10f;

    private Rigidbody2D rb;
    private bool isRespawning = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        if (transform.position.y <= voidYLevel && !isRespawning)
        {
            StartCoroutine(HandleVoidFall());
        }
    }

    System.Collections.IEnumerator HandleVoidFall()
    {
        isRespawning = true;

        TakeDamage(1);

        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(Respawn());
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        healthBarFill.fillAmount = (float)currentHealth / maxHealth;
    }

    System.Collections.IEnumerator Respawn()
    {
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        yield return new WaitForSeconds(0.2f);

        transform.position = respawnPoint.position;

        //  RESETEAMOS VIDA AL RESPawnear
        currentHealth = maxHealth;
        UpdateHealthUI();

        rb.simulated = true;

        yield return new WaitForSeconds(0.2f);

        isRespawning = false;
    }
}

