using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameWindow.Components.PhysicsEngine
{
    internal class Time
    {
        public static double deltaTime;
        private static double lastTime;
        public static void StartDeltaTime()
        {
            while (true)
            {
                long now = DateTime.Now.Ticks;
                double dT = (now - lastTime) / 1000000;
                lastTime = now;
                deltaTime = dT;
                //Thread.Sleep(1);
            }
        }
    }
}