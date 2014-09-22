using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public delegate float DistanceFunc(Vector3 ptA, Vector3 ptB);
public delegate float CombiningFunc(float[] distances);
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

	public VoronoiDiagram(int size, DistanceFunc distanceFunc, CombiningFunc combiningFunc, int numberOfFeaturePoints, int numberOfSubregions, uint seed)
	{
		mSize = size;
		delDistanceFunc = distanceFunc;
		delCombiningFunc = combiningFunc;
		mNumFeaturePoints = numberOfFeaturePoints;
		mNumSubregions = numberOfSubregions;
		mRNG = new LCGRandom(seed);
		aField = new float[0];
		aFeaturePoints = new int[mNumFeaturePoints];
	}

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
		int subRegionHeight = 0;
		while(mNumSubregions % subRegionWidth != 0 && subRegionWidth > 0)
		{
			subRegionWidth--;
		}
		if(subRegionWidth > 0)
		{	
			subRegionHeight = mNumSubregions / subRegionWidth;
			Debug.Log(subRegionWidth + ", " + subRegionHeight);
			List<int> pointsList = new List<int>();
			for(int startR=0;startR<(mNumSubregions-1)*subRegionHeight;startR+=subRegionHeight)
			{
				for(int startC=0;startC<(mNumSubregions-1)*subRegionWidth;startC+=subRegionWidth)
				{
					int[] indices = new int[subRegionWidth*subRegionHeight];
					int i=0;
					for(int r=startR;r<subRegionHeight;++r)
					{
						for(int c=startC;c<subRegionHeight;++c)
						{
							indices[i++] = r*mSize+c;
						}
					}

					for(int j=0;j<indices.Length;++j)
					{
						int rndIdx = (int)(mRNG.Next() % (indices.Length-j));
						int tmp = indices[j];
						indices[j] = indices[rndIdx];
						indices[rndIdx] = tmp;
					}

					for(int k=0;k<mNumFeaturePoints && k<indices.Length;++k)
					{
						pointsList.Add(indices[k]);
					}
				}
			}
			aFeaturePoints = pointsList.ToArray();
			string dbgStr = aFeaturePoints.Length.ToString() + ":";
			for(int i=0;i<aFeaturePoints.Length;++i)
			{
				dbgStr+=aFeaturePoints[i].ToString() + ",";
			}
			Debug.Log(dbgStr.Substring(0, dbgStr.Length-1));
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
