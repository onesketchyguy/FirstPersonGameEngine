using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace JunoEngine.Systems
{
    public static class Debug
    {
        public static void RecieveFrameTime(double lastFrameTime)
        {
            frameRate = 1 / lastFrameTime;

            AddFrameRate(frameRate);

            if (lowestFrameRate == 0 || frameRate < lowestFrameRate)
                lowestFrameRate = frameRate;

            if (highestFrameRate == 0 || frameRate > highestFrameRate)
                highestFrameRate = frameRate;
        }

        private static void AddFrameRate(double frameRate)
        {
            frameRates.Add(frameRate);
            if (frameRates.Count > 10)
            {
                double val = 0;
                foreach (var item in frameRates)
                {
                    val += item;
                }

                val /= frameRates.Count;
                avgFrameRate = val;

                frameRates.Clear();
            }
        }

        private static HashSet<double> frameRates = new HashSet<double>();
        public static double avgFrameRate;
        public static double frameRate;
        public static double lowestFrameRate, highestFrameRate;

        public static bool Displaying;

        public static void Log(string debug)
        {
            Console.WriteLine(debug);
        }
    }
}