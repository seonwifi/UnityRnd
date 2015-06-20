using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BundleLoader
{ 
	public AssetBundle 			m_assetBundle = null;
	public bool					m_isLoaded	= false;
	public bool					m_isLoading	= false;
	public string				m_name = "";
	public GameObject 			m_sendMessageGo = null;
	public string 				m_messageFunction = ""; 
	public float			    m_downLoadProgress = 0;
	public bool					m_isRemove = false;
	public string				m_error = "";
	public string				m_url = "";
	public void SetLoadComplateFunction(GameObject messageGo, string function)
	{
		m_sendMessageGo   = messageGo;
		m_messageFunction = function;
		if(m_isLoaded)
		{ 
			if(m_sendMessageGo != null)
			{
				m_sendMessageGo.SendMessage(m_messageFunction, this, SendMessageOptions.DontRequireReceiver);
			}
		} 
	} 
}

static public class AssetBundleManager 
{
 	static AssetBundleManager ()
	{
      	 m_bundles = new Dictionary<string, BundleLoader>();
		m_sceneBundles = new Dictionary<string, BundleLoader>();
   	}
	
	static Dictionary<string, BundleLoader> m_bundles = new Dictionary<string, BundleLoader>();
	static Dictionary<string, BundleLoader> m_sceneBundles = new Dictionary<string, BundleLoader>();
	public static BundleLoader GetBundleLoader(string name)
	{
		BundleLoader bundleLoader = null;
        if (m_bundles.TryGetValue(name, out bundleLoader))
		{
			return bundleLoader; 
		} 
		return null;
	}
	
	public static bool ContainBundleLoader(string name)
	{
		return m_bundles.ContainsKey(name);
	}

	public static BundleLoader LoadAssetBundle(MonoBehaviour mono, string url, string name, int version, GameObject messageGo, string function)
	{
		BundleLoader bundleLoader = null;
		if(m_bundles.ContainsKey(name))
		{
			bundleLoader = m_bundles[name];
		}
		else
		{
			bundleLoader = new BundleLoader();  
			bundleLoader.m_url = url;
			//m_bundles[name] = bundleLoader;
			bundleLoader.SetLoadComplateFunction(messageGo, function);
			mono.StartCoroutine(Load(url, name, version, bundleLoader));
			 
		}
		return bundleLoader;
	}
 
 	public static IEnumerator Load(string url, string name, int version, BundleLoader bundleLoader)
	{  
 	
		bundleLoader.m_assetBundle = null;
		bundleLoader.m_isLoading = true;
		// Wait for the Caching system to be ready
        while (!Caching.ready)
        { 
            yield return new WaitForSeconds(0.1f);
        }  
		 
		using (WWW www = WWW.LoadFromCacheOrDownload(url, version))
        {    
			while(!www.isDone)
			{
 
				bundleLoader.m_downLoadProgress = www.progress;
				yield return null;
			}//*/
			 
			bundleLoader.m_downLoadProgress = 1;
           // yield return bundleLoader.m_www;
            if (www.error != null)
            {
				Debug.LogError(url + "AssetBundle - WWW download:" + www.error); 
            }  
			 
			 
			if(bundleLoader.m_isRemove)
			{
 				if(www.assetBundle != null)
				{
					www.assetBundle.Unload(true);
					www.Dispose();
				}
			}
			else
			{
				Debug.Log("Keep AssetBundle:" + url);
				bundleLoader.m_assetBundle 	    = www.assetBundle;
				bundleLoader.m_isLoaded 		= true; 
				if(bundleLoader.m_sendMessageGo != null)
				{
					bundleLoader.m_sendMessageGo.SendMessage(bundleLoader.m_messageFunction, bundleLoader, SendMessageOptions.DontRequireReceiver);
				}
			} 
        }	 
	}

	public static BundleLoader DownLoadAssetBundle(MonoBehaviour mono, string url, string name, int version, GameObject messageGo, string function)
	{
		BundleLoader bundleLoader = null; 
		bundleLoader = new BundleLoader();   
		bundleLoader.m_url = url;
		bundleLoader.SetLoadComplateFunction(messageGo, function);
		mono.StartCoroutine(DownLoad(url, name, version, bundleLoader)); 
		return bundleLoader;
	}

