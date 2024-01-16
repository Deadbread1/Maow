using UnityEngine;

public class Drone : MonoBehaviour
{
    public float moveSpeed = 100f;
    public float rotationSpeed = 200.0f;
    public float evasionDistance = 2.0f;
    public float evasionDuration = 2.0f;
    public float attackDuration = 2.0f;
    public int damageToPlayer = 1;
    public AudioClip flyingSound;
    public AudioClip attackSound;
    public Transform target;
    public Transform boss; // Reference to the boss for regrouping

    private AudioSource audioSource;
    private float evasionTimer;
    private float attackTimer;
    private Vector3 evasionDirection;
    private Camera mainCamera;
    private Vector3 regroupPosition; // Position near the boss for regrouping
    private bool isEvasive; // Indicates whether the drone is currently evading

    private enum DroneState { Attacking, Evading, Regrouping };
    private DroneState currentState;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        mainCamera = Camera.main;

        audioSource = GetComponent<AudioSource>();
        PlaySound(flyingSound, true);

        attackTimer = attackDuration;
        isEvasive = false; // Initialize isEvasive to false
        currentState = DroneState.Attacking;
    }

    private void Update()
    {
        switch (currentState)
        {
            case DroneState.Attacking:
                AttackTarget();
                break;
            case DroneState.Evading:
                EvasiveManeuver();
                break;
            case DroneState.Regrouping:
                RegroupWithBoss();
                break;
        }
    }

    void AttackTarget()
    {
        if (target != null)
        {
            FaceTarget();
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0 && !isEvasive)
        {
            isEvasive = true;
            PrepareEvasion();
        }
    }

    void FaceTarget()
    {
        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.z = 0;
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 180));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void PrepareEvasion()
    {
        currentState = DroneState.Evading;
        evasionTimer = evasionDuration;
        evasionDirection = (Vector3)Random.insideUnitCircle.normalized * evasionDistance;
    }

    void EvasiveManeuver()
    {
        transform.position += evasionDirection * moveSpeed * Time.deltaTime;

        evasionTimer -= Time.deltaTime;
        if (evasionTimer <= 0)
        {
            currentState = DroneState.Regrouping;
            regroupPosition = boss.position + (Vector3)Random.insideUnitCircle.normalized * 5f;
        }
    }

    void RegroupWithBoss()
    {
        FaceTarget();
        transform.position = Vector3.MoveTowards(transform.position, regroupPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, regroupPosition) < 1f)
        {
            currentState = DroneState.Attacking;
            attackTimer = attackDuration;
            isEvasive = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject); // Drone destroyed on bullet collision
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damageToPlayer);
                PlaySound(attackSound, false);
            }
        }
    }

    void PlaySound(AudioClip clip, bool loop)
    {
        if (clip != null)
        {
            if (loop)
            {
                audioSource.clip = clip;
                audioSource.loop = true;
                audioSource.Play();
            }
            else
            {
                AudioSource.PlayClipAtPoint(clip, transform.position);
            }
        }
    }
}
