using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int health = 20;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float fireRate = 1.0f;
    public AudioClip explosionSound;
    public GameObject explosionPrefab;
    public float moveSpeed = 2.0f; // Adjustable speed

    private float nextFireTime = 0f;
    private AudioSource audioSource;
    private PlayerController playerController;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("Missing AudioSource component on EnemyController");
        }

        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController not found in scene by EnemyController");
        }
    }

    void Update()
    {
        MoveEnemy();

        if (Time.time > nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }

        if (transform.position.x < Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x)
        {
            if (playerController != null)
            {
                playerController.TakeDamage(1);
            }
            Destroy(gameObject);
        }
    }

    void MoveEnemy()
    {
        Vector3 movement = new Vector3(-1, Random.Range(-0.5f, 0.5f), 0).normalized;
        transform.position += movement * moveSpeed * Time.deltaTime;
    }

    void Shoot()
    {
        if (bulletPrefab != null)
        {
            Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            Debug.Log("Bullet shot.");
        }
        else
        {
            Debug.LogError("Bullet prefab is null in EnemyController");
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }
        else
        {
            Debug.LogError("Explosion sound or AudioSource is null in EnemyController");
        }

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Explosion prefab is null in EnemyController");
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
    }
}
