using UnityEngine;

public class PianoKeyboard : MonoBehaviour
{
    public Transform[] Octaves;

    public void Start()
    {
        short noteNumber = 24;
        foreach (var octave in Octaves)
        {
            foreach (Transform key in octave)
            {
                var noteScript = key.GetComponent<NoteMidiEvent>();
                noteScript.NoteNumber = noteNumber ++;
            }
        }
    }
}
