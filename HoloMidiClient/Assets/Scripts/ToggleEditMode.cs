using System.Linq;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class ToggleEditMode : MonoBehaviour
{
    private GameObject[] _manipulationHandles;

    public void Start()
    {
        _manipulationHandles = 
            FindObjectsOfType<ObjectManipulator>()
            .Select(c => c.gameObject)
            .ToArray();
    }

    public void ToggleHandlers()
    {
        foreach (var manipulationHandle in _manipulationHandles)
            manipulationHandle.SetActive(!manipulationHandle.activeInHierarchy);
    }
}
