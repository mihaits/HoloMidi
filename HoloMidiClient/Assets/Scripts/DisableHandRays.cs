using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class DisableHandRays : MonoBehaviour
{
    public void Start()
    {
        PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff);
    }
}
