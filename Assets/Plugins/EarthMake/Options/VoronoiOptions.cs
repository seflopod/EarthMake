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
	public DistanceFunctions.DistanceMetric metric;
	public CombinerFunctions.CombineFunction combiner;
	public int numberOfFeaturePoints;
	public int numberOfSubregions;
	public float multiplier;

	public void OnEnable()
	{
		metric = DistanceFunctions.DistanceMetric.EuclidianSq;
		combiner = CombinerFunctions.CombineFunction.D2MinusD1;
		numberOfFeaturePoints = 8;
		numberOfSubregions = 8;
		multiplier = 10f;
	}
	
	public override string ToString()
	{
		return string.Format("[VoronoiOptions]\n\tDistance Metric: {0}\n\t" +
								"Combine Type: {1}\n\t" +
								"Feature Points: {2}\n\t" +
								"Subregions: {3}\n\t" +
		                     	"Multiplier: {4}",
								metric, combiner, numberOfFeaturePoints, numberOfSubregions, multiplier);
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
		        a.numberOfSubregions == b.numberOfSubregions &&
		        a.multiplier == b.multiplier);
	}
	
	public static bool operator!=(VoronoiOptions a, VoronoiOptions b)
	{
		return !(a == b);
	}
}