using UnityEngine;

public class LargeProjectile : MonoBehaviour
{
    public GameObject smallProjectilePrefab; // Prefab for smaller projectiles
    public int numberOfSmallProjectiles = 5; // Number of smaller projectiles to spawn
    public float timeBeforeSplit = 3.0f;     // Time in seconds before splitting
    public float splitSpeed = 5.0f;          // Speed of the smaller projectiles
    public int largeProjectileDamage = 10;   // Damage for the large projectile
    public int smallProjectileDamage = 5;    // Damage for each small projectile

    private float splitTimer;

    void Start()
    {
        splitTimer = timeBeforeSplit;
    }

    void Update()
    {
        splitTimer -= Time.deltaTime;
        if (splitTimer <= 0)
        {
            SplitProjectile();
            Destroy(gameObject); // Destroy the large projectile after splitting
        }
    }

    void SplitProjectile()
    {
        //for (int i = 0; i < numberOfSmallProjectiles; i++)
        //{
        //    GameObject smallProjectile = Instantiate(smallProjectilePrefab, transform.position, Quaternion.identity);
        //    SmallProjectile smallProjectileScript = smallProjectile.GetComponent<SmallProjectile>();

        //    if (smallProjectileScript != null)
        //    {
        //        smallProjectileScript.SetDamage(smallProjectileDamage);
        //    }

        //    // Randomize direction
        //    Vector2 randomDirection = Random.insideUnitCircle.normalized;
        //    smallProjectile.GetComponent<Rigidbody2D>().velocity = randomDirection * splitSpeed;
        //}
    }

    public void SetDamage(int largeDamage, int smallDamage)
    {
        largeProjectileDamage = largeDamage;
        smallProjectileDamage = smallDamage;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
         {
           collision.gameObject.GetComponent<PlayerController>().TakeDamage(largeProjectileDamage);
        }
        Destroy(gameObject); // Destroy the large projectile on collision
    }
}
