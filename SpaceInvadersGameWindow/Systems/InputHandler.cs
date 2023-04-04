using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GameWindow.Systems
{
    public class InputHandler
    {
        public static List<Key> keysDown = new List<Key>();
        public static bool Disabled;

        public InputHandler(Window TargetInputWindow)
        {
            // Subscribe to Input methods:
            TargetInputWindow.KeyDown += (s, e) => KeyDown(e);
            TargetInputWindow.KeyUp += (s, e) => KeyUp(e);
            TargetInputWindow.LostKeyboardFocus += (s, e) => { keysDown.Clear(); };

            InputUpdateLoop();
        }
        private async void InputUpdateLoop()
        {
            while (true)
            {
                if (!Disabled)
                    inputLoopDel?.Invoke();

                await Task.Delay(1000 / (MainWindow.TARGET_FPS * 2));
            }
        }

        private static Action? inputLoopDel;
        public static void AddInputLoop(Action del)
        {
            inputLoopDel += del;
        }
        public static void RemoveInputLoop(Action del)
        {
            inputLoopDel -= del;
        }

        #region delegates
        private static Action? keyUpDel;
        private static Action? keyPressDel;

        public static void AddKeyUpDel(Action del)
        {
            keyUpDel += del;
        }
        public static void AddKeyDownDel(Action del)
        {
            keyPressDel += del;
        }
        #endregion
        
        public static void KeyDown(KeyEventArgs e)
        {
            if (!keysDown.Contains(e.Key))
                keysDown.Add(e.Key);
            keyPressDel?.Invoke();
        }
        public static void KeyUp(KeyEventArgs e)
        {
            keysDown.Remove(e.Key);
            keyUpDel?.Invoke();
        }
        public static int GetAxis(string axis)
        {
            if (axis == "Horizontal")
                return GetHorizontalAxis();
            else if (axis == "Vertical")
                return GetVerticalAxis();
            throw new Exception("Invalid Parameters");
        }
        private static int GetHorizontalAxis()
        {
            int xAxis = 0;
            if (keysDown.Contains(Key.A))
                xAxis -= 1;
            if (keysDown.Contains(Key.D))
                xAxis += 1;
            return xAxis;
        }
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