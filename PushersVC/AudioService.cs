using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NWaves;
using NWaves.Signals;
using NWaves.Audio;
using NWaves.Effects;
using System.IO;
using NWaves.Signals.Builders;
using NWaves.Utils;

namespace PushersVC
{
    public class AudioService
    {
        private WaveFile ApplyEffect(NWaves.Effects.Base.AudioEffect effect, DiscreteSignal signal)
        {
            var waveFile = new WaveFile(signal);

            var effected = effect.ApplyTo(waveFile.Signals[0], NWaves.Filters.Base.FilteringMethod.OverlapSave);

            waveFile.Signals[0] = effected; ;

            return waveFile;
        }

        public NWaves.Audio.WaveFile SpeedHalfX(string audioPath, int sampleRate)
        {
            DiscreteSignal signal;

            using (FileStream streamIn = new FileStream(audioPath, FileMode.Open))
            {
                var length = (int)streamIn.Length / 2;

                signal = new DiscreteSignal(sampleRate, length * 2);

                using (var reader = new BinaryReader(streamIn))
                {
                    int b = -1;

                    var lastWorkWith = reader.ReadInt16() / 32768f;
                    for (var i = 0; i < length * 2; i++)
                    {
                        if (b < (length * 2) - 3)
                        {
                            b++;
                            signal[b] = lastWorkWith;
                            b++;
                            var currentItem = reader.ReadInt16() / 32768f;
                            signal[b] = (lastWorkWith + currentItem) / 2;
                            lastWorkWith = currentItem;
                        }
                    }
                }
            }
            var pitchShift = new PitchShiftEffect(2,1024);
            return ApplyEffect(pitchShift, signal);
        }

        public NWaves.Audio.WaveFile Speed2X(string audioPath, int sampleRate)
        {
            DiscreteSignal signal;

            using (FileStream streamIn = new FileStream(audioPath, FileMode.Open))
            {
                var length = (int)streamIn.Length / 2;

                signal = new DiscreteSignal(sampleRate, length / 2);

                using (var reader = new BinaryReader(streamIn))
                {
                    //var lastWorkWith = reader.ReadInt16() / 32768f;
                    //var currentItem = reader.ReadInt16() / 32768f;


                    for (var i = 0; i < length / 2; i++)
                    {
                        //signal[i] = (lastWorkWith + currentItem) / 2;
                        //lastWorkWith = currentItem;
                        //currentItem = reader.ReadInt16() / 32768f;
                        //signal[i + 1] = (lastWorkWith + currentItem) / 2;
                        //lastWorkWith = currentItem;
                        //currentItem = reader.ReadInt16() / 32768f;
                        //signal[i + 2] = (lastWorkWith + currentItem) / 2;

                        signal[i] = ((reader.ReadInt16() / 32768f) + (reader.ReadInt16() / 32768f)) / 2;
                        //signal[i] = reader.ReadInt16() / 32768f;
                        //signal[i] = reader.ReadInt16() / 32768f;
                    }
                }
            }

            var pitchShift = new PitchShiftEffect(0.5, 1024);
            return ApplyEffect(pitchShift, signal);

            return new WaveFile(signal);
        }

        public WaveFile PitchShift(float shiftSize, int windowSize, DiscreteSignal signal)
        {
            var pitchShift = new PitchShiftEffect(shiftSize, windowSize);
            return ApplyEffect(pitchShift, signal);
        }

        public WaveFile Tremolo(DiscreteSignal signal)
        {
            var tremolo = new TremoloEffect(signal.SamplingRate, 2, 5, 0.9f);

            var modulator = new SineBuilder()
                                    .SetParameter("freq", 5)
                                    .SetParameter("min", 0)
                                    .SetParameter("max", 7)
                                    .SampledAt(signal.SamplingRate);

            tremolo = new TremoloEffect(modulator);

            return ApplyEffect(tremolo, signal);
        }

        public WaveFile Delay(DiscreteSignal signal)
        {
            var delay = new DelayEffect(signal.SamplingRate, 0.024f/*sec*/, 0.4f);
            return ApplyEffect(delay, signal);
        }

        public WaveFile Distortion(DiscreteSignal signal)
        {
            var dist = new DistortionEffect(DistortionMode.SoftClipping, 20, -12);
            return ApplyEffect(dist, signal);
        }

        public WaveFile Robotize(DiscreteSignal signal)
        {
            var robot = new RobotEffect(hopSize: 250, fftSize: 2048);
            var robotized = robot.ApplyTo(signal);

            var waveFile = new WaveFile(signal);

            waveFile.Signals[0] = robotized;

            return waveFile;
        }

        public WaveFile Whisperize(DiscreteSignal signal)
        {
            var whisper = new WhisperEffect(hopSize: 60, fftSize: 256);
            var whispered = whisper.ApplyTo(signal);

            var waveFile = new WaveFile(signal);

            waveFile.Signals[0] = whispered;

            return waveFile;
        }
    }
}