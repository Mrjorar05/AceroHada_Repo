using UnityEngine;
using TMPro;

public class PlayerPickup : MonoBehaviour
{
    public int totalPickups = 3;
    private int currentPickups = 0;

    public TextMeshProUGUI textoFusibles; // Referencia al texto

    void Start()
    {
        ActualizarTexto();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pickup"))
        {
            
            currentPickups++;
            Destroy(collision.gameObject);

            ActualizarTexto();

            if (currentPickups >= totalPickups)
            {
                Debug.Log("¡Todos los fusibles recogidos!");
            }
            AudioManager.Instance.PlaySFX(1);
        }
    }

    void ActualizarTexto()
    {
        textoFusibles.text = "Fusibles: " + currentPickups + "/" + totalPickups;
    }
}
