using System;

namespace GraphGame.Logic
{
    public enum Color
    {
        None,
        Red,
        Green,
        Blue,
        Max,
    }

    public static class Common
    {
        public static void SafeInvoke(this Action action)
        {
            if (action == null)
                return;

            try
            {
                action.Invoke();
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.Assert(false, err.Message);
            }
        }
    }
}