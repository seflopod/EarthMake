using UnityEngine;
using System.Collections;

public static class CombinerFunctions
{
	public enum CombineFunction
	{
		D1,
		D2MinusD1,
		D3MinusD1
	};

	public static float D1(float[] distances)
	{
		if(distances.Length >= 1)
		{
			return distances[0];
		}
		return 0f;
	}

	public static float D2MinusD1(float[] distances)
	{
		if(distances.Length >= 2)
		{
			return distances[1] - distances[0];
		}
		return 0f;
	}

	public static float D3MinusD1(float[] distances)
	{
		if(distances.Length >= 3)
		{
			return distances[2] - distances[0];
		}
		return 0f;
	}

	public static CombiningFunc GetFunction(CombineFunction combineFunc)
	{
		CombiningFunc ret;
		switch(combineFunc)
		{
		case CombineFunction.D1:
			ret = new CombiningFunc(D1);
			break;
		case CombineFunction.D2MinusD1:
			ret = new CombiningFunc(D2MinusD1);
			break;
		case CombineFunction.D3MinusD1:
			ret = new CombiningFunc(D3MinusD1);
			break;
		default:
			ret = new CombiningFunc(D2MinusD1);
			break;
		}
		return ret;
	}
}