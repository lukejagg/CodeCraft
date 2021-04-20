using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using ObjectEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Programmer
{
	public enum EqualType : byte
	{
		Equals = 0,
		NotEquals = 1,
		Greater = 2,
		Less = 3,
		GreaterEqual = 4,
		LessEqual = 5,
	}

    public class Equal : IValue
    {
        public bool ReadOnly => true;

        IValue value1 = null;
		IValue value2 = null;

		private EqualType equalType;

        public Equal(IValue value1, IValue value2, EqualType equalType)
        {
            this.value1 = value1;
            this.value2 = value2;
            this.equalType = equalType;
        }

        public bool Equals(object v1, object v2)
        {
	        if (v1 is int i)
	        {
		        if (v2 is int j)
			        return i == j;
		        if (v2 is BigInteger bj)
			        return i == bj;
		        if (v2 is float f)
			        return i == f;
	        }
	        else if (v1 is float fi)
	        {
		        if (v2 is int j)
			        return fi == j;
		        if (v2 is BigInteger bj)
			        return fi == (float)bj;
		        if (v2 is float f)
			        return fi == f;
	        }
	        else if (v1 is BigInteger bi)
	        {
		        if (v2 is int j)
			        return bi == j;
		        if (v2 is BigInteger bj)
			        return bi == bj;
		        if (v2 is float f)
			        return (float)bi == f;
	        }
	        else if (v1 is Vector3 vec1)
	        {
		        if (v2 is Vector3 vec2)
			        return vec1 == vec2;
		        if (v2 is Vector3Int vec2i)
			        return vec1 == vec2i;
	        }
	        else if (v1 is Vector3Int vec1i)
	        {
		        if (v2 is Vector3 vec2)
			        return vec1i == vec2;
		        if (v2 is Vector3Int vec2i)
			        return vec1i == vec2i;
	        }
	        else if (v1 is Quaternion q)
	        {
		        if (v2 is Quaternion q2)
			        return q == q2;
	        }
	        // strings, tables
	        else if (v1 is string str)
	        {
		        if (v2 is string str2)
		        {
			        return str == str2;
		        }
	        }
	        else if (v1 is bool b)
	        {
		        if (v2 is bool b2)
		        {
			        return b == b2;
		        }
	        }
	        return v1 == v2;
		}

        public bool Greater(object v1, object v2)
        {
	        if (v1 is int i)
	        {
		        if (v2 is int j)
			        return i > j;
		        if (v2 is BigInteger bj)
			        return i > bj;
		        if (v2 is float f)
			        return i > f;
	        }
	        else if (v1 is float fi)
	        {
		        if (v2 is int j)
			        return fi > j;
		        if (v2 is BigInteger bj)
			        return fi > (float)bj;
		        if (v2 is float f)
			        return fi > f;
	        }
	        else if (v1 is BigInteger bi)
	        {
		        if (v2 is int j)
			        return bi > j;
		        if (v2 is BigInteger bj)
			        return bi > bj;
		        if (v2 is float f)
			        return (float)bi > f;
	        }
	        //else if (v1 is Vector3 vec1)
	        //{
		    //    if (v2 is Vector3 vec2)
			//        return vec1 > vec2;
		    //    if (v2 is Vector3Int vec2i)
			//        return vec1 > vec2i;
	        //}
	        //else if (v1 is Vector3Int vec1i)
	        //{
		    //    if (v2 is Vector3 vec2)
			//        return vec1i == vec2;
		    //    if (v2 is Vector3Int vec2i)
			//        return vec1i == vec2i;
	        //}
	        //else if (v1 is Quaternion q)
	        //{
		    //    if (v2 is Quaternion q2)
			//        return q == q2;
	        //}
	        // strings, tables
	        //else if (v1 is string str)
	        //{
		    //    if (v2 is string str2)
		    //    {
			//        return str == str2;
		    //    }
	        //}
	        //else if (v1 is bool b)
	        //{
		    //    if (v2 is bool b2)
		    //    {
			//        return b == b2;
		    //    }
	        //}
	        return false;
		}

        public bool Less(object v1, object v2)
        {
	        if (v1 is int i)
	        {
		        if (v2 is int j)
			        return i < j;
		        if (v2 is BigInteger bj)
			        return i < bj;
		        if (v2 is float f)
			        return i < f;
	        }
	        else if (v1 is float fi)
	        {
		        if (v2 is int j)
			        return fi < j;
		        if (v2 is BigInteger bj)
			        return fi < (float)bj;
		        if (v2 is float f)
			        return fi < f;
	        }
	        else if (v1 is BigInteger bi)
	        {
		        if (v2 is int j)
			        return bi < j;
		        if (v2 is BigInteger bj)
			        return bi < bj;
		        if (v2 is float f)
			        return (float)bi < f;
	        }
	        return false;
        }

		public object GetValue(CodeExecutor obj)
        {
	        var v1 = value1?.GetValue(obj);
			var v2 = value2?.GetValue(obj);

			switch (equalType)
			{
				case EqualType.Equals:
					return Equals(v1, v2);
				case EqualType.NotEquals:
					return !Equals(v1, v2);
				case EqualType.Greater:
					return Greater(v1, v2);
				case EqualType.Less:
					return Less(v1, v2);
				case EqualType.GreaterEqual:
					return Equals(v1, v2) || Greater(v1, v2);
				case EqualType.LessEqual:
					return Equals(v1, v2) || Less(v1, v2);
			}

			return false;
        }
    }
}