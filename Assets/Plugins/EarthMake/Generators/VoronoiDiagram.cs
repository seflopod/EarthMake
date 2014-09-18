using UnityEngine;
using System.Collections;
using System;

public delegate float DistanceFunc(Vector3 ptA, Vector3 ptB);
public delegate float CombiningFunc(float[] distances);
public class VoronoiDiagram
{
	private int mSize;
	private DistanceFunc delDistanceFunc;
	private CombiningFunc delCombiningFunc;
	private int mNumFeaturePoints;
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

		aFeaturePoints = new int[mNumFeaturePoints];
		mRNG = new LCGRandom(seed);
	}

	private void generateFeaturePoints()
	{
		uint subRegionWidth = (uint)Mathf.Sqrt(mNumSubregions);
		uint subRegionHeight = 0;
		while(mNumSubregions % subRegionWidth != 0 && subRegionWidth > 0)
		{
			subRegionWidth--;
		}
		if(subRegionWidth > 0)
		{	//this does not guarantee any sort of even distribution of the 
			//feature points.  The use of sub regions is handy because it
			//narrows the focus for each random index.  This would probably
			//be more useful if I tried to guarantee some sort of even
			//distribution over the regions, or attempt to keep to some sort of
			//density requirement, but that has not been done.
			subRegionHeight = (uint)mNumSubregions / subRegionWidth;
			for(int i=0;i<mNumFeaturePoints;++i)
			{
				uint regionRow = mRNG.Next() % subRegionHeight;
				uint regionCol = mRNG.Next() % subRegionWidth;

				//the minimum index of a row that contains an index that is
				//part of the subregion can be found by dividing the size of a
				//side by the subRegionHeight (which we already have in
				//subRegionWidth) and multiplying it by the row of the region.
				uint regionRowMin = subRegionWidth * regionRow;

				//The max row index is the minimum for the NEXT row, minus one,
				//so the feature row is just a random number from 0 to the width
				//plus the min
				uint featureRow = mRNG.Next() % (subRegionWidth) + regionRowMin;

				//I think the column info is found the exact same way
				uint regionColMin = subRegionHeight * regionCol;
				uint featureCol = mRNG.Next() % (subRegionHeight) + regionColMin;

				aFeaturePoints[i] = (int)(featureRow * featureCol);
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
