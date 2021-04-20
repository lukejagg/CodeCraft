using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Interpreter
{
    public static class Colors
    {
        public static Color EventColor = new Color(0.21f, 0.295f, 0.65f);
        public static Color FunctionColor = new Color(1, 0.43f, 0.43f);
        public static Color ControlColor = Color.HSVToRGB(30f / 360, 0.80f, 1f);
        public static Color ValueColor = new Color(0.31f, 0.59f, 0.8f);
        public static Color PrimitiveColor;

        public static Color ConditionalColor = Color.HSVToRGB(230f / 360, 0.7f, 0.8f);

        public static Color InputColor = Color.HSVToRGB(220f / 360, 0.75f, 0.8f);

        public static Color ListColor = Color.HSVToRGB(15f / 360, 0.85f, 1f);
        public static Color VariableColor = Color.HSVToRGB(24f / 360, 0.85f, 1f);
        public static Color VariableValueColor = Color.HSVToRGB(10f / 360, 0.85f, 1f);
        public static Color OperatorColor = Color.HSVToRGB(115f / 360, 0.7f, 0.8f);
        public static Color GeneralColor = Color.HSVToRGB(200f / 360, 0.7f, 0.8f);
        public static Color MoveColor = Color.HSVToRGB(220f / 360, 0.7f, 0.8f);
        public static Color NotColor = Color.HSVToRGB(130f / 360, 0.7f, 0.5f);

        public static Color RotationColor = Color.HSVToRGB(20f / 360, 0.85f, 1f);
        public static Color ScaleColor = Color.HSVToRGB(30f / 360, 0.85f, 1f);

        public static Color ColorColor = Color.HSVToRGB(115f / 360, 0.65f, 0.65f);

        public static Color GeneralPhysics = Color.HSVToRGB(250f / 360, 0.7f, .8f);
    }
}