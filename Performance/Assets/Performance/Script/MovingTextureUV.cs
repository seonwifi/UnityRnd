using UnityEngine;
using System.Collections;

public class MovingTextureUV : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		float deltaTime = Monitoring.GetDeltaTime();
		
		Vector2 offset = this.renderer.material.mainTextureOffset;
		offset.x += deltaTime;
		this.renderer.material.mainTextureOffset = offset;
 
	}

}
