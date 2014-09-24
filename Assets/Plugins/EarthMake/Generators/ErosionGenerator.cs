using UnityEngine;
using System.Collections;

public class ErosionGenerator
{
	private float[] mOrigHeightMap;
	private float[] mCurHeightMap;

	public ErosionGenerator(float[] heightMap)
	{
		mOrigHeightMap = heightMap;
		mCurHeightMap = heightMap;
	}

	public float[] ThermalErosion(int iterations)
	{
		float talus = 12f / mCurHeightMap.Length;
		float c = 0.5f;
		int side = (int)Mathf.Sqrt(mCurHeightMap.Length);
		for(int i=0;i<iterations;++i)
		{
			for(int j=0;j<mCurHeightMap.Length;++j)
			{
				float[] neighborD = new float[4];
				float totD = 0f;
				float maxD = 0f;

				//first calculate stuff related to d, max and total
				for(int k=0;k<4;++k)
				{
					int nIdx = j;
					switch(k)
					{
					case 0:
						nIdx = j - side;
						break;
					case 1:
						nIdx = j + 1;
						break;
					case 2:
						nIdx = j - 1;
						break;
					case 3:
						nIdx = j + side;
						break;
					}
					if(nIdx < 0 || nIdx >= mCurHeightMap.Length)
					{
						continue;
					}
					float curD = mCurHeightMap[j] - mCurHeightMap[nIdx];
					neighborD[k] = curD;
					if(curD > talus)
					{
						if(curD > maxD)
						{
							maxD = curD;
							totD += curD;
						}
					}
				}

				//and now the consequences of the above calculations
				for(int k=0;k<4;++k)
				{
					int nIdx = j;
					switch(k)
					{
					case 0:
						nIdx = j - side;
						break;
					case 1:
						nIdx = j + 1;
						break;
					case 2:
						nIdx = j - 1;
						break;
					case 3:
						nIdx = j + side;
						break;
					}
					if(nIdx < 0 || nIdx >= mCurHeightMap.Length || neighborD[k] <= talus)
					{
						continue;
					}
					float toMove = c * neighborD[k] / totD;
					mCurHeightMap[nIdx] += toMove;
					mCurHeightMap[j] -= toMove;
				}
			}
		}

		return mCurHeightMap;

	}

	public float[] HydroErosion(int numIterations)
	{
		//too tired to really follow the paper at the moment.
		return mCurHeightMap;
	}

	public float[] HeightMap
	{
		get { return mCurHeightMap; }
		set { mCurHeightMap = value; }
	}

	public float[] OriginalHeightMap
	{
		get { return mOrigHeightMap; }
	}
}

