using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectEditor
{
    public class Arrow
    {
        // x rate, y rate, constant, sqrt(x^2 + y^2)
        Vector2 origin;
        float a, b, c, d;
        public Vector2 vec;

        public Arrow()
        {
            a = 0;
            b = 0;
            c = 0;
            d = 1;
            vec = Vector2.zero;
        }

        public void Update(Vector2 p0, Vector2 p1)
        {
            origin = p0;
            var dp = p1 - p0;
            a = -dp.y;
            b = dp.x;
            c = -a * p0.x - b * p0.y;
            d = dp.magnitude;

            vec.x = b;
            vec.y = -a;
            vec.Normalize();
        }

        public void Update(Vector4 p0)
        {
            Update(new Vector2(p0.x, p0.y), new Vector2(p0.z, p0.w));
        }

        public float GetDistance(Vector2 p)
        {
            if (d == 0)
            {
                return 1000000;
            }
            return Mathf.Abs(a * p.x + b * p.y + c) / d;
        }

        public bool AboveLine(Vector2 point)
        {
            var offset = point - origin;
            return (-offset.x * b + offset.y * a) < 0;
        }
    }
}