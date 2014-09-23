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
		delDistanceFunc = DistanceFunctions.GetDistanceFunction(options.metric);
		delCombiningFunc = CombinerFunctions.GetFunction(options.combiner);
		mNumFeaturePoints = options.numberOfFeaturePoints;
		mNumSubregions = options.numberOfSubregions;
		
		aFeaturePoints = new int[0];
		mRNG = new LCGRandom(seed);
	}

	//according to the paper I'm using as reference, the field is split into regions and feature points are added.
	//I'm not particularly good at that sort of thing, so this is a best-guess as to how that should be done.
	//my feeling is that the main weakness of the generator will come from here.
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

			int rowsPerRegion = mSize / subRegionHeight;
			int colsPerRegion = mSize / subRegionWidth;
			List<int> indices = new List<int>();
			for(int i=0;i<mNumSubregions;++i)
			{
				int j = 0;
				int regionR = i / subRegionHeight;
				int regionC = i % subRegionHeight;
				int testRMin = regionR * rowsPerRegion;
				int testCMin = regionC * colsPerRegion;
				int testRMax = (regionR+1) * rowsPerRegion;
				int testCMax = (regionC+1) * colsPerRegion;
				while(j < allIndices.Length && indices.Count < mNumFeaturePoints)
				{
					int idx = allIndices[j++];
					int idxR = idx / rowsPerRegion;
					int idxC = (colsPerRegion * regionC) + idx % rowsPerRegion;
					if((idxR >= testRMin && idxR < testRMax) && (idxC >= testCMin && idxC < testCMax))
					{
						indices.Add(idx);
					}
				}
				pointsList.AddRange(indices);
				indices.Clear();
			}
			aFeaturePoints = pointsList.ToArray();
		}
	}
	
	public float[] GetNewField()
	{
		aField = new float[mSize * mSize];
		generateFeaturePoints();
		
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
					distances[j] = delDistanceFunc(newPoint, curPoint);
				}
				Array.Sort<float>(distances, new Comparison<float>(
					(ele1, ele2) => ele1.CompareTo(ele2)));
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
