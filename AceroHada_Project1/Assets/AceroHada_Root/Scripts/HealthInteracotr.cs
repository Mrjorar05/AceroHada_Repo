using UnityEngine;

public class HealtInteracotr : MonoBehaviour
{

    [SerializeField] bool isPositive;
    [SerializeField] float quantity;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.playerHealth += quantity;
            AudioManager.Instance.PlaySFX(1);
            gameObject.SetActive(false);
        }
        else
        {
            GameManager.Instance.playerHealth -= quantity;
            AudioManager.Instance.PlaySFX(2);
            gameObject.SetActive(false);
        }
    }
}
