using UnityEngine;

public class HadaPickups : MonoBehaviour
{
    public int totalPickups = 3;   // Cantidad necesaria
    private int currentPickups = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pickup"))
        {
            currentPickups++;
            Destroy(collision.gameObject);

            Debug.Log("Pickups recogidos: " + currentPickups);

            if (currentPickups >= totalPickups)
            {
                Debug.Log("¡Has recogido todos los pickups!");
            }
        }
    }
}
