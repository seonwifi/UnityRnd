using UnityEngine;
using System.Collections;

public class KtBundleLoader : MonoBehaviour {

	string url = "file://E:/AssetBundleSample/AssetBundles/AssetBundleFileName14.unity3d";
	IEnumerator Start () 
	{ 
	    // Start a download of the given URL
	   using ( WWW www = WWW.LoadFromCacheOrDownload (url, 1))
		{
		    // Wait for download to complete
		    yield return www;
		
		    // Load and retrieve the AssetBundle
		    AssetBundle bundle = www.assetBundle;
	 
				//string matPath = "Assets/NewMaterial01.mat";
				//UnityEngine.Object testObject = AssetDatabase.LoadAssetAtPath("Assets/ContentsData/Cube.prefab", typeof(GameObject));
				//UnityEngine.Object testObject2 = AssetDatabase.LoadAssetAtPath("Assets/ContentsData/CubeTexture.png", typeof(Texture));
				//UnityEngine.Object testObject3 = AssetDatabase.LoadAssetAtPath("Assets/ContentsData/CubeTextureMat.mat", typeof(Material));
				//string bundlePath = Application.dataPath + exportLocation + "/prefabSample.unity3d";
			
		    // Load the GameObject 
	 		UnityEngine.Object []objs = bundle.LoadAll();
			foreach(UnityEngine.Object obj in objs)
			{
				 GameObject go = obj  as GameObject;
				if(go != null)
				{
					Instantiate(go);
				}
			} 
		} 
	}
}
