using UnityEngine;

public class RotateY : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objectsToRotate; // Array of GameObjects to rotate

    [SerializeField]
    private float rotationSpeed = 45.0f; // Rotation speed in degrees per second

    void Update()
    {
        // Rotate each GameObject in the array
        foreach (GameObject obj in objectsToRotate)
        {
            if (obj != null) // Check if the GameObject is not null
            {
                // Calculate the rotation for this frame
                float rotationAmount = rotationSpeed * Time.deltaTime;

                // Apply the rotation to the GameObject
                obj.transform.Rotate(0, rotationAmount, 0);
            }
        }
    }
}