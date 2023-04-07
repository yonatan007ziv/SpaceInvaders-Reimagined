using SharpDX;
using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

public enum Sounds
{
    MenuClick,
    PlayerDeath,
    InvaderDeath,
    BulletInitiated,
    UFO
}
public static class SoundManager
{
    private static float currentVol = 1;
    private static readonly XAudio2 _xaudio= new XAudio2();
    private static readonly MasteringVoice _masterVoice = new MasteringVoice(_xaudio);
    private static readonly Dictionary<Sounds, AudioBuffer> _audioBuffers = new Dictionary<Sounds, AudioBuffer>();
    private static readonly Dictionary<Sounds, List<SourceVoice>> currentSounds = new Dictionary<Sounds, List<SourceVoice>>();

    static SoundManager()
    {
        LoadSound(Sounds.MenuClick, "pack://application:,,,/Resources/Sounds/MenuClick.wav");
        LoadSound(Sounds.PlayerDeath, "pack://application:,,,/Resources/Sounds/PlayerDeath.wav");
        LoadSound(Sounds.InvaderDeath, "pack://application:,,,/Resources/Sounds/InvaderDeath.wav");
        LoadSound(Sounds.BulletInitiated, "pack://application:,,,/Resources/Sounds/BulletInitiated.wav");
        LoadSound(Sounds.UFO, "pack://application:,,,/Resources/Sounds/UFO.wav");
    }

    public static void SetVolume(float vol)
    {
        currentVol = vol;
        foreach (List<SourceVoice> ss in currentSounds.Values)
            foreach (SourceVoice s in ss)
                s.SetVolume(vol);
    }
    public static void PlaySound(Sounds sound)
    {
        if (_audioBuffers.ContainsKey(sound))
        {
            var sourceVoice = new SourceVoice(_xaudio, new WaveFormat());
            sourceVoice.SubmitSourceBuffer(_audioBuffers[sound], null);
            sourceVoice.SetVolume(currentVol);
            sourceVoice.Start();

            if (!currentSounds.ContainsKey(sound))
                currentSounds[sound] = new List<SourceVoice>();
            currentSounds[sound].Add(sourceVoice);
        }
    }
    public static void StopAllSounds()
    {
        foreach (List<SourceVoice> ss in currentSounds.Values)
            foreach (SourceVoice s in ss)
                s.Stop();
    }
    public static void StopSound(Sounds sound)
    {
        foreach (SourceVoice c in currentSounds[sound])
            c.Stop();
    }
    private static void LoadSound(Sounds sound, string uri)
    {
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
