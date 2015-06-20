using UnityEngine;
using System.Collections;

public class SinScaleX : MonoBehaviour {

	Vector3 localScale;
	// Use this for initialization
	void Start ()
	{
		localScale = transform.localScale;
	}
	float speed = 0;
	// Update is called once per frame
	void LateUpdate () 
	{
		speed += Time.smoothDeltaTime*10;//Monitoring.GetDeltaTime();
		float scale = 1+(Mathf.Sin(speed)+1)*0.3f; 
		Vector3 tLocalScale = localScale;
		tLocalScale.x *= scale;
		transform.localScale = tLocalScale;
	}
}
