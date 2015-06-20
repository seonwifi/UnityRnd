using UnityEngine;
using System.Collections;

public class SamplePrefab : MonoBehaviour {

	public string filePrefix = "http://www.angrypower.com/skyhigh/prefab.unity3d";
	public int 	  m_version = 1;
	public string m_targetGameObjectName = "";
	// Use this for initialization
	void Start () 
	{ 
		SystemProfiler.StartProfile();
		AssetBundleManager.LoadAssetBundle(this, filePrefix, filePrefix, m_version, this.gameObject, "OnLoadedBundle");
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	//http://106.240.255.155:1280/file/kt
	void OnLoadedBundle(BundleLoader loader)
	{ 
		if(loader.m_assetBundle != null)
		{
			if(string.IsNullOrEmpty(m_targetGameObjectName))
			{
				Object []objs = loader.m_assetBundle.LoadAll();
				
				foreach(Object obj in objs)
				{
					if(obj as GameObject != null)
					{ 
						Instantiate(obj); 
					}	
				} 
			}
			else
			{
				Object obj = loader.m_assetBundle.Load(m_targetGameObjectName, typeof(GameObject));
				if(obj != null)
				{
					Instantiate(obj);
				} 
				else
				{
					Debug.LogError("Can't Load:" + m_targetGameObjectName);
				}
			}  
		}
		//loader.m_assetBundle.Unload(false);
		//System.GC.Collect();
	}
}
