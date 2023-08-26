using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using System;
using System.Windows;

namespace GameWindow.Factories
{
    public class UIElementFactory
    {
        private static readonly Guid InstantiationKey = new Guid("00000000-0000-0000-0000-000000000000");

        /// <summary>
        /// Builds and returns sprite UI element with an image
        /// </summary>
        public static Sprite CreateSprite(Transform transform, Image image)
        {
            return Application.Current.Dispatcher.Invoke(() => { return new Sprite(InstantiationKey, transform, image); });
        }

        /// <summary>
        /// Builds and returns button UI element
        /// </summary>
        public static CustomButton CreateButton(Transform transform, Action onClick, System.Windows.Media.Color color, string text)
        {
            return Application.Current.Dispatcher.Invoke(() => { return new CustomButton(InstantiationKey, transform, onClick, color, text); });
        }

        /// <summary>
        /// Builds and returns label UI element
        /// </summary>
        public static CustomLabel CreateLabel(Transform transform, string text, System.Windows.Media.Color TextColor)
        {
            return Application.Current.Dispatcher.Invoke(() => { return new CustomLabel(InstantiationKey, transform, text, TextColor); });
        }

        /// <summary>
        /// Builds and returns textbox UI element
        /// </summary>
        public static CustomTextBox CreateTextBox(Transform transform, string defaultText, Action textChanged)
        {
            return Application.Current.Dispatcher.Invoke(() => { return new CustomTextBox(InstantiationKey, transform, defaultText, textChanged); });
        }
    }
}