using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class HandRaysToggle : MonoBehaviour
{
    private bool _handRaysEnabled = true;

    public void ToggleHandRays()
    {
        _handRaysEnabled = !_handRaysEnabled;

        PointerUtils.SetHandRayPointerBehavior(
            _handRaysEnabled 
                ? PointerBehavior.AlwaysOn
                : PointerBehavior.AlwaysOff);
    }
}
 