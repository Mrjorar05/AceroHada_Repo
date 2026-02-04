using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Configuration")]
    [SerializeField] float speed;
    public bool isfacingRight;

    SpriteRenderer bulletSprite;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bulletSprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        BulletMove();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void BulletMove()
    {
        if (isfacingRight)
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
        else
        {


            bulletSprite.flipX = true;
            transform.Translate(Vector3.left * speed * Time.deltaTime);


        }

    }
}