	public static IEnumerator DownLoad(string url, string name, int version, BundleLoader bundleLoader)
	{  
		bundleLoader.m_assetBundle = null;
		bundleLoader.m_isLoading = true;
		// Wait for the Caching system to be ready
		while (!Caching.ready)
		{ 
			yield return new WaitForSeconds(0.1f);
		}  
		
		using (WWW www = WWW.LoadFromCacheOrDownload(url, version))
		{    
			while(!www.isDone)
			{ 
				bundleLoader.m_downLoadProgress = www.progress;
				yield return null;
			}//*/
			
			bundleLoader.m_downLoadProgress = 1;
			// yield return bundleLoader.m_www;
			bundleLoader.m_error = www.error;
			if (string.IsNullOrEmpty(www.error) == false)
			{ 
				Debug.LogError(url + "AssetBundle - WWW download:" + www.error); 
			}  
			 
			if(bundleLoader.m_isRemove)
			{
				if(www.assetBundle != null)
				{
					www.assetBundle.Unload(true);
					www.Dispose();
				}
			}
			else
			{
				bundleLoader.m_assetBundle 	    = www.assetBundle;
				bundleLoader.m_isLoaded 		= true; 
				if(bundleLoader.m_sendMessageGo != null)
				{
					bundleLoader.m_sendMessageGo.SendMessage(bundleLoader.m_messageFunction, bundleLoader, SendMessageOptions.DontRequireReceiver);
				} 
			} 
		}
		System.GC.Collect();
		yield return null;
	}


	public static void UnLoad(string name, bool allObjects)
	{
		BundleLoader bundleLoader = null;
       if (m_bundles.TryGetValue(name, out bundleLoader))
		{
			if(bundleLoader.m_assetBundle != null)
			{  bundleLoader.m_isRemove = true;
			   bundleLoader.m_sendMessageGo = null;
	           bundleLoader.m_assetBundle.Unload(allObjects);
	           bundleLoader.m_assetBundle = null;
	           m_bundles.Remove(name);
			} 
       } 
	}
   	public static void RemoveAllAssetBundle()
	{
		foreach(KeyValuePair<string, BundleLoader> keyValue in m_bundles)
		{
			keyValue.Value.m_isRemove 	= true;
			keyValue.Value.m_sendMessageGo = null;
			if(keyValue.Value.m_assetBundle != null)
			{ 
				keyValue.Value.m_assetBundle.Unload(true);
				keyValue.Value.m_assetBundle = null;
			}
			keyValue.Value.m_sendMessageGo = null;
		}
		m_bundles.Clear(); 
	}
   	public static void CleanAllCashFile()
	{
		RemoveAllAssetBundle();
		Caching.CleanCache();
	}
	static string m_baseURL = "";
	static string m_sceneExt = ".scene3d";
	static string m_resourceExt = ".unity3d";
	public static void SetBaseURL(string baseURL)
	{
		m_baseURL = baseURL;
	}
	public static void SetSceneExt(string sceneExt)
	{
		m_sceneExt = sceneExt;
	}
	public static void SetResourceExt(string resourceExt)
	{
		m_resourceExt = resourceExt;
	}
	public static string GetBaseURL()
	{
		return m_baseURL;
	}
	public static string GetSceneExt()
	{
		return m_sceneExt;
	}
	public static string GetResourceExt()
	{
		return m_resourceExt;
	}
	//static Dictionary<string, SceneAssetBundleInfo> m_bundles = new Dictionary<string, BundleLoader>();
 

