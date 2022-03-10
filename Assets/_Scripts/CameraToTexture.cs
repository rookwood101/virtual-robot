using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CameraToTexture : MonoBehaviour
{
    private new Camera camera;
    private Texture2D texture;
    public readonly EventManager eventManager = new EventManager();
    public static readonly EventType RenderedToTextureEvent = new EventType(typeof(Null));

    async void Start()
    {
        camera = transform.Find("Body/Camera").GetComponent<Camera>();
        texture = new Texture2D(camera.targetTexture.width, camera.targetTexture.height, TextureFormat.RGB24, false);
        while(this.isActiveAndEnabled) {
            // source: https://gist.github.com/danielbierwirth/10965844fecc38243007f0cd21843d90
            await new WaitForEndOfFrame();
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = camera.targetTexture;
            camera.Render();
            texture.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0, false);
            texture.Apply();
            RenderTexture.active = currentRT;
            eventManager.TriggerEvent(RenderedToTextureEvent, null);
            await new WaitForSeconds(1f / 15f);
        }
    }
}
