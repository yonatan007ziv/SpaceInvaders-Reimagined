using GameWindow;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GameWindow.Systems
{
    public class InputHandler
    {
        public List<Key> keysDown = new List<Key>();

        public InputHandler(Window TargetInputWindow)
        {
            // Subscribe to Input methods:
            TargetInputWindow.KeyDown += (s, e) => KeyDown(e);
            TargetInputWindow.KeyUp += (s, e) => KeyUp(e);
            TargetInputWindow.LostKeyboardFocus += (s, e) => { keysDown.Clear(); };

            InputUpdateLoop();
        }

        Action? inputLoopDel;
        public void AddInputLoop(Action del)
        {
            inputLoopDel += del;
        }
        public void RemoveInputLoop(Action del)
        {
            inputLoopDel -= del;
        }
        public async void InputUpdateLoop()
        {
            while (true)
            {
                inputLoopDel?.Invoke();
                await Task.Delay(1000 / (MainWindow.TARGET_FPS * 2));
            }
        }

        #region delegates
        Action? keyUpDel;
        Action? keyPressDel;

        public void AddKeyUpDel(Action del)
        {
            keyUpDel += del;
        }
        public void AddKeyDownDel(Action del)
        {
            keyPressDel += del;
        }
        #endregion
        
        public void KeyDown(KeyEventArgs e)
        {
            if (!keysDown.Contains(e.Key))
                keysDown.Add(e.Key);
            keyPressDel?.Invoke();
        }
        public void KeyUp(KeyEventArgs e)
        {
            keysDown.Remove(e.Key);
            keyUpDel?.Invoke();
        }
        public int GetAxis(string axis)
        {
            if (axis == "Horizontal")
                return GetHorizontalAxis();
            else if (axis == "Vertical")
                return GetVerticalAxis();
            throw new Exception("Invalid Parameters");
        }
        private int GetHorizontalAxis()
        {
            int xAxis = 0;
            if (keysDown.Contains(Key.A))
                xAxis -= 1;
            if (keysDown.Contains(Key.D))
                xAxis += 1;
            return xAxis;
        }
        private int GetVerticalAxis()
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