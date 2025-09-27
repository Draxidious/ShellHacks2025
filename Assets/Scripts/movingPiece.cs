using UnityEngine;
using System.Collections;
// comment
public class movingCube : MonoBehaviour
{
    public float moveDistance = 2f;   // distance per step
    public float hopHeight = 1f;      // hop height
    public float hopDuration = 0.5f;  // time per hop

    private bool isMoving = false;

    // Public method so any script can call this
    public IEnumerator MoveSteps(int steps)
    {
        if (isMoving) yield break; // don't allow overlapping moves
        isMoving = true;

        for (int i = 0; i < steps; i++)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + Vector3.forward * moveDistance;

            float elapsed = 0;
            while (elapsed < hopDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / hopDuration;

                Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);

                // parabola hop
                float height = 4 * hopHeight * t * (1 - t);
                currentPos.y += height;

                transform.position = currentPos;
                yield return null;
            }

            transform.position = endPos;
        }

        isMoving = false;
    }
}
