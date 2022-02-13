using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class PianoChordTrigger : MonoBehaviour
{
    private Interactable[] _interactables = new Interactable[0];

    public void Init(Interactable[] interactables)
    {
        _interactables = interactables;
    }

    public void ChordOn()
    {
        foreach (var interactable in _interactables)
        {
            interactable.HasPress = true;
        }
    }

    public void ChordOff()
    {
        foreach (var interactable in _interactables)
        {
            interactable.HasPress = false;
        }
    }
}