using UnityEngine;
using System.Collections;
 
public class SampleSound : MonoBehaviour {
	public string filePrefix = "http://www.angrypower.com/skyhigh/sound.unity3d";
	
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
		if(loader.m_assetBundle == null)
		{
			return;
		}
		Object []objs = loader.m_assetBundle.LoadAll();
		AudioClip audioC = null;
		foreach(Object obj in objs)
		{
			if(obj as AudioClip != null)
			{
				audioC = obj as AudioClip;
				break;
			}	
		}
		loader.m_assetBundle.Unload(false);
		//AudioClip audioC = loader.m_assetBundle.Load("Waiting_Room_Song") as AudioClip;
 		if(audioC != null)
		{
			this.audio.PlayOneShot(audioC);
		} 
	}
}
