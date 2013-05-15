using UnityEngine;
using System;
using System.Collections;

public class CloudFractal
{
	private float[] _midpoints;
	private int _size;
	private uint _seed;
	private float[] _startVals;
	private LCGRandom _lcg;
	
	public CloudFractal(int size)
	{
		_lcg = new LCGRandom(0);
		_seed = 0;
		Init (size, new float[] { _lcg.NextPct(), _lcg.NextPct(), _lcg.NextPct(),
									_lcg.NextPct()});
	}
	
	public CloudFractal(int size, uint seed)
	{
		_lcg = new LCGRandom(seed);
		_seed = seed;
		Init (size, new float[] { _lcg.NextPct(), _lcg.NextPct(), _lcg.NextPct(),
									_lcg.NextPct()});
	}
	
	public CloudFractal(int size, uint seed, float[] startVals)
	{
		_lcg = new LCGRandom(seed);
		_seed  = seed;
		Init(size, startVals);
	}
	
	private void Init(int size, float[] startVals)
	{
		_size = size;
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
		return max * (_lcg.NextPct() - 0.5f);
	}
		
	public uint Seed
	{
		get { return _seed; }
		set { _seed = value; _lcg.Seed = _seed;}
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
			GeneratePoints(0, 0, _size, _size, _startVals[0], _startVals[1],
												_startVals[2], _startVals[3]);
			return _midpoints;
		}
	}
	
	public float[] Field { get { return _midpoints; } }
}
