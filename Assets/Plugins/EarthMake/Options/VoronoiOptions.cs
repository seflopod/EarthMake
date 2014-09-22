// 
// WorleyOptions.cs
//  
// Author:
//       Peter Bartosch <bartoschp@gmail.com>
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
		numberOfFeaturePoints = 2;
		numberOfSubregions = 4;
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