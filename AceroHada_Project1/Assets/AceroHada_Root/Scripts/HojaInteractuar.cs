using UnityEngine;

public class HojaInteractuar : MonoBehaviour
{
    public GameObject canvasInstrucciones;

    private bool jugadorCerca = false;

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            canvasInstrucciones.SetActive(true);
            Time.timeScale = 0f; // Pausa el juego
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }

    public void CerrarCanvas()
    {
        canvasInstrucciones.SetActive(false);
        Time.timeScale = 1f; // Reanuda el juego
    }
}

