using UnityEngine;
using System.Collections;

public class CharacterAnimation : MonoBehaviour {

	public WoTrailLauncher m_TrailLauncher;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Ae_BeginTrail()
	{
		m_TrailLauncher.EmitOn();
	}
	void Ae_EndTrail()
	{
		m_TrailLauncher.EmitOff();
	}
}
