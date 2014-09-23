using UnityEngine;

public delegate float DistanceFunc(Vector3 ptA, Vector3 ptB);
public static class DistanceFunctions
{
	public enum DistanceMetric
	{
		EuclidianSq,
		Euclidian,
		Manhattan,
		Chebyshev	
	};

	public static float EuclidianSq(Vector3 p1, Vector3 p2)
	{
		return (p1 - p2).sqrMagnitude;
	}
	
	public static float Euclidian(Vector3 p1, Vector3 p2)
	{
		return (p1 - p2).magnitude;
	}
	
	public static float Manhattan(Vector3 p1, Vector3 p2)
	{
		Vector3 d = p1 - p2;
		return Abs(d.x) + Abs(d.y) + Abs(d.z);
	}
	
	public static float Chebyshev(Vector3 p1, Vector3 p2)
	{
		Vector3 d = p1 - p2;
		return Max(Max(Abs(d.x), Abs(d.y)), Abs(d.z));
	}

	public static DistanceFunc GetDistanceFunction(DistanceMetric distanceMetric)
	{
		DistanceFunc ret;
		switch(distanceMetric)
		{
		case DistanceMetric.EuclidianSq:
			ret = new DistanceFunc(EuclidianSq);
			break;
		case DistanceMetric.Euclidian:
			ret = new DistanceFunc(Euclidian);
			break;
		case DistanceMetric.Manhattan:
			ret = new DistanceFunc(Manhattan);
			break;
		case DistanceMetric.Chebyshev:
			ret = new DistanceFunc(Chebyshev);
			break;
		default:
			ret = new DistanceFunc(EuclidianSq);
			break;
		}
		return ret;
	}
	
	//Some rewrites of other funcs.  May or may not be faster
	private static float Max(float a, float b)
	{
		return (a > b) ? a : b;
	}
	
	private static float Min(float a, float b)
	{
		return (a < b) ? a : b;
	}
	
	private static float Abs(float a)
	{
		return (a < 0) ? -a : a;
	}
}
