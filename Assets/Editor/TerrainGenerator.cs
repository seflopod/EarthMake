// 
// TerrainGenerator.cs
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

public class TerrainGenerator
{
	private NormalOptions _normalOpt;
	private CloudOptions _cloudOpt;
	private WorleyOptions _worleyOpt;
	private float[] _heightMap;
	private CloudFractal _cfGen;
	private WorleyNoise _wnGen;
	
	public TerrainGenerator()
	{
		Init(
		(NormalOptions)ScriptableObject.CreateInstance(typeof(NormalOptions)),
		(CloudOptions)ScriptableObject.CreateInstance(typeof(CloudOptions)),
		(WorleyOptions)ScriptableObject.CreateInstance(typeof(WorleyOptions)));
	}
	
	public TerrainGenerator(NormalOptions normalOpt, CloudOptions cloudOpt, WorleyOptions worleyOpt)
	{
		Init(normalOpt, cloudOpt, worleyOpt);
	}
	
	private void Init(NormalOptions normalOpt, CloudOptions cloudOpt, WorleyOptions worleyOpt)
	{
		_normalOpt = normalOpt;
		_cloudOpt = cloudOpt;
		_worleyOpt = worleyOpt;
		
		_cfGen = new CloudFractal(_normalOpt.size, (uint)_normalOpt.seed, _cloudOpt.ToArray());
		_wnGen = new WorleyNoise(_normalOpt.size, _worleyOpt.zoomLevel, (uint)_normalOpt.seed,
									_worleyOpt.metric, _worleyOpt.combiner);
		_heightMap = new float[_normalOpt.size * _normalOpt.size];
	}
	
	public void CreateNewHeightMap()
	{
		_heightMap = new float[_normalOpt.size * _normalOpt.size];
		float[] cloud = _cfGen.NewField;
		float[] worley = _wnGen.NewField;
		for(int i=0; i<_heightMap.Length; ++i)
			_heightMap[i] = _normalOpt.worleyInf * worley[i] + _normalOpt.cloudInf * cloud[i];
	}
	
	public Texture2D GetAsTexture2D()
	{
		Texture2D ret = new Texture2D(_normalOpt.size, _normalOpt.size, TextureFormat.RGB24, false);
		Color[] colors = new Color[_heightMap.Length];
		for(int i=0; i<_heightMap.Length; ++i)
			colors[i] = new Color(_heightMap[i], _heightMap[i], _heightMap[i]);
		
		ret.SetPixels(colors);
		ret.Apply();
		return ret;
	}
	
	public byte[] GetAsPNG()
	{
		return this.GetAsTexture2D().EncodeToPNG();
	}
	
	public float[] HeightMap { get { return _heightMap; } }
	
	public NormalOptions NormalOpt
	{
		get { return _normalOpt; }
		set
		{
			_normalOpt = value;
			_cfGen.Seed = (uint)_normalOpt.seed;
			_cfGen.Size = _normalOpt.size;
			_wnGen.Seed = (uint)_normalOpt.seed;
			_wnGen.Size = _normalOpt.size;
		}
	}
	
	public CloudOptions CloudOpt
	{
		get { return _cloudOpt; }
		set
		{
			_cloudOpt = value;
			_cfGen = new CloudFractal(_normalOpt.size, (uint)_normalOpt.seed,
										_cloudOpt.ToArray());
		}
	}
	
	public WorleyOptions WorleyOpt
	{
		get { return _worleyOpt; }
		set
		{
			_worleyOpt = value;
			_wnGen = new WorleyNoise(_normalOpt.size, _worleyOpt.zoomLevel,
									(uint)_normalOpt.seed, _worleyOpt.metric,
									_worleyOpt.combiner);
		}
	}
}