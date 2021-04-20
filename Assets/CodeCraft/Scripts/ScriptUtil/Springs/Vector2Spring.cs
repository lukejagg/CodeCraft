using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Springs
{
    public class Vector2Spring
    {
        const float EPSILON = 0.0001f;

        public Vector2 position, velocity, target;
        public float angularFrequency, dampingRatio;

        public Vector2Spring(Vector2 p0, Vector2 v0, Vector2 target, float angularFrequency = 10, float dampingRatio = 1)
        {
            position = p0;
            velocity = v0;
            this.target = target;
            this.angularFrequency = angularFrequency;
            this.dampingRatio = dampingRatio;
        }

        public void Update(float dt)
        {

            float aF = angularFrequency;
            float dR = dampingRatio;

            if (aF < EPSILON) return;
            if (dR < 0) dR = 0;

            var epos = target;
            var dpos = position - epos;
            var dvel = velocity;

            if (dR > 1 + EPSILON)
            {
                float za = -aF * dR;
                float zb = aF * Mathf.Sqrt(dR * dR - 1);
                float z1 = za - zb;
                float z2 = za + zb;
                float expTerm1 = Mathf.Exp(z1 * dt);
                float expTerm2 = Mathf.Exp(z2 * dt);

                var c1 = (dvel - dpos * z2) / (-2 * zb);
                var c2 = dpos - c1;
                position = epos + c1 * expTerm1 + c2 * expTerm2;
                velocity = c1 * z1 * expTerm1 + c2 * z2 * expTerm2;
            }
            else if (dR > 1 - EPSILON)
            {
                float expTerm = Mathf.Exp(-aF * dt);

                var c1 = dvel + aF * dpos;
                var c2 = dpos;
                var c3 = (c1 * dt + c2) * expTerm;

                position = epos + c3;
                velocity = (c1 * expTerm) - (c3 * aF);
            }
            else
            {
                float omegaZeta = aF * dR;
                float alpha = aF * Mathf.Sqrt(1 - dR * dR);
                float expTerm = Mathf.Exp(-omegaZeta * dt);
                float cosTerm = Mathf.Cos(alpha * dt);
                float sinTerm = Mathf.Sin(alpha * dt);

                var c1 = dpos;
                var c2 = (dvel + omegaZeta * dpos) / alpha;
                position = epos + expTerm * (c1 * cosTerm + c2 * sinTerm);
                velocity = -expTerm * ((c1 * omegaZeta - c2 * alpha) * cosTerm + (c1 * alpha + c2 * omegaZeta) * sinTerm);
            }
        }
    }
}