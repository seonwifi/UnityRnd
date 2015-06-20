using UnityEngine;
using System.Collections;

public class SinPos : MonoBehaviour {

	Vector3 Pos;
	// Use this for initialization
	void Start ()
	{
		Pos = transform.position;
	}
	float speed = 0;
	// Update is called once per frame
	void LateUpdate () 
	{
		speed += Time.smoothDeltaTime*10;//Monitoring.GetDeltaTime();
		float scale = (Mathf.Sin(speed)); 
		Vector3 tPos = Pos;
		tPos.y *= scale;
		transform.position = tPos;
	}
	

}
