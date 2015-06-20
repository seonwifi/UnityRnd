using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LoAssetBundleDatabase 
{
	public const string asset_bundle_type_resource 			= "resource";
	public const string asset_bundle_type_scene	 			= "scene";
	public const string asset_bundle_type_resource_data_base	= "resource_data_base";
	public class BundleDatabaseInfo
	{
		public int m_index;
		public int m_version;
		public string m_bundleName;
		public string m_assetbundleObjectName;
		public System.Type m_type;

		public AssetBundle m_assetbundle;
		public Object	   m_refObject;
	}

	List<BundleDatabaseInfo> m_bundleList = new List<BundleDatabaseInfo>();
	Dictionary<string, BundleDatabaseInfo> m_bundleResourceDic = new Dictionary<string, BundleDatabaseInfo>();
	Dictionary<string, BundleDatabaseInfo> m_bundleSceneDic = new Dictionary<string, BundleDatabaseInfo>();
	public void AddResourceAssetBundleInfo(string path, BundleDatabaseInfo assetBundleInfo)
	{
		m_bundleResourceDic[path] = assetBundleInfo;
	}
	public void AddSceneAssetBundleInfo(string path, BundleDatabaseInfo assetBundleInfo)
	{
		m_bundleSceneDic[path] = assetBundleInfo;
	}

	public Object Load(string v_name)
	{
		BundleDatabaseInfo l_bundleDatabaseInfo = null;
		if(m_bundleResourceDic.TryGetValue(v_name, out l_bundleDatabaseInfo))
		{
			if(l_bundleDatabaseInfo.m_refObject == null)
			{
				l_bundleDatabaseInfo.m_refObject = l_bundleDatabaseInfo.m_assetbundle.Load(l_bundleDatabaseInfo.m_assetbundleObjectName);
				l_bundleDatabaseInfo.m_assetbundle.Unload(false);
				//m_assetbundle
			}
			return l_bundleDatabaseInfo.m_refObject;
		} 
		return null;
	}
	public List<Object> LoadAll()
	{
		List<Object>  loadedObject = new List<Object>();
		for(int i = 0; i < m_bundleList.Count; ++i)
		{
			if(m_bundleList[i].m_refObject != null)
			{
				loadedObject.Add(m_bundleList[i].m_refObject);
			}
			else
			{
				Debug.LogError("LoadFail:" + m_bundleList[i].m_assetbundleObjectName);
			}
		}
		return loadedObject;
	}

	public int GetIndex(string v_name)
	{
		BundleDatabaseInfo l_bundleDatabaseInfo = null;
		if(m_bundleResourceDic.TryGetValue(v_name, out l_bundleDatabaseInfo))
		{
			return l_bundleDatabaseInfo.m_index;
		}
		return -1;
	}

	public Object LoadFromIndex(int v_index)
	{
		if(v_index >= 0 && v_index < m_bundleList.Count)
		{
			BundleDatabaseInfo l_bundleDatabaseInfo = m_bundleList[v_index];
			if(l_bundleDatabaseInfo.m_refObject == null)
			{
				l_bundleDatabaseInfo.m_refObject = l_bundleDatabaseInfo.m_assetbundle.Load(l_bundleDatabaseInfo.m_assetbundleObjectName);
				l_bundleDatabaseInfo.m_assetbundle.Unload(false); 
			}
			return l_bundleDatabaseInfo.m_refObject;
		}
		return null;
	}
	public void SetAssetBundle()
	{
 
	}

	public void BuildIndexCash()
	{
		m_bundleList.Clear();
		foreach (KeyValuePair<string, BundleDatabaseInfo> objPair in m_bundleResourceDic)
		{ 
			BundleDatabaseInfo l_bundleDatabaseInfo = objPair.Value; 
			m_bundleList.Add(l_bundleDatabaseInfo);
			l_bundleDatabaseInfo.m_index = m_bundleList.Count-1;
		}
	}

	public void SaveFileCsv(string path)
	{  
		StreamWriter streamWriter = new StreamWriter(path);
		string csvLine = "id,resource_load_path,bundle_load_path,type,version,asset_bundle_filename,asset_bundle_type";
		streamWriter.WriteLine(csvLine);  
		int l_id = 0;
		l_id = WriteFileBundleDic( streamWriter, l_id, "resource", m_bundleResourceDic);
		WriteFileBundleDic( streamWriter, l_id, "scene", m_bundleSceneDic);
		streamWriter.Close();
	}

	public int WriteFileBundleDic(StreamWriter streamWriter, int v_id, string v_bundleType, Dictionary<string, BundleDatabaseInfo> v_bundleDic)
	{
		string csvLine = "";
		foreach (KeyValuePair<string, BundleDatabaseInfo> objPair in v_bundleDic)
		{ 
			BundleDatabaseInfo l_bundleDatabaseInfo = objPair.Value; 
			string typeName = l_bundleDatabaseInfo.m_type.Name;   

			csvLine = v_id + ","//0													
					 + objPair.Key + ","//1
					 + l_bundleDatabaseInfo.m_assetbundleObjectName + ","//2
					 + typeName + ","//3
					 + l_bundleDatabaseInfo.m_version.ToString() + ","//4
					 + l_bundleDatabaseInfo.m_bundleName +","//5
					 + v_bundleType;//6
			streamWriter.WriteLine(csvLine);  
			v_id++;
		}
		return v_id;
	}

	public void LoadCsv(string v_Sample)
	{  
		#if UNITY_EDITOR
		if (v_Sample == null)
		{
			 //error
		}
		#endif
		
		if (string.IsNullOrEmpty(v_Sample) == false)
		{
			string[] lines = v_Sample.Split("\n"[0]);
			//StreamReader l_StreamReader = new StreamReader(v_fileName,Encoding.GetEncoding("euc-kr"));
			for (int i = 0; i < lines.Length; ++i)
			{
				if (lines[i].Length == 0 || lines[i] == "" || lines[i].Substring(0, 1) == ",")
					continue;
				
				//lines[i] = lines[i].Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
				lines[i] = lines[i].Replace("\r", "");
				if (i == 0)
				{
				 
 
				}
				else
				{
					string [] datas = lines[i].Split(','); 
					BundleDatabaseInfo l_bundleDatabaseInfo = new BundleDatabaseInfo();
					string l_object_path						 = datas[1];
					l_bundleDatabaseInfo.m_assetbundleObjectName = datas[2];
					string	assetbundleObjectType				 = datas[3];
					l_bundleDatabaseInfo.m_version 				 = int.Parse(datas[4]);
					l_bundleDatabaseInfo.m_bundleName 			 = datas[5];
					if(datas[6] == asset_bundle_type_resource)
					{
						AddResourceAssetBundleInfo( l_object_path, l_bundleDatabaseInfo);
					}
					else
					{
						AddSceneAssetBundleInfo( l_object_path, l_bundleDatabaseInfo);
					} 
				} 
			}
		} 
		BuildIndexCash();
	}

	public void SetAssetBundle(string v_AssetBundleName, AssetBundle v_assetBundle)
	{
		Debug.Log("SetAssetBundle:" + v_AssetBundleName);
		foreach (KeyValuePair<string, BundleDatabaseInfo> objPair in m_bundleResourceDic)
		{ 
			BundleDatabaseInfo l_bundleDatabaseInfo = objPair.Value; 
			if(l_bundleDatabaseInfo.m_bundleName == v_AssetBundleName)
			{
				l_bundleDatabaseInfo.m_refObject = v_assetBundle.Load(l_bundleDatabaseInfo.m_assetbundleObjectName);
				if(l_bundleDatabaseInfo.m_refObject != null)
				{
					Debug.Log("pre Load:" + l_bundleDatabaseInfo.m_refObject.name);
				}
				else
				{
					Debug.LogError("pre Load NotFound : l_bundleDatabaseInfo.m_assetbundleObjectName");
				}
			}
		}
	}
}
