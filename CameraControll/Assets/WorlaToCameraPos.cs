using UnityEngine;
using System.Collections;

public class WorlaToCameraPos : MonoBehaviour {
	
	public Camera	  camera;
	public Transform NoParentObject;
	public Transform CameraParentObject;
	public Vector3   NoParentCameraPos = new Vector3(0,0,0);
	public Vector3   CameraParentLocalPos = new Vector3(0,0,0);
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		NoParentCameraPos = camera.worldToCameraMatrix.MultiplyPoint(NoParentObject.position);
		
		guiText.text = "\n NoParentObject: ";
		guiText.text += NoParentCameraPos.ToString();
		guiText.text += "\n CameraParentlocalPosition: ";
		guiText.text += CameraParentObject.localPosition;
		CameraParentLocalPos = CameraParentObject.localPosition;
	
	}
}
