using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    public float minIntensity = 0f;
    public float maxIntensity = 1f;
    public float flickerSpeed = 0.05f;

    [Header("Flicker Style")]
    public FlickerMode flickerMode = FlickerMode.Random;
    public bool flickerOn = true;

    [Header("Stutter Settings")]
    public float stutterChance = 0.05f;
    public float stutterDuration = 0.1f;

    [Header("Pattern Settings")]
    public float[] customPattern = { 1f, 0f, 1f, 0f, 0f, 1f };
    public float patternSpeed = 0.1f;

    public enum FlickerMode
    {
        Random,
        Stutter,
        Pattern,
        SmoothWave
    }

    private Light flickerLight;
    private float timer = 0f;
    private float stutterTimer = 0f;
    private bool isStuttering = false;
    private int patternIndex = 0;
    private float patternTimer = 0f;
    private float targetIntensity;
    private float originalIntensity;

    void Start()
    {
        flickerLight = GetComponent<Light>();

        if (flickerLight != null)
        {
            originalIntensity = flickerLight.intensity;
            targetIntensity = originalIntensity;
            maxIntensity = originalIntensity;
        }
    }

    void Update()
    {
        if (!flickerOn || flickerLight == null) return;

        switch (flickerMode)
        {
            case FlickerMode.Random:
                RandomFlicker();
                break;
            case FlickerMode.Stutter:
                StutterFlicker();
                break;
            case FlickerMode.Pattern:
                PatternFlicker();
                break;
            case FlickerMode.SmoothWave:
                SmoothWaveFlicker();
                break;
        }
    }

    void RandomFlicker()
    {
        timer += Time.deltaTime;
        if (timer >= flickerSpeed)
        {
            timer = 0f;
            flickerLight.intensity = Random.Range(minIntensity, maxIntensity);
        }
    }

    void StutterFlicker()
    {
        if (isStuttering)
        {
            stutterTimer += Time.deltaTime;
            flickerLight.intensity = minIntensity;

            if (stutterTimer >= stutterDuration)
            {
                isStuttering = false;
                stutterTimer = 0f;
                flickerLight.intensity = maxIntensity;
            }
        }
        else
        {
            // Random chance to stutter each frame
            if (Random.value < stutterChance)
            {
                isStuttering = true;
                stutterTimer = 0f;
            }
        }
    }

    void PatternFlicker()
    {
        patternTimer += Time.deltaTime;
        if (patternTimer >= patternSpeed)
        {
            patternTimer = 0f;
            patternIndex = (patternIndex + 1) % customPattern.Length;
            flickerLight.intensity = customPattern[patternIndex] * maxIntensity;
        }
    }

    void SmoothWaveFlicker()
    {
        float wave = Mathf.Sin(Time.time * flickerSpeed * 10f);
        flickerLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, (wave + 1f) / 2f);
    }

    // Call these from other scripts if needed
    public void TurnOn()
    {
        flickerOn = true;
        if (flickerLight != null)
            flickerLight.enabled = true;
    }

    public void TurnOff()
    {
        flickerOn = false;
        if (flickerLight != null)
        {
            flickerLight.enabled = false;
        }
    }

    public void SetMode(FlickerMode mode)
    {
        flickerMode = mode;
    }
}