using UnityEngine;
using UnityEngine.UI;

public class ScanLineEffect : MonoBehaviour
{
    public RawImage scanLineImage;
    public float scrollSpeed = 0.5f;

    void Update()
    {
        Vector2 offset = scanLineImage.uvRect.position;
        offset.y -= scrollSpeed * Time.deltaTime;
        scanLineImage.uvRect = new Rect(offset, scanLineImage.uvRect.size);
    }
}