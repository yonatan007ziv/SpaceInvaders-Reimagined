using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Media;
using SpaceInvaders.Resources;
using SpaceInvaders.Components.GameComponents;
using System.Numerics;
using System.Diagnostics;

namespace SpaceInvaders.Systems
{
    internal static class SoundManager
    {
        public static void PlaySound(UnmanagedMemoryStream sound)
        {
            new SoundPlayer(sound).PlaySync();
        }
    }
}
