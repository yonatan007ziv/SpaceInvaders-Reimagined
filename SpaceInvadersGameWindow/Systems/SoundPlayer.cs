using GameWindow;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Media;

namespace GameWindow.Systems
{
    internal static class SoundManager
    {
        [DllImport("winmm.dll", CharSet = CharSet.Unicode)]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);
        static int MediaCount = 0;
        public static void PlaySound(string path)
        {
            MediaCount++;
            string strFilePath = $"{Environment.CurrentDirectory}/{path}";//"pack://application:,,,/" + path;
            //string strFilePath = @"D:\Code\VS Community\SpaceInvaders-Reimagined\SpaceInvadersGameWindow\Resources
            //\Sounds\Shoot.wav";
            string sCommand = "open \"" + strFilePath + $"\" type mpegvideo alias SOUND{MediaCount}";
            mciSendString(sCommand, null, 0, IntPtr.Zero);


            sCommand = $"play SOUND{MediaCount} notify";
            mciSendString(sCommand, null, 0, IntPtr.Zero);
        }
    }
}