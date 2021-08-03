using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class ControlEventRouter : MonoBehaviour
{
    public Connection Connection;

    public Interactable NoteButtonInteractable;
    public PinchSlider ControlSlider;
    public PinchSlider PitchBendSlider;

    public void Start()
    {
        var onPressReceiver = NoteButtonInteractable.AddReceiver<InteractableOnPressReceiver>();

        onPressReceiver.OnPress.AddListener(NoteOnHandler);
        onPressReceiver.OnRelease.AddListener(NoteOffHandler);

        ControlSlider.OnValueUpdated.AddListener(ControlChangeSliderHandler);

        PitchBendSlider.OnValueUpdated.AddListener(PitchBendSliderHandler);
        PitchBendSlider.OnInteractionEnded.AddListener(data =>
        {
            data.Slider.SliderValue = .5f;
        });
    }

    public void NoteOnHandler()
    {
        Connection.SendNoteOn(0, 64, 64);
    }

    public void NoteOffHandler()
    {
        Connection.SendNoteOff(0, 64);
    }

    public void ControlChangeSliderHandler(SliderEventData sliderEventData)
    {
        var controlValue = (int)(sliderEventData.NewValue * 127);
        Connection.SendControlChange(0, 0, controlValue);
    }

    public void PitchBendSliderHandler(SliderEventData sliderEventData)
    {
        var pitchValue = (ushort) (sliderEventData.NewValue * 16383);
        Connection.SendPitchBend(0, pitchValue);
    }
}
