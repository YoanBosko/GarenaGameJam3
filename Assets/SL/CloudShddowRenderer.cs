// CloudShadowRenderer.cs
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CloudShadowRenderer : MonoBehaviour
{
    [Header("Sun & Cloud References")]
    public Light sunLight; // Drag your main Directional Light here
    public Material cloudMaterial; // Drag your volumetric cloud shader material here

    [Header("Shadow Map Settings")]
    public int resolution = 512; // Low resolution for performance
    public float coverageSize = 1000f; // How large an area the shadow covers (in world units)
    public float cloudLayerHeight = 1500f; // Should match your cloud shader's height
    public float cloudLayerThickness = 800f; // Should match your cloud shader's thickness

    [Header("Shadow Properties")]
    [Range(0, 1)] public float shadowStrength = 0.7f;
    public float shadowSoftness = 2.0f; // Kernel size for blur
    public bool updateEveryFrame = true;

    private Camera shadowCam;
    private RenderTexture shadowRT;
    private RenderTexture blurRT;
    private Material blurMaterial;
    private static readonly int CloudShadowMap = Shader.PropertyToID("_CloudShadowMap");
    private static readonly int CloudShadowParams = Shader.PropertyToID("_CloudShadowParams");

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        // 1. Configure the Camera Component
        shadowCam = GetComponent<Camera>();
        shadowCam.enabled = false; // We will render manually
        shadowCam.orthographic = true;
        shadowCam.orthographicSize = coverageSize * 0.5f;
        shadowCam.nearClipPlane = 0.3f;
        shadowCam.farClipPlane = cloudLayerHeight + cloudLayerThickness + 1000f;
        shadowCam.clearFlags = CameraClearFlags.SolidColor;
        shadowCam.backgroundColor = Color.clear; // No cloud = clear

        // 2. Create Render Textures
        shadowRT = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.R8);
        shadowRT.wrapMode = TextureWrapMode.Clamp;
        shadowRT.filterMode = FilterMode.Bilinear;
        shadowRT.autoGenerateMips = false;

        blurRT = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.R8);
        blurRT.wrapMode = TextureWrapMode.Clamp;
        blurRT.filterMode = FilterMode.Bilinear;

        // 3. Create a simple blur material (Box blur for speed)
        blurMaterial = new Material(Shader.Find("Hidden/BoxBlur"));
        blurMaterial.SetFloat("_KernelSize", shadowSoftness);
    }

    void Update()
    {
        if (sunLight == null || cloudMaterial == null) return;

        if (updateEveryFrame || !shadowRT.IsCreated())
        {
            RenderShadowMap();
        }
    }

    void RenderShadowMap()
    {
        // Position the camera at the sun, looking straight down onto the cloud layer
        transform.position = sunLight.transform.position;
        transform.rotation = sunLight.transform.rotation;
        // Tilt the camera to look straight down for an orthographic projection
        transform.rotation = Quaternion.LookRotation(-Vector3.up, Vector3.forward);

        // Set the camera's target
        shadowCam.targetTexture = shadowRT;

        // Prepare cloud material for the "Density-Only" pass
        // We'll use a special shader keyword. You must add this to your cloud shader.
        cloudMaterial.EnableKeyword("SHADOW_MAP_RENDER");
        cloudMaterial.SetFloat("_ShadowMode", 1.0f);

        // Render the clouds from the sun's perspective
        shadowCam.RenderWithShader(cloudMaterial.shader, "RenderType"); // This is key

        // Disable the mode for the main camera's rendering
        cloudMaterial.DisableKeyword("SHADOW_MAP_RENDER");
        cloudMaterial.SetFloat("_ShadowMode", 0.0f);

        // Apply a simple blur to soften the shadow edges
        BlurShadowMap();

        // Pass the final blurred shadow map and parameters to all shaders
        Shader.SetGlobalTexture(CloudShadowMap, blurRT);
        Shader.SetGlobalVector(CloudShadowParams, new Vector4(
            coverageSize,
            cloudLayerHeight,
            shadowStrength,
            0 // reserved
        ));
    }

    void BlurShadowMap()
    {
        if (shadowSoftness <= 1f)
        {
            Graphics.Blit(shadowRT, blurRT); // No blur, just copy
            return;
        }
        // Two-pass blur for better softness
        RenderTexture temp = RenderTexture.GetTemporary(resolution, resolution, 0, RenderTextureFormat.R8);
        // Horizontal pass
        Graphics.Blit(shadowRT, temp, blurMaterial, 0);
        // Vertical pass
        Graphics.Blit(temp, blurRT, blurMaterial, 1);
        RenderTexture.ReleaseTemporary(temp);
    }

    void OnDestroy()
    {
        if (shadowRT != null) shadowRT.Release();
        if (blurRT != null) blurRT.Release();
        if (blurMaterial != null) Destroy(blurMaterial);
    }

#if UNITY_EDITOR
    // A simple button to render manually in the editor
    [ContextMenu("Render Shadow Map Now")]
    void RenderNow()
    {
        if (Application.isPlaying) RenderShadowMap();
    }
#endif
}