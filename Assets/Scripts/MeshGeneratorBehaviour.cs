using UnityEngine;
using System.Collections;

public class MeshGeneratorBehaviour : MonoBehaviour
{
	private MeshFilter _mf;
	private FractalMesh _fm;
	public Material defaultMaterial;
	public bool showSeams = true;
	public float multiplier = 1.0f;
	public bool randomSeed = false;
	public uint seed = 0;
	// Use this for initialization
	void Start ()
	{
		if(randomSeed)
			seed = (uint)Random.Range(0, 10000);
		
		_fm = new FractalMesh(32, seed, multiplier, showSeams);
		gameObject.AddComponent<MeshRenderer>();
		_mf = gameObject.AddComponent<MeshFilter>();
		_mf.mesh = _fm.CreateNewMesh();
		gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
	}
}
