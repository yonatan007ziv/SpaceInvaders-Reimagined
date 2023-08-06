using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace GameWindow.Systems
{
    /// <summary>
    /// Represents a sound from the classic arcade game Space Invaders (1978)
    /// </summary>
    public enum Sound
    {
        MenuClick,
        PlayerDeath,
        InvaderDeath,
        BulletInitiated,
        UFO,
        CycleBeat1,
        CycleBeat2
    }

    /// <summary>
    /// A class for playing and managing sounds
    /// </summary>
    public static class SoundManager
    {
        public static float currentVol = 1;
        private static readonly XAudio2 _xaudio = new XAudio2();
        private static readonly Dictionary<Sound, AudioBuffer> _audioBuffers = new Dictionary<Sound, AudioBuffer>();
        private static readonly Dictionary<Sound, List<SourceVoice>> currentSounds = new Dictionary<Sound, List<SourceVoice>>();

        /// <summary>
        /// Builds a sound manager device
        /// </summary>
        static SoundManager()
        {
            new MasteringVoice(_xaudio);
            for (int i = 0; i < Enum.GetNames(typeof(Sound)).Length; i++)
                LoadSound((Sound)i, $"pack://application:,,,/Resources/Sounds/{(Sound)i}.wav");
        }

        /// <summary>
        /// Set volume of all current and future sounds
        /// </summary>
        /// <param name="newVol"> The new volume </param>
        public static void SetVolume(float newVol)
        {
            currentVol = newVol;
            foreach (List<SourceVoice> ss in currentSounds.Values)
                foreach (SourceVoice s in ss)
                    s.SetVolume(newVol);
        }

        /// <summary>
        /// Plays a sound
        /// </summary>
        /// <param name="sound"> The <see cref="Sound"/> to play </param>
        /// <param name="speed"> The speed at which the sound is played at </param>
        public static void PlaySound(Sound sound, float speed = 1)
        {
            if (_audioBuffers.ContainsKey(sound))
            {
                var sourceVoice = new SourceVoice(_xaudio, new WaveFormat());

                sourceVoice.SetVolume(currentVol);
                sourceVoice.SetFrequencyRatio(speed);

                sourceVoice.SubmitSourceBuffer(_audioBuffers[sound], null);
                sourceVoice.Start();

                currentSounds[sound].Add(sourceVoice);
            }
        }

        /// <summary>
        /// Stop all sounds
        /// </summary>
        public static void StopAllSounds()
        {
            foreach (List<SourceVoice> ss in currentSounds.Values)
                foreach (SourceVoice s in ss)
                    s.Stop();
        }

        /// <summary>
        /// Stop a specific sound
        /// </summary>
        /// <param name="sound"> The <see cref="Sound"/> to stop </param>
        public static void StopSound(Sound sound)
        {
            foreach (SourceVoice c in currentSounds[sound])
                c.Stop();
        }

        /// <summary>
        /// Load a sound to <see cref="currentSounds"/>
        /// </summary>
        /// <param name="sound"> The <see cref="Sound"/>'s "identity" </param>
        /// <param name="uri"> Location of the sound </param>
        private static void LoadSound(Sound sound, string uri)
        {
            currentSounds[sound] = new List<SourceVoice>();

            var ms = new MemoryStream();
            using (var stream = Application.GetResourceStream(new Uri(uri)).Stream)
                stream.CopyTo(ms);

            var audioBuffer = new AudioBuffer();

            byte[] buffer = ms.ToArray();
            IntPtr intPtr = Marshal.AllocHGlobal(buffer.Length);
            Marshal.Copy(buffer, 0, intPtr, buffer.Length);

            audioBuffer.AudioDataPointer = intPtr;
            audioBuffer.AudioBytes = (int)ms.Length;
            audioBuffer.Flags = BufferFlags.EndOfStream;

            _audioBuffers[sound] = audioBuffer;
        }
    }
}