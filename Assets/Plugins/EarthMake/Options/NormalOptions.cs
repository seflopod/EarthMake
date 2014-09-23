// 
// NormalOptions.cs
//  
// Author:
//       Peter Bartosch <bartoschp@gmail.com>


using UnityEditor;
using UnityEngine;

[System.Serializable]
public class NormalOptions : ScriptableObject
{
	public int size;
	public float multiplier;
	public int seed;
	public float voronoiInf;
	public float cloudInf;
	public bool showSeams;
	
	/// <summary>
	/// Constructor replacement for serializable ScriptableObject
	/// </summary>
	/// <description>
	/// Sets default values for all fields.
	/// </description>
	public void OnEnable()
	{
		voronoiInf = 0.33f;
		cloudInf = 0.67f;
		seed = 0;
		size = 256;
		multiplier = 1.0f;
		showSeams = false;
	}
	
	public override string ToString()
	{
		return string.Format("[Normal Options]\n\tSize: {0}\n\t" +
								"Multiplier: {1}\n\t" +
								"Seed: {2}\n\t" +
								"Cloud Fractal Influence: {3:0.0%}\n\t" +
								"Voronoi Diagram Influence: {4:0.0%}\n\t" +
								"Show Seams: {5}",
								size, multiplier, seed, cloudInf, voronoiInf,
								showSeams);
	}
	
	public override int GetHashCode()
	{
		FNVHash fnv = new FNVHash();
		uint floats = fnv.Hash((uint)multiplier, (uint)voronoiInf,
						(uint)cloudInf);
		uint ints = fnv.Hash((uint)size, (uint)seed, (uint)seed);
		return (int)fnv.Hash(floats, ints, (uint)seed);
	}
	
	public override bool Equals(object o)
	{
		return (this == (NormalOptions)o);
	}
	
	public static bool operator==(NormalOptions a, NormalOptions b)
	{
		return (a.size == b.size && a.multiplier == b.multiplier &&
				a.seed == b.seed && a.voronoiInf == b. voronoiInf &&
				a.cloudInf == b.cloudInf && a.showSeams == b.showSeams);
	}
	
	public static bool operator!=(NormalOptions a, NormalOptions b)
	{
		return !(a == b);
	}
}