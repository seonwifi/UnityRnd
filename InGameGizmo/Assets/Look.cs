using UnityEngine;
using System.Collections;

public class Look : MonoBehaviour {
	
	public IGGArrowImage m_IGGArrowImage;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_IGGArrowImage.SetEndPos(this.transform.position);
	}
}
