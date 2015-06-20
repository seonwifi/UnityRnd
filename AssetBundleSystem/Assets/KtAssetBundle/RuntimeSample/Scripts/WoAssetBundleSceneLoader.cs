using UnityEngine;
using System.Collections;

public class WoAssetBundleSceneLoader : MonoBehaviour {

	public float m_progress = 0;

	public static WoAssetBundleSceneLoader LoadLevelAsync(string v_levelName)
	{
		GameObject l_gameObject = new GameObject("LoadLevelAsync");
		WoAssetBundleSceneLoader l_assetBundleSceneLoader = l_gameObject.AddComponent<WoAssetBundleSceneLoader>();
		l_assetBundleSceneLoader.StartLoadLevelAsync(v_levelName);
		return l_assetBundleSceneLoader;
	}
	public static WoAssetBundleSceneLoader LoadLevel(string v_levelName)
	{
		GameObject l_gameObject = new GameObject("LoadLevel");
		WoAssetBundleSceneLoader l_assetBundleSceneLoader = l_gameObject.AddComponent<WoAssetBundleSceneLoader>();
		l_assetBundleSceneLoader.StartLoadLevel(v_levelName);
		return l_assetBundleSceneLoader;
	}
	// Use this for initialization
	void Start ()
	{


	}
	BundleLoader m_bundleLoader = null; 
	string m_levelName = "";
	void StartLoadLevelAsync(string v_levelName)
	{ 
		m_levelName = v_levelName;
		int version = 1;
		string l_url = AssetBundleManager.GetBaseURL() + v_levelName + AssetBundleManager.GetSceneExt();
 
		m_bundleLoader = new BundleLoader(); 
		m_bundleLoader.SetLoadComplateFunction(this.gameObject, "OnStartLevelLoad");
		StartCoroutine(AssetBundleManager.LoadLevelAsyncCoroutine(l_url, version, m_bundleLoader, v_levelName)); 
 
	}
	void StartLoadLevel(string v_levelName)
	{ 
		StartCoroutine(StartLoadLevelCorutine(v_levelName));   
	}
	IEnumerator StartLoadLevelCorutine(string v_levelName)
	{
		m_levelName = v_levelName;
		int version = 1;
		string l_url = AssetBundleManager.GetBaseURL() + v_levelName + AssetBundleManager.GetSceneExt();
		
		m_bundleLoader = new BundleLoader(); 
		m_bundleLoader.SetLoadComplateFunction(this.gameObject, "OnStartLevelLoad");
		yield return StartCoroutine(AssetBundleManager.LoadLevelCoroutine(l_url, version, m_bundleLoader, v_levelName)); 
	}

	public void OnStartLevelLoad(BundleLoader v_bundleLoader)
	{
		 
	}

	// Update is called once per frame
	void Update () 
	{
		if(m_bundleLoader != null)
		{
			m_progress = m_bundleLoader.m_downLoadProgress;	
		} 
	}
}
