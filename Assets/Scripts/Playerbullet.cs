using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 1; // Damage dealt by the bullet
    public float offScreenTime = 1f; // Time after which the bullet is destroyed if off-screen

    private float offScreenTimer;

    void Start()
    {
        offScreenTimer = offScreenTime;
    }

    void Update()
    {
        // Move the bullet forward
        transform.Translate(Vector2.right * speed * Time.deltaTime);

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

    bool RendererIsVisible()
    {
        var renderer = GetComponent<Renderer>();
        return renderer != null && renderer.isVisible;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Assuming the enemy has a method to take damage
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject); // Destroy the bullet
        }
    }
}
