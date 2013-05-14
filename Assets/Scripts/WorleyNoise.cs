using UnityEngine;

public class WorleyNoise
{
	private int _size;
	private float _zoom;
	private uint _seed;
	private DistanceMetric _metric;
	private System.Func<Vector3, Vector3, float> _distanceFunc;
	private CombineType _combiner;
	private System.Func<float[], float> _combinerFunc;
	
	private float[] _field;
	private LCGRandom _lcg;
	private FNVHash _fnv;
	
	#region ctors_init
	public WorleyNoise(int size, float zoom, uint seed, DistanceMetric metric, CombineType combiner)
	{
		Init(size, zoom, seed, metric, combiner);
	}
	
	public WorleyNoise(int size, float zoom, uint seed)
	{
		Init(size, zoom, seed, DistanceMetric.EuclidianSq, CombineType.D2MinusD1);
	}
	
	private void Init(int size, float zoom, uint seed, DistanceMetric metric, CombineType combiner)
	{
		_size = size;
		_zoom = zoom;
		_seed = seed;
		Metric = metric;
		Combiner = combiner;
		
		_field = new float[_size*_size];
		_lcg = new LCGRandom(_seed);
		_fnv = new FNVHash();
	}
	#endregion
	
	private void GeneratePoints()
	{
		for(int r=0;r<_size;++r)
		{
			for(int c=0;c<_size;++c)
			{
				Vector3 pos = new Vector3((float)c/_size, (float)r/_size, 0.0f);
				pos*=_zoom;				
				_field[r*_size+c] = NoiseFunc(pos);
			}
		}
	}
	
	public float NoiseFunc(Vector3 pos)
	{
		//Declare some values for later use
		uint lastRandom, numberFeaturePoints;
		Vector3 randomDiff, featurePoint;
		int cubeX, cubeY, cubeZ;
		
		//Initialize values in distance array to large values
		float[] dist = new float[]{9999.9f, 9999.9f, 9999.9f};
		
		//1. Determine which cube the evaluation point is in
		int evalCubeX = Mathf.FloorToInt(pos.x);
		int evalCubeY = Mathf.FloorToInt(pos.y);
		int evalCubeZ = Mathf.FloorToInt(pos.z);

		for (int i = -1; i < 2; ++i)
		{
			for (int j = -1; j < 2; ++j)
			{
				for (int k = -1; k < 2; ++k)
				{
					cubeX = evalCubeX + i;
					cubeY = evalCubeY + j;
					cubeZ = evalCubeZ + k;
					
					//2. Generate a reproducible random number generator for the cube
					uint hash = _fnv.Hash((uint)cubeX + _seed, (uint)(cubeY), (uint)(cubeZ));
					lastRandom = _lcg.Next(hash);
					
					//3. Determine how many feature points are in the cube
					numberFeaturePoints = ProbLookup(lastRandom);
					
					//4. Randomly place the feature points in the cube
					for (int l = 0; l < numberFeaturePoints; ++l)
					{
						randomDiff = new Vector3((float)_lcg.Next() / 0x100000000,
											(float)_lcg.Next() / 0x100000000,
											(float)_lcg.Next() / 0x100000000);
						
						featurePoint = new Vector3(randomDiff.x + cubeX,
													randomDiff.y + cubeY,
													randomDiff.z+cubeZ);

						//5. Find the feature point closest to the evaluation point. 
						//This is done by inserting the distances to the feature points into a sorted list
						Insert(dist, _distanceFunc(pos, featurePoint));
					}
					//6. Check the neighboring cubes to ensure there are no closer evaluation points.
					// This is done by repeating steps 1 through 5 above for each neighboring cube
				}
			}
		}

		float ret = _combinerFunc(dist);
		if(ret < 0) ret = 0;
		if(ret > 1) ret = 1;
		return ret;
	}
	
