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

    private Vector2 drawPosition = new Vector2(1000, 1000);
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        textureDrawer.Draw(brush, destination, drawPosition, scale, opacity, blendMaterial);
        drawPosition.x += 0.5f;
    }
}
