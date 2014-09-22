using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;



public class VoronoiDiagram
{
	private int mSize;
	private DistanceFunc delDistanceFunc;
	private CombiningFunc delCombiningFunc;
	private int mNumFeaturePoints; //per-region
	private int mNumSubregions;
	private int[] aFeaturePoints;
	private float[] aField; //row-major
	private LCGRandom mRNG;

	public VoronoiDiagram(int size, int seed, VoronoiOptions options)
	{
		mSize = size;
		delDistanceFunc = DistanceFuncs.GetDistanceFunction(options.metric);
		delCombiningFunc = CombinerFunctions.GetFunction(options.combiner);
		mNumFeaturePoints = options.numberOfFeaturePoints;
		mNumSubregions = options.numberOfSubregions;

		aFeaturePoints = new int[0];
		mRNG = new LCGRandom(seed);
	}

	private void generateFeaturePoints()
	{
		int subRegionWidth = (int)Mathf.Sqrt(mNumSubregions);
		while(mNumSubregions % subRegionWidth != 0 && subRegionWidth > 0)
		{
			subRegionWidth--;
		}
		if(subRegionWidth > 0)
		{	
			int subRegionHeight = mNumSubregions / subRegionWidth;
			int c = mSize / subRegionWidth;
			int r = mNumSubregions / c;
			Debug.Log(subRegionWidth + ", " + subRegionHeight);
			List<int> pointsList = new List<int>();
			int[] allIndices = new int[mSize * mSize];
			for(int i=0;i<allIndices.Length;++i)
			{
				int j = (int)(mRNG.Next() % (i+1));
				if(j != i)
				{
					allIndices[i] = j;
				}
				allIndices[j] = i;
			}
			for(int i=0;i<mNumSubregions;++i)
			{
				List<int> indices = new List<int>();
				int j = 0;
				while(j < allIndices.Length && indices.Count < mNumFeaturePoints)
				{
					indices.Clear();
					int idx = allIndices[j++];
					for(int k=0;k<mSize / subRegionHeight; ++k)
					{
						if(idx < mSize*k+i*mSize / subRegionWidth)
						{
							indices.Add(idx);
							break;
						}
					}
				}
				pointsList.AddRange(indices);
			}
		}
	}

	public float[] GetNewField()
	{
		aField = new float[mSize * mSize];
		generateFeaturePoints();

		for(int i=0;i<aFeaturePoints.Length;++i)
		{
			aField[aFeaturePoints[i]] = 0;
		}

		float[] distances = new float[aFeaturePoints.Length];
		for(int r=0;r<mSize;++r)
		{
			for(int c=0;c<mSize;++c)
			{
				Vector3 curPoint = new Vector3(c / (float)mSize, r / ((float)mSize), 0f);
				//create the distance array
				for(int j=0;j<distances.Length;++j)
				{
					Vector3 newPoint = new Vector3((aFeaturePoints[j]%mSize) / (float)mSize, (aFeaturePoints[j]/mSize) / (float)mSize, 0f);
					distances[j] = 5*delDistanceFunc(newPoint, curPoint);
				}
				Array.Sort(distances);
				aField[r*mSize+c] = delCombiningFunc(distances);
			}
		}
		return aField;
	}

	public float[] Field
	{
		get
		{
			if(aField.Length == 0)
			{
				GetNewField();
			}
			return aField;
		}
	}

	public int Seed
	{
		get { return (int)mRNG.Seed; }
		set { mRNG.Seed = (uint)value; }
	}

	public int Size
	{
		get { return mSize; }
		set { mSize = value; }
	}
}
