using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class TitleVideoManager : MonoBehaviour
{
    [Header("References")]
    public VideoPlayer videoPlayer;
    public RawImage videoBackground;
    public RenderTexture renderTexture;

    [Header("Settings")]
    public float fadeInDuration = 0f;

    private CanvasGroup canvasGroup;

    void Start()
    {
        // Get or add canvas group for fade
        canvasGroup = videoBackground.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = videoBackground.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;

        // Clear render texture
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = null;

        // Setup video player
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoLoop;
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.Prepare();
        }
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        vp.Play();
        if (canvasGroup != null)
        canvasGroup.alpha = 1f;
    }

    void OnVideoLoop(VideoPlayer vp)
    {
        // Seamless loop
        vp.time = 0;
        vp.Play();
    }

    System.Collections.IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }
}