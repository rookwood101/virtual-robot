using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLocator : MonoBehaviour
{
    private CameraToTexture cameraTexture;
    void Awake()
    {
        cameraTexture = GetComponent<CameraToTexture>();
        cameraTexture.eventManager.AddListener(CameraToTexture.RenderedToTextureEvent, LocateBallInFrame);
    }

    void LocateBallInFrame(object _frame)
    {
        var frame = (Texture2D)_frame;
        
    }
}
