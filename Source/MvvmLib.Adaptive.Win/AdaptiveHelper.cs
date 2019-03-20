using System.Collections.Generic;

namespace MvvmLib.Adaptive
{

    public class AdaptiveHelper
    {
        public static double GuessActiveWidth(double width, List<double> breakpoints)
        {
            double lastWidth = -1;
            foreach (var breakpoint in breakpoints)
            {
                if (breakpoint < width)
                {
                    lastWidth = breakpoint;
                }
                else
                {
                    return lastWidth;
                }
            }
            return lastWidth;
        }

    }

}
