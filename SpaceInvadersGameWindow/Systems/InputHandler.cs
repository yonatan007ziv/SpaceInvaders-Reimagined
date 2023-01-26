using SpaceInvadersGameWindow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpaceInvaders.Systems
{
    internal class InputHandler
    {
        public delegate void VoidDel();
        public static InputHandler? instance;
        public List<Key> keysDown = new List<Key>();

        public InputHandler()
        {
            instance = this;

            // Subscribe to Input methods:
            MainWindow.instance!.KeyDown += (object? sender, KeyEventArgs e) => KeyDown(e);
            MainWindow.instance!.KeyUp += (object? sender, KeyEventArgs e) => KeyUp(e);

            InputUpdate();
        }

        VoidDel? inputLoopDel;
        public void AddInputLoop(VoidDel del)
        {
            inputLoopDel += del;
        }
        public async void InputUpdate()
        {
            while (true)
            {
                inputLoopDel?.Invoke();
                Debug.WriteLine(keysDown.Contains(Key.A));
                await Task.Delay(1000/120);
            }
        }

        #region delegates
        VoidDel? keyUpDel;
        VoidDel? keyPressDel;

        public void AddKeyUpDel(VoidDel del)
        {
            keyUpDel += del;
        }
        public void AddKeyDownDel(VoidDel del)
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