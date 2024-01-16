using UnityEngine;

public class SimpleBossAI : MonoBehaviour
{
    public int health = 20;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float moveSpeed = 2f;
    public float moveDistance = 2f;

    private bool isMovingUp = true;
    private int enemyCount;
    private bool transitioning = false;
    public GameObject stage2Prefab; // Prefab for stage 2

    void Start()
    {
        // Assuming enemies increment the enemyCount when they are defeated
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    void Update()
    {
        if (!transitioning)
        {
            MoveUpDown();

            if (health <= 10)
            {
                TransitionToStage2();
            }
            else
            {
                ShootBullet();
            }
        }
    }

    void MoveUpDown()
    {
        float moveDirection = isMovingUp ? 1f : -1f;
        transform.Translate(Vector3.up * moveDirection * moveSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.y) >= moveDistance)
        {
            isMovingUp = !isMovingUp;
        }
    }

    void ShootBullet()
    {
        Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }

    void TransitionToStage2()
    {
        transitioning = true;
        // Move out of camera view
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        // Assuming there's a trigger for stage 2 transition when reaching a certain position
        if (transform.position.x >= Camera.main.ViewportToWorldPoint(new Vector3(1.2f, 0.5f, 0)).x)
        {
            Instantiate(stage2Prefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void EnemyDefeated()
    {
        enemyCount--;

        // Check if all enemies are defeated
        if (enemyCount == 0)
        {
            health = Mathf.Clamp(health - 5, 0, health); // Decrease health when all enemies are defeated
        }
    }
}
