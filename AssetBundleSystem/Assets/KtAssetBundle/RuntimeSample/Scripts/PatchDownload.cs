using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatchDownload : MonoBehaviour {
	
	[System.Serializable]
	public class PatchInfo
	{
		public string 	m_uri;
		public int 		m_version;
		public bool 	m_downLoaded = false;
		BundleLoader  m_bundleLoader;
		public BundleLoader GetBundleLoader()
		{
			return m_bundleLoader;
		}
		public void SetBundleLoader(BundleLoader bundleLoader)
		{
			m_bundleLoader = bundleLoader;
		}
	}
	
	public List<PatchInfo> m_patchInfos = new List<PatchInfo>();
	public bool m_seqLoad = false;
	public bool m_bDownLoading = false;
	PatchInfo m_currentDownLoad = null;

	// Use this for initialization
	void Start () 
	{  
		//LoVersion.mjPatchVersion = 1;
		//DontDestroyOnLoad(this.gameObject);
		SystemProfiler.StartProfile();

		//AssetBundleManager.SetBaseURL("http://192.168.92.21:8000/");
		AssetBundleManager.SetBaseURL("file://F:/UnityProject//R&D/AssetBundleSystem/AssetBundle/Android/");
	}

	float progress = 0;
 
	string UrlToAssetBundleFilName(string url) 
	{
		string []urlSplit = url.Split(new char[]{'/','\\'});
		string assetBundleFilName = urlSplit[urlSplit.Length-1];
		string []assetBundleFilNameSplit = assetBundleFilName.Split(new char[]{'.'});
		return assetBundleFilNameSplit[0];
	}

	// Update is called once per frame 
	public static LoAssetBundleDatabase m_assetBundleDatabase = new LoAssetBundleDatabase();
	LoPatchList 			m_patchList = new LoPatchList(); 
	IEnumerator DownloadCache() 
	{
		if(m_bDownLoading == false)
		{
			m_bDownLoading = true;
			
			yield return StartCoroutine(DownLoadPatchList(AssetBundleManager.GetBaseURL() + "PatchList.txt"));
			
			yield return new WaitForEndOfFrame();
			foreach(LoPatchList.LoPatchListInfo patchListInfo in m_patchList.m_assetbundleResourceDatabaseList)
			{
				string resourceDatabasePath = AssetBundleManager.GetBaseURL() + patchListInfo.m_file;
				yield return StartCoroutine(DownLoadAssetDatabase(resourceDatabasePath));
			}
			yield return new WaitForEndOfFrame();
			m_patchInfos.Clear();
			foreach(LoPatchList.LoPatchListInfo patchListInfo in m_patchList.m_patchInfoList)
			{
				PatchInfo patchInfo = new PatchInfo();
				patchInfo.m_uri = AssetBundleManager.GetBaseURL() + patchListInfo.m_file;
				patchInfo.m_version = patchListInfo.m_version;
				m_patchInfos.Add(patchInfo); 
			}
			
			if(Caching.enabled)
			{
				Debug.Log("Caching.enabled == true");
			}
			else
			{
				Debug.LogError("Caching.enabled == false");
			}
			/*
		if(m_patchInfos.Count > 0)
		{
			m_currentDownLoad = m_patchInfos[0];
		}//*/
			
			//AssetBundleManager.RemoveAllAssetBundle();
			foreach(PatchInfo patchInfo in m_patchInfos)
			{
				string assetBundleName = UrlToAssetBundleFilName(patchInfo.m_uri);
				
				BundleLoader bundleLoader = null; 
				bundleLoader = new BundleLoader();   
				bundleLoader.SetLoadComplateFunction(this.gameObject, "OnLoadedBundle");
				patchInfo.SetBundleLoader(bundleLoader);
				bundleLoader.m_url = patchInfo.m_uri;
				yield return StartCoroutine(AssetBundleManager.DownLoad(patchInfo.m_uri, assetBundleName, patchInfo.m_version, bundleLoader)); 
				 
			} 
			//*/
			//m_bDownLoading = false;
		}

		yield return null;
	}
	void DownloadPatch(PatchInfo v_patchInfo) 
	{
		 
		StartCoroutine(DownloadPatchCoroutine(v_patchInfo)); 
	}
	IEnumerator DownloadPatchCoroutine(PatchInfo v_patchInfo) 
	{
		BundleLoader bundleLoader = null; 
		bundleLoader = new BundleLoader();   
		bundleLoader.SetLoadComplateFunction(this.gameObject, "OnLoadedBundle");
		string assetBundleName = UrlToAssetBundleFilName(v_patchInfo.m_uri);
		bundleLoader.m_url = v_patchInfo.m_uri;
		StartCoroutine(AssetBundleManager.DownLoad(v_patchInfo.m_uri, assetBundleName, v_patchInfo.m_version, bundleLoader)); 
		yield return null;
	}

	public IEnumerator DownLoadAssetDatabase(string url)
	{   
		WWW www = new WWW(url);
		yield return www;
		string textAssetDatabase = www.text;
		textAssetDatabase = textAssetDatabase.ToString();
		m_assetBundleDatabase = new LoAssetBundleDatabase();
		m_assetBundleDatabase.LoadCsv(textAssetDatabase);
	}


	public IEnumerator DownLoadPatchList(string url)
	{   //PatchList.txt

		WWW www = new WWW(url);
		yield return www;
		string textAssetDatabase = www.text;
		m_patchList.LoadCSV(textAssetDatabase); 
	} 

	void Update () 
	{
		if(m_bDownLoading)
		{
			progress = 0;
			foreach(PatchInfo patchInfo in m_patchInfos)
			{ 
				if(Caching.IsVersionCached(patchInfo.m_uri, 1))
				{
					progress += 1;
				}
				else
				{
					if(patchInfo.GetBundleLoader() != null)
					{
						progress += patchInfo.GetBundleLoader().m_downLoadProgress;
					} 
				}
				
			}
			if(m_patchInfos.Count > 0)
			{
				progress = progress/(float)m_patchInfos.Count;
			} 
			 
		} 

		if(m_bDownLoading == false)
		{
			if(m_currentDownLoad != null)
			{
				DownloadPatch(m_currentDownLoad);
			}
		}
	}
 
	WoAssetBundleSceneLoader m_assetBundleSceneLoader = null;
	Vector2 scrollPos = new Vector2(0,0);
	void OnGUI()
	{
		float downLoadPer = progress*100.0f;
		string report = "Download: " + downLoadPer.ToString("0.00") + "%";
		int heightCoord = 30;
		GUI.Label(new Rect(0,0,150,30), report); 
		if(GUI.Button(new Rect(Screen.width -150, heightCoord,150,30), "DownloadCash"))
		{
			StartCoroutine(DownloadCache());
		}		
		heightCoord += 50;
		if(GUI.Button(new Rect(Screen.width -150, heightCoord,150,30), "CleanCash"))
		{
			AssetBundleManager.CleanAllCashFile();
		} 
		if(bAllLoadedCaching)
		{
			heightCoord += 50;
			GUI.Label(new Rect(Screen.width -150, heightCoord,150,30), "DownLoad Complete");
		}
		heightCoord += 50;
		if(GUI.Button(new Rect(Screen.width -150, heightCoord, 150,30), "GC.Collect()"))
		{
			System.GC.Collect();
		} 

		heightCoord += 50;
		if(GUI.Button(new Rect(Screen.width -150, heightCoord, 150,30), "Load Level"))
		{
			m_assetBundleSceneLoader = WoAssetBundleSceneLoader.LoadLevelAsync("loader");
		} 
		if(m_assetBundleSceneLoader != null)
		{
			heightCoord += 50;
			GUI.Label(new Rect(Screen.width -150, heightCoord, 150,30), "LoadLevel:" + ((int)(m_assetBundleSceneLoader.m_progress*100)).ToString() + "%"); 
		}

		heightCoord += 50;
		if(GUI.Button(new Rect(Screen.width -150, heightCoord, 150,30), "Load Level AllLoadPrefab"))
		{

			Application.LoadLevel("AssetBundleDatabaseSample");
		}  
	}
	bool bAllLoadedCaching = false; 
	void OnLoadedBundle(BundleLoader loader)
	{ 
		if(loader.m_assetBundle != null)
		{
			string []urlSplit = loader.m_url.Split(new char[]{'/','\\'});
			string []urlSplitName = urlSplit[urlSplit.Length-1].Split(new char[]{'.'});

			m_assetBundleDatabase.SetAssetBundle(urlSplitName[0], loader.m_assetBundle);
			loader.m_assetBundle.Unload(false);

			loader.m_assetBundle = null;
		}
		else
		{

		}
		bAllLoadedCaching = true; 
		foreach(PatchInfo patchInfo in m_patchInfos)
		{
			if(Caching.IsVersionCached(patchInfo.m_uri, patchInfo.m_version) == false)
			{
				bAllLoadedCaching = false;
			}
		} 

		if(bAllLoadedCaching == true)
		{

		}
		//m_bDownLoading = false;
	}
}
 
 



