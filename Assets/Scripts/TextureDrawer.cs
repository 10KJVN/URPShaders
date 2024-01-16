using System.Collections.Generic;
using UnityEngine;

public class TextureDrawer : MonoBehaviour
{
    private Dictionary<RenderTexture, RenderTexture> frameBufferCache = new Dictionary<RenderTexture, RenderTexture>();
    private Dictionary<RenderTexture, RenderTexture> destinationCopies = new Dictionary<RenderTexture, RenderTexture>();

    // todo: ombouwen naar Generic Dictionary waar verschillende blend materials in kunnen zitten gekoppeld aan enum
    [SerializeField] private Material defaultBlendMaterial;

    private void CopyTexture(Texture source, RenderTexture destination, RenderTexture frameBuffer, RenderTexture copy,
        int x, int y, Material blendMaterial)
    {
        Graphics.Blit(destination, copy);
        frameBuffer.Release();

        Graphics.CopyTexture(source, 0, 0, 0, 0, source.width, source.height, frameBuffer, 0, 0, x, y);
        Graphics.Blit(frameBuffer, destination, blendMaterial);
    }

    private RenderTexture GetFrameBuffer(RenderTexture destination, Dictionary<RenderTexture, RenderTexture> targetCache)
    {
        if (!targetCache.ContainsKey(destination)) targetCache[destination] = Instantiate(destination);

        return targetCache[destination];
    }

    public void Draw(Texture brush, RenderTexture destination, int x, int y, float scale = 1f, float opacity = 1f,
        Material blendMaterial = null)
    {
        var bounds = new Vector2(brush.width, brush.height);
        bounds *= scale;
        x = x - (int) bounds.x / 2;
        y = y - (int) bounds.y / 2;
        var frameBuffer = GetFrameBuffer(destination, frameBufferCache);
        var copy = GetFrameBuffer(destination, destinationCopies);
        var offset = new Vector2();
        offset.x = x / (float) destination.width;
        offset.y = y / (float) destination.height;
        var newPosition = offset * scale;
        var delta = newPosition - offset;
        var position = delta / scale;

        blendMaterial = blendMaterial != null ? blendMaterial : defaultBlendMaterial;

        blendMaterial.SetTexture("_Destination", copy);
        blendMaterial.SetFloat("_Opacity", opacity);
        blendMaterial.SetFloat("_Scale", scale);
        blendMaterial.SetVector("_Position", position);
        blendMaterial.SetTexture("_MainTex", brush);
        CopyTexture(brush, destination, frameBuffer, copy, x, y, blendMaterial);
    }

    public void Draw(Texture brush, RenderTexture destination, Vector2 drawPosition, float scale = 1f,
        float opacity = 1f, Material blendMaterial = null)
    {
        Draw(brush, destination, (int) drawPosition.x, (int) drawPosition.y, scale, opacity, blendMaterial);
    }

    public bool InDrawBounds(Texture brush, RenderTexture destination, Vector2 drawPosition, float scale = 1f)
    {
        var bounds = new Vector2(brush.width, brush.height);
        bounds *= scale;
        return drawPosition.x - bounds.x / 2 > 0 && drawPosition.x + bounds.x / 2 < destination.width
                                                 && drawPosition.y - bounds.y / 2 > 0 &&
                                                 drawPosition.y + bounds.y / 2 < destination.height;
    }
}