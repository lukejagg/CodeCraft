using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using ObjectEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Programmer
{
	public enum MathOperators : int
	{
		sin = 0,
		cos = 1,
		tan = 2,
		asin = 3,
		acos = 4,
		atan = 5,
		deg = 6,
		rad = 7,
		abs = 8,
		floor = 9,
		ceil = 10,
		round = 11,
		exp = 12,
		log = 13,
		tento = 14,
		log10 = 15,
		sign = 16,
		sqrt = 17,
		noise = 18,
	}

	// abs, ceil, floor, round, log, exp, 10^, log10, sqrt, sign

	public class MathOperator : IValue
	{
		public bool ReadOnly => false;

		private IValue value;
		private MathOperators mathOperator;

		public MathOperator(IValue value, MathOperators mathOperator)
		{
			this.value = value;
			this.mathOperator = mathOperator;
		}

		public float Do(float v)
		{
			switch (mathOperator)
			{
				case MathOperators.sin: return Mathf.Sin(v);
				case MathOperators.cos: return Mathf.Cos(v);
				case MathOperators.tan: return Mathf.Tan(v);
				case MathOperators.asin: return Mathf.Asin(v);
				case MathOperators.acos: return Mathf.Acos(v);
				case MathOperators.atan: return Mathf.Atan(v);

				case MathOperators.deg: return Mathf.Rad2Deg * v;
				case MathOperators.rad: return Mathf.Rad2Deg * v;

				case MathOperators.abs: return Mathf.Abs(v);

				case MathOperators.floor: return Mathf.FloorToInt(v);
				case MathOperators.ceil: return Mathf.CeilToInt(v);
				case MathOperators.round: return Mathf.RoundToInt(v);

				case MathOperators.exp: return Mathf.Exp(v);
				case MathOperators.log: return Mathf.Log(v);
				case MathOperators.tento: return Mathf.Pow(10, v);
				case MathOperators.log10: return Mathf.Log10(v);

				case MathOperators.sign: return Mathf.RoundToInt(Mathf.Sign(v));
				case MathOperators.sqrt: return Mathf.Sqrt(v);

				case MathOperators.noise: return Mathf.PerlinNoise(v, 0);
			}

			return 0;
		}

		public object GetValue(CodeExecutor obj)
		{
			var v1 = value?.GetValue(obj);
			if (v1 is BigInteger v)
			{
				switch (mathOperator)
				{
					case MathOperators.abs: return v > 0 ? v : -v;
					case MathOperators.floor: return v;
					case MathOperators.ceil: return v;
					case MathOperators.round: return v;

					case MathOperators.sign:
						if (v > 0) return 1;
						if (v < 0) return -1;
						return 0;
				}
			}
			else if (v1 is int vi)
			{
				switch (mathOperator)
				{
					case MathOperators.abs: return vi > 0 ? vi : -vi;
					case MathOperators.floor: return vi;
					case MathOperators.ceil: return vi;
					case MathOperators.round: return vi;

					case MathOperators.sign:
						if (vi > 0) return 1;
						if (vi < 0) return -1;
						return 0;
				}
			}
			else if (v1 is Vector3 vec)
			{
				return new Vector3(Do(vec.x), Do(vec.y), Do(vec.z));
			}
			else if (v1 is Vector3Int veci)
			{
				return new Vector3(Do(veci.x), Do(veci.y), Do(veci.z));
			}



			/*
			 * sin = 0,
			   cos = 1,
			   tan = 2,
			   asin = 3,
			   acos = 4,
			   atan = 5,
			   deg = 6,
			   rad = 7,
			   abs = 8,
			   floor = 9,
			   ceil = 10,
			   round = 11,
			   exp = 12,
			   log = 13,
			   tento = 14,
			   log10 = 15,
			   sign = 16,
			   sqrt = 17,
			   noise = 18,
			 */

			if (v1 is float f) return Do(f);
			if (v1 is int i) return Do(i);
			if (v1 is BigInteger b) return Do((float) b);
			return 0;
		}
	}
}