using UnityEngine;
using System.Collections;

public class follow : MonoBehaviour {
	
	public Transform followpos;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		transform.position = followpos.position;
	}
}
