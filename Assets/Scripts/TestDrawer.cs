using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDrawer : MonoBehaviour
{

    [SerializeField] private Texture brush;
    [SerializeField] private RenderTexture destination;
    [SerializeField] private float scale = 1;
    [SerializeField] private float opacity = 1;
    [SerializeField] private TextureDrawer textureDrawer;
    [SerializeField] private Material blendMaterial;
    [SerializeField] private Vector2 defaultPosition = new(0, 0);

    [SerializeField] private GameObject player;

    // private Vector2 drawPosition = new Vector2(1000, 1000);
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var drawPosition = defaultPosition;
        //player.transform.TransformVector(player.transform.x, player.transform.y, player.transform.z) = new Vector3(0, 0);
        /*if (Input.GetKeyDown(KeyCode.T))
        {
            Instantiate(player);
            Debug.Log("Player has been instantiated");
        }
        */

        if (player != null)
        {
            var playerPosition = player.transform.position;
            drawPosition = playerPosition;
        }

        else
        {
            Debug.LogError("Player is not assigned!");
        }

        drawPosition += new Vector2(1000, 1000);
        Debug.Log(drawPosition);
        textureDrawer.Draw(brush, destination, drawPosition, scale, opacity, blendMaterial);
        // drawPosition.x += 0.5f;
    }
} 
