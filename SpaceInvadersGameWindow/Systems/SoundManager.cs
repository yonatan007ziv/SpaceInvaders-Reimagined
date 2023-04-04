using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Resources;

namespace GameWindow.Systems
{
    internal static class SoundManager
    {
        private static readonly List<WaveOutEvent> waveOutEvents = new List<WaveOutEvent>();
        private static float currentVol = 0.1f;
        public enum Sounds
        {
            MenuClick,
            PlayerDeath,
            InvaderDeath,
            BulletInitiated,
            BulletExplosion,
            UFO,
        }

        private static Dictionary<Sounds, Uri> sounds = new Dictionary<Sounds, Uri>();
        static SoundManager()
        {
            sounds[Sounds.MenuClick] = new Uri($"pack://application:,,,/Resources/Sounds/MenuClick.wav");
            sounds[Sounds.PlayerDeath] = new Uri($"pack://application:,,,/Resources/Sounds/PlayerDeath.wav");
            sounds[Sounds.InvaderDeath] = new Uri($"pack://application:,,,/Resources/Sounds/InvaderDeath.wav");
            sounds[Sounds.BulletInitiated] = new Uri($"pack://application:,,,/Resources/Sounds/BulletInitiated.wav");
            sounds[Sounds.BulletExplosion] = new Uri($"pack://application:,,,/Resources/Sounds/BulletExplosion.wav");
            sounds[Sounds.UFO] = new Uri($"pack://application:,,,/Resources/Sounds/UFO.wav");
        }
        public static void PlaySound(Sounds sound)
        {
            /*
            Uri uri = sounds[sound];
            string uriString = uri.ToString();
            StreamResourceInfo streamResourceInfo = Application.GetResourceStream(new Uri(uriString));

            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".wav");
            using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                streamResourceInfo.Stream.CopyTo(fileStream);

            AudioFileReader wave = new AudioFileReader(tempPath);
            WaveOutEvent outputDevice = new WaveOutEvent();
            outputDevice.Init(wave);
            outputDevice.Play();
            outputDevice.Volume = currentVol;
            outputDevice.PlaybackStopped += (sender, args) => waveOutEvents.Remove(outputDevice);

            waveOutEvents.Add(outputDevice);
            */
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