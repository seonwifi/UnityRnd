using UnityEngine;
using System.Collections;

public class AxisAngle : MonoBehaviour {
	
	public enum MultiplyOther
	{
		Front = 0,
		Back  = 1,
	};
	public MultiplyOther multiplyOther = MultiplyOther.Front;
	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () 
	{
		Vector3 forward = transform.forward;
		float rad = 3.14f*Time.deltaTime;
 
		//Rotate2d.RotateInOut(ref forward.x, ref forward.z, rad); 
		//forward.Normalize();
		transform.forward = forward;
		if(multiplyOther == MultiplyOther.Front)
		{
			//Quaternion.AngleAxis First
			//transform.rotation = transform.rotation*Quaternion.AngleAxis(0.3f, new Vector3(0.0f,1.0f,0.0f));
		}
		else
		{
			//Quaternion.AngleAxis Back
			//transform.rotation = Quaternion.AngleAxis(0.3f, new Vector3(0.0f,1.0f,0.0f))* transform.rotation;
		}

 
	}
}
