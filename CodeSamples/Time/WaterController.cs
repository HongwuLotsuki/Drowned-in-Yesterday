using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class WaterController : MonoBehaviour
{
    [Header("Water Configuration")]
    [Range(0f, 1f)] public float waterLevel = 0.5f;
    public Color waterColor = new Color(0.2f, 0.5f, 1.0f, 0.5f);
    public Color edgeColor = Color.white;
    public float edgeThickness = 0.02f;
    public float waveSpeed = 2.0f;
    public float waveHeight = 0.05f;
    public float waveFrequency = 15.0f;
    public float causticStrength = 0.3f;
    public float causticScale = 20.0f;
    public float distortionAmount = 0.02f;
    public float pixelSize = 128.0f;

    private Renderer _targetRenderer;
    private MaterialPropertyBlock _propBlock;

    private static readonly int WaterLevelId = Shader.PropertyToID("_WaterLevel");
    private static readonly int WaterColorId = Shader.PropertyToID("_WaterColor");
    private static readonly int EdgeColorId = Shader.PropertyToID("_EdgeColor");
    private static readonly int EdgeThicknessId = Shader.PropertyToID("_EdgeThickness");
    private static readonly int WaveSpeedId = Shader.PropertyToID("_WaveSpeed");
    private static readonly int WaveHeightId = Shader.PropertyToID("_WaveHeight");
    private static readonly int WaveFrequencyId = Shader.PropertyToID("_WaveFrequency");
    private static readonly int CausticStrengthId = Shader.PropertyToID("_CausticStrength");
    private static readonly int CausticScaleId = Shader.PropertyToID("_CausticScale");
    private static readonly int DistortionAmountId = Shader.PropertyToID("_DistortionAmount");
    private static readonly int PixelSizeId = Shader.PropertyToID("_PixelSize");

    private void OnEnable()
    {
        _targetRenderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        SyncShaderParameters();
    }

    private void SyncShaderParameters()
    {
        if (_targetRenderer == null) return;

        _targetRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat(WaterLevelId, waterLevel);
        _propBlock.SetColor(WaterColorId, waterColor);
        _propBlock.SetColor(EdgeColorId, edgeColor);
        _propBlock.SetFloat(EdgeThicknessId, edgeThickness);
        _propBlock.SetFloat(WaveSpeedId, waveSpeed);
        _propBlock.SetFloat(WaveHeightId, waveHeight);
        _propBlock.SetFloat(WaveFrequencyId, waveFrequency);
        _propBlock.SetFloat(CausticStrengthId, causticStrength);
        _propBlock.SetFloat(CausticScaleId, causticScale);
        _propBlock.SetFloat(DistortionAmountId, distortionAmount);
        _propBlock.SetFloat(PixelSizeId, pixelSize);
        _targetRenderer.SetPropertyBlock(_propBlock);
    }

    public bool IsUnderwater(Vector3 targetWorldPos, float? specificTime = null)
    {
        if (_targetRenderer == null) return false;

        // 1. Get the water renderer's absolute bounds in world space
        Bounds bounds = _targetRenderer.bounds;
        if (bounds.size.x <= 0f || bounds.size.y <= 0f) return false;

        // 2. Map the world position into 0-1 UV space
        // bounds.min is the bottom-left corner, bounds.max the top-right
        float uvX = (targetWorldPos.x - bounds.min.x) / bounds.size.x;
        float uvY = (targetWorldPos.y - bounds.min.y) / bounds.size.y;

        // 3. X bounds check: outside the water's width means not underwater
        if (uvX < 0f || uvX > 1f) return false;

        // 4. Reproduce the shader's wave function on the CPU
        float pixelatedUVX = Pixelate(uvX, pixelSize);
        float baseTime = specificTime ?? (Application.isPlaying ? Time.time : Time.realtimeSinceStartup);
        float t = baseTime * waveSpeed;

        float phaseOffset = waterLevel * 10.0f;
        float wave1 = Mathf.Sin(t + pixelatedUVX * waveFrequency + phaseOffset);
        float wave2 = Mathf.Cos(t * 1.3f - pixelatedUVX * waveFrequency * 2.3f - phaseOffset);
        float wave = (wave1 + wave2 * 0.5f) * waveHeight;

        wave = Pixelate(wave, pixelSize);
        float currentWaterLevel = waterLevel + wave;

        // 5. Y depth check
        return uvY < currentWaterLevel;
    }

    private float Pixelate(float value, float resolution)
    {
        return Mathf.Floor(value * resolution) / resolution;
    }
}