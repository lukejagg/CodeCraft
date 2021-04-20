using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Programmer;

namespace Interpreter
{
    public class CodeUtil
    {
	    public static float SubWidth => 69;

        public static void Run(ICode code)
        {

        }

        public static Color ChangeValueColor(Color from, int depth)
        {
	        depth = depth + 1;
            Color.RGBToHSV(from, out var H, out var S, out var V);
            switch (depth % 4)
            {
                case 0:
					return Color.HSVToRGB(H, S, V + 0.0f);
                case 1:
	                return Color.HSVToRGB(H, S, V + 0.13f);
                case 2:
	                return Color.HSVToRGB(H, S, V - 0.0f);
                case 3:
	                return Color.HSVToRGB(H, S, V - 0.09f);
            }

            return Color.HSVToRGB(H, S, V);
        }

        public static Color ChangeFunctionColor(Color from, int depth)
        {
	        return from;
	        //Color.RGBToHSV(from, out var H, out var S, out var V);
	        //H = (H - depth * 0.017f) % 1;
	        //if (H < 0)
	        //    H++;
	        //return Color.HSVToRGB(H, S, V * Mathf.Pow(0.95f, depth));
        }

        public static Color ChangeControlColor(Color from, int depth)
        {
	        return from;
	        //Color.RGBToHSV(from, out var H, out var S, out var V);
	        //H = (H - depth * 0.02f) % 1;
	        //if (H < 0)
	        //    H++;
	        //return Color.HSVToRGB(H, S, V * Mathf.Pow(0.94f, depth));
        }
    }
}