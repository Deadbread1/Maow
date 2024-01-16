using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float shakeIntensity = 0.1f;
    public float zoomSize = 4.5f; // Smaller value for more zoom
    private float originalSize;
    private Rigidbody2D playerRigidbody;

    void Start()
    {
        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody2D>();
        }
        originalSize = Camera.main.orthographicSize;
    }

    void Update()
    {
        if (playerRigidbody != null)
        {
            float speed = playerRigidbody.velocity.magnitude;
            if (speed >= 3000f && speed <= 5000f)
            {
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, zoomSize, Time.deltaTime);
                ShakeCamera();
            }
            else
            {
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, originalSize, Time.deltaTime);
            }
        }
    }

    void ShakeCamera()
    {
        Vector3 originalPosition = transform.position;
        float shakeX = Random.value * shakeIntensity * 2 - shakeIntensity;
        float shakeY = Random.value * shakeIntensity * 2 - shakeIntensity;
        transform.position = new Vector3(originalPosition.x + shakeX, originalPosition.y + shakeY, originalPosition.z);
    }
}
