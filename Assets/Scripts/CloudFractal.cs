using UnityEngine;
using System.Collections;

public class CloudFractal
{
	private float[] _midpoints;
	private int _size;
	private uint _seed;
	private float[] _startVals;
	
	public CloudFractal(int size)
	{
		Init (size, 0, new float[] { CoherentNoise((float)(_seed++)),
										CoherentNoise((float)(_seed++)),
										CoherentNoise((float)(_seed++)),
										CoherentNoise((float)(_seed++)) });
	}
	
	public CloudFractal(int size, uint seed)
	{
		Init (size, seed, new float[] { CoherentNoise((float)(_seed++)),
										CoherentNoise((float)(_seed++)),
										CoherentNoise((float)(_seed++)),
										CoherentNoise((float)(_seed++)) });
	}
	
	public CloudFractal(int size, uint seed, float[] startVals)
	{
		Init(size, seed, startVals);
	}
	
	private void Init(int size, uint seed, float[] startVals)
	{
		_size = size;
		_seed = seed;
		_startVals = startVals;
		
		_midpoints = new float[(_size) * (_size)];
		for(int i=0;i<_midpoints.Length;++i)
			_midpoints[i] = -1.0f;
	}
	
	private void GeneratePoints(int x, int y, int w, int h, float c1, float c2, float c3, float c4)
	{
		float e1, e2, e3, e4, m;
		int nW = w >> 1;
		int nH = h >> 1;
		
		if(w > 1 || h > 1)
		{
			m = (c1+c2+c3+c4)/4 + Displace(nW+nH);
			e1 = (c1 + c2) / 2;
			e2 = (c2 + c3) / 2;
			e3 = (c3 + c4) / 2;
			e4 = (c4 + c1) / 2;
			
			//these checks may not be necessary
			if(m < 0)
				m = 0;
			else if(m > 1.0f)
				m = 1.0f;
			
			GeneratePoints(x, y, nW, nH, c1, e1, m, e4);
			GeneratePoints(x+nW, y, nW, nH, e1, c2, e2, m);
			GeneratePoints(x+nW, y+nH, nW, nH, m, e2, c3, e3);
			GeneratePoints(x, y+nH, nW, nH, e4, m, e3, c4);
		}
		else
		{
			_midpoints[_size*y+x] = (c1+c2+c3+c4)/4;
		}
	}
	
	private float Displace(float n)
	{
		float max = n/(2.0f*_size)*3.0f;
		return max * (Mathf.Abs(CoherentNoise(_seed)) - 0.5f);
	}
	
	private float IntegerNoise(int n)
	{
		//the large ints are primes, they can be modified as long as they remain
		//prime.
		n = (n >> 13) ^ n;
		n = (n * (n * n * 60493 + 19990303) + 1376312589) & 0x7fffffff;
		return 1.0f - (n/1073741824.0f);
	}
	
	private float CoherentNoise(float x)
	{
		int xi = Mathf.FloorToInt(x);
		float n0 = IntegerNoise(xi);
		float n1 = IntegerNoise(xi+1);
		float weight = x - Mathf.Floor(x);
		float noise = Mathf.Lerp(n0, n1, SCurve(weight));
		return noise;
	}
	
	private float SCurve(float x)
	{
		return x*x*(-2*x+3);
		
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
			GeneratePoints(0,0,_size,_size, _startVals[0], _startVals[1],
											_startVals[2], _startVals[3]);
			return _midpoints;
		}
	}
	
	public float[] Field { get { return _midpoints; } }
}
