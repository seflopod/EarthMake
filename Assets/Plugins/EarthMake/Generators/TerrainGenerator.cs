// 
// TerrainGenerator.cs
//  
// Author:
//       Peter Bartosch <bartoschp@gmail.com>

using UnityEngine;

public class TerrainGenerator
{
	private NormalOptions _normalOpt;
	private CloudOptions _cloudOpt;
	private VoronoiOptions _voronoiOpt;
	private float[] _heightMap;
	private CloudFractal _cfGen;
	private VoronoiDiagram _vnGen;
	private ErosionGenerator _erGen;
	
	public TerrainGenerator()
	{
		Init(
			(NormalOptions)ScriptableObject.CreateInstance(typeof(NormalOptions)),
			(CloudOptions)ScriptableObject.CreateInstance(typeof(CloudOptions)),
			(VoronoiOptions)ScriptableObject.CreateInstance(typeof(VoronoiOptions)));
	}
	
	public TerrainGenerator(NormalOptions normalOpt, CloudOptions cloudOpt, VoronoiOptions voronoiOpt)
	{
		Init(normalOpt, cloudOpt, voronoiOpt);
	}
	
	private void Init(NormalOptions normalOpt, CloudOptions cloudOpt, VoronoiOptions voronoiOpt)
	{
		_normalOpt = normalOpt;
		_cloudOpt = cloudOpt;
		_voronoiOpt = voronoiOpt;
		
		_cfGen = new CloudFractal(_normalOpt.size, _normalOpt.seed, _cloudOpt);
		_vnGen = new VoronoiDiagram(_normalOpt.size, _normalOpt.seed, _voronoiOpt);
		_heightMap = new float[_normalOpt.size * _normalOpt.size];
	}
	
	public float[] CreateNewHeightMap()
	{
		_heightMap = new float[_normalOpt.size * _normalOpt.size];
		float[] cloud = _cfGen.GetNewField();
		float[] voronoi = _vnGen.GetNewField();
		for(int i=0; i<_heightMap.Length; ++i)
		{
			_heightMap[i] = _normalOpt.voronoiInf * voronoi[i] + _normalOpt.cloudInf * cloud[i];
		}
		_erGen = new ErosionGenerator(_heightMap);
		if(_normalOpt.useThermalErosion)
		{
			_heightMap = _erGen.ThermalErosion(3);
		}
		if(_normalOpt.useHydroErosion)
		{

		}

		return _heightMap;
	}
	
	public Texture2D GetAsTexture2D()
	{
		Texture2D ret = new Texture2D(_normalOpt.size, _normalOpt.size, TextureFormat.RGB24, false);
		Color[] colors = new Color[_heightMap.Length];
		for(int i=0; i<_heightMap.Length; ++i)
		{
			colors[i] = new Color(_heightMap[i], _heightMap[i], _heightMap[i]);
		}
		
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
			_cfGen.Seed = _normalOpt.seed;
			_cfGen.Size = _normalOpt.size;
			_vnGen.Seed = _normalOpt.seed;
			_vnGen.Size = _normalOpt.size;
		}
	}
	
	public CloudOptions CloudOpt
	{
		get { return _cloudOpt; }
		set
		{
			_cloudOpt = value;
			_cfGen = new CloudFractal(_normalOpt.size, _normalOpt.seed, _cloudOpt);
		}
	}
	
	public VoronoiOptions VoronoiOpt
	{
		get { return _voronoiOpt; }
		set
		{
			_voronoiOpt = value;
			_vnGen = new VoronoiDiagram(_normalOpt.size, _normalOpt.seed, _voronoiOpt);
		}
	}
}