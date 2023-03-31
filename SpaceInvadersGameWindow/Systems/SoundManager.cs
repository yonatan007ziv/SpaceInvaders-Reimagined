using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace GameWindow.Systems
{
    internal static class SoundManager
    {
        private static readonly List<WaveOutEvent> waveOutEvents = new List<WaveOutEvent>();
        private static float currentVol = 0.1f;
        public enum Sounds
        {
            MenuClick = 0,
            PlayerDeath = 1,
            BulletInitiated = 2,
            BulletExplosion = 3,
            UFO = 4,
        }

        private static Uri[] sounds = new Uri[5];
        static SoundManager()
        {
            sounds[0] = new Uri($"pack://application:,,,/Resources/Sounds/MenuClick.wav");
            sounds[1] = new Uri($"pack://application:,,,/Resources/Sounds/PlayerDeath.wav");
            sounds[2] = new Uri($"pack://application:,,,/Resources/Sounds/BulletInitiated.wav");
            sounds[3] = new Uri($"pack://application:,,,/Resources/Sounds/BulletExplosion.wav");
            sounds[4] = new Uri($"pack://application:,,,/Resources/Sounds/UFO.wav");
        }
        public static void PlaySound(Sounds sound)
        {
            var uri = sounds[(int)sound];
            var uriString = uri.ToString();
            var streamResourceInfo = Application.GetResourceStream(new Uri(uriString));

            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".wav");
            using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                streamResourceInfo.Stream.CopyTo(fileStream);
            }

            var wave = new AudioFileReader(tempPath);
            var outputDevice = new WaveOutEvent();
            outputDevice.Init(wave);
            outputDevice.Play();
            outputDevice.Volume = currentVol;
            outputDevice.PlaybackStopped += (sender, args) => waveOutEvents.Remove(outputDevice);

            waveOutEvents.Add(outputDevice);
        }
        public static void ChangeVolume(float vol)
        {
            currentVol = vol;
            foreach (WaveOutEvent device in waveOutEvents)
                device.Volume = vol;
        }
        public static void StopAll()
        {
            foreach (var outputDevice in waveOutEvents)
            {
                outputDevice.Stop();
                outputDevice.Dispose();
            }

            waveOutEvents.Clear();
        }
    }
}