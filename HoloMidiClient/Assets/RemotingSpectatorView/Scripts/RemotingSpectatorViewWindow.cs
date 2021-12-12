using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Displays an editor window for the remoting spectator view 
/// </summary>
public class RemotingSpectatorViewWindow : EditorWindow
{
    private static bool IsCameraRunning => Target.IsRunning;
    private static Texture RenderTexture => Target.RenderTexture;

    private float _margin = 10f;
    private Rect _imageRect;

    private static RemotingSpectatorView _target;
    private static RemotingSpectatorView Target
    {
        get
        {
            if (_target == null)
            {
                _target = FindObjectOfType<RemotingSpectatorView>();
                if (_target == null)
                {
                    var go = Resources.Load("SpectatorCameraRoot");
                    var instance = Instantiate(go);
                    _target = FindObjectOfType<RemotingSpectatorView>();
                }
            }

            return _target;
        }
    }

    private enum RectPosition
    {
        Start,
        End,
        Full,
        Image
    }

    private Rect GetNextRect(int line, float startPosition, RectPosition rectPosition, float width, int height = 0)
    {
        var rect = new Rect();

        rect.y = 5f + line * 18f + line * 5f;
        rect.height = height == 0f ? 18f : height;

        switch (rectPosition)
        {
            case RectPosition.Start:
                rect.x = _margin;
                rect.width = position.width * width - _margin * 1.5f;
                break;
            case RectPosition.End:
                rect.x = position.width * startPosition + _margin * 0.5f;
                rect.width = position.width - (position.width * startPosition) - _margin * 1.5f;
                break;
            case RectPosition.Image:
                rect.height = position.height - rect.y - _margin;
                rect.x = _margin;
                rect.width = position.width - _margin * 2f;
                break;
            case RectPosition.Full:
                rect.x = _margin;
                rect.width = position.width - _margin * 2f;
                break;
        }

        return rect;
    }

    private static void SetPlayerPrefsToTarget()
    {
        Target.Camera.fieldOfView = EditorPrefs.GetFloat("FieldOfView", 60);
        Target.Width  = EditorPrefs.GetInt("Width", 1280);
        Target.Height = EditorPrefs.GetInt("Height", 720);
    }

    [MenuItem("Remoting Spectator View/Show window")]
    private static void ShowWindow()
    {
        GetWindow<RemotingSpectatorViewWindow>("Remoting Spectator View");
    }

    private void OnDestroy()
    {
        if (IsCameraRunning)
        {
            StopCamera();
        }
    }

    private void Awake()
    {
        SetPlayerPrefsToTarget();
    }

    private void Update()
    {
        if (IsCameraRunning)
            Repaint();
    }

    private void OnGUI()
    {
        var rowNumber = 0;

        if (GUI.Button(GetNextRect(rowNumber, 0f, RectPosition.Start, 1f), IsCameraRunning ? "Stop" : "Play"))
        {
            if (IsCameraRunning)
            {
                StopCamera();
            }
            else
            {
                StartCamera();
            }
        }

        rowNumber ++;

        EditorGUI.BeginDisabledGroup(Target.IsRunning);

        var fov = EditorGUI.FloatField(GetNextRect(rowNumber, 0, RectPosition.Start, 1), "Vertical FOV", Target.Camera.fieldOfView);

        if (Math.Abs(fov - Target.Camera.fieldOfView) > 0.001f)
        {
            EditorPrefs.SetFloat("FieldOfView", fov);
            Target.Camera.fieldOfView = fov;
        }

        rowNumber ++;

        var width  = EditorGUI.IntField(GetNextRect(rowNumber, 0, RectPosition.Start, .5f), "Width",  Target.Width);
        var height = EditorGUI.IntField(GetNextRect(rowNumber, .5f, RectPosition.End, .5f), "Height", Target.Height);

        if (width > 0)
        {
            EditorPrefs.SetInt("Width", width);
            Target.Width = width;
        }
        if (height > 0)
        {
            EditorPrefs.SetInt("Height", height);
            Target.Height = height;
        }

        rowNumber ++;
        EditorGUI.EndDisabledGroup();
        
        if (IsCameraRunning)
        {
            _imageRect = GetNextRect(rowNumber, 0f, RectPosition.Image, 0f);

            var widthAspect = RenderTexture.width / (float) RenderTexture.height;
            var rectAspect = _imageRect.width / _imageRect.height;

            if (rectAspect > widthAspect)
            {
                var aspectRatio = _imageRect.height / RenderTexture.height;
                _imageRect.width = RenderTexture.width * aspectRatio;
                _imageRect.x = (position.width - _imageRect.width) / 2f;
            }
            else
            {
                var aspectRatio = _imageRect.width / RenderTexture.width;
                _imageRect.height = RenderTexture.height * aspectRatio;
            }

            EditorGUI.DrawPreviewTexture(_imageRect, RenderTexture, null, ScaleMode.ScaleToFit);
        }
    }

    private static void StartCamera()
    {
        Target.StartCamera();
    }

    private static void StopCamera()
    {
        Target.StopCamera();
    }
}