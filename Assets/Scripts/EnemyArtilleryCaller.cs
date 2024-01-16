using UnityEngine;

public class EnhancedFlybyEnemy : MonoBehaviour
{
    public float entrySpeed = 10f;
    public float exitSpeed = 10f;
    public AudioClip flybySound;
    public GameObject missileRainControllerPrefab;
    public float entryDelay = -5f; // Delay before starting to move
    public float exitDelay = 1f;  // Delay after entering the screen before exiting
    public float disappearDelay = 2f; // Time to disappear after exiting the screen

    private AudioSource audioSource;
    private Vector3 entryTarget;
    private Vector3 exitTarget;
    private float delayTimer;
    private bool isEntering = true;
    private bool hasPlayedSound = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SetEntryAndExitPoints();
        delayTimer = entryDelay;
        // Move enemy to starting position just off-screen
        transform.position = entryTarget + Vector3.right * 1000f; // Modify this to ensure it's off-screen
    }

    void Update()
    {
        if (delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        if (isEntering)
        {
            MoveToPosition(entryTarget, entrySpeed);
            if (!hasPlayedSound && IsInView())
            {
                audioSource.PlayOneShot(flybySound);
                hasPlayedSound = true;
                delayTimer = exitDelay; // Set delay before starting to exit
            }
            else if (hasPlayedSound && delayTimer <= 0)
            {
                isEntering = false;
            }
        }
        else
        {
            MoveToPosition(exitTarget, exitSpeed);
            if (IsOutOfView())
            {
                if (disappearDelay > 0)
                {
                    disappearDelay -= Time.deltaTime;
                }
                else
                {
                    StartMissileRain();
                    Destroy(gameObject);
                }
            }
        }
    }

    void SetEntryAndExitPoints()
    {
        // Assuming the enemy enters from the right and exits to the left
        entryTarget = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0.5f, 10f));
        exitTarget = Camera.main.ViewportToWorldPoint(new Vector3(-0.1f, 0.5f, 10f));
    }

    void MoveToPosition(Vector3 targetPosition, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    bool IsInView()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        return screenPoint.x >= 0 && screenPoint.x <= 1;
    }

    bool IsOutOfView()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        return screenPoint.x < 0;
    }

    void StartMissileRain()
    {
        Instantiate(missileRainControllerPrefab, Vector3.zero, Quaternion.identity);
    }
}
