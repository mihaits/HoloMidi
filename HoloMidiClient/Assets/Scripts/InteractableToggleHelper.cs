using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class InteractableToggleHelper : MonoBehaviour
{
    public Interactable[] Toggles;

    public void DeselectInteractableToggles()
    {
        foreach (var interactable in Toggles)
        {
            if (interactable.IsToggled)
                interactable.TriggerOnClick();
        }
    }
}
