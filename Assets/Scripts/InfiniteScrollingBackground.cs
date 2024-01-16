using UnityEngine;

public class InfiniteScrollingBackground : MonoBehaviour
{
    public float scrollSpeed = 1.0f;
    public Transform[] backgrounds;

    private float backgroundWidth;

    void Start()
    {
        if (backgrounds.Length < 2)
        {
            Debug.LogError("Assign at least two backgrounds for the infinite scroll to work.");
            return;
        }

        // Assuming both backgrounds are of the same width
        backgroundWidth = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        foreach (var bg in backgrounds)
        {
            bg.transform.Translate(Vector2.left * scrollSpeed * Time.deltaTime);

            // Check if the background has moved completely out of view
            if (bg.transform.position.x <= -backgroundWidth)
            {
                // Reposition the background to create an endless loop effect
                float resetPositionX = bg.transform.position.x + 2 * backgroundWidth;
                bg.transform.position = new Vector3(resetPositionX, bg.transform.position.y, bg.transform.position.z);
            }
        }
    }
}
