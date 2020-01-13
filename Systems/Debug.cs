using System;

namespace JunoEngine.Systems
{
    public static class Debug
    {
        public static bool Displaying;

        public static void Log(string debug)
        {
            Console.WriteLine(debug);
        }
    }
}