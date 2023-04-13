using GameWindow.Components.Initializers;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using System.Numerics;
using System.Windows;

namespace GameWindow.Components.Pages
{
    /// <summary>
    /// A page representing the options menu
    /// </summary>
    internal class OptionsMenu
    {
        CustomLabel volumeLabel;
        CustomTextInput volumeInput;
        CustomButton backButton;

        public OptionsMenu()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                volumeLabel = new CustomLabel(new Transform(new Vector2(100, 50), new Vector2(50, 50)), "Volume:", System.Windows.Media.Colors.White);
                volumeInput = new CustomTextInput(new Transform(new Vector2(50, 50), new Vector2(150, 50)), "" + (int)(SoundManager.currentVol * 100), () => UpdateVolume(volumeInput!.box.Text));
                backButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(50, 150)), Back, "", "Back");
            });

            // Suppressing the "Null When Leaving a Constructor" warning
            volumeLabel!.ToString();
            volumeInput!.ToString();
            backButton!.ToString();
        }

        public void UpdateVolume(string text)
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

        public void Back()
        {
            SoundManager.StopSound(Sound.UFO);
            Dispose();
            GameInitializers.StartGameMenu(GameInitializers.username);
        }

        public void Dispose()
        {
            volumeLabel.Dispose();
            volumeInput.Dispose();
            backButton.Dispose();
        }
    }
}
