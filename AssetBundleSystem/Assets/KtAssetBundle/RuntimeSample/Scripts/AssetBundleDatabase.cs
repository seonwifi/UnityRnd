using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetBundleDatabase : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{



		if(PatchDownload.m_assetBundleDatabase != null)
		{
			List<Object> objects = PatchDownload.m_assetBundleDatabase.LoadAll();
			for(int i = 0; i < objects.Count; ++i)
			{
				GameObject go = objects[i] as GameObject;
				if(go != null)
				{
					Instantiate(go);
				}
				else
				{

				}
				Debug.Log("Type is: " + objects[i].GetType().Name);
			}
		}
	
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
}
