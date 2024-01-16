using UnityEngine;

public class VortexProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float distanceBeforeActivation = 5f;
    public float pullForce = 15f;
    public float pullRadius = 3f;
    public LayerMask affectedLayers;
    public float activeLifespan = 10f; // Time before the projectile disappears
    private bool isActive = false;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        Invoke("ActivateVortex", distanceBeforeActivation / speed); // Activate after moving certain distance
        Invoke("DestroyVortex", activeLifespan + distanceBeforeActivation / speed); // Destroy after total lifespan
    }

    void Update()
    {
        if (!isActive)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime); // Assumes projectile moves to the right
        }
        else
        {
            PullObjects();
        }
    }

    void ActivateVortex()
    {
        isActive = true;
        // Optional: Add visual or audio effects here to indicate activation
    }

    void PullObjects()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pullRadius, affectedLayers);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                if (collider.gameObject.CompareTag("Boss"))
                {
                    // Deal damage to Boss instead of pulling
                    // Assuming Boss has a method TakeDamage
                    collider.gameObject.SendMessage("TakeDamage", 1, SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    Vector2 direction = transform.position - collider.transform.position;
                    collider.GetComponent<Rigidbody2D>().AddForce(direction.normalized * pullForce);
                }
            }
        }
    }

    void DestroyVortex()
    {
        Destroy(gameObject);
    }
}
