using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Numerics;
using System.Text;

public class Table
{
	public List<object> List { get; }
	public Dictionary<object, object> Hash { get; }

	public Table()
	{
		List = new List<object>();
		Hash = new Dictionary<object, object>();
	}

	public int Length => List.Count;

	public object this[object index]
	{
		get
		{
			if (index == null) return null;

			if (index is float f) index = Mathf.RoundToInt(f);
			if (index is BigInteger b) index = (int)b;

			if (index is int i)
			{
				if (i < List.Count && i >= 0)
					return List[i];
			}

			if (Hash.TryGetValue(index, out var obj))
			{
				return obj;
			}

			return null;
		}
		set
		{
			if (index == null) return;

			if (index is float f) index = Mathf.RoundToInt(f);
			if (index is BigInteger b) index = (int)b;

			if (index is int i)
			{
				// if it's at the end, then add it
				if (i == List.Count)
				{
					List.Add(value);

					var n = List.Count;
					while (Hash.TryGetValue(n, out var val))
					{
						Hash.Remove(n);
						List.Add(val);
						n++;
					}

					return;
				}
				else if (i < List.Count && i >= 0)
				{
					List[i] = value;
					return;
				}
			}

			if (Hash.ContainsKey(index))
			{
				Hash[index] = value;
			}
			else
			{
				Hash.Add(index, value);
			}
		}
	}

	public void Add(object obj) => this[Length] = obj;

	public bool RemoveAt(object index)
	{
		if (index == null) return false;

		if (List.Count > 0)
		{
			if (index is float f) index = Mathf.RoundToInt(f);
			if (index is BigInteger b) index = (int) b;

			if (index is int i)
			{
				if (i < List.Count && i >= 0)
				{
					List.RemoveAt(i);
					return true;
				}
			}
		}

		if (Hash.ContainsKey(index))
		{
			Hash.Remove(index);
			return true;
		}

		return false;
	}

	public void Insert(object index, object value)
	{
		if (index == null) return;

		if (index is float f) index = Mathf.RoundToInt(f);
		if (index is BigInteger b) index = (int)b;

		if (index is int i)
		{
			if (i <= List.Count && i >= 0)
			{
				List.Insert(i, value);
				return;
			}
		}

		this[index] = value;
	}

	public void Clear()
	{
		List.Clear();
		Hash.Clear();
	}

	public override string ToString()
	{
		return ToString(0);
	}

	string KeyValue(object index, object value, int depth)
	{
		string key, val;

		if (index is Table tbl1) key = tbl1.ToString(depth + 1);
		else key = index.ToString();

		if (value is Table tbl2) val = tbl2.ToString(depth + 1);
		else val = value.ToString();

		key.Replace("\n", "\n  ");
		val.Replace("\n", "\n  ");

		return $"- [{key}]: {val}\n";
	}

	public string ToStringTitle()
	{
		var len = Length + Hash.Count;
		return len == 1 ? "Table (1 Element)" : $"Table ({len} Elements)";
	}

	public string ToString(int depth)
	{
		if (depth > 0) return ToStringTitle();

		var builder = new StringBuilder();
		builder.Append($"{ToStringTitle()}\n");

		for (int i = 0; i < List.Count; i++)
		{
			builder.Append(KeyValue(i, List[i], depth));
		}

		foreach (var v in Hash)
		{
			builder.Append(KeyValue(v.Key, v.Value, depth));
		}

		return builder.ToString().TrimEnd();
	}
}
