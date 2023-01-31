using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SpaceInvaders.Systems
{
    internal static class SoundManager
    {
        //[DllImport("winmm.dll")]
        //private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        public static void PlaySound(string path)
        {
            //mciSendString(String.Format(@"open ""{0}"" type mpegvideo alias MediaFile", path), null, 0, IntPtr.Zero);
            // fix later on, 1 sound at once currently 
            Task.Factory.StartNew(() => {
                MediaPlayer player = new MediaPlayer();
                player.Open(new Uri("pack://application:,,,/" + path));
                player.Play();
            });
            MediaPlayer player = new MediaPlayer();
            player.Open(new Uri("pack://application:,,,/" + path));
            player.Play();
        }
        public static void PlaySound(string path, Action del)
        {
            //mciSendString(String.Format(@"open ""{0}"" type mpegvideo alias MediaFile", path), null, 0, IntPtr.Zero);
            // fix later on, 1 sound at once currently 
            Task.Factory.StartNew(() => {
                MediaPlayer player = new MediaPlayer();
                player.Open(new Uri("pack://application:,,,/" + path));
                player.MediaEnded += (s,e) => del();
                player.Play();
            });
            MediaPlayer player = new MediaPlayer();
            player.Open(new Uri("pack://application:,,,/" + path));
            player.MediaEnded += (s, e) => del();
            player.Play();
        }
    }
}