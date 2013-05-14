using UnityEngine;
using System.Collections;

public class CreateBalls : MonoBehaviour
{
	public GameObject ballPrefab;
	public float forceMin = 100.0f;
	public float forceMax = 1000.0f;
	void Start () 
	{
		for(int i=0;i<1000;++i)
		{
			GameObject go = (GameObject)GameObject.Instantiate(ballPrefab, (new Vector3(Random.Range(-750,750),ballPrefab.transform.localScale.y/2.0f,Random.Range (-750,750))), Quaternion.identity);
			go.transform.rigidbody.AddForce(Random.Range (forceMin, forceMax)*(new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f))).normalized, ForceMode.Impulse);
		}
	}
}