using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float avoidanceSpeed = 3f;
    public float avoidanceHeight = 1f;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public int burstCount = 3;
    public float fireRate = 0.5f;
    public float burstCooldown = 5f;
    public int health = 10; // Enemy health
    public AudioClip shootingSound;
    public AudioClip hitSound;
    public AudioClip flyingSound;

    private AudioSource audioSource;
    private float nextFireTime = 0f;
    private int shotsFiredInBurst = 0;
    private float burstTimer = 0f;
    private float avoidanceTimer = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlaySound(flyingSound);
    }

    void Update()
    {
        MoveWithAvoidance();
        AttemptShooting();
    }

    void MoveWithAvoidance()
    {
        // Basic leftward movement
        Vector3 leftwardMovement = Vector2.left * moveSpeed * Time.deltaTime;

        // Up and down movement to avoid bullets
        avoidanceTimer += Time.deltaTime;
        float verticalMovement = Mathf.Sin(avoidanceTimer * avoidanceSpeed) * avoidanceHeight;

        transform.Translate(leftwardMovement + new Vector3(0, verticalMovement, 0));
    }

    void AttemptShooting()
    {
        if (burstTimer > 0)
        {
            burstTimer -= Time.deltaTime;
            return;
        }

        if (shotsFiredInBurst < burstCount && Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
            shotsFiredInBurst++;
        }
        else if (shotsFiredInBurst >= burstCount)
        {
            burstTimer = burstCooldown;
            shotsFiredInBurst = 0;
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        PlaySound(shootingSound);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            PlayerBullet playerBullet = other.GetComponent<PlayerBullet>();
            if (playerBullet != null)
            {
                
            }
            PlaySound(hitSound);
        }
    }

    void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // TODO: Add logic for enemy death (e.g., explosion effect, score increment)
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
