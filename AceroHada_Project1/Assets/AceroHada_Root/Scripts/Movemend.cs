using UnityEngine;

public class Movemend : MonoBehaviour
{
    private Rigidbody2D playerRb;
    private float horizontalInput;


    public float speed;
    public float jumpForce;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         playerRb = GetComponent<Rigidbody2D>(); 

         
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Jump();
    }

    void Movement()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        playerRb.linearVelocity = new Vector2(horizontalInput * speed, playerRb.linearVelocity.y);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);

        }

        
    }
}
