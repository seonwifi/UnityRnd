using UnityEngine;
using System.Collections;

public class Sin : MonoBehaviour {
	
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
		float scale = (Mathf.Sin(speed)+2)*1; 
		transform.localScale = localScale*scale;
	}
}
