using UnityEngine;

public class NoteMidiEvent : MonoBehaviour
{
    public short Channel = 0;
    public short NoteNumber = 69;
    public short Velocity = 127;

    private bool _isNoteOn;

    public void OnDisable()
    {
        if (_isNoteOn)
            NoteOff();
    }

    public void NoteOn()
    {
        if (enabled)
        {
            Connection.Instance.SendNoteOn(Channel, NoteNumber, Velocity);
            _isNoteOn = true;
        }
    }

    public void NoteOff()
    {
        if (enabled)
        {
            Connection.Instance.SendNoteOff(Channel, NoteNumber);
            _isNoteOn = false;
        }
    }
}
