using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class ControlMidiEvent : MonoBehaviour
{
    public short Channel = 0;
    public short ControlNumber = 0;
    
    public void OnControlSliderUpdate(SliderEventData sliderEventData)
    {
        var controlValue = (int) (sliderEventData.NewValue * 127);
        Connection.Instance.SendControlChange(Channel, ControlNumber, controlValue);
    }
}