	public static void LoadLevelAsync(MonoBehaviour mono, string url, int version, GameObject messageGo, string function, string levelName)
	{
		BundleLoader bundleLoader = null; 
		bundleLoader = new BundleLoader(); 
		bundleLoader.m_url = url;
		bundleLoader.SetLoadComplateFunction(messageGo, function);
		mono.StartCoroutine(LoadLevelAsyncCoroutine(url, version, bundleLoader, levelName)); 
	}
	public static IEnumerator LoadLevelAsyncCoroutine(string url, int version, BundleLoader bundleLoader, string levelName)
	{  
		System.GC.Collect();
		AssetBundle l_assetBundle = null;
		if(m_sceneBundles.ContainsKey(url) == false)
		{
			// Wait for the Caching system to be ready
			while (!Caching.ready)
			{ 
				yield return new WaitForSeconds(0.1f);
			}  
			
			using (WWW www = WWW.LoadFromCacheOrDownload(url, version))
			{    
				while(!www.isDone)
				{
					
					bundleLoader.m_downLoadProgress = www.progress;
					yield return null;
				} 
				
				bundleLoader.m_downLoadProgress = 1;
				// yield return bundleLoader.m_www;
				bundleLoader.m_error = www.error;
				if (string.IsNullOrEmpty(www.error) == false)
				{
					Debug.LogError(url + "AssetBundle LoadLevelAsync - WWW download:" + www.error); 
				}
				
				if(bundleLoader.m_isRemove)
				{
					if(www.assetBundle != null)
					{
						www.assetBundle.Unload(true);
						www.Dispose();
					}
				}
				else
				{ 
					l_assetBundle = www.assetBundle; 
					bundleLoader.m_isLoaded	= true;   
				}  
			}  
			BundleLoader newBundleLoader = null; 
			newBundleLoader = new BundleLoader(); 
			newBundleLoader.m_url = url;
			newBundleLoader.m_assetBundle = l_assetBundle;
			m_sceneBundles.Add(url, newBundleLoader);
		}


		/*
		if(bundleLoader.m_sendMessageGo != null)
		{
			bundleLoader.m_sendMessageGo.SendMessage(bundleLoader.m_messageFunction, bundleLoader, SendMessageOptions.DontRequireReceiver);
		} //*/

		if(l_assetBundle != null)
		{
 
		} 
		AsyncOperation asyncOperation = Application.LoadLevelAsync(levelName);
		if(asyncOperation != null)
		{
			while(!asyncOperation.isDone)
			{ 
				bundleLoader.m_downLoadProgress = asyncOperation.progress;
				yield return null;
			} 
		} 
		OptimizeScene( url);
		System.GC.Collect();
		yield return null;
	}

	public static IEnumerator LoadLevelCoroutine(string url, int version, BundleLoader bundleLoader, string levelName)
	{   
		System.GC.Collect();
		AssetBundle l_assetBundle = null;
		if(m_sceneBundles.ContainsKey(url) == false)
		{
			// Wait for the Caching system to be ready
			while (!Caching.ready)
			{ 
				yield return new WaitForSeconds(0.1f);
			}  
			
			using (WWW www = WWW.LoadFromCacheOrDownload(url, version))
			{    
				while(!www.isDone)
				{
					bundleLoader.m_downLoadProgress = www.progress;
					yield return null;
				} 
				
				bundleLoader.m_downLoadProgress = 1;
				// yield return bundleLoader.m_www;
				bundleLoader.m_error = www.error;
				if (string.IsNullOrEmpty(www.error) == false)
				{
					Debug.LogError(url + "AssetBundle LoadLevel- WWW download:" + www.error); 
				}
				
				if(bundleLoader.m_isRemove)
				{
					if(www.assetBundle != null)
					{
						www.assetBundle.Unload(true);
						www.Dispose();
					}
				}
				else
				{ 
					l_assetBundle = www.assetBundle; 
					bundleLoader.m_isLoaded	= true;   
				}  
			}  
			/*
			if(bundleLoader.m_sendMessageGo != null)
			{
				bundleLoader.m_sendMessageGo.SendMessage(bundleLoader.m_messageFunction, bundleLoader, SendMessageOptions.DontRequireReceiver);
			} //*/
			
			if(l_assetBundle != null)
			{
				
			} //*/
			BundleLoader newBundleLoader = null; 
			newBundleLoader = new BundleLoader(); 
			newBundleLoader.m_url = url;
			newBundleLoader.m_assetBundle = l_assetBundle;
			m_sceneBundles.Add(url, newBundleLoader);
		}
		else
		{

		}
 
		Application.LoadLevel(levelName);  
	 
		OptimizeScene( url);
		System.GC.Collect(); 
		yield return null;
	}
	static void OptimizeScene(string currentUrl)
	{ 
		List<string> removeList = new List<string>();
		foreach (KeyValuePair<string, BundleLoader> objPair in m_sceneBundles)
		{ 
			if(objPair.Key != currentUrl)
			{
				BundleLoader bundleLoader = objPair.Value;
				if(bundleLoader != null && bundleLoader.m_assetBundle != null)
				{
					bundleLoader.m_assetBundle.Unload (true);
					bundleLoader.m_assetBundle = null;
				}
				removeList.Add(objPair.Key);

			} 
		}

		foreach ( string removeUrl in removeList)
		{
			m_sceneBundles.Remove(removeUrl);
		} 
	}
}


