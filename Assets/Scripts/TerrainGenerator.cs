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
		_wnGen = new WorleyNoise(_normalOpt.size, _worleyOpt.zoomLevel, (uint)_normalOpt.seed, _worleyOpt.metric, _worleyOpt.combiner);
		_heightMap = new float[_normalOpt.size*_normalOpt.size];
	}
	
	public void CreateNewHeightMap()
	{
		_heightMap = new float[_normalOpt.size*_normalOpt.size];
		float[] cloud = _cfGen.NewField;
		float[] worley = _wnGen.NewField;
		for(int i=0;i<_heightMap.Length;++i)
			_heightMap[i] = _normalOpt.worleyInf * worley[i] + _normalOpt.cloudInf * cloud[i];
	}
	
	public Texture2D GetAsTexture2D()
	{
		Texture2D ret = new Texture2D(_normalOpt.size, _normalOpt.size, TextureFormat.RGB24, false);
		Color[] colors = new Color[_heightMap.Length];
		for(int i=0;i<_heightMap.Length;++i)
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