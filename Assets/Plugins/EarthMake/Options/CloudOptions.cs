// 
// CloudOptions.cs
//  
// Author:
//       Peter Bartosch <bartoschp@gmail.com>
// 
// Copyright (c) 2013 Peter Bartosch
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEditor;
using UnityEngine;

[System.Serializable]
public class CloudOptions : ScriptableObject
{
	public float upperLeftStart;
	public float lowerLeftStart;
	public float lowerRightStart;
	public float upperRightStart;
	
	public void OnEnable()
	{
		upperLeftStart = 1.0f;
		lowerLeftStart = 0.75f;
		lowerRightStart = 0.5f;
		upperRightStart = 0.25f;
	}
	
	public float[] GetStartArray()
	{
		return new float[] { upperLeftStart, lowerLeftStart, lowerRightStart,
								upperRightStart };
	}
	
	public override string ToString()
	{
		return string.Format("[CloudOptions]\n\tUpper Left Start: {0:0.00}\n\t" +
								"Lower Left Start: {1:0.00}\n\t" +
								"Lower Right Start: {2:0.00}\n\t" +
								"Upper Right Start: {3:0.00}",
								upperLeftStart, lowerLeftStart, lowerRightStart,
								upperRightStart);
	}
	
	public override int GetHashCode()
	{
		FNVHash fnv = new FNVHash();
		uint pass1 = fnv.Hash((uint)upperLeftStart, (uint)lowerLeftStart, (uint)lowerRightStart);
		uint pass2 = fnv.Hash((uint)lowerLeftStart, (uint)lowerRightStart, (uint)upperRightStart);
		uint pass3 = fnv.Hash((uint)lowerRightStart, (uint)upperRightStart, (uint)upperLeftStart);
		return (int)fnv.Hash(pass1, pass2, pass3);
	}
	
	public override bool Equals(object o)
	{
		return (this == (CloudOptions)o);
	}
	
	public static bool operator==(CloudOptions a, CloudOptions b)
	{
		return (a.upperLeftStart == b.upperLeftStart &&
				a.lowerLeftStart == b.lowerLeftStart &&
				a.lowerRightStart == b.lowerRightStart &&
				a.upperRightStart == b.upperRightStart);
	}
	
	public static bool operator!=(CloudOptions a, CloudOptions b)
	{
		return !(a == b);
	}
}