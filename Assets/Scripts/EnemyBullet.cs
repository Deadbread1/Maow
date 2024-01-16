using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 20f;
    public float offScreenTime = 3f; // Time after which the bullet is destroyed if off-screen

    private float offScreenTimer;

    void Start()
    {
        offScreenTimer = offScreenTime;
    }

    void Update()
    {
        // Move the bullet to the left
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // Check if the bullet is off-screen
        if (!RendererIsVisible())
        {
            offScreenTimer -= Time.deltaTime;
            if (offScreenTimer <= 0)
            {
                Destroy(gameObject); // Destroy bullet after off-screen for a set time
            }
        }
        else
        {
            offScreenTimer = offScreenTime; // Reset timer when bullet is on screen
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Assuming the player has a PlayerController script
            other.GetComponent<PlayerController>().TakeDamage(1);
            Destroy(gameObject); // Destroy the bullet
        }
    }


    bool RendererIsVisible()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer)
        {
            return renderer.isVisible;
        }
        return false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Add collision logic here (e.g., impact on enemies)
        Destroy(gameObject); // Destroy the bullet on collision
    }
}
