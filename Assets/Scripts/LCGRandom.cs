// 
// LCGRandom.cs
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



/// <summary>
/// LCG Random number generator.
/// </summary>
/// <description>
/// See http://en.wikipedia.org/wiki/Linear_congruential_generator for a full
/// description.
/// </description>
public class LCGRandom
{
	private static LCGRandom _instance = null;
	
	public static LCGRandom Instance
	{
		get
		{
			if(_instance == null)
				_instance = new LCGRandom(System.DateTime.Now.Second + System.DateTime.Now.Millisecond);
			
			return _instance;
		}
	}
	
	/// <summary>
	/// The _modulus.
	/// </summary>
	/// <description>
	/// <code>0 &lt; _modulus</code>
	/// </description>
	private ulong _modulus;
	/// <summary>
	/// The _multiplier.
	/// </summary>
	/// <description>
	/// <code>0 &lt; a &lt; <see cref="_modulus"></code>
	/// </description>
	private ulong _multiplier;
	/// <summary>
	/// The _increment.
	/// </summary>
	/// <description>
	/// <code>0 &le; c &le; <see cref="_modulus"></code>
	/// </description>
	private ulong _increment;
	private uint _last;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="LCGRandom"/> class.
	/// </summary>
	/// <description>
	/// Uses the values provided for Numerical Recipes on
	/// http://en.wikipedia.org/wiki/Linear_congruential_generator
	/// <code><see cref="_modulus"/>=(1<<32), <see cref="_multiplier"/>=1664525,
	/// and <see cref="_increment"/>=1013904223</code>
	/// </description>
	public LCGRandom()
	{
		Init(0x100000000u, 1664525u, 1013904223u, 0);
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="LCGRandom"/> class.
	/// </summary>
	/// <param name='m'>
	/// <see cref="_modulus"/>
	/// </param>
	/// <param name='a'>
	/// <see cref="_multiplier"/>
	/// </param>
	/// <param name='c'>
	/// <see cref="_increment"/>
	/// </param>
	public LCGRandom(ulong m, ulong a, ulong c)
	{
		Init(m, a, c, 0);
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="LCGRandom"/> class.
	/// </summary>
	/// <param name='seed'>
	/// The seed to be used.
	/// </param>
	/// <description>
	/// Uses the values provided for Numerical Recipes on
	/// http://en.wikipedia.org/wiki/Linear_congruential_generator
	/// <code><see cref="_modulus"/>=(1<<32), <see cref="_multiplier"/>=1664525,
	/// and <see cref="_increment"/>=1013904223</code>
	/// </description>
	public LCGRandom(uint seed)
	{
		Init(0x100000000u, 1664525u, 1013904223u, seed);
	}
	
	public LCGRandom(int seed)
	{
		Init(0x100000000u, 1664525u, 1013904223u, (uint)seed);
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="LCGRandom"/> class.
	/// </summary>
	/// <param name='m'>
	/// <see cref="_modulus"/>
	/// </param>
	/// <param name='a'>
	/// <see cref="_multiplier"/>
	/// </param>
	/// <param name='c'>
	/// <see cref="_increment"/>
	/// </param>
	/// <param name='seed'>
	/// The seed to be used.
	/// </param>
	public LCGRandom(ulong m, ulong a, ulong c, uint seed)
	{
		Init(m, a, c, seed);
	}
	
	private void Init(ulong m, ulong a, ulong c, uint seed)
	{
		_modulus = m;
		_multiplier = a;
		_increment = c;
		_last = seed;
	}
	
	/// <summary>
	/// Returns the next number in the LCG
	/// </summary>
	/// <description>
	/// If there has been no seed provided then a default seed of 0 is used.
	/// </description>
	public uint Next()
	{
		_last = (uint)((_multiplier * _last + _increment) % _modulus);
		return _last;
	}
	
	/// <summary>
	/// Returns the next number in the LCG
	/// </summary>
	/// <param name='seed'>
	/// The seed to use.  This is the same as changing the last value generated.
	/// </param>
	public uint Next(uint seed)
	{
		_last = seed;
		return Next();
	}
	
	public float NextPct()
	{
		return (Next() % 101) / 100.0f;
	}
	
	public float NextPct(uint seed)
	{
		_last = seed;
		return NextPct();
	}
	
	public ulong Modulus
	{
		get { return _modulus; }
		set { _modulus = value; }
	}
	
	public ulong Multiplier
	{
		get { return _multiplier; }
		set { _multiplier = value; }
	}
	
	public ulong Increment
	{
		get { return _increment; }
		set { _increment = value; }
	}
	
	public uint Seed { set { _last = value; } }
	
	public uint Last { get { return _last; } }
}