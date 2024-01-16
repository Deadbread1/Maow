using UnityEngine;

public class SmallProjectile : MonoBehaviour
{
    public int smallProjectileDamage = 5; // Damage caused by the small projectile

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        collision.gameObject.GetComponent<PlayerController>().TakeDamage(smallProjectileDamage);
        Destroy(gameObject); // Destroy the small projectile on collision
    }
}
