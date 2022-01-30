using UnityEditor;
using UnityEngine;

public class HologramsViewerWindow : EditorWindow
{
    private static bool IsCameraRunning => Target.IsRunning;
    private static Texture RenderTexture => Target.RenderTexture;

    private const float Margin = 5f;
    private const float LineSpacing = 2.5f;
    private const float LineHeight = 18f;

    private Rect _imageRect;

    private static HologramsViewer _target;
    private static HologramsViewer Target
    {
        get
        {
            if (_target == null)
            {
                _target = FindObjectOfType<HologramsViewer>();
                if (_target == null)
                {
                    var prefab = Resources.Load("HologramsViewerRoot");
                    Instantiate(prefab);
                    _target = FindObjectOfType<HologramsViewer>();
                }
            }

            return _target;
        }
    }

    private Rect GetRect(int line, float xPositionRelative = 0, float widthRelative = 1, bool stretchLast = false)
    {
        var rect = new Rect();

        rect.x = Margin + xPositionRelative * (position.width - Margin * 2f);
        rect.y = Margin + line * (LineHeight + LineSpacing);
        
        rect.width = widthRelative * (position.width - Margin * 2f);
        rect.height = stretchLast ? position.height - rect.y - Margin : LineHeight;

        return rect;
    }

    private static void SetPlayerPrefsToTarget()
    {
        Target.Camera.focalLength = EditorPrefs.GetFloat("35mmFocalLength", 35);
        Target.Width  = EditorPrefs.GetInt("Width", 1280);
        Target.Height = EditorPrefs.GetInt("Height", 720);
    }

    [MenuItem("Holograms Viewer/Show window")]
    private static void ShowWindow()
    {
        GetWindow<HologramsViewerWindow>("Holograms Viewer");
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
        if (GUI.Button(GetRect(0), IsCameraRunning ? "Stop" : "Play"))
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

        EditorGUI.BeginDisabledGroup(Target.IsRunning);

        var focalLength = EditorGUI.FloatField(GetRect(1), "35mm focal length", Target.Camera.focalLength);
        var width = EditorGUI.IntField(GetRect(2, 0, .5f), "Width", Target.Width);
        var height = EditorGUI.IntField(GetRect(2, .5f, .5f), "Height", Target.Height);

        if (focalLength > 0.001f)
        {
            Target.Camera.focalLength = focalLength;
            EditorPrefs.SetFloat("35mmFocalLength", focalLength);
        }
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
        
        EditorGUI.EndDisabledGroup();
        
        if (IsCameraRunning)
        {
            _imageRect = GetRect(3, 0, 1, true);

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