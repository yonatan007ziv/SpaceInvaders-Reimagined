using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GameWindow.Systems
{
    /// <summary>
    /// The base for all input loops
    /// </summary>
    public class InputHandler
    {
        private static bool Disabled = false;
        private static Action? inputLoopDel;
        public static readonly List<Key> keysDown = new List<Key>();

        /// <summary>
        /// Starts listening for key presses
        /// </summary>
        /// <param name="TargetInputWindow"> The targeted window </param>
        public InputHandler(Window TargetInputWindow)
        {
            // Subscribe to Input methods:
            TargetInputWindow.KeyDown += (s, e) => KeyDown(e);
            TargetInputWindow.KeyUp += (s, e) => KeyUp(e);
            TargetInputWindow.LostKeyboardFocus += (s, e) => { keysDown.Clear(); };

            InputUpdateLoop();
        }

        /// <summary>
        /// Set if the input is disabled
        /// </summary>
        /// <param name="disabled"> Whether input is disabled or not </param>
        public static void Disable(bool disabled)
        {
            InputHandler.Disabled = disabled;
        }

        /// <summary>
        /// The main input loop
        /// </summary>
        private static async void InputUpdateLoop()
        {
            while (true)
            {
                if (!Disabled)
                    inputLoopDel?.Invoke();

                await Task.Delay(250 / MainWindow.TARGET_FPS); // Less delay = A more responsive input
            }
        }

        /// <summary>
        /// Add input loop
        /// </summary>
        /// <param name="del"> The loop to add </param>
        public static void AddInputLoop(Action del)
        {
            inputLoopDel += del;
        }

        /// <summary>
        /// Remove input loop
        /// </summary>
        /// <param name="del"> The loop to remove </param>
        public static void RemoveInputLoop(Action del)
        {
            inputLoopDel -= del;
        }

        /// <summary>
        /// Occurs when a key is pressed
        /// </summary>
        /// <param name="e"> The key's data </param>
        public static void KeyDown(KeyEventArgs e)
        {
            if (!keysDown.Contains(e.Key))
                keysDown.Add(e.Key);
        }

        /// <summary>
        /// Occurs when a key is released
        /// </summary>
        /// <param name="e"> The key's data </param>
        public static void KeyUp(KeyEventArgs e)
        {
            keysDown.Remove(e.Key);
        }
        
        /// <summary>
        /// Get input on axis
        /// </summary>
        /// <param name="axis"> The axis to get </param>
        /// <returns></returns>
        /// <exception cref="Exception"> Thrown if no such axis exists </exception>
        public static int GetAxis(string axis)
        {
            if (axis == "Horizontal")
                return GetHorizontalAxis();
            else if (axis == "Vertical")
                return GetVerticalAxis();
            throw new Exception("Invalid Parameters");
        }

        /// <summary>
        /// Gets the "Horizontal" axis
        /// </summary>
        /// <returns> The "Horizontal" axis input value </returns>
        private static int GetHorizontalAxis()
        {
            int xAxis = 0;
            if (keysDown.Contains(Key.A))
                xAxis -= 1;
            if (keysDown.Contains(Key.D))
                xAxis += 1;
            return xAxis;
        }

        /// <summary>
        /// Gets the "Vertical" axis
        /// </summary>
        /// <returns> The "Vertical" axis input value </returns>
        private static int GetVerticalAxis()
        {
            int yAxis = 0;
            if (keysDown.Contains(Key.S))
                yAxis -= 1;
            if (keysDown.Contains(Key.W))
                yAxis += 1;
            return yAxis;
        }
    }
}