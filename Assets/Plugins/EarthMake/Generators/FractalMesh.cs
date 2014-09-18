using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FractalMesh
{
	/*private CloudFractal _cfGen;
	private VoronoiDiagram _vnGen;
	private uint _seed;
	private int _size;
	private float _mult;
	private bool _showSeams;
	
	public FractalMesh(int size, uint seed, float mult, bool showSeams)
	{
		_seed = seed;
		_size = size;
		_mult = mult;
		_cfGen = new CloudFractal(_size);
		_cfGen.Seed = _seed++;
		_vnGen = new WorleyNoise(_size, 5.0f, _seed, WorleyNoise.DistanceMetric.EuclidianSq, WorleyNoise.CombineType.D2MinusD1);
		_showSeams = showSeams;
	}
	
	public Mesh CreateNewMesh()
	{
		float[] worley = _vnGen.Field;
		float[] cloud = _cfGen.NewField;
		float[] points = new float[worley.Length];
		for(int i=0;i<points.Length;++i)
			points[i] = 0.33f * worley[i] + 0.67f * cloud[i];
		
		Mesh mesh = MeshGenerator.CreateNewMeshPlane((uint)_size, (uint)_size, _showSeams);
		Vector3[] newVerts = mesh.vertices;
		int[] newTris = mesh.triangles;
		for(int i=0;i<newVerts.Length;++i)
		{
			int r = (int)(newVerts[i].z+_size/2);
			if((float)r/_size >= 1)
			{
				if(r == _size)
					r-=1;
				else
					r-=2;
			}
			int c = (int)(newVerts[i].x+_size/2);
			if((float)c/_size >= 1)
			{
				if(c == _size)
					c-=1;
				else
					c-=2;
			}
			newVerts[i].y = _mult*points[(r*_size + c)];
		}
		
		mesh.vertices = newVerts;
		mesh.triangles = newTris;
		mesh.RecalculateNormals();
		mesh.tangents = MeshGenerator.calculateMeshTangents(mesh);
		mesh.Optimize();
		return mesh;
	}*/
}
