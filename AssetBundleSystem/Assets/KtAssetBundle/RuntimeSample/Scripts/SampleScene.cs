using UnityEngine;
using System.Collections;

public class SampleScene : MonoBehaviour {

	public string filePrefix = "http://www.angrypower.com/skyhigh/exampleScene.scene";
	public string sceneName = "BladeMaster Example";
	// Use this for initialization
	void Start () 
	{ 
		AssetBundleManager.LoadAssetBundle(this, filePrefix, filePrefix, 1, this.gameObject, "OnLoadedBundle");
 
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
		
	void OnLoadedBundle(BundleLoader loader)
	{ 
		if(loader.m_assetBundle != null)
		{
			Application.LoadLevel(sceneName);
		}
	}
}
