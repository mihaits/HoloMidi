using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;

namespace HoloMidiServer
{
    public class MidiDevice
    {
        private readonly OutputDevice _outputDevice;

        public MidiDevice()
        {
            _outputDevice = OutputDevice.GetByName("loopMIDI Port");
        }

        public void NoteOn(byte note, byte velocity)
        {
            var n = new SevenBitNumber(note);
            var v = new SevenBitNumber(velocity);

            _outputDevice.SendEvent(new NoteOnEvent(n, v));
        }

        public void NoteOff(byte note)
        {
            var n = new SevenBitNumber(note);
            var v = new SevenBitNumber(0);

            _outputDevice.SendEvent(new NoteOffEvent(n, v));
        }

        public void ControlChange(byte controlNumber, byte controlValue)
        {
            var n = new SevenBitNumber(controlNumber);
            var v = new SevenBitNumber(controlValue);

            _outputDevice.SendEvent(new ControlChangeEvent(n, v));
        }

        ~MidiDevice()
        {
            _outputDevice.Dispose();
        }
    }
}
