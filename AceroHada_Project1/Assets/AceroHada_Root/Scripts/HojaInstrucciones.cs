using UnityEngine;

public class HojaInstrucciones : MonoBehaviour
{
    public GameObject canvasTexto;  // Arrastra aquí el Canvas desde el inspector
    private bool jugadorCerca = false;

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            canvasTexto.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            canvasTexto.SetActive(false);
        }
    }
}
