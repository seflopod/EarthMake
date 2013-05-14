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
	
	public float[] ToArray()
	{
		return new float[] { upperLeftStart, lowerLeftStart, lowerRightStart,
								upperRightStart };
	}
	
	public override string ToString ()
	{
		return string.Format ("[CloudOptions]\n\tUpper Left Start: {0:0.00}\n\t" +
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
		return !(a==b);
	}
}