using UnityEngine;
using System.Collections;

public class SceneAssetBundleLoader : MonoBehaviour {

 
	string url = "file://E:/AssetBundleSample/AssetBundles/AssetBundleSampleAndroidScene0.unity3d";
	WWW www = null;
	
	IEnumerator Start () 
	{   
	    // Start a download of the given URL
	   using (www = WWW.LoadFromCacheOrDownload (url, 1))
		{
		    // Wait for download to complete
		    yield return www;
		
		    // Load and retrieve the AssetBundle
		    AssetBundle bundle = www.assetBundle;
	 
			Application.LoadLevel("NoAssetBundleScene");
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
		} //*/
	}
}
