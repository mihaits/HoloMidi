using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class ChordMode : MonoBehaviour
{
    public GameObject PianoRoot;

    public GameObject ChordTriggerPrefab;
    public Vector3 NewChordTriggerOffset;
    public Theme SelectedNoteTheme;

    public List<Interactable> LastChord;

    private Interactable[] _interactables;
    private NoteMidiEvent[] _midiScripts;

    private GameObject _newChordTrigger;

    public void Start()
    {
        _midiScripts = PianoRoot.GetComponentsInChildren<NoteMidiEvent>();
        _interactables = _midiScripts.Select(c => c.GetComponent<Interactable>()).ToArray();
    }
    
    public void EnableChordMode()
    {
        // foreach (var midiScript in _midiScripts)
        //     midiScript.enabled = false;
        
        foreach (var interactable in _interactables)
        {
            interactable.ResetAllStates();
            interactable.NumOfDimensions = 2;
            interactable.OnClick.AddListener(() => OnNoteToggle(interactable));

            var profiles = interactable.Profiles;
            var shouldUpdateThemeProfiles = false;
            foreach (var profileItem in profiles)
            {
                if (profileItem.Themes.Count == 1)
                {
                    profileItem.Themes.Add(SelectedNoteTheme);
                    shouldUpdateThemeProfiles = true;
                }
            }

            if (shouldUpdateThemeProfiles)
            {
                interactable.Profiles = profiles;
            }
        }

        LastChord = new List<Interactable>();

        SetupNewChordTrigger();
    }
    
    public void DisableChordMode()
    {
        // foreach (var midiScript in _midiScripts)
        //     midiScript.enabled = true;

        foreach (var interactable in _interactables)
        {
            interactable.OnClick.RemoveAllListeners();
            interactable.NumOfDimensions = 1;
            interactable.ResetAllStates();
        }

        Destroy(_newChordTrigger);
    }

    private void OnNoteToggle(Interactable interactable)
    {
        if (interactable.IsToggled)
            LastChord.Add(interactable);
        else
            LastChord.Remove(interactable);
    }

    private void SetupNewChordTrigger()
    {
        _newChordTrigger = Instantiate(
            ChordTriggerPrefab,
            transform.position + transform.rotation * NewChordTriggerOffset, transform.rotation);
        var chordTriggerManipulator = _newChordTrigger.GetComponentInChildren<ObjectManipulator>();
        chordTriggerManipulator.OnManipulationStarted.AddListener(FinishChordTrigger);
    }

    public void FinishChordTrigger(ManipulationEventData _)
    {
        var chordTriggerScript = _newChordTrigger.GetComponent<PianoChordTrigger>();
        chordTriggerScript.Init(LastChord.ToArray());

        var chordTriggerManipulator = _newChordTrigger.GetComponentInChildren<ObjectManipulator>();
        chordTriggerManipulator.OnManipulationStarted.RemoveAllListeners();

        SetupNewChordTrigger();
    }
}
