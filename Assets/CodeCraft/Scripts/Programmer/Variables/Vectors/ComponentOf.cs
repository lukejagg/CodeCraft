using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using ObjectEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Programmer
{
	public enum VectorComponent
	{
		X = 1, 
		Y = 2, 
		Z = 3,
	}

	public class ComponentOf : IValue
	{
		private VectorComponent component;

		public bool ReadOnly => true;
		private IValue value = null;

		public ComponentOf(IValue value, VectorComponent component)
		{
			this.value = value;
			this.component = component;
		}

		public object GetValue(CodeExecutor obj)
		{
			var v1 = value?.GetValue(obj);
			if (v1 != null)
			{
				if (v1 is Vector3Int vi)
				{
					switch (component)
					{
						case VectorComponent.X:
							return new BigInteger(vi.x);
						case VectorComponent.Y:
							return new BigInteger(vi.y);
						case VectorComponent.Z:
							return new BigInteger(vi.z);
					}
				}

				if (v1 is Vector3 v)
				{
					switch (component)
					{
						case VectorComponent.X:
							return v.x;
						case VectorComponent.Y:
							return v.y;
						case VectorComponent.Z:
							return v.z;
					}
				}

				if (v1 is Quaternion q)
				{
					switch (component)
					{
						case VectorComponent.X:
							return q.x;
						case VectorComponent.Y:
							return q.y;
						case VectorComponent.Z:
							return q.z;
					}
				}
			}

			return 0;
		}
	}
}