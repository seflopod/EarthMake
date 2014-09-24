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

	public int seed;
	public float voronoiInf;
	public float cloudInf;
	public bool useThermalErosion;
	public bool useHydroErosion;
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
		useThermalErosion = true;
		useHydroErosion = true;
		showSeams = false;
	}
	
	public override string ToString()
	{
		return string.Format("[Normal Options]\n\tSize: {0}\n\t" +
								"Seed: {1}\n\t" +
								"Cloud Fractal Influence: {2:0.0%}\n\t" +
								"Voronoi Diagram Influence: {3:0.0%}\n\t" +
								"Use Thermal Erosion: {4}\n\t"+
								"Use Hyrdo Erosion: {5}\n\t" +
								"Show Seams: {6}",
			                     size, seed, cloudInf, voronoiInf, useThermalErosion, useHydroErosion, showSeams);
	}
	
	public override int GetHashCode()
	{
		FNVHash fnv = new FNVHash();
		uint floats = fnv.Hash((uint)voronoiInf, (uint)cloudInf, (uint)1f);
		uint ints = fnv.Hash((uint)size, (uint)seed, (uint)seed);
		return (int)fnv.Hash(floats, ints, (uint)seed);
	}
	
	public override bool Equals(object o)
	{
		return (this == (NormalOptions)o);
	}
	
	public static bool operator==(NormalOptions a, NormalOptions b)
	{
		return (a.size == b.size && a.seed == b.seed &&
		        a.voronoiInf == b. voronoiInf &&
				a.cloudInf == b.cloudInf && a.showSeams == b.showSeams);
	}
	
	public static bool operator!=(NormalOptions a, NormalOptions b)
	{
		return !(a == b);
	}
}