	#region util_func
	/// <summary>
	/// Given a uniformly distributed random number this function returns the number of feature points in a given cube.
	/// </summary>
	/// <param name="value">a uniformly distributed random number</param>
	/// <returns>The number of feature points in a cube.</returns>
	/// <description>
	/// Generated using mathmatica with "AccountingForm[N[Table[CDF[PoissonDistribution[4], i], {i, 1, 9}], 20]*2^32]"
	/// </description>
	private uint ProbLookup(uint value)
	{
		if (value < 393325350) return 1;
		if (value < 1022645910) return 2;
		if (value < 1861739990) return 3;
		if (value < 2700834071) return 4;
		if (value < 3372109335) return 5;
		if (value < 3819626178) return 6;
		if (value < 4075350088) return 7;
		if (value < 4203212043) return 8;
		return 9;
	}
	
	/// <summary>
	/// Inserts value into array using insertion sort. If the value is greater than the largest value in the array
	/// it will not be added to the array.
	/// </summary>
	/// <param name="arr">The array to insert the value into.</param>
	/// <param name="x">The value to insert into the array.</param>
	private void Insert(float[] a, float x)
	{
		float tmp;
		for (int i = a.Length - 1; i >= 0; i--)
		{
			if (x > a[i]) break;
			tmp = a[i];
			a[i] = x;
			if (i + 1 < a.Length) a[i + 1] = tmp;
		}
	}
	#endregion
	
	#region properties
	public DistanceMetric Metric
	{
		get { return _metric; }
		
		set
		{
			_distanceFunc = null;
			_metric = value;
			switch(value)
			{
			case DistanceMetric.Chebyshev:
				_distanceFunc = DistanceFuncs.Chebyshev;
				break;
			case DistanceMetric.Euclidian:
				_distanceFunc = DistanceFuncs.Euclidian;
				break;
			case DistanceMetric.EuclidianSq:
				_distanceFunc = DistanceFuncs.EuclidianSq;
				break;
			case DistanceMetric.Manhattan:
				_distanceFunc = DistanceFuncs.Manhattan;
				break;
			default:
				_distanceFunc = DistanceFuncs.EuclidianSq;
				_metric = DistanceMetric.EuclidianSq;
				break;
			}
		}
	}
	
	public CombineType Combiner
	{
		get { return _combiner; }
		
		set
		{
			_combinerFunc = null;
			_combiner = value;
			switch(value)
			{
			case CombineType.D1:
				_combinerFunc = x => x[0];
				break;
			case CombineType.D2MinusD1:
				_combinerFunc = x => x[1] - x[0];
				break;
			case CombineType.D3MinusD1:
				_combinerFunc = x => x[2] - x[0];
				break;
			default:
				_combinerFunc = x => x[1] - x[0];
				_combiner = CombineType.D2MinusD1;
				break;
			}
		}
	}
	
	public uint Seed
	{
		get { return _seed; }
		set { _seed = value; }
	}
	
	public int Size
	{
		get { return _size; }
		set { _size = value; }
	}
	
	public float[] NewField
	{
		get
		{
			GeneratePoints ();
			return _field;
		}
	}
	
	public float[] Field { get { return _field; } }
	#endregion
	
	#region util_classes
	public enum DistanceMetric
	{
		EuclidianSq,
		Euclidian,
		Manhattan,
		Chebyshev	
	};
	
	public enum CombineType
	{
		D1,
		D2MinusD1,
		D3MinusD1
	};
	
	public static class DistanceFuncs
	{
		public static float EuclidianSq(Vector3 p1, Vector3 p2)
		{
			return (p1-p2).sqrMagnitude;
		}
		
		public static float Euclidian(Vector3 p1, Vector3 p2)
		{
			return (p1-p2).magnitude;
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
		
		//Some rewrites of other funcs.  May or may not be faster
		private static float Max(float a, float b) { return (a > b)?a:b; }
		private static float Min(float a, float b) { return (a < b)?a:b; }
		private static float Abs(float a) { return (a<0)?-a:a; }
	}
	#endregion
}