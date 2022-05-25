using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using NWaves.Audio;
using NWaves.Effects;
using NWaves.Signals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushersVC
{
    public class Recorder
    {
        #region Properties
        int SAMPLING_RATE_IN_HZ = 17000;
        ChannelIn CHANNEL_CONFIG = ChannelIn.Mono;
        Android.Media.Encoding AUDIO_FORMAT = Android.Media.Encoding.Pcm16bit;
        int BUFFER_SIZE;
        bool RecordingInProgress = false;
        private AudioRecord _recorder = null;
        public string _audioPath;
        public string _audioName;
        System.IO.Stream audioBuffer;
        AudioService audioService;
        #endregion


        public void Record()
        {
            //BUFFER_SIZE = AudioRecord.GetMinBufferSize(SAMPLING_RATE_IN_HZ,
            //CHANNEL_CONFIG, AUDIO_FORMAT) * BUFFER_SIZE_FACTOR;
            if (_recorder == null)
            {
                BUFFER_SIZE = 2;
                _recorder = new AudioRecord(AudioSource.Mic, SAMPLING_RATE_IN_HZ,
                    CHANNEL_CONFIG, AUDIO_FORMAT, BUFFER_SIZE);

                _recorder.StartRecording();
                RecordingInProgress = true;
                RecordingTask();
            }
        }
        public void Stop(Tuple<float, int, bool, bool,bool,bool> audioParams)
        {
            if (null == _recorder)
            {
                return;
            }

            RecordingInProgress = false;

            _recorder.Stop();

            _recorder.Release();

            _recorder = null;

            WaveFile waveFile = pcmTOwav();

            audioService = new AudioService();

            waveFile = audioService.PitchShift(audioParams.Item1 / 10, audioParams.Item2, waveFile.Signals[0]);
            if (audioParams.Item3)
            {
                waveFile = audioService.Robotize(waveFile.Signals[0]);
            }
            if (audioParams.Item4)
            {
                waveFile = audioService.Tremolo(waveFile.Signals[0]);
            }
            if (audioParams.Item5)
            {
                waveFile = audioService.Speed2X(_audioPath, waveFile.Signals[0].SamplingRate);
            }
            if (audioParams.Item6)
            {
                waveFile = audioService.SpeedHalfX(_audioPath, waveFile.Signals[0].SamplingRate);
            }
            using (var stream = new FileStream(_audioPath, FileMode.Create))
            {
                waveFile.SaveTo(stream);
            }
            _recorder = null;
        }
        public Task Play()
        {
            return Task.Run(() =>
            {
                if (_audioPath != null)
                {
                    MediaPlayer mediaPlayer = new MediaPlayer();
                    mediaPlayer.SetDataSource(_audioPath);
                    mediaPlayer.Prepare();
                    mediaPlayer.Start();
                }
            });
        }
        public Task RecordingTask()
        {
            audioBuffer = new MemoryStream();
            _audioName = DateTime.Now.ToShortDateString().Replace('/', '-') + "_" + DateTime.Now.ToLongTimeString().Replace(':', ' ') + ".wav";
            _audioPath = "/storage/emulated/0/Download/" + _audioName;
            return Task.Run(() =>
            {
                MemoryStream Buffer = new MemoryStream(BUFFER_SIZE);
                FileOutputStream outStream = new FileOutputStream(_audioPath);
                while (RecordingInProgress)
                {
                    int result = _recorder.Read(Buffer.GetBuffer(), 0, BUFFER_SIZE);
                    if (result < 0)
                    {
                        throw new Exception("Reading of audio buffer failed: ");
                    }
                    audioBuffer.Write(Buffer.GetBuffer(), 0, BUFFER_SIZE);
                    outStream.Write(Buffer.GetBuffer(), 0, BUFFER_SIZE);
                }
            });
        }
        public WaveFile pcmTOwav()
        {
            DiscreteSignal signal;
            using (FileStream streamIn = new FileStream(_audioPath, FileMode.Open))
            {
                var length = (int)streamIn.Length / 2;     // divide by 2, because each sample is represented with 2 bytes (Pcm16bit)

                signal = new DiscreteSignal(SAMPLING_RATE_IN_HZ, length);    // reserve space in the signal for number of samples: length

                using (var reader = new BinaryReader(streamIn))
                {
                    for (var i = 0; i < length; i++)    // just read samples
                    {
                        signal[i] = reader.ReadInt16() / 32768f;
                    }
                }
            }
            //var waveFile = new WaveFile(signal);
            //using (var stream = new FileStream(_audioPath, FileMode.Create))
            //{
            //    waveFile.SaveTo(stream);
            //}

            return new WaveFile(signal);
        }
        public unsafe void pcmTOwav2()
        {
            DiscreteSignal signal;

            var length = (int)audioBuffer.Length / 2;     // divide by 2, because each sample is represented with 2 bytes (Pcm16bit)

            signal = new DiscreteSignal(SAMPLING_RATE_IN_HZ, length);    // reserve space in the signal for number of samples: length

            using (var reader = new BinaryReader(audioBuffer))
            {
                for (var i = 0; i < length; i++)    // just read samples
                {
                    signal[i] = ((Int16)reader.Read()) / 32768f;
                }
            }
            var waveFile = new WaveFile(signal);
            using (var stream = new FileStream(_audioPath + "haji.wav", FileMode.Create))
            {
                waveFile.SaveTo(stream);
            }
        }
    }
}
