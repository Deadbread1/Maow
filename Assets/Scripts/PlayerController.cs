using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public int Damage = 10;
    public float moveSpeed = 5.0f;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float fireRate = 0.5f;
    public GameObject vortexProjectilePrefab; // Assign the vortex prefab in the inspector
    public float vortexCooldown = 60f;

    public int health = 3;
    public int score = 0;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI scoreText;

    private float nextFireTime = 0f;
    private float vortexCooldownTimer = 0f;
    private bool isVortexReady = true;

    void Start()
    {
        UpdateUI(); // Initial UI update
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
        HandleVortexAbility();
        ClampPositionToCameraView();
        UpdateUI();
    }

    void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0.0f);
        transform.position += movement * moveSpeed * Time.deltaTime;
    }

    void HandleShooting()
    {
        if (Input.GetButton("Fire1") && Time.time > nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void HandleVortexAbility()
    {
        if (isVortexReady && Input.GetKeyDown(KeyCode.F)) // Use KeyCode.F for activating the ability
        {
            Instantiate(vortexProjectilePrefab, transform.position, Quaternion.identity);
            isVortexReady = false;
            vortexCooldownTimer = vortexCooldown;
        }

        if (!isVortexReady)
        {
            vortexCooldownTimer -= Time.deltaTime;
            if (vortexCooldownTimer <= 0)
            {
                isVortexReady = true;
            }
        }
    }

    void ClampPositionToCameraView()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        viewPos.x = Mathf.Clamp(viewPos.x, 0.05f, 0.95f);
        viewPos.y = Mathf.Clamp(viewPos.y, 0.05f, 0.95f);
        transform.position = Camera.main.ViewportToWorldPoint(viewPos);
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject); // Destroy the enemy
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // Handle player death (e.g., game over screen, respawn)
        }
    }

    public void AddScore(int points)
    {
        score += points;
    }

    private void UpdateUI()
    {
        if (healthText) healthText.text = "Health: " + health;
        if (scoreText) scoreText.text = "Score: " + score;
    }

}
