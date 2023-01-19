using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Text;

namespace SpaceInvaders.Components.PhysicsEngine
{
    internal class Time
    {
        public static double deltaTime;
        private static double lastTime;
        public static async void StartDeltaTime()
        {
            while (true)
            {
                long now = DateTime.Now.Ticks;
                double dT = (now - lastTime) / 1000000;
                lastTime = now;
                deltaTime = dT;
                await Task.Delay(1);
            }
        }
    }
}
