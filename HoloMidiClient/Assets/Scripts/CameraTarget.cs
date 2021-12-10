using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public Transform CameraTransform;

    public void Update()
    {
        var dir = transform.position - CameraTransform.position;
        var rotation = Quaternion.LookRotation(dir);
        CameraTransform.rotation = rotation;
    }
}
