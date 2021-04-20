using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubstringOptimizedString
{
	private string String;
	private int startIndex = 0;
	private int length = 0;

	public SubstringOptimizedString(string str)
	{
		String = str;
		startIndex = 0;
		length = String.Length;
	}

	public SubstringOptimizedString(string str, int start, int length)
	{
		String = str;
		startIndex = start;
		this.length = length;
	}

	// String Functions

	public char this[int i] => String[startIndex + i];
	public int Length => length;

	public int IndexOf(char c)
	{
		for (int i = 0; i < length; i++)
		{
			if (String[startIndex + i] == c)
			{
				return i;
			}
		}
		return -1;
	}

	public SubstringOptimizedString Substring(int startIndex)
	{
		return new SubstringOptimizedString(String, this.startIndex + startIndex, length - startIndex);
	}

	public SubstringOptimizedString Substring(int startIndex, int len)
	{
		return new SubstringOptimizedString(String, this.startIndex + startIndex, len);
	}

	public new string ToString()
	{
		if (length == 0) return "";
		return String.Substring(startIndex, length);
	}
}