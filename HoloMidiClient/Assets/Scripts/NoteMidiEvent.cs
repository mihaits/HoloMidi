using UnityEngine;

public class NoteMidiEvent : MonoBehaviour
{
    public short Channel = 0;
    public short NoteNumber = 69;
    public short Velocity = 127;
    
    public void NoteOn()
    {
        Connection.Instance.SendNoteOn(Channel, NoteNumber, Velocity);
    }

    public void NoteOff()
    {
        Connection.Instance.SendNoteOff(Channel, NoteNumber);
    }
}
