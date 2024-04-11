using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Vector2 PlayerPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        Vector3 newPosition = transform.position;
        newPosition.x = 5.0f;
        newPosition.y = 10.0f;
        transform.position = newPosition;

        SetPositionXY(15.0f, 20.0f);
    }

    void SetPositionXY(float newX, float newY)
    {
        Vector3 currentPosition = transform.position;

        currentPosition.x = newX;
        currentPosition.y = newY;

        transform.position = currentPosition;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
