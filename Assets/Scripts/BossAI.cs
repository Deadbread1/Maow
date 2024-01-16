using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    public int health = 100;
    public GameObject minionPrefab;
    public GameObject[] bulletPrefabs;
    public Transform bulletSpawnPoint;
    public float summonInterval = 10f;

    public GameObject[] transformationPrefabs;
    public GameObject dronePrefab;
    public GameObject largeProjectilePrefab;

    public int numberOfDrones = 5;
    public float splittingProjectileSpeed = 5f;
    public int splittingProjectileSplitCount = 3;
    public float transformationDuration = 5f;

    public Material flashMaterial;
    public Material defaultMaterial;
    public float flashDuration = 0.1f;
    public AudioClip explosionSound;
    public AudioClip[] bulletSounds;
    public AudioClip bossMusic;

    private AudioSource audioSource;
    private AudioSource musicSource;
    private float summonTimer;
    private Renderer bossRenderer;
    private float shootingTimer;

    private bool isTransforming = false;
    private float lastDirectionChangeTime;
    public float directionChangeInterval = 1f;
    private Vector2 avoidanceDirection;
    public float avoidanceRadius = 5f;
    public float avoidanceSpeed = 3f;

    // Cooldown properties
    public float rapidTransformationCooldown = 20f;
    public float droneSummonCooldown = 30f;
    public float splittingProjectileCooldown = 40f;

    private float rapidTransformationTimer;
    private float droneSummonTimer;
    private float splittingProjectileTimer;

    // Spread shot properties
    private float spreadShotTimer;
    private float nextSpreadShotTime;
    private float spreadShotIntervalMin = 0f;
    private float spreadShotIntervalMax = 1f;
    private int spreadShotBulletCount = 5;
    private float spreadShotAngle = 45f;

    // Movement Bounds
    public Vector2 movementBounds = new Vector2(0.8f, 0.8f);

    void Start()
    {
        bossRenderer = GetComponent<Renderer>();
        defaultMaterial = bossRenderer.material;
        summonTimer = summonInterval;
        audioSource = GetComponent<AudioSource>();

        rapidTransformationTimer = 0;
        droneSummonTimer = 0;
        splittingProjectileTimer = 0;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = bossMusic;
        musicSource.loop = true;
        musicSource.Play();
        ChangeAvoidanceDirection();

        ResetSpreadShotTimer();
    }

    void Update()
    {
        AvoidPlayerBullets();
        HandleShooting();
        HandleAbilities();

        ClampPositionToCameraView();

        UpdateCooldownTimers();

        summonTimer -= Time.deltaTime;
        if (summonTimer <= 0)
        {
            SummonDrones();
            summonTimer = summonInterval;
        }
    }

    void UpdateCooldownTimers()
    {
        if (rapidTransformationTimer > 0) rapidTransformationTimer -= Time.deltaTime;
        if (droneSummonTimer > 0) droneSummonTimer -= Time.deltaTime;
        if (splittingProjectileTimer > 0) splittingProjectileTimer -= Time.deltaTime;
    }

    void AvoidPlayerBullets()
    {
        if (Time.time > lastDirectionChangeTime + directionChangeInterval)
        {
            ChangeAvoidanceDirection();
        }
        transform.Translate(avoidanceDirection * avoidanceSpeed * Time.deltaTime);
    }

    void ChangeAvoidanceDirection()
    {
        float horizontal = Random.Range(-1f, 1f);
        float vertical = Random.Range(-1f, 1f);
        avoidanceDirection = new Vector2(horizontal, vertical).normalized;
        lastDirectionChangeTime = Time.time;
    }

    void ClampPositionToCameraView()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        float minX = 0.5f - movementBounds.x / 2;
        float maxX = 0.5f + movementBounds.x / 2;
        float minY = 0.5f - movementBounds.y / 2;
        float maxY = 0.5f + movementBounds.y / 2;

        viewPos.x = Mathf.Clamp(viewPos.x, minX, maxX);
        viewPos.y = Mathf.Clamp(viewPos.y, minY, maxY);
        transform.position = Camera.main.ViewportToWorldPoint(viewPos);
    }

    void HandleShooting()
    {
        // Switch to spread shot only when health is 25% or lower
        if ((float)health / 100 <= 0.25f)
        {
            spreadShotTimer += Time.deltaTime;
            if (spreadShotTimer >= nextSpreadShotTime)
            {
                ShootSpreadShot();
                ResetSpreadShotTimer();
            }
        }
        else
        {
            shootingTimer -= Time.deltaTime;
            if (shootingTimer <= 0)
            {
                ShootRandomBullet();
                shootingTimer = Random.Range(1f, 3f);
            }
        }
    }

    void ShootRandomBullet()
    {
        if (bulletPrefabs.Length > 0)
        {
            int bulletIndex = Random.Range(0, bulletPrefabs.Length);
            GameObject bulletPrefab = bulletPrefabs[bulletIndex];
            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            if (bulletIndex < bulletSounds.Length)
            {
                audioSource.PlayOneShot(bulletSounds[bulletIndex]);
            }
        }
    }

    void ShootSpreadShot()
    {
        for (int i = 0; i < spreadShotBulletCount; i++)
        {
            float angle = spreadShotAngle / (spreadShotBulletCount - 1) * i - spreadShotAngle / 2;
            Quaternion rotation = Quaternion.Euler(0, 0, angle) * bulletSpawnPoint.rotation;
            Instantiate(bulletPrefabs[0], bulletSpawnPoint.position, rotation);
        }
    }

    void ResetSpreadShotTimer()
    {
        spreadShotTimer = 0;
        nextSpreadShotTime = Random.Range(spreadShotIntervalMin, spreadShotIntervalMax);
    }

    void HandleAbilities()
    {
        // Implement boss abilities logic here
    }

    void SummonDrones()
    {
        for (int i = 0; i < numberOfDrones; i++)
        {
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * 2f;
            Instantiate(dronePrefab, spawnPosition, Quaternion.identity);
        }
    }

    void FireSplittingProjectile()
    {
        // Implement logic for firing a splitting projectile
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(Flash());
        }
    }

    IEnumerator Flash()
    {
        bossRenderer.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);
        bossRenderer.material = defaultMaterial;
    }

    void Die()
    {
        if (explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            PlayerBullet playerBullet = other.GetComponent<PlayerBullet>();
            if (playerBullet != null)
            {
                TakeDamage(playerBullet.damage);
            }
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(player.Damage);
            }
        }
    }
}
