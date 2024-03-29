﻿using GameWindow.Components.Miscellaneous;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GameWindow.Components.UIElements
{
    /// <summary>
    /// A custom <see cref="TextBox"/> implementation
    /// </summary>
    public partial class CustomTextBox : TextBox
    {
        public Transform transform;
        public bool Censor = false;

        /// <summary>
        /// Builds a text box UI element
        /// </summary>
        /// <param name="transform"> The <see cref="Transform"/> to link </param>
        /// <param name="defaultText"> The default displayed text </param>
        /// <param name="textChanged"> An <see cref="Action"/> that will be called when the text changes </param>
        public CustomTextBox(Guid key, Transform transform, string defaultText, Action textChanged) // Called within an STA thread
        {
            InitializeComponent();

            this.transform = transform;
            transform.PositionChanged += SetPosition;
            transform.ScaleChanged += SetScale;

            Text = defaultText;
            TextChanged += (s, e) => textChanged();

            MainWindow.instance!.CenteredCanvas.Children.Add(this);
        }

        /// <summary>
        /// Sets the position according to the linked transform
        /// </summary>
        public void SetPosition()
        {
            Dispatcher.Invoke(() =>
            { // UI Objects need to be changed in an STA thread
                SetValue(Canvas.LeftProperty, (double)transform.CenteredPosition.X);
                SetValue(Canvas.TopProperty, (double)transform.CenteredPosition.Y);
            });
        }

        /// <summary>
        /// Sets the scale according to the linked transform
        /// </summary>
        public void SetScale()
        {
            Dispatcher.Invoke(() =>
            { // UI Objects need to be changed in an STA thread
                Width = transform.ActualScale.X;
                Height = transform.ActualScale.Y;
            });
        }

        /// <summary>
        /// Sets the visibility
        /// </summary>
        /// <param name="visible"> Whether it should be visible or not </param>
        public void Visible(bool visible)
        {
            // UI Objects need to be changed in an STA thread
            Dispatcher.Invoke(() => Visibility = visible ? Visibility.Visible : Visibility.Hidden);
        }

        /// <summary>
        /// Only allow A-Z, a-z, 0-9, @, and a dot (.) in the text box
        /// </summary>
        /// <param name="sender"> The sender TextBox </param>
        /// <param name="e"> The event parameters </param>
        private void BoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Check if the input is a valid character
            if (!Regex.IsMatch(e.Text, "[a-z0-9A-Z@.]"))
            {
                e.Handled = true; // Ignore the input
            }
        }

        /// <summary>
        /// Disposes the <see cref="CustomTextBox"/>
        /// </summary>
        public void Dispose()
        {
            // UI Objects need to be changed in an STA thread
            Dispatcher.Invoke(() => MainWindow.instance!.CenteredCanvas.Children.Remove(this));
        }
    }
}