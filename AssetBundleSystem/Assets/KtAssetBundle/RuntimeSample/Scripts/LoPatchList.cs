using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LoPatchList
{
	public class LoPatchListInfo 
	{
		public string 	m_file;
		public string 	m_url_path;
		public int 		m_version;
		public string  	m_asset_bundle_type;
	}
	public List<LoPatchListInfo> m_patchInfoList 					= new List<LoPatchListInfo>();
	public List<LoPatchListInfo> m_assetbundleResourceDatabaseList 	= new List<LoPatchListInfo>();
	// Update is called once per frame
	public void LoadCSV(string v_Sample) 
	{
		m_patchInfoList.Clear();
		m_assetbundleResourceDatabaseList.Clear();
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
					LoPatchListInfo l_patchListInfo = new LoPatchListInfo();
					l_patchListInfo.m_file						 = datas[1];
					l_patchListInfo.m_url_path					 = datas[2];
					l_patchListInfo.m_version					 = int.Parse(datas[3]);
					l_patchListInfo.m_asset_bundle_type			 = datas[4]; 
					if(l_patchListInfo.m_asset_bundle_type == LoAssetBundleDatabase.asset_bundle_type_resource_data_base)
					{
						m_assetbundleResourceDatabaseList.Add(l_patchListInfo); 
					}
					else
					{
						m_patchInfoList.Add(l_patchListInfo);
					}
				} 
			}
		} 
	}

	public void SaveCSV(string v_fileName) 
	{
		List<LoPatchListInfo> l_saveInfo = new List<LoPatchListInfo>();
		for(int i = 0; i < m_assetbundleResourceDatabaseList.Count; ++i)
		{
			l_saveInfo.Add(m_assetbundleResourceDatabaseList[i]);
		}
		for(int i = 0; i < m_patchInfoList.Count; ++i)
		{
			l_saveInfo.Add(m_patchInfoList[i]);
		}

		StreamWriter streamWriter = new StreamWriter(v_fileName);
		string csvLine = "id,file,url_path,version,asset_bundle_type";
		streamWriter.WriteLine(csvLine);  
		int l_id = 0;
		for(int i = 0; i < l_saveInfo.Count; ++i)
		{ 
			LoPatchListInfo l_patchListInfo = l_saveInfo[i];
			csvLine = l_id.ToString() 						+ ","
					+ l_patchListInfo.m_file 				+ ","
					+ l_patchListInfo.m_url_path 			+ ","
					+ l_patchListInfo.m_version.ToString() 	+ ","
					+ l_patchListInfo.m_asset_bundle_type;
			streamWriter.WriteLine(csvLine); 
			l_id++;
		} 
		streamWriter.Close();
	}

	public void LoadXml() 
	{
		
	}

	public void LoadJson() 
	{
		
	}

}
