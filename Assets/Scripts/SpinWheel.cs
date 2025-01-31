using System.Collections;
using UnityEngine;

public class SpinWheel : MonoBehaviour
{
    public float RotatePower; // Power applied to rotate the wheel
    public float StopPower; // Deceleration rate

    private Rigidbody2D rbody;
    private int inRotate;
    private float t;

    private void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Apply gradual stopping force
        if (rbody.angularVelocity > 0)
        {
            rbody.angularVelocity -= StopPower * Time.deltaTime;
            rbody.angularVelocity = Mathf.Clamp(rbody.angularVelocity, 0, 1440);
        }

        // Detect when the wheel stops and trigger the reward
        if (rbody.angularVelocity == 0 && inRotate == 1)
        {
            t += Time.deltaTime;
            if (t >= 0.5f) // Delay before calculating the reward
            {
                GetReward();
                inRotate = 0;
                t = 0;
            }
        }
    }

    // Method to start spinning the wheel
    public void Rotete()
    {
        if (inRotate == 0)
        {
            rbody.AddTorque(RotatePower);
            inRotate = 1;
        }
    }

    // Determine the reward based on final rotation
    public void GetReward()
    {
        float rot = transform.eulerAngles.z;
        float targetAngle = 0;
        int score = 1;

        if (rot > 0 + 22 && rot <= 45 + 22) { targetAngle = 45; score = 2; }
        else if (rot > 45 + 22 && rot <= 90 + 22) { targetAngle = 90; score = 3; }
        else if (rot > 90 + 22 && rot <= 135 + 22) { targetAngle = 135; score = 4; }
        else if (rot > 135 + 22 && rot <= 180 + 22) { targetAngle = 180; score = 5; }
        else if (rot > 180 + 22 && rot <= 225 + 22) { targetAngle = 225; score = 6; }
        else if (rot > 225 + 22 && rot <= 270 + 22) { targetAngle = 270; score = 7; }
        else if (rot > 270 + 22 && rot <= 315 + 22) { targetAngle = 315; score = 8; }
        else if (rot > 315 + 22 && rot <= 360 + 22) { targetAngle = 0; score = 1; }

        // Smoothly rotate the wheel to align with the winning segment
        StartCoroutine(SmoothRotate(targetAngle, 0.5f)); // Rotates over 0.5 seconds
        Win(score);
    }

    // Coroutine to smoothly rotate the wheel to the exact reward angle
    IEnumerator SmoothRotate(float targetAngle, float duration)
    {
        float startAngle = transform.eulerAngles.z;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newAngle = Mathf.LerpAngle(startAngle, targetAngle, elapsed / duration);
            transform.eulerAngles = new Vector3(0, 0, newAngle);
            yield return null;
        }

        transform.eulerAngles = new Vector3(0, 0, targetAngle); // Ensure exact target rotation
    }

    // Display the winning result
    public void Win(int Score)
    {
        print("DAY " + Score);
    }
}
