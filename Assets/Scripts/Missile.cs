using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;
    public GameObject explosionEffect;
    public int damage = 10;

    private Transform target;
    private Vector3 direction;

    void Start()
    {
        // Destroy the missile after a certain time if it doesn't hit anything
        Destroy(gameObject, lifetime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }

    void Update()
    {
        if (target != null)
        {
            // Missile homing towards a target
            direction = (target.position - transform.position).normalized;
        }

        // Move the missile
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            // TODO: Apply damage to the target

            // Instantiate explosion effect
            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject); // Destroy the missile
        }
    }
}
