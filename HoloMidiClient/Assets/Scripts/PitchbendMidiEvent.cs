using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class PitchbendMidiEvent : MonoBehaviour
{
    public short Channel = 0;

    public void OnPitchBendSliderUpdate(SliderEventData sliderEventData)
    {
        var pitchValue = (ushort) (sliderEventData.NewValue * 16383);
        Connection.Instance.SendPitchBend(Channel, pitchValue);
    }
}
