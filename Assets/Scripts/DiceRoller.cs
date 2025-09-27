using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class DiceRoller : MonoBehaviour
{
    public Transform die1;
    public Transform die2;

    public bool testRoll = false; // Toggle in Inspector to test a roll

    private Vector3 die1StartPos;
    private Vector3 die2StartPos;

    // Adjustable parameters
    public float liftHeight = 1f;       // Lower lift
    public float rollDuration = 0.5f;   // Duration to lift/drop
    public float spinSpeed = 1800f;     // Degrees per second

    public int sum = 0;
    void Start()
    {
        die1StartPos = die1.position;
        die2StartPos = die2.position;
    }

    void Update()
    {
        if (testRoll)
        {
            testRoll = false;
            RollDice();
        }
    }

    public void RollDice()
    {
        StartCoroutine(RollDiceRoutine());
        GameManager.Instance.DiceRolled(sum);
    }

    private IEnumerator RollDiceRoutine()
    {
        Vector3 die1UpPos = die1StartPos + Vector3.up * liftHeight;
        Vector3 die2UpPos = die2StartPos + Vector3.up * liftHeight;

        float t = 0f;

        // --- Lift Up with fast spin ---
        while (t < rollDuration)
        {
            t += Time.deltaTime;
            float lerp = t / rollDuration;

            die1.position = Vector3.Lerp(die1StartPos, die1UpPos, lerp);
            die2.position = Vector3.Lerp(die2StartPos, die2UpPos, lerp);

            die1.Rotate(Random.insideUnitSphere * spinSpeed * Time.deltaTime);
            die2.Rotate(Random.insideUnitSphere * spinSpeed * Time.deltaTime);

            yield return null;
        }

        // --- Pick random numbers ---
        int result1 = Random.Range(1, 7);
        int result2 = Random.Range(1, 7);

        // --- Face the chosen numbers up ---
        SetDieFace(die1, result1);
        SetDieFace(die2, result2);

        // --- Drop back down ---
        t = 0f;
        while (t < rollDuration)
        {
            t += Time.deltaTime;
            float lerp = t / rollDuration;

            die1.position = Vector3.Lerp(die1UpPos, die1StartPos, lerp);
            die2.position = Vector3.Lerp(die2UpPos, die2StartPos, lerp);

            yield return null;
        }

        sum = result1 + result2;
        Debug.Log($"You rolled: {result1} + {result2} = {sum}");
    }

    private void SetDieFace(Transform die, int value)
    {
        Vector3 localDir = Vector3.up;

        switch (value)
        {
            case 1: localDir = Vector3.forward; break;
            case 2: localDir = Vector3.left; break;
            case 3: localDir = Vector3.back; break;
            case 4: localDir = Vector3.right; break;
            case 5: localDir = Vector3.up; break;
            case 6: localDir = Vector3.down; break;
        }

        die.rotation = Quaternion.FromToRotation(localDir, Vector3.up);
    }
}
