using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

public class ImageResolver : MonoBehaviour
{
    const string SHADER_PROPERTY_MAIN_TEX = "_MainTex";
    private static int mainTex = -1;
    private static int MainTex
    {
        get
        {
            if (mainTex == -1)
                mainTex = Shader.PropertyToID(SHADER_PROPERTY_MAIN_TEX);
            return mainTex;
        }
    }

    RenderTexture outputTexture;
    public RenderTexture OutputTexture => outputTexture;

    [SerializeField]
    ARCameraBackground arCameraBackground;
    [SerializeField]
    ARCameraManager arCameraManager;

    [SerializeField]
    UnityEvent<Texture> TextureEvent =  new UnityEvent<Texture>();

    private void Start()
    {
        arCameraManager.frameReceived += OnCameraFrame;
    }
    private void OnDestroy()
    {
        if (arCameraManager != null)
            arCameraManager.frameReceived -= OnCameraFrame;

        if (outputTexture != null)
        {
            outputTexture.Release();
        }
    }

    private void OnCameraFrame(ARCameraFrameEventArgs obj)
    {
        if (!isActiveAndEnabled)
            return;

        var texIndex = obj.propertyNameIds.IndexOf(MainTex);
        if (texIndex == -1)
        {
            Debug.LogWarning($"Shader.{SHADER_PROPERTY_MAIN_TEX} not found.");
            return;
        }

        var texture = obj.textures[texIndex];
        if (texture == null)
        {
            Debug.LogWarning($"Texture not found from camera frame");
            return;
        }

        if (outputTexture == null)
        {
            var size = new Vector2Int(texture.width, texture.height);
            if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
            {
                var width = size.x;
                size.x = size.y;
                size.y = width;
            }

            Debug.Log($"Creating GPU Render texture({size.x}, {size.y})");
            outputTexture = new RenderTexture(size.x, size.y, 0, RenderTextureFormat.ARGB32);
        }

        Graphics.Blit(null, outputTexture, arCameraBackground.material);
        outputTexture.IncrementUpdateCount();

        TextureEvent.Invoke(outputTexture);
    }
}
