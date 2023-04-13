using GameWindow.Systems.Networking;
using System.Net.Sockets;

namespace GameWindow.Components.Miscellaneous
{
    public static class Delegates
    {
        public delegate void Action();
        public delegate void ActionString(string str);
        public delegate void ActionRegistValidator(RegisterValidator validator);
    }
}