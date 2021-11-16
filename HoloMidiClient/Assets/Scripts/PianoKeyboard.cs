using UnityEngine;

public class PianoKeyboard : MonoBehaviour
{
    public void Start()
    {
        short noteNumber = 24;
        foreach (Transform octave in transform)
        {
            foreach (Transform key in octave)
            {
                var noteScript = key.GetComponent<NoteMidiEvent>();
                noteScript.NoteNumber = noteNumber++;
            }
        }
    }

}
