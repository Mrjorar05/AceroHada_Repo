using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class MovilSprite : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private Transform[] waypoints; 
    [SerializeField] private float speed = 2f; 
    [SerializeField] private float waitTime = 1f; 

    [Header("Opciones")]
    [SerializeField] private bool loop = true; 
    [SerializeField] private bool pingPong = false; 

    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private bool movingForward = true;

    private void Start()
    {
        
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("No se han asignado waypoints a la plataforma: " + gameObject.name);
        }
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
            }
            return;
        }

        MovePlatform();
    }

    private void MovePlatform()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];

       
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWaypoint.position,
            speed * Time.deltaTime
        );

        
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.01f)
        {
            isWaiting = true;
            SelectNextWaypoint();
        }
    }

    private void SelectNextWaypoint()
    {
        if (pingPong)
        {
            
            if (movingForward)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Length)
                {
                    currentWaypointIndex = waypoints.Length - 2;
                    movingForward = false;
                }
            }
            else
            {
                currentWaypointIndex--;
                if (currentWaypointIndex < 0)
                {
                    currentWaypointIndex = 1;
                    movingForward = true;
                }
            }
        }
        else
        {
            
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                if (loop)
                {
                    currentWaypointIndex = 0;
                }
                else
                {
                    currentWaypointIndex = waypoints.Length - 1;
                }
            }
        }
    }

  
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

   
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            
            Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);

         
            if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }

            
            if (loop && i == waypoints.Length - 1)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
            }
        }
    }
}

