using UnityEditor;
using UnityEngine;

[System.Serializable]
public class WorleyOptions : ScriptableObject
{
	public WorleyNoise.DistanceMetric metric;
	public WorleyNoise.CombineType combiner;
	public float zoomLevel;
	
	public void OnEnable()
	{
		metric = WorleyNoise.DistanceMetric.EuclidianSq;
		combiner = WorleyNoise.CombineType.D2MinusD1;
		zoomLevel = 5.0f;
	}
	
	public override string ToString ()
	{
		return string.Format("[WorleyOptions]\n\tDistance Metric: {0}\n\t" +
								"Combine Type: {1}\n\t" +
								"Zoom Level: {2:0.0}",
								metric, combiner, zoomLevel);
	}
	
	public override int GetHashCode()
	{
		FNVHash fnv = new FNVHash();
		return (int)fnv.Hash((uint)metric, (uint)combiner, (uint)zoomLevel);
	}
	
	public override bool Equals(object o)
	{
		return (this == (WorleyOptions)o);
	}
	
	public static bool operator==(WorleyOptions a, WorleyOptions b)
	{
		return (a.metric == b.metric && a.combiner == b.combiner &&
				a.zoomLevel == b.zoomLevel);
	}
	
	public static bool operator!=(WorleyOptions a, WorleyOptions b)
	{
		return !(a==b);
	}
}