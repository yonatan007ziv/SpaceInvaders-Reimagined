using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

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
public static class SoundManager
{
    public static float currentVol = 1;
    private static readonly XAudio2 _xaudio= new XAudio2();
    private static readonly Dictionary<Sound, AudioBuffer> _audioBuffers = new Dictionary<Sound, AudioBuffer>();
    private static readonly Dictionary<Sound, List<SourceVoice>> currentSounds = new Dictionary<Sound, List<SourceVoice>>();

    static SoundManager()
    {
        new MasteringVoice(_xaudio);
        LoadSound(Sound.MenuClick, "pack://application:,,,/Resources/Sounds/MenuClick.wav");
        LoadSound(Sound.PlayerDeath, "pack://application:,,,/Resources/Sounds/PlayerDeath.wav");
        LoadSound(Sound.InvaderDeath, "pack://application:,,,/Resources/Sounds/InvaderDeath.wav");
        LoadSound(Sound.BulletInitiated, "pack://application:,,,/Resources/Sounds/BulletInitiated.wav");
        LoadSound(Sound.UFO, "pack://application:,,,/Resources/Sounds/UFO.wav");
        LoadSound(Sound.CycleBeat1, "pack://application:,,,/Resources/Sounds/CycleBeat1.wav");
        LoadSound(Sound.CycleBeat2, "pack://application:,,,/Resources/Sounds/CycleBeat2.wav");
    }

    public static void SetVolume(float vol)
    {
        currentVol = vol;
        foreach (List<SourceVoice> ss in currentSounds.Values)
            foreach (SourceVoice s in ss)
                s.SetVolume(vol);
    }
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
    public static void StopAllSounds()
    {
        foreach (List<SourceVoice> ss in currentSounds.Values)
            foreach (SourceVoice s in ss)
                s.Stop();
    }
    public static void StopSound(Sound sound)
    {
        foreach (SourceVoice c in currentSounds[sound])
            c.Stop();
    }
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
