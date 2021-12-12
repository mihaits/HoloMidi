using UnityEngine;

public class RemotingSpectatorView : MonoBehaviour
{
    public bool IsRunning { get; private set; }

    public int Width { get; set; } = 1280;
    public int Height { get; set; } = 720;

    public RenderTexture RenderTexture => _camera.targetTexture;

    private Camera _camera;
    public Camera Camera
    {
        get
        {
            if (_camera == null)
                _camera = GetComponentInChildren<Camera>(true);

            return _camera;
        }
    }

    private void OnDisable()
    {
        StopCamera();
    }

    public void StartCamera()
    {
        Camera.targetTexture = new RenderTexture(Width, Height, 32);

        IsRunning = true;
    }

    public void StopCamera()
    {
        var renderTexture = Camera.targetTexture;
        Camera.targetTexture = null;
        DestroyImmediate(renderTexture);

        IsRunning = false;
    }
}
