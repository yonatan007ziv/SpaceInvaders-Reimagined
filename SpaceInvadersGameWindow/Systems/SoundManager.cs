using System;
using System.IO;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace GameWindow.Systems
{
    internal static class SoundManager
    {
        [DllImport("winmm.dll", CharSet = CharSet.Unicode)]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        static Assembly assembly = Assembly.GetExecutingAssembly();
        public static void PlaySound(string soundName)
        {
            // Load the audio file into a MemoryStream
            using (Stream stream = assembly.GetManifestResourceStream($"GameWindow.Resources.Sounds.{soundName}.wav")!)
            {
                // Play the audio from the MemoryStream using SoundPlayer
                SoundPlayer soundPlayer = new SoundPlayer(stream);
                soundPlayer.Load();
                soundPlayer.Play();
            }
        }
    }
}