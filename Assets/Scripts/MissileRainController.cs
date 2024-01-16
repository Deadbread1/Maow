using UnityEngine;

public class MissileRainController : MonoBehaviour
{
    public GameObject missilePrefab;
    public int missileCount = 10;
    public float spawnInterval = 0.5f;
    public AudioClip sirenSound;

    private AudioSource audioSource;
    private float nextSpawnTime = 0f;
    private int spawnedMissiles = 0;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = sirenSound;
        audioSource.loop = true;
        audioSource.Play();
    }

    void Update()
    {
        if (Time.time > nextSpawnTime && spawnedMissiles < missileCount)
        {
            SpawnMissile();
            nextSpawnTime = Time.time + spawnInterval;
            spawnedMissiles++;
        }

        if (spawnedMissiles >= missileCount)
        {
            audioSource.Stop();
            Destroy(gameObject, 2f); // Destroy after a delay to let last missiles fly out of screen
        }
    }

    void SpawnMissile()
    {
        Vector3 spawnPosition = GetRandomOffscreenPosition();
        Instantiate(missilePrefab, spawnPosition, Quaternion.identity);
    }

    Vector3 GetRandomOffscreenPosition()
    {
        // Calculate a random position outside the screen bounds
        // Adjust this method to suit your game's needs
        return Vector3.zero; // Placeholder, implement the actual position logic
    }
}
