// 
// WorleyOptions.cs
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
public class VoronoiOptions : ScriptableObject
{
	public DistanceFuncs.DistanceMetric metric;
	public CombinerFunctions.CombineFunction combiner;
	public int numberOfFeaturePoints;
	public int numberOfSubregions;

	public void OnEnable()
	{
		metric = DistanceFuncs.DistanceMetric.EuclidianSq;
		combiner = CombinerFunctions.CombineFunction.D2MinusD1;
		numberOfFeaturePoints = 10;
		numberOfSubregions = 9;
	}
	
	public override string ToString()
	{
		return string.Format("[WorleyOptions]\n\tDistance Metric: {0}\n\t" +
								"Combine Type: {1}\n\t" +
								"Feature Points: {2}\n\t" +
								"Subregions: {3}",
								metric, combiner, numberOfFeaturePoints, numberOfSubregions);
	}
	
	public override int GetHashCode()
	{
		FNVHash fnv = new FNVHash();
		return (int)fnv.Hash((uint)metric, (uint)combiner, (uint)(numberOfFeaturePoints^numberOfSubregions));
	}
	
	public override bool Equals(object o)
	{
		return (this == (VoronoiOptions)o);
	}
	
	public static bool operator==(VoronoiOptions a, VoronoiOptions b)
	{
		return (a.metric == b.metric && a.combiner == b.combiner &&
				a.numberOfFeaturePoints == b.numberOfFeaturePoints &&
		        a.numberOfSubregions == b.numberOfSubregions);
	}
	
	public static bool operator!=(VoronoiOptions a, VoronoiOptions b)
	{
		return !(a == b);
	}
}