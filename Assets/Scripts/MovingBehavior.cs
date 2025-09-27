using UnityEngine;
using System.Collections;

public class MovingBehavior : MonoBehaviour
{
    public float hopHeight = 1f;      // maximum hop height
    public float hopDuration = 0.5f;  // total time per hop

    private bool isMoving = false;

    // Public method to hop to a destination
    public IEnumerator HopToPosition(Vector3 destination)
    {
        if (isMoving) yield break; // prevent overlapping moves
        isMoving = true;

        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < hopDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / hopDuration);

            // Lerp horizontally
            Vector3 currentPos = Vector3.Lerp(startPos, destination, t);

            // Parabola for vertical hop
            float height = 4 * hopHeight * t * (1 - t);
            currentPos.y += height;

            transform.position = currentPos;
            yield return null;
        }

        transform.position = destination; // snap to target
        isMoving = false;
    }
}
