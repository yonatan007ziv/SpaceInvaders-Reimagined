using System.Diagnostics;

namespace SpaceInvaders.Systems
{
    internal class InputHandler
    {
        public delegate void VoidDel();
        public static InputHandler? instance;
        public List<Keys> keysDown = new List<Keys>();

        public InputHandler()
        {
            instance = this;

            // Subscribe to Input methods:
            GameWindow.Instance!.KeyDown += (object? sender, KeyEventArgs e) => KeyDown(sender,e);
            GameWindow.Instance!.KeyUp += (object? sender, KeyEventArgs e) => KeyUp(sender,e);
        }

        VoidDel? inputLoopDel;
        public void AddInputLoop(VoidDel del)
        {
            inputLoopDel += del;
        }
        public void InputUpdate()
        {
            inputLoopDel?.Invoke();
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

        public void KeyDown(object? sender, KeyEventArgs e)
        {
            if (!keysDown.Contains(e.KeyData))
                keysDown.Add(e.KeyData);
            keyPressDel?.Invoke();
        }
        public void KeyUp(object? sender, KeyEventArgs e)
        {
            keysDown.Remove(e.KeyData);
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
            if (keysDown.Contains(Keys.A))
                xAxis -= 1;
            if (keysDown.Contains(Keys.D))
                xAxis += 1;
            return xAxis;
        }
        private int GetVerticalAxis()
        {
            int yAxis = 0;
            if (keysDown.Contains(Keys.S))
                yAxis -= 1;
            if (keysDown.Contains(Keys.W))
                yAxis += 1;
            return yAxis;
        }
    }
}