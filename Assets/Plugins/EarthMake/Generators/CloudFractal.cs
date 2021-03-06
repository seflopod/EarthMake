// 
// CloudFractal.cs
//  
// Author:
//       Peter Bartosch <bartoschp@gmail.com>
// 
// Copyright (c) 2013 Peter Bartosch
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEngine;
using System;
using System.Collections;

public class CloudFractal
{
	private float[] _field;
	private int _size;
	private float[] _startVals;
	private LCGRandom _lcg;

	public CloudFractal(int size, int seed, float[] startVals)
	{
		_lcg = new LCGRandom((uint)seed);
		_size = size;
		_startVals = startVals;
		
		_field = new float[(_size) * (_size)];
		for(int i=0; i<_field.Length; ++i)
			_field[i] = -1.0f;
	}

	public CloudFractal(int size, int seed, CloudOptions options)
	{
		_size = size;
		_startVals = options.GetStartArray();

		_lcg = new LCGRandom((uint)seed);
		_field = new float[(_size) * (_size)];
		for(int i=0; i<_field.Length; ++i)
			_field[i] = -1.0f;

	}

	public float[] GetNewField()
	{
		_field = new float[(_size) * (_size)];
		for(int i=0; i<_field.Length; ++i)
			_field[i] = -1.0f;

		generatePoints(0, 0, _size, _size, _startVals[0], _startVals[1],
		               _startVals[2], _startVals[3]);

		return _field;
	}

	private void generatePoints(int x, int y, int w, int h, float c1, float c2, float c3, float c4)
	{
		float e1, e2, e3, e4, m;
		int nW = w >> 1;
		int nH = h >> 1;
		
		if(w > 1 || h > 1)
		{
			m = (c1 + c2 + c3 + c4) / 4 + displace(nW + nH);
			e1 = (c1 + c2) / 2;
			e2 = (c2 + c3) / 2;
			e3 = (c3 + c4) / 2;
			e4 = (c4 + c1) / 2;
			
			//these checks may not be necessary
			if(m < 0)
				m = 0;
			else if(m > 1.0f)
				m = 1.0f;
			
			generatePoints(x, y, nW, nH, c1, e1, m, e4);
			generatePoints(x + nW, y, nW, nH, e1, c2, e2, m);
			generatePoints(x + nW, y + nH, nW, nH, m, e2, c3, e3);
			generatePoints(x, y + nH, nW, nH, e4, m, e3, c4);
		}
		else
			_field[_size*y+x] = (c1+c2+c3+c4)/4;
	}
	
	private float displace(float n)
	{
		float max = n / (2.0f * _size) * 3.0f;
		return max * (_lcg.NextPct() - 0.5f);
	}
	
	public int Size
	{
		get { return _size; }
		set { _size = value; }
	}

	public int Seed
	{
		get { return (int)_lcg.Seed; }
		set { _lcg.Seed = (uint)value; }
	}
	
	public float[] Field { get { return _field; } }
}
