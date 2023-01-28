using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SpaceInvaders.Systems
{
    internal static class SoundManager
    {
        public static void PlaySound(string path)
        {
            // fix later on, 1 sound at once currently 
            Task.Factory.StartNew(() => {
                MediaPlayer player = new MediaPlayer();
                player.Open(new Uri("pack://application:,,,/" + path));
                player.Play();
            });
        }
        public static void PlaySound(string path, Action del)
        {
            // fix later on, 1 sound at once currently 
            Task.Factory.StartNew(() => {
                MediaPlayer player = new MediaPlayer();
                player.Open(new Uri("pack://application:,,,/" + path));
                player.MediaEnded += (s,e) => del();
                player.MediaFailed += (o, args) =>
                {
                    MessageBox.Show("Media Failed!!");
                };
                player.Play();
            });
        }
    }
}