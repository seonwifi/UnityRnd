using UnityEngine;
using System.Collections;

public class XmlSampleTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		XMLReadWrite.LodXml("F:/UnityProject/Samples/FileSample/Assets/settings.xml");
		XMLReadWrite.SaveXml("F:/UnityProject/Samples/FileSample/xmlSample.xml");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		
	}
}
