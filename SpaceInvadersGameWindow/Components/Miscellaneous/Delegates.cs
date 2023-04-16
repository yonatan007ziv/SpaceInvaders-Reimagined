using System;

namespace GameWindow.Components.Miscellaneous
{
    /// <summary>
    /// Helper class implementing custom delegates and actions
    /// </summary>
    public static class DelegatesActions
    {
        public delegate void ActionString(string str);
        public static readonly Action EmptyAction = () => { };
    }
}