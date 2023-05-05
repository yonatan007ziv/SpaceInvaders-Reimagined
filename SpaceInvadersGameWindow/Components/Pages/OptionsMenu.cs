using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using GameWindow.Systems;
using System.Numerics;
using System.Windows;

namespace GameWindow.Components.Pages
{
    /// <summary>
    /// A page representing the options menu
    /// </summary>
    internal class OptionsMenu
    {
        private CustomLabel volumeLabel;
        private CustomTextBox volumeInput;
        private CustomButton backButton;

        /// <summary>
        /// Builds the options menu page
        /// </summary>
        public OptionsMenu()
        {
            Application.Current.Dispatcher.Invoke((System.Delegate)(() =>
            {
                volumeLabel = new CustomLabel(new Transform(new Vector2(100, 50), new Vector2(50, 50)), "Volume:", System.Windows.Media.Colors.White);
                volumeInput = new CustomTextBox(new Transform(new Vector2(50, 50), new Vector2(150, 50)), "" + (int)(SoundManager.currentVol * 100), () => UpdateVolume(volumeInput!.Text));
                backButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(50, 150)), Back, System.Windows.Media.Color.FromRgb(0, 255, 0), "Back");
            }));

            // Suppressing the "Null When Leaving a Constructor" warning
            volumeLabel!.ToString();
            volumeInput!.ToString();
            backButton!.ToString();
        }

        /// <summary>
        /// Updates the <see cref="SoundManager.currentVol"/>
        /// </summary>
        /// <param name="text"> The new volume </param>
        private static void UpdateVolume(string text)
        {
            if (int.TryParse(text, out int volume))
            {
                if (0 <= volume && volume <= 100)
                    SoundManager.SetVolume(volume / 100f);
                else
                    SoundManager.SetVolume(0);
            }
            else
                SoundManager.SetVolume(0);

            SoundManager.StopSound(Sound.UFO);
            SoundManager.PlaySound(Sound.UFO);
        }

        /// <summary>
        /// Goes back to the Main Menu page
        /// </summary>
        private void Back()
        {
            SoundManager.StopSound(Sound.UFO);
            Dispose();
            new GameMainMenu();
        }

        /// <summary>
        /// Disposes the current <see cref="OptionsMenu"/> page
        /// </summary>
        private void Dispose()
        {
            volumeLabel.Dispose();
            volumeInput.Dispose();
            backButton.Dispose();
        }
    }
}
