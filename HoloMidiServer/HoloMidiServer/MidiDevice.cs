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

        // 0
        public void NoteOn(byte channel, byte note, byte velocity)
        {
            var midiEvent = new NoteOnEvent
            {
                Channel = new FourBitNumber(channel),
                NoteNumber = new SevenBitNumber(note),
                Velocity = new SevenBitNumber(velocity)
            };

            _outputDevice.SendEvent(midiEvent);
        }

        // 1
        public void NoteOff(byte channel, byte note)
        {
            var midiEvent = new NoteOffEvent
            {
                Channel    = new  FourBitNumber(channel),
                NoteNumber = new SevenBitNumber(note),
                Velocity   = new SevenBitNumber(0)
            };
            
            _outputDevice.SendEvent(midiEvent);
        }

        // 2
        public void ControlChange(byte channel, byte controlNumber, byte controlValue)
        {
            var midiEvent = new ControlChangeEvent
            {
                Channel = new FourBitNumber(channel),
                ControlNumber = new SevenBitNumber(controlNumber),
                ControlValue  = new SevenBitNumber(controlValue)
            };

            _outputDevice.SendEvent(midiEvent);
        }

        // 3
        public void PitchBend(byte channel, ushort pitchValue)
        {
            var midiEvent = new PitchBendEvent
            {
                Channel = new FourBitNumber(channel),
                PitchValue = pitchValue
            };

            _outputDevice.SendEvent(midiEvent);
        }

        ~MidiDevice()
        {
            _outputDevice.Dispose();
        }
    }
}
