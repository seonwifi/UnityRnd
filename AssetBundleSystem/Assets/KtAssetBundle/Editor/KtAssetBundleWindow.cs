//
//  AssetBundleWindow.cs
//
//  Created by Niklas Borglund
//  Copyright (c) 2012 Cry Wolf Studios. All rights reserved.
//  crywolfstudios.net
//
// Editor window that lets the user decide the settings for the
// Asset Bundles. 

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class KtAssetBundleWindow : EditorWindow 
{
	public enum eExportFile
	{
		prefab = 0,
		image,
		matrial,
		custom,
	}
	public List<string> GetExportFileExts(eExportFile exportFile)
	{
		List<string> exts = new List<string>();
		switch(exportFile)
		{
		case eExportFile.prefab:
			exts.Add(".unity");
			break;
		case eExportFile.image:
			exts.Add(".png");
			break;
		} 
		return exts;
	}
	public class TypeExt
	{
		public TypeExt(string pExt, System.Type pType)
		{
			ext  = pExt;
			type = pType;
		}
		public string  		ext;
		public System.Type 	type;
	}
	
	public class AssetBundleInfoResourceEnum
	{
		public bool prefab 	 		= true; 
		public bool materialFile 	= false;
		public bool imageFile 		= false; 
		public bool animationFile 	= false;
		public bool textFile 		= false;
		public bool filterFile	 	= true;
		public bool soundFile		= false;
		public string otherFileExt	 	= "";
	}
	public class AssetBundleInfo
	{ 
		public bool						isScene 				= false;
		public AssetBundleInfoResourceEnum	resourceEnum 		= null; 
		public int      				version					= 1;
		public int      				myPatchVersion			= 10000;
		public string   				assetBundleFileName 	= "AssetBundleFileName"; 
		public System.Type   			assetType 				=  typeof(UnityEngine.Object);
		public bool  					uiToggleGroup 			= true;  
		public bool						m_folderRef				= false;
		public Dictionary<int, Object>  m_refFolders			= new Dictionary<int, Object>(); 
		//public bool  					uiToggleAssetObjectView	= true;
	
		public Dictionary<int, Object>  myObjects 				= new Dictionary<int, Object>(); 
		public bool		 				uiEditingData			= false;
		public bool  					uiToggleAutoVersion 	= true;  
		public void     SetModeResource()
		{ 
			assetType 			=  typeof(UnityEngine.GameObject);
			isScene             = false;
			resourceEnum 		= new AssetBundleInfoResourceEnum();
		}
		public void     SetModeScene()
		{ 
			assetType 			=  typeof(UnityEngine.Object);
			isScene             = true;
			resourceEnum		= null;
		}		
		public List<TypeExt> GetTypeExt()
		{
			List<TypeExt> typeExts  = new List<TypeExt>();
			if(isScene)
			{
				TypeExt typeExt = new TypeExt(".unity", typeof(UnityEngine.Object));
				typeExts.Add(typeExt);
				 
			}
			else 
			{
				if(resourceEnum != null)
				{
					if(resourceEnum.prefab)
					{
						TypeExt typeExt = new TypeExt(".prefab", typeof(UnityEngine.GameObject));
						typeExts.Add(typeExt);
					}
					
					if(resourceEnum.materialFile)
					{
						TypeExt typeExt = new TypeExt(".mat", typeof(UnityEngine.Material));
						typeExts.Add(typeExt);
					}
					
					if(resourceEnum.imageFile)
					{
						TypeExt typeExt = new TypeExt(".*", typeof(UnityEngine.Texture2D));
						typeExts.Add(typeExt);
					}
					
					if(resourceEnum.animationFile)
					{
						TypeExt typeExt = new TypeExt(".anim", typeof(UnityEngine.AnimationClip));
						typeExts.Add(typeExt);
					}
					if(resourceEnum.soundFile)
					{
						TypeExt typeExt = new TypeExt(".*", typeof(UnityEngine.AudioClip));
						typeExts.Add(typeExt);
					}
					
					if(resourceEnum.textFile)
					{
						TypeExt typeExt = new TypeExt(".txt", typeof(UnityEngine.TextAsset));
						typeExts.Add(typeExt);
						typeExt = new TypeExt(".xml", typeof(UnityEngine.TextAsset));
						typeExts.Add(typeExt);
						typeExt = new TypeExt(".csv", typeof(UnityEngine.TextAsset));
						typeExts.Add(typeExt);
					}	
					
 					if(!string.IsNullOrEmpty(resourceEnum.otherFileExt))
					{
						string []exts = resourceEnum.otherFileExt.Split(new char[1]{'.'}, System.StringSplitOptions.RemoveEmptyEntries);
						foreach(string ext in exts)
						{
							string extLow = "." + ext.ToLower(); 
							TypeExt typeExt = new TypeExt(extLow, typeof(UnityEngine.Object));
							typeExts.Add(typeExt);
						}
					}
					//resourceEnum.filterFile
				}
			}
			return typeExts;
		}
		public void ClearData()
		{
			myObjects.Clear();
			m_refFolders.Clear();
		}
	}
 
	public string 		m_downURL	= "http://192.168.92.21:8000/"; 
	public string 		exportLocation 				= "AssetBundle";
    public string 		resourceBundleFileExtension	= ".unity3d"; 
	public string 		sceneBundleFileExtension	= ".scene"; 
    //public bool 		optionalSettings 			= false;
    public BuildTarget 	buildTarget 				= BuildTarget.Android;
	
	public bool addFolderBuildTarget = true;
    //BuildAssetBundleOptions
    public bool buildAssetBundleOptions 	= true;
    public bool collectDependencies 		= true;
    public bool completeAssets 				= true;
    public bool disableWriteTypeTree	 	= false;
    public bool deterministicAssetBundle 	= false;
    public bool uncompressedAssetBundle 	= false; 
	
    public Dictionary<string, int> bundleVersions 			= new Dictionary<string, int>();
    public Dictionary<string, List<string>> bundleContents 	= new Dictionary<string, List<string>>();
    public Dictionary<string, float> bundleFileSizes 		= new Dictionary<string, float>();
	
	//The position of the scrollview
	private Vector2 scrollPosition = Vector2.zero;
	
	//The undo manager
	//HOEditorUndoManager undoManager;
	
	public List<AssetBundleInfo> resourceAssetBundleInfos = new List<AssetBundleInfo>();
	public List<AssetBundleInfo> sceneAssetBundleInfos = new List<AssetBundleInfo>();
 
	bool patchEnable 	= false; 
	public int m_firstVersion = 100000;
	public int m_patchVersion = 100000;
	private void OnEnable()
	{

		// Instantiate undoManager
		//undoManager = new HOEditorUndoManager( this, "AssetBundleCreator" );
	}
  	Vector2 scrollPositionScreen = new Vector2(0,0);
	
	
    void OnGUI()
    {
		//undoManager.CheckUndo();
		
        GUILayout.Label ("Export Settings", EditorStyles.boldLabel);
 
		scrollPositionScreen = EditorGUILayout.BeginScrollView(scrollPositionScreen, GUILayout.MaxWidth(this.position.width), GUILayout.MinWidth(this.position.width));
 		EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset Settings"))
        {
			ClearPreferences(this); 
        }
 
        if (GUILayout.Button("Save Editor Setting"))
        {
			WriteEditorPrefs(this);
		}		
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		string assetBundle = this.GetAssetBundleFolder(false);
		EditorGUILayout.LabelField(assetBundle);
		if(GUILayout.Button("Open Folder", GUILayout.Width(130)))
		{
			
			string []folderSplits = this.GetAssetBundleFolder(true).Split(new char[2]{'/','\\'}, System.StringSplitOptions.RemoveEmptyEntries);
			string folder = "";
			foreach(string folderSplit in folderSplits)
			{
				folder += folderSplit;
				if(folderSplit != folderSplits[folderSplits.Length-1])
					folder += "\\";
			}
			System.Diagnostics.Process.Start("explorer.exe", folder);
			//EditorUtility.
		}
		EditorGUILayout.EndHorizontal();
		
		 addFolderBuildTarget	= EditorGUILayout.Toggle("Add Folder Build Target", addFolderBuildTarget);
		
        exportLocation 					= EditorGUILayout.TextField("Export folder", exportLocation);
		m_downURL 						= EditorGUILayout.TextField("Down URL", m_downURL); 
        resourceBundleFileExtension 	= EditorGUILayout.TextField("Bundle Resource file ext.", resourceBundleFileExtension); 
		sceneBundleFileExtension 		= EditorGUILayout.TextField("Bundle Scene file ext.", sceneBundleFileExtension); 

        buildAssetBundleOptions 		= EditorGUILayout.BeginToggleGroup("BuildAssetBundleOptions", buildAssetBundleOptions);
        collectDependencies 			= EditorGUILayout.Toggle("CollectDependencies", collectDependencies);
        completeAssets 					= EditorGUILayout.Toggle("CompleteAssets", completeAssets);
        disableWriteTypeTree 			= EditorGUILayout.Toggle("DisableWriteTypeTree", disableWriteTypeTree);
        deterministicAssetBundle 		= EditorGUILayout.Toggle("DeterministicAssetBundle", deterministicAssetBundle);
        uncompressedAssetBundle 		= EditorGUILayout.Toggle("UncompressedAssetBundle", uncompressedAssetBundle);
        EditorGUILayout.EndToggleGroup();

        //optionalSettings 	= EditorGUILayout.BeginToggleGroup("Optional Settings", optionalSettings);
        buildTarget 		= (BuildTarget)EditorGUILayout.EnumPopup("Build Target", buildTarget);
        //EditorGUILayout.EndToggleGroup();
 		/*
		//test patch
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Test Patch Upload", GUILayout.Width(150)))
        {
			UploadFile.Upload("E:/AssetBundleSample/AssetBundle/StandaloneWindows/AssetBundleSample.xml", "http://www.angrypower.com/AssetBundleSample.xml");
		}
		GUILayout.Label("http://www.patch.com/testfolder/");
		GUILayout.EndHorizontal();
		
		//patch
		patchEnable = EditorGUILayout.BeginToggleGroup("patch", patchEnable);
		GUILayout.BeginHorizontal(); 
		if (GUILayout.Button("Patch Upload", GUILayout.Width(150)))
        { 

		}
		GUILayout.Label("http://www.patch.com/folder/"); 
		GUILayout.EndHorizontal();
		EditorGUILayout.EndToggleGroup();//*/
		bool isCreateAllAssetBundle = false;
		if (GUILayout.Button("Create All", GUILayout.Width(80)))
        {
			isCreateAllAssetBundle = true; 
		}
		EditorGUILayout.BeginHorizontal();
		int newPatchVersion = EditorGUILayout.IntField("Patch Version", m_patchVersion);
		if(newPatchVersion > m_patchVersion)
		{
			m_patchVersion = newPatchVersion;
		}
		if (GUILayout.Button("Version Up"))
		{
			m_patchVersion++;
		}
		if (GUILayout.Button("Back To Version"))
		{
			m_patchVersion = m_firstVersion;
		}
		EditorGUILayout.EndHorizontal();
		//Editor
		//m_patchVersion
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add New Resource AssetBundle"))
        {
			AssetBundleInfo assetBundleInfo = new AssetBundleInfo(); 
			assetBundleInfo.SetModeResource();
			assetBundleInfo.assetBundleFileName = GetNewAssetBundleFileName(false);
			assetBundleInfo.myPatchVersion 		= m_patchVersion;
			resourceAssetBundleInfos.Add(assetBundleInfo);
		}
		if (GUILayout.Button("<>", GUILayout.Width(25)))
		{
			for(int i = 0; i <  resourceAssetBundleInfos.Count;++i)
			{
				AssetBundleInfo assetBundleInfo = resourceAssetBundleInfos[i];  
				assetBundleInfo.uiToggleGroup = true;
			} 
		}
		if (GUILayout.Button("><", GUILayout.Width(25)))
		{
			for(int i = 0; i <  resourceAssetBundleInfos.Count;++i)
			{
				AssetBundleInfo assetBundleInfo = resourceAssetBundleInfos[i];  
				assetBundleInfo.uiToggleGroup = false;
			} 
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add Select Resource One", GUILayout.Width(200)))
		{ 
			Object []newObjMultiSelects = Selection.GetFiltered(typeof(UnityEngine.Object),SelectionMode.Assets);
			if(newObjMultiSelects.Length > 0)
			{
				AssetBundleInfo assetBundleInfo = new AssetBundleInfo(); 
				assetBundleInfo.SetModeResource();
				assetBundleInfo.assetBundleFileName = newObjMultiSelects[0].name;
				assetBundleInfo.myPatchVersion 		= m_patchVersion;
				resourceAssetBundleInfos.Add(assetBundleInfo);
	  
				AddObjects( assetBundleInfo, newObjMultiSelects);  
			}

		}
		if (GUILayout.Button("Add Select Resource Part", GUILayout.Width(200)))
		{
			Object []newObjMultiSelects = Selection.GetFiltered(typeof(UnityEngine.Object),SelectionMode.Assets);
			for(int i = 0;  i < newObjMultiSelects.Length; ++i)
			{  
				AssetBundleInfo assetBundleInfo = new AssetBundleInfo(); 
				assetBundleInfo.SetModeResource();
				assetBundleInfo.assetBundleFileName = newObjMultiSelects[i].name;
				assetBundleInfo.myPatchVersion 		= m_patchVersion;
				resourceAssetBundleInfos.Add(assetBundleInfo);

				Object []newObj = new Object[1]{newObjMultiSelects[i]};
				AddObjects( assetBundleInfo, newObj);   
			}
		} 
		EditorGUILayout.EndHorizontal();

		AssetBundleInfo buttonClickedCreateAssetBundleInfo = OnGUIAssetBundleInfo(resourceAssetBundleInfos);

		EditorGUILayout.BeginHorizontal();
 		if (GUILayout.Button("Add New Scene AssetBundle"))
        {
			AssetBundleInfo assetBundleInfo = new AssetBundleInfo(); 
			assetBundleInfo.SetModeScene();
			assetBundleInfo.assetBundleFileName = GetNewAssetBundleFileName(true);
			assetBundleInfo.myPatchVersion 		= m_patchVersion;
			sceneAssetBundleInfos.Add(assetBundleInfo);
		}
		if (GUILayout.Button("<>", GUILayout.Width(25)))
		{
			for(int i = 0; i <  sceneAssetBundleInfos.Count;++i)
			{
				AssetBundleInfo assetBundleInfo = sceneAssetBundleInfos[i];  
				assetBundleInfo.uiToggleGroup = true;
			} 
		}
		if (GUILayout.Button("><", GUILayout.Width(25)))
		{
			for(int i = 0; i <  sceneAssetBundleInfos.Count;++i)
			{
				AssetBundleInfo assetBundleInfo = sceneAssetBundleInfos[i];  
				assetBundleInfo.uiToggleGroup = false;
			} 
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add Select Scene One", GUILayout.Width(200)))
		{  
			Object []newObjMultiSelects = Selection.GetFiltered(typeof(UnityEngine.Object),SelectionMode.Assets);
			if(newObjMultiSelects.Length > 0)
			{
				AssetBundleInfo assetBundleInfo = new AssetBundleInfo(); 
				assetBundleInfo.SetModeScene();
				assetBundleInfo.assetBundleFileName = newObjMultiSelects[0].name;
				assetBundleInfo.myPatchVersion 		= m_patchVersion;
				sceneAssetBundleInfos.Add(assetBundleInfo);
				
				AddObjects( assetBundleInfo, newObjMultiSelects);  
			}
			
		}
		if (GUILayout.Button("Add Select Scene Part", GUILayout.Width(200)))
		{
			Object []newObjMultiSelects = Selection.GetFiltered(typeof(UnityEngine.Object),SelectionMode.Assets);
			for(int i = 0;  i < newObjMultiSelects.Length; ++i)
			{  
				AssetBundleInfo assetBundleInfo = new AssetBundleInfo(); 
				assetBundleInfo.SetModeScene();
				assetBundleInfo.assetBundleFileName = newObjMultiSelects[i].name;
				assetBundleInfo.myPatchVersion 		= m_patchVersion;
				sceneAssetBundleInfos.Add(assetBundleInfo);
				
				Object []newObj = new Object[1]{newObjMultiSelects[i]};
				AddObjects( assetBundleInfo, newObj);   
			}

		} 
		EditorGUILayout.EndHorizontal();

		AssetBundleInfo tempButtonClickedCreateAssetBundleInfo =OnGUIAssetBundleInfo(sceneAssetBundleInfos);
		if(tempButtonClickedCreateAssetBundleInfo != null)
		{
			buttonClickedCreateAssetBundleInfo = tempButtonClickedCreateAssetBundleInfo;
		}
 
		EditorGUILayout.EndScrollView();
		
		if(buttonClickedCreateAssetBundleInfo != null)
		{ 
			string currentBundleFileExtension = this.resourceBundleFileExtension;
			if(buttonClickedCreateAssetBundleInfo.isScene)
			{
				currentBundleFileExtension = this.sceneBundleFileExtension;
			} 

			string assetBundleFolder = this.GetAssetBundleFolder(true);
			string exportLocationFull = assetBundleFolder + "/" + buttonClickedCreateAssetBundleInfo.assetBundleFileName + currentBundleFileExtension;
			
			if(buttonClickedCreateAssetBundleInfo.isScene)
			{ 
				if(buttonClickedCreateAssetBundleInfo.uiToggleAutoVersion)
				{
					if(buttonClickedCreateAssetBundleInfo.myPatchVersion < m_patchVersion)
					{
						buttonClickedCreateAssetBundleInfo.version++;
					}  
				}
				buttonClickedCreateAssetBundleInfo.myPatchVersion = m_patchVersion;
				if(KtCreateAssetBundles.BuildAssetBundleSceneFromDictionary(this, buttonClickedCreateAssetBundleInfo.myObjects, exportLocationFull))
				{
					buttonClickedCreateAssetBundleInfo.myPatchVersion = m_patchVersion; 
					SaveTotalAssetBundle(this);
					SaveAssetBundleVersionLog(buttonClickedCreateAssetBundleInfo);
					SaveAssetBundleVersionLogCSV(buttonClickedCreateAssetBundleInfo);
					WriteEditorPrefs(this);
					SeveAssetBundleDatabase();
					SavePatchList();
				}
			}
			else
			{
				if(buttonClickedCreateAssetBundleInfo.uiToggleAutoVersion)
				{
					if(buttonClickedCreateAssetBundleInfo.myPatchVersion < m_patchVersion)
					{
						buttonClickedCreateAssetBundleInfo.version++;
					}  
				}
				buttonClickedCreateAssetBundleInfo.myPatchVersion = m_patchVersion;
				if(KtCreateAssetBundles.BuildAssetBundleFromDictionary(this, buttonClickedCreateAssetBundleInfo.myObjects, exportLocationFull) != null)
				{
					buttonClickedCreateAssetBundleInfo.myPatchVersion = m_patchVersion; 
					SaveTotalAssetBundle(this);
					SaveAssetBundleVersionLog(buttonClickedCreateAssetBundleInfo);
					SaveAssetBundleVersionLogCSV(buttonClickedCreateAssetBundleInfo);
					WriteEditorPrefs(this);
					SeveAssetBundleDatabase();
					SavePatchList();
				}
			} 
		}
		if(isCreateAllAssetBundle)
		{
			string assetBundleFolder = this.GetAssetBundleFolder(true);
			
			foreach(AssetBundleInfo assetBundleInfo in this.resourceAssetBundleInfos)
			{   
				if(assetBundleInfo.uiToggleAutoVersion)
				{
					if(assetBundleInfo.myPatchVersion < m_patchVersion)
					{
						assetBundleInfo.version++;
					}  
				}
				assetBundleInfo.myPatchVersion = m_patchVersion;
				string exportLocationFull = assetBundleFolder + "/" + assetBundleInfo.assetBundleFileName + resourceBundleFileExtension;
				if(KtCreateAssetBundles.BuildAssetBundleFromDictionary(this, assetBundleInfo.myObjects, exportLocationFull) != null)
				{ 
					assetBundleInfo.myPatchVersion = m_patchVersion; 
					SaveAssetBundleVersionLog(assetBundleInfo);
					SaveAssetBundleVersionLogCSV(assetBundleInfo); 
				}
			}
			foreach(AssetBundleInfo assetBundleInfo in this.sceneAssetBundleInfos)
			{   
				if(assetBundleInfo.uiToggleAutoVersion)
				{
					if(assetBundleInfo.myPatchVersion < m_patchVersion)
					{
						assetBundleInfo.version++;
					}  
				}
				assetBundleInfo.myPatchVersion = m_patchVersion;
				string exportLocationFull = assetBundleFolder + "/" + assetBundleInfo.assetBundleFileName + sceneBundleFileExtension;
				if(KtCreateAssetBundles.BuildAssetBundleSceneFromDictionary(this, assetBundleInfo.myObjects, exportLocationFull))
				{ 
					assetBundleInfo.myPatchVersion = m_patchVersion; 
					SaveAssetBundleVersionLog(assetBundleInfo);
					SaveAssetBundleVersionLogCSV(assetBundleInfo); 

				}
			} 
			SaveTotalAssetBundle(this);
			WriteEditorPrefs(this);
			SeveAssetBundleDatabase();
			SavePatchList();
			EditorUtility.DisplayDialog("AssetBundle All Setting Create Complate!","", "OK");
		}
 		//
 		/*
        GUILayout.Label("Bundle Versions", EditorStyles.boldLabel);
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        foreach (KeyValuePair<string, int> bundleVersion in bundleVersions)
        {
            float bundleFileSize = 0;
            bundleFileSizes.TryGetValue(bundleVersion.Key, out bundleFileSize);
            if (GUILayout.Button(bundleVersion.Key + ", Version:" + bundleVersion.Value + ", Size: " + bundleFileSize + "kb"))
            {
                List<string> assetsInBundle = null;
                bundleContents.TryGetValue(bundleVersion.Key, out assetsInBundle);
                if (assetsInBundle != null)
                {
                    CreateContentWindow();
                    contentWindow.SelectAssetBundle(bundleVersion.Key, assetsInBundle, Application.dataPath + exportLocation, bundleFileSize);
                    contentWindow.ShowTab();
                }
            }
        }//*/
		
		//EditorGUILayout.EndScrollView();
    }
	AssetBundleInfo OnGUIAssetBundleInfo(List<AssetBundleInfo> assetBundleInfos)
    {
		AssetBundleInfo buttonClickedCreateAssetBungle = null;
		List<AssetBundleInfo> removes = new List<AssetBundleInfo>();
		foreach(AssetBundleInfo assetBundleInfo in assetBundleInfos)
		{    
			EditorGUILayout.BeginHorizontal();
			GUIContent content = new GUIContent();
			string currentBundleFileExtension = this.resourceBundleFileExtension;
			if(assetBundleInfo.isScene)
			{
				currentBundleFileExtension = this.sceneBundleFileExtension;
			} 
			
			content.text = assetBundleInfo.assetBundleFileName + currentBundleFileExtension;
			content.tooltip = "에셋번들을 만듭니다.";
			 
			bool isClickedAssetBundleCreate = false;
			//assetBundleInfo.uiToggleGroup = EditorGUILayout.BeginToggleGroup(content, assetBundleInfo.uiToggleGroup); 
			assetBundleInfo.uiToggleGroup = EditorGUILayout.ToggleLeft(content, assetBundleInfo.uiToggleGroup, GUILayout.Width(100)); 

			//GUILayout.Label ("	Create", EditorStyles.boldLabel, GUILayout.Width(55));
			string createButtonName = assetBundleInfo.assetBundleFileName + currentBundleFileExtension;
			if(!assetBundleInfo.uiToggleGroup)
			{  
				if (GUILayout.Button("Create(" +createButtonName + ")"))
				{
					isClickedAssetBundleCreate = true; 
					buttonClickedCreateAssetBungle = assetBundleInfo;
				} 
				 
				if (GUILayout.Button("Delete"))
				{
					removes.Add(assetBundleInfo);
				}
			}
 
			EditorGUILayout.EndHorizontal();

			string commentPatchVersion = "";
			if(assetBundleInfo.myPatchVersion == this.m_patchVersion)
			{
				commentPatchVersion = " =";
			}
			else if(assetBundleInfo.myPatchVersion < this.m_patchVersion)
			{
				commentPatchVersion = " <";
			}
			else
			{
				commentPatchVersion = " > (Worring)";
			}
			
			commentPatchVersion +=  "	(Current Version: " + this.m_patchVersion.ToString(); 
			commentPatchVersion += "	Version Gap: " + (m_patchVersion - assetBundleInfo.myPatchVersion).ToString();
			commentPatchVersion += ")";
			EditorGUILayout.LabelField("	Final Build Patch Version: " + assetBundleInfo.myPatchVersion.ToString() +  commentPatchVersion);


			if(assetBundleInfo.uiToggleGroup)
			{  
				GUILayout.BeginHorizontal(); 
				//EditorGUILayout.BeginHorizontal();
				if(assetBundleInfo.uiToggleGroup)
				{  
					if (GUILayout.Button("Create(" +createButtonName + ")"))
					{
						isClickedAssetBundleCreate = true; 
						buttonClickedCreateAssetBungle = assetBundleInfo;
					} 
					
					if (GUILayout.Button("Delete"))
					{
						removes.Add(assetBundleInfo);
					}
				}

				GUILayout.EndHorizontal();

				//EditorGUILayout.EndHorizontal();
				if(assetBundleInfo.resourceEnum != null)
				{   
					
					//assetBundleInfo.resourceEnum.filterFile		= EditorGUILayout.BeginToggleGroup("Filter", assetBundleInfo.resourceEnum.filterFile);
					//if(assetBundleInfo.resourceEnum.filterFile)
					//{
						assetBundleInfo.resourceEnum.prefab 		= EditorGUILayout.Toggle("	Prefab (.prefab)	", assetBundleInfo.resourceEnum.prefab);
	 					assetBundleInfo.resourceEnum.materialFile 	= EditorGUILayout.Toggle("	MaterialFile (.mat)	", assetBundleInfo.resourceEnum.materialFile);
						assetBundleInfo.resourceEnum.imageFile 		= EditorGUILayout.Toggle("	ImageFile (.png~)	", assetBundleInfo.resourceEnum.imageFile);
						assetBundleInfo.resourceEnum.animationFile 	= EditorGUILayout.Toggle("	AnimationFile (.anim)	", assetBundleInfo.resourceEnum.animationFile);
						assetBundleInfo.resourceEnum.soundFile	 	= EditorGUILayout.Toggle("	SoundFile (.wav.ogg.mp3..)	", assetBundleInfo.resourceEnum.soundFile);
					
						assetBundleInfo.resourceEnum.textFile 		= EditorGUILayout.Toggle("	TextFile(.txt .csv .xml)	", assetBundleInfo.resourceEnum.textFile); 
 						assetBundleInfo.resourceEnum.otherFileExt   = EditorGUILayout.TextField("	Other Extension", assetBundleInfo.resourceEnum.otherFileExt);
					//} 
					//EditorGUILayout.EndToggleGroup();
					//assetBundleInfo.resourceEnum.anythingFile	= EditorGUILayout.Toggle("AllFile	", assetBundleInfo.resourceEnum.anythingFile); 
				}
				
				//version
				GUI.backgroundColor = Color.blue;

				//

				EditorGUILayout.BeginHorizontal();
				int newVersion 			    = EditorGUILayout.IntField("	Version", assetBundleInfo.version); 
				if(assetBundleInfo.uiToggleAutoVersion == false)
				{
					assetBundleInfo.version = newVersion;
				}
				assetBundleInfo.uiToggleAutoVersion 	= EditorGUILayout.ToggleLeft("AutoChange", assetBundleInfo.uiToggleAutoVersion); 
				EditorGUILayout.EndHorizontal();

				assetBundleInfo.assetBundleFileName = EditorGUILayout.TextField("	Create FileName", assetBundleInfo.assetBundleFileName);

				EditorGUILayout.BeginHorizontal();
				Object objMultiSelect = EditorGUILayout.ObjectField("	Drag Data Here ->", null, typeof(UnityEngine.Object), true);
				GUI.backgroundColor = Color.white;
				GUILayout.FlexibleSpace();
				assetBundleInfo.m_folderRef = EditorGUILayout.ToggleLeft("Folder Ref", assetBundleInfo.m_folderRef); 
				GUI.backgroundColor = Color.blue;

				EditorGUILayout.EndHorizontal();

 				if(objMultiSelect != null)
				{
					Object []newObjMultiSelects = Selection.GetFiltered(typeof(UnityEngine.Object),SelectionMode.Assets);
					bool findObj = false;
					 
					foreach(Object newObjMultiSelect in newObjMultiSelects)
					{
						if(newObjMultiSelect.GetInstanceID() == objMultiSelect.GetInstanceID())
						{
							findObj = true;
							break;
						}
					}

					if(!findObj)
					{
						newObjMultiSelects = new Object[1]{objMultiSelect}; 
					}  
					AddObjects( assetBundleInfo, newObjMultiSelects); 
				}
				GUI.backgroundColor = Color.white;

				if(assetBundleInfo.m_folderRef == true)
				{
					EditorGUILayout.LabelField("	Ref Folders");
					List<UnityEngine.Object> removeObjects = new List<UnityEngine.Object>();
					foreach(KeyValuePair<int, Object>  keyValuePair in assetBundleInfo.m_refFolders)
					{
						EditorGUILayout.BeginHorizontal(); 
						Object obj = keyValuePair.Value;
						EditorGUILayout.ObjectField("	Folder", obj, typeof(UnityEngine.Object), true);
						
						if(GUILayout.Button("X", GUILayout.Width(25)))
						{
							removeObjects.Add(keyValuePair.Value);
						}
						EditorGUILayout.EndHorizontal();
					}

					foreach(UnityEngine.Object removeObj in removeObjects)
					{ 
						assetBundleInfo.m_refFolders.Remove(removeObj.GetInstanceID());
					} 
				}

				string datasCaption = "	Datas";
				if(assetBundleInfo.m_folderRef == true)
				{
					datasCaption = "	Folder In Datas";
				}
				else
				{
					datasCaption = "	Datas";
				}
				 

				//assetBundleInfo.uiEditingData = EditorGUILayout.BeginToggleGroup("Editing Datas", assetBundleInfo.uiEditingData);
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(datasCaption, GUILayout.Width(80));
				if(GUILayout.Button("Clear Data", GUILayout.Width(80)))
				{
					assetBundleInfo.ClearData();
				}
				GUILayout.EndHorizontal();

				GUI.backgroundColor = Color.green;  
				if(assetBundleInfo.myObjects != null)
				{
					List<UnityEngine.Object> removeObjects = new List<UnityEngine.Object>();
					bool firstLoop = true;
					foreach(KeyValuePair<int, Object>  keyValuePair in assetBundleInfo.myObjects)
					{
						EditorGUILayout.BeginHorizontal(); 
						Object obj = keyValuePair.Value;  
 
						EditorGUILayout.ObjectField("		Data", obj, typeof(UnityEngine.Object), true);
						 
						if(assetBundleInfo.assetBundleFileName == obj.name)
						{
							Color backupButtonShap = GUI.color;
							GUI.color = Color.yellow;
							if(GUILayout.Button("#", GUILayout.Width(25)))
							{
								assetBundleInfo.assetBundleFileName = obj.name;
							}
							GUI.color = backupButtonShap;
						}
						else
						{
							if(GUILayout.Button("#", GUILayout.Width(25)))
							{
								assetBundleInfo.assetBundleFileName = obj.name;
							}
						}
						if(GUILayout.Button("X", GUILayout.Width(25)))
						{
							removeObjects.Add(keyValuePair.Value);
						}
						EditorGUILayout.EndHorizontal();
						firstLoop = false;
					}
					
					foreach(UnityEngine.Object removeObj in removeObjects)
					{ 
						assetBundleInfo.myObjects.Remove(removeObj.GetInstanceID());
					}
				} 
				GUI.backgroundColor = Color.white;
				//EditorGUILayout.EndToggleGroup();
				
				GUILayout.Label ("---------------------------------------------");
				//EditorGUILayout.EndToggleGroup();
			} 
			
			//EditorGUILayout.EndToggleGroup();

		}
		foreach(AssetBundleInfo assetBundleInfo in removes)
		{
			assetBundleInfos.Remove(assetBundleInfo);
		}
		return buttonClickedCreateAssetBungle;
	}
	
    public void AddObjects(AssetBundleInfo assetBundleInfo, Object [] newObjMultiSelects)
    {
		foreach(Object newObjMultiSelect in newObjMultiSelects)
		{
			List<TypeExt> typeExts = assetBundleInfo.GetTypeExt(); 
			bool isTypeOK = false;
			foreach(TypeExt typeExt in typeExts)
			{
				if(isWrite(newObjMultiSelect, typeExt.ext, typeExt.type))
				{
					isTypeOK = true;
					break;
				}
			}
			if(assetBundleInfo.m_folderRef)
			{
				string outPath = "";
				if(isFolder( newObjMultiSelect,ref outPath))
				{   
					foreach(TypeExt typeExt in typeExts)
					{
						GetFolderObj(outPath, assetBundleInfo.myObjects, typeExt.ext, typeExt.type); 
					} 
						//GetFolderObj(string objPath, Dictionary<int, Object> objs, string ext, System.Type type) 
					assetBundleInfo.m_refFolders[newObjMultiSelect.GetInstanceID()] = newObjMultiSelect;
				} 
			}
			else
			{
				if(isTypeOK)
				{
					assetBundleInfo.myObjects[newObjMultiSelect.GetInstanceID()] = newObjMultiSelect;
				} 
				else 
				{
					string outPath = "";
					if(isFolder( newObjMultiSelect,ref outPath))
					{  
						foreach(TypeExt typeExt in typeExts)
						{
							GetFolderObj(outPath, assetBundleInfo.myObjects, typeExt.ext, typeExt.type); 
						} 
					} 
				}
			} 
		}
	}
	
    public void ReadBundleFileSizes()
    {
		bundleFileSizes.Clear();
        if (bundleVersions.Count > 0)
        {
            foreach (KeyValuePair<string, int> bundleVersion in bundleVersions)
            {
                if (File.Exists(Application.dataPath + exportLocation + bundleVersion.Key))
                {
                    FileInfo thisFileInfo = new FileInfo(Application.dataPath + exportLocation + bundleVersion.Key);
                    bundleFileSizes.Add(bundleVersion.Key, (thisFileInfo.Length / 1024));
                }
            }
        }
    }
 
    private static string GetXmlFileName()
    {
		string assetBundleEditorOption = "AssetBundleEditorOption.xml";
		string pathAssetBundleEditorOption = Application.dataPath + "/../" + assetBundleEditorOption;
		return pathAssetBundleEditorOption;
	}
    private static void ReadEditorPrefs(KtAssetBundleWindow thisWindow)
    {      
 		System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument(); 
		string pathAssetBundleEditorOption = GetXmlFileName();
		string temp = "";
 
		try
		{
			xmlDocument.Load(pathAssetBundleEditorOption);
			XmlElement exportSetting = xmlDocument.DocumentElement.SelectSingleNode("ExportSetting") as XmlElement;
			if(exportSetting != null)
			{      
				temp = exportSetting.GetAttribute("PatchVersion");
				if(!string.IsNullOrEmpty(temp))
				{
					thisWindow.m_patchVersion = int.Parse(temp);  
					thisWindow.m_firstVersion = thisWindow.m_patchVersion;
				}
					

				temp = exportSetting.GetAttribute("buildAssetBundleOptions");
				if(!string.IsNullOrEmpty(temp))
					thisWindow.buildAssetBundleOptions = bool.Parse(temp);  
				temp = exportSetting.GetAttribute("collectDependencies");
				if(!string.IsNullOrEmpty(temp))
					thisWindow.collectDependencies = bool.Parse(temp);  
				temp = exportSetting.GetAttribute("completeAssets");
				if(!string.IsNullOrEmpty(temp))
					thisWindow.completeAssets = bool.Parse(temp);  
				temp = exportSetting.GetAttribute("disableWriteTypeTree");
				if(!string.IsNullOrEmpty(temp))
					thisWindow.disableWriteTypeTree = bool.Parse(temp);  
				temp = exportSetting.GetAttribute("deterministicAssetBundle");
				if(!string.IsNullOrEmpty(temp))
					thisWindow.deterministicAssetBundle = bool.Parse(temp);  
				temp = exportSetting.GetAttribute("uncompressedAssetBundle");
				if(!string.IsNullOrEmpty(temp))
					thisWindow.uncompressedAssetBundle = bool.Parse(temp);   

				thisWindow.exportLocation 				= exportSetting.GetAttribute("ExportFolder");
				thisWindow.resourceBundleFileExtension 	= exportSetting.GetAttribute("ResourceBundleFileExtension");   
				thisWindow.sceneBundleFileExtension 	= exportSetting.GetAttribute("SceneBundleFileExtension"); 
				string buildTarget						= exportSetting.GetAttribute("BuildTarget");
				thisWindow.buildTarget 					= GetBuildTargetFromString(buildTarget); 
			}
			XmlElement assetBundleSettings = null;
			assetBundleSettings = xmlDocument.DocumentElement.SelectSingleNode("ResourceAssetBundleInfos") as XmlElement;
			if(assetBundleSettings != null)
			{ 
				thisWindow.resourceAssetBundleInfos.Clear();
				XmlNodeList assetBundleInfos = assetBundleSettings.SelectNodes("AssetBundleInfo");
				ReadEditorAssetBundleInfo(thisWindow, assetBundleInfos, thisWindow.resourceAssetBundleInfos, false, true);
			}
			
			assetBundleSettings = xmlDocument.DocumentElement.SelectSingleNode("SceneAssetBundleInfos") as XmlElement; 
			if(assetBundleSettings != null)
			{ 
				thisWindow.sceneAssetBundleInfos.Clear();
				XmlNodeList assetBundleInfos = assetBundleSettings.SelectNodes("AssetBundleInfo");
				ReadEditorAssetBundleInfo(thisWindow, assetBundleInfos, thisWindow.sceneAssetBundleInfos, true, true);
			}
		}
		catch(System.Xml.XmlException e)
		{
			Debug.LogError("Xml Load Error: " + pathAssetBundleEditorOption);
			Debug.LogError(e.Message);
		}
		
		//xmlDocument.
    }
    private static void ReadEditorAssetBundleInfo(KtAssetBundleWindow thisWindow, XmlNodeList xmlAssetBundleInfos, List<AssetBundleInfo> assetBundleInfos,
												  bool isScene, bool isEditor)
    {
		bool btrue  = true; 
		string trueString  = btrue.ToString(); 
		
		foreach(XmlNode xmlNode in xmlAssetBundleInfos)
		{
			XmlElement assetBundleSettingInfo = xmlNode as XmlElement;
			if(assetBundleSettingInfo != null)
			{ 
				AssetBundleInfo	assetBundleInfo 		= new AssetBundleInfo();  
				string temp; 
				if(isEditor == true)
				{ 
					temp = assetBundleSettingInfo.GetAttribute("RefFolderData"); 
					if(!string.IsNullOrEmpty(temp))
					{
						assetBundleInfo.m_folderRef = bool.Parse(temp);
					} 
				} 

				temp = assetBundleSettingInfo.GetAttribute("PatchVersion"); 
				if(!string.IsNullOrEmpty(temp))
				{
					assetBundleInfo.myPatchVersion = int.Parse(temp);
				} 

				temp = assetBundleSettingInfo.GetAttribute("Version"); 
				
				if(!string.IsNullOrEmpty(temp))
				{
					assetBundleInfo.version = int.Parse(temp);
				} 
				
				temp = assetBundleSettingInfo.GetAttribute("AssetBundleFileName"); 
				if(!string.IsNullOrEmpty(temp))
				{
					assetBundleInfo.assetBundleFileName = temp;
				}
 
				if(isScene)
				{
					assetBundleInfo.SetModeScene();
				}
				else 	
				{ 
					assetBundleInfo.SetModeResource();
					assetBundleInfo.resourceEnum = new AssetBundleInfoResourceEnum();
					temp = assetBundleSettingInfo.GetAttribute("prefab");
					if(!string.IsNullOrEmpty(temp))
					{   
						if(trueString == temp) 
							assetBundleInfo.resourceEnum.prefab = true; 
						else
							assetBundleInfo.resourceEnum.prefab = false;
					} 
					temp = assetBundleSettingInfo.GetAttribute("materialFile");
					if(!string.IsNullOrEmpty(temp))
					{   
						if(trueString == temp) 
							assetBundleInfo.resourceEnum.materialFile = true; 
						else
							assetBundleInfo.resourceEnum.materialFile = false;
					}
					temp = assetBundleSettingInfo.GetAttribute("imageFile");
					if(!string.IsNullOrEmpty(temp))
					{   
						if(trueString == temp) 
							assetBundleInfo.resourceEnum.imageFile = true; 
						else
							assetBundleInfo.resourceEnum.imageFile = false;
					}
					
					temp = assetBundleSettingInfo.GetAttribute("animationFile");
					if(!string.IsNullOrEmpty(temp))
					{   
						if(trueString == temp) 
							assetBundleInfo.resourceEnum.animationFile = true; 
						else
							assetBundleInfo.resourceEnum.animationFile = false;
					}
					
					temp = assetBundleSettingInfo.GetAttribute("soundFile");
					if(!string.IsNullOrEmpty(temp))
					{   
						if(trueString == temp) 
							assetBundleInfo.resourceEnum.soundFile = true; 
						else
							assetBundleInfo.resourceEnum.soundFile = false;
					}
					
					
					temp = assetBundleSettingInfo.GetAttribute("textFile");
					if(!string.IsNullOrEmpty(temp))
					{   
						if(trueString == temp) 
							assetBundleInfo.resourceEnum.textFile = true; 
						else
							assetBundleInfo.resourceEnum.textFile = false;
					}
					temp = assetBundleSettingInfo.GetAttribute("filterFile");
					if(!string.IsNullOrEmpty(temp))
					{   
						if(trueString == temp) 
							assetBundleInfo.resourceEnum.filterFile = true; 
						else
							assetBundleInfo.resourceEnum.filterFile = false;
					}
					temp = assetBundleSettingInfo.GetAttribute("otherFileExt");
					if(!string.IsNullOrEmpty(temp))
					{     
						assetBundleInfo.resourceEnum.otherFileExt = temp;  
					}
				}
 
				
				XmlNodeList xmlObjectInfos = assetBundleSettingInfo.SelectNodes("Object");
				foreach(XmlNode xmlNodeObjectInfo in xmlObjectInfos)
				{
					XmlElement xmlElementObjectInfo = xmlNodeObjectInfo as XmlElement;
					if(xmlElementObjectInfo != null)
					{  
						string path = xmlElementObjectInfo.GetAttribute("path");  
						string typeString = xmlElementObjectInfo.GetAttribute("type");
 						System.Type type = null; 
						if(typeString == typeof(GameObject).FullName)
						{
							type = typeof(GameObject);
						}
						else if(typeString == typeof(Material).FullName)
						{
							type = typeof(Material);
						}
						else if(typeString == typeof(AnimationClip).FullName)
						{
							type = typeof(AnimationClip);
						}
						else if(typeString == typeof(TextAsset).FullName)
						{
							type = typeof(TextAsset);
						}
						else
						{
							type = typeof(UnityEngine.Object);
						}
						Object obj = AssetDatabase.LoadAssetAtPath(path, type); 
						if(obj != null)
						{
							assetBundleInfo.myObjects[obj.GetInstanceID()] = obj;
						}
						else
						{
							Debug.LogError("Asset Loading Error: " +typeString + " Path->"+path);
						}
					} 
				}
				assetBundleInfos.Add(assetBundleInfo);
			}
		}
	}
    private static void WriteEditorPrefs(KtAssetBundleWindow thisWindow)
    {
		System.Xml.XmlWriterSettings xmlWriterSettings;
  
 		System.Xml.XmlWriter xmlWriter = new System.Xml.XmlTextWriter(GetXmlFileName(), System.Text.Encoding.UTF8);
		xmlWriter.WriteStartDocument();
		xmlWriter.WriteStartElement("Root");
		  
		xmlWriter.WriteStartElement("ExportSetting");
 
		xmlWriter.WriteAttributeString("buildAssetBundleOptions", thisWindow.buildAssetBundleOptions.ToString());
		xmlWriter.WriteAttributeString("collectDependencies", thisWindow.collectDependencies.ToString());
		xmlWriter.WriteAttributeString("completeAssets", thisWindow.completeAssets.ToString());
		xmlWriter.WriteAttributeString("disableWriteTypeTree", thisWindow.disableWriteTypeTree.ToString());
		xmlWriter.WriteAttributeString("deterministicAssetBundle", thisWindow.deterministicAssetBundle.ToString());
		xmlWriter.WriteAttributeString("uncompressedAssetBundle", thisWindow.uncompressedAssetBundle.ToString());
		
		xmlWriter.WriteAttributeString("ExportFolder", thisWindow.exportLocation);
		xmlWriter.WriteAttributeString("ResourceBundleFileExtension", thisWindow.resourceBundleFileExtension);
		xmlWriter.WriteAttributeString("SceneBundleFileExtension", thisWindow.sceneBundleFileExtension); 
		xmlWriter.WriteAttributeString("BuildTarget", thisWindow.buildTarget.ToString());
		xmlWriter.WriteAttributeString("PatchVersion", thisWindow.m_patchVersion.ToString());
		xmlWriter.WriteEndElement();
		 
		xmlWriter.WriteStartElement("ResourceAssetBundleInfos");
		WriteEditorAssetBundleInfo(thisWindow, xmlWriter, thisWindow.resourceAssetBundleInfos, true); 
		xmlWriter.WriteEndElement();
		
		xmlWriter.WriteStartElement("SceneAssetBundleInfos");
		WriteEditorAssetBundleInfo(thisWindow, xmlWriter, thisWindow.sceneAssetBundleInfos, true); 
		xmlWriter.WriteEndElement();		
		
		xmlWriter.WriteEndElement();
		xmlWriter.WriteEndDocument();
		xmlWriter.Flush();
		xmlWriter.Close();
    }
    private static void WriteEditorAssetBundleInfo(KtAssetBundleWindow thisWindow, System.Xml.XmlWriter xmlWriter, List<AssetBundleInfo> assetBundleInfos, bool isEditor)
    { 
		foreach(AssetBundleInfo assetBundleInfo in assetBundleInfos)
		{ 
			WriteXmlAssetBundleInfo( thisWindow, xmlWriter, assetBundleInfo, isEditor);
		} 
	}
	private static void WriteXmlAssetBundleInfo(KtAssetBundleWindow thisWindow, System.Xml.XmlWriter xmlWriter, AssetBundleInfo assetBundleInfo
	                                            , bool editor)
	{
		string assetBundleFolder = thisWindow.GetAssetBundleFolder(true);
		string bundleExt = thisWindow.resourceBundleFileExtension;
		long  fileSize = -1;
		if(assetBundleInfo.isScene)
		{
			bundleExt = thisWindow.sceneBundleFileExtension;
		}  
		//fileSize
		string assetBundlePath = assetBundleFolder + "/" + assetBundleInfo.assetBundleFileName + bundleExt; 
		if (File.Exists(assetBundlePath))
		{
			FileInfo thisFileInfo = new FileInfo(assetBundlePath);
			fileSize = thisFileInfo.Length; 
		}

		string temp = "";
		xmlWriter.WriteStartElement("AssetBundleInfo");
		if(editor == true)
		{ 
			xmlWriter.WriteAttributeString("RefFolderData", assetBundleInfo.m_folderRef.ToString());  
		}
		 
		xmlWriter.WriteAttributeString("PatchVersion", assetBundleInfo.myPatchVersion.ToString()); 
		xmlWriter.WriteAttributeString("Version", assetBundleInfo.version.ToString()); 
		xmlWriter.WriteAttributeString("AssetBundleFileName", assetBundleInfo.assetBundleFileName); 
		xmlWriter.WriteAttributeString("FileSize", fileSize.ToString()); 
		xmlWriter.WriteAttributeString("BuildTarget", thisWindow.buildTarget.ToString()); 

		if(assetBundleInfo.resourceEnum != null)
		{  
			temp = assetBundleInfo.resourceEnum.prefab.ToString();
			xmlWriter.WriteAttributeString("prefab", temp);
			temp = assetBundleInfo.resourceEnum.materialFile.ToString();
			xmlWriter.WriteAttributeString("materialFile", temp);
			temp = assetBundleInfo.resourceEnum.imageFile.ToString();
			xmlWriter.WriteAttributeString("imageFile", temp);
			temp = assetBundleInfo.resourceEnum.animationFile.ToString();
			xmlWriter.WriteAttributeString("animationFile", temp);
			temp = assetBundleInfo.resourceEnum.soundFile.ToString();
			xmlWriter.WriteAttributeString("soundFile", temp);
			
			temp = assetBundleInfo.resourceEnum.textFile.ToString();
			xmlWriter.WriteAttributeString("textFile", temp);
			temp = assetBundleInfo.resourceEnum.filterFile.ToString();
			xmlWriter.WriteAttributeString("filterFile", temp);
			temp = assetBundleInfo.resourceEnum.otherFileExt;
			if(!string.IsNullOrEmpty(temp))
				xmlWriter.WriteAttributeString("otherFileExt", temp);
		}
		
		foreach(KeyValuePair<int, Object>  objPair in assetBundleInfo.myObjects)
		{ 
			//System.Reflection.Assembly  asa = new System.Reflection.Assembly.g
			
			string assetOrScenePath  = AssetDatabase.GetAssetOrScenePath(objPair.Value);
			string typeName = objPair.Value.GetType().Name;   
			xmlWriter.WriteStartElement("Object");
			xmlWriter.WriteAttributeString("path", assetOrScenePath); 
			xmlWriter.WriteAttributeString("type", typeName);
			xmlWriter.WriteEndElement();
		}
		
		xmlWriter.WriteEndElement();
	}

    public static void ClearPreferences(KtAssetBundleWindow thisWindow)
    { 
		/*
        thisWindow.exportLocation = "AssetBundles";
        thisWindow.resourceBundleFileExtension = ".unity3d";
		 thisWindow.sceneBundleFileExtension = ".scene3d";

       // thisWindow.optionalSettings = false;
        thisWindow.buildTarget = BuildTarget.WebPlayer;

        //BuildAssetBundleOptions
        thisWindow.buildAssetBundleOptions = true;
        thisWindow.collectDependencies = true;
        thisWindow.completeAssets = true;
        thisWindow.disableWriteTypeTree = false;
        thisWindow.deterministicAssetBundle = false;
        thisWindow.uncompressedAssetBundle = false;
        thisWindow.bundleVersions.Clear();
        thisWindow.bundleContents.Clear();
        thisWindow.bundleFileSizes.Clear();
		thisWindow.resourceAssetBundleInfos.Clear();
		thisWindow.sceneAssetBundleInfos.Clear();
		//*/
    }

    //Show window
    [MenuItem("Assets/KT Asset Bundle Maker")]
    public static void ShowWindow()
    {
        KtAssetBundleWindow thisWindow = (KtAssetBundleWindow)EditorWindow.GetWindow(typeof(KtAssetBundleWindow));
        thisWindow.title = "Bundle Creator"; 
        ReadEditorPrefs(thisWindow);  
    }
	
	public static BuildTarget GetBuildTargetFromString( string u)
	{
		if(u == BuildTarget.Android.ToString())
		{
			return BuildTarget.Android;
		}
		else if(u == BuildTarget.BB10.ToString())
		{
			return BuildTarget.BB10;
		}
		else if(u == BuildTarget.FlashPlayer.ToString())
		{
			return BuildTarget.FlashPlayer;
		}
		else if(u == BuildTarget.MetroPlayer.ToString())
		{
			return BuildTarget.MetroPlayer;
		}
		else if(u == BuildTarget.NaCl.ToString())
		{
			return BuildTarget.NaCl;
		}
		else if(u == BuildTarget.PS3.ToString())
		{
			return BuildTarget.PS3;
		}
		else if(u == BuildTarget.StandaloneGLESEmu.ToString())
		{ 
			return BuildTarget.StandaloneGLESEmu;
		} 
		else if(u == BuildTarget.StandaloneLinux.ToString())
		{
			return BuildTarget.StandaloneLinux;
		}
		else if(u == BuildTarget.StandaloneLinux64.ToString())
		{
			return BuildTarget.StandaloneLinux64;
		}
		else if(u == BuildTarget.StandaloneLinuxUniversal.ToString())
		{
			return BuildTarget.StandaloneLinuxUniversal;
		}
		else if(u == BuildTarget.StandaloneOSXIntel.ToString())
		{
			return BuildTarget.StandaloneOSXIntel;
		}
		else if(u == BuildTarget.StandaloneOSXIntel64.ToString())
		{
			return BuildTarget.StandaloneOSXIntel64;
		}
		else if(u == BuildTarget.StandaloneOSXUniversal.ToString())
		{ 
			return BuildTarget.StandaloneOSXUniversal;
		}
		else if(u == BuildTarget.StandaloneWindows.ToString())
		{
			return BuildTarget.StandaloneWindows;
		}
		else if(u == BuildTarget.StandaloneWindows64.ToString())
		{
			return BuildTarget.StandaloneWindows64;
		}
		else if(u == BuildTarget.WebPlayer.ToString())
		{
			return BuildTarget.WebPlayer;
		}
		else if(u == BuildTarget.WebPlayerStreamed.ToString())
		{
			return BuildTarget.WebPlayerStreamed;
		}
		else if(u == BuildTarget.Wii.ToString())
		{ 
			return BuildTarget.Wii;
		}
		else if(u == BuildTarget.WP8Player.ToString())
		{ 
			return BuildTarget.WP8Player;
		}
		else if(u == BuildTarget.XBOX360.ToString())
		{  
			return BuildTarget.XBOX360;
		}		
		return BuildTarget.StandaloneWindows;
	}
	
	string GetAssetBundleFolder(bool autoMakeFolder)
	{
		string folder = "";
		string []splitedApplicationContentsPath = Application.dataPath.Split(new char[2]{'\\','/'}, System.StringSplitOptions.RemoveEmptyEntries);
		for(int i = 0; i < splitedApplicationContentsPath.Length-1; ++i)
		{
			folder += splitedApplicationContentsPath[i]+"/";
		}
		folder += this.exportLocation;
		if(addFolderBuildTarget)
		{
			folder += "/" + this.buildTarget.ToString();
		}
		if(autoMakeFolder)
		{
			if(!System.IO.Directory.Exists(folder))
			{
				System.IO.Directory.CreateDirectory(folder);
			}
		}

		return folder;
	}
		
	string GetNewAssetBundleFileName(bool isScene)
	{
		string fileName = "TempAssetBundle"; 
		string bundleFileExtension = "";
		string []splitedApplicationContentsPath = Application.dataPath.Split(new char[2]{'\\','/'}, System.StringSplitOptions.RemoveEmptyEntries);
		fileName = splitedApplicationContentsPath[splitedApplicationContentsPath.Length-2];
		//fileName += this.buildTarget.ToString(); 
		if(isScene)
		{
			//fileName += "Scene";
			bundleFileExtension = this.sceneBundleFileExtension;
		}
		else
		{
			//fileName += "Resource";
			bundleFileExtension = this.resourceBundleFileExtension;
		}
		
		string assetBundleFolder = GetAssetBundleFolder(false);
 		for(int i = 0; i < 100000000; ++i)
		{
			bool isOverLabFile = false;
			string newFileName = fileName + i.ToString();
			foreach(AssetBundleInfo assetBundleInfo in resourceAssetBundleInfos)
			{
				if(assetBundleInfo.assetBundleFileName == newFileName)
				{
					isOverLabFile = true;
					break;
				} 
			} 
 			if(isOverLabFile) 
				continue;
			foreach(AssetBundleInfo assetBundleInfo in sceneAssetBundleInfos)
			{
				if(assetBundleInfo.assetBundleFileName == newFileName)
				{
					isOverLabFile = true;
					break;
				} 
			}
 			if(isOverLabFile) 
				continue;	 
			string fullPath = assetBundleFolder +"/" + newFileName + bundleFileExtension;
			if(!System.IO.File.Exists(fullPath))
			{
				return newFileName;
			} 
		} 
 
		return "TempAssetBundle";
	}
	
	bool isWrite(Object obj, string ext, System.Type type)
	{ 
		if(ext == ".*")
		{
			if(obj.GetType().FullName == type.FullName)
			{
				return true;
			} 
			else
			{
				return false;
			}
		}
			
		string objPath = AssetDatabase.GetAssetOrScenePath(obj);
		objPath = objPath.ToLower();
		if(objPath.EndsWith(ext))
		{
			return true;
		}
		return false;
	}

	bool isFolder(Object obj, ref string outPath)
	{
		string objPath =  AssetDatabase.GetAssetOrScenePath(obj);  
		System.IO.FileAttributes attr = System.IO.File.GetAttributes(objPath);

		if(attr == FileAttributes.Directory)
		{  
			outPath = objPath;
			return true;
		} 	
		return false;
	}

	int GetFolderObj(string objPath, Dictionary<int, Object> objs, string ext, System.Type type)
	{  
		int addCount = 0;
		ext = "*" + ext;
		string []files = System.IO.Directory.GetFiles(objPath, ext, SearchOption.AllDirectories);
		foreach(string file in files)
		{   
			string []paths = file.Split(new char[]{'/', '\\'}, System.StringSplitOptions.RemoveEmptyEntries);
			
			string assetPath = "";
			bool insert = false;
			foreach(string spPath in paths)
			{
				if(spPath == "Assets" || insert)
				{ 
					if(insert)
					{
						assetPath += "/"; 
					} 
					assetPath += spPath;
					insert = true;
				} 
			}
			Object obj = AssetDatabase.LoadAssetAtPath(assetPath, type); 
			if(obj != null)
			{
				addCount++;
				int id = obj.GetInstanceID();
				objs[id] = obj;
			} 
		} 
		return addCount;
	}
	
	void SaveAssetBundleVersionLog(AssetBundleInfo info)
	{  
		string assetBundleFolder = this.GetAssetBundleFolder(true);
		string exportLocationFull = assetBundleFolder + "/" + info.assetBundleFileName + ".xml";
  
 		System.Xml.XmlWriter xmlWriter = new System.Xml.XmlTextWriter(exportLocationFull, System.Text.Encoding.UTF8);
		xmlWriter.WriteStartDocument();
		xmlWriter.WriteStartElement("Root");
		WriteXmlAssetBundleInfo(this, xmlWriter, info, false);  
		xmlWriter.WriteEndElement();
		xmlWriter.WriteEndDocument();
		xmlWriter.Flush();
		xmlWriter.Close(); 

		//public string 		resourceBundleFileExtension	= ".unity3d"; 
		//public string 		sceneBundleFileExtension	= ".scene"; 

	}
	
    void SaveAssetBundleVersionLogCSV(AssetBundleInfo info)
    {
		string assetBundleFolder = this.GetAssetBundleFolder(true);
		string path = assetBundleFolder + "/" + info.assetBundleFileName + ".csv";
        StreamWriter streamWriter = new StreamWriter(path);
		string csvLine = "name,extansion,type";
		streamWriter.WriteLine(csvLine);  
        foreach (KeyValuePair<int, Object> objPair in info.myObjects)
        {
			string assetOrScenePath  = AssetDatabase.GetAssetOrScenePath(objPair.Value);
			string []assetOrScenePathSplit = assetOrScenePath.Split(new char[2]{'\\', '/'}, System.StringSplitOptions.RemoveEmptyEntries);
			string []assetNameSplit = assetOrScenePathSplit[assetOrScenePathSplit.Length-1].Split(new char[1]{'.'}, System.StringSplitOptions.RemoveEmptyEntries);
			 
			string typeName = objPair.Value.GetType().Name;   
			csvLine = objPair.Value.name + "," + assetNameSplit[assetNameSplit.Length-1] + "," + typeName;
			streamWriter.WriteLine(csvLine);  
        }
        streamWriter.Close();
    }
	void SaveTotalAssetBundle(KtAssetBundleWindow thisWindow)
	{
		string assetBundlePath = this.GetAssetBundleFolder(true);

		string []splitedApplicationContentsPath = Application.dataPath.Split(new char[2]{'\\','/'}, System.StringSplitOptions.RemoveEmptyEntries);
		string fileName = splitedApplicationContentsPath[splitedApplicationContentsPath.Length-2];
		assetBundlePath += "/" + fileName+".xml";
		
 		System.Xml.XmlWriter xmlWriter = new System.Xml.XmlTextWriter( assetBundlePath, System.Text.Encoding.UTF8);
		xmlWriter.WriteStartDocument();
		xmlWriter.WriteStartElement("Root");
		  
		xmlWriter.WriteStartElement("ExportSetting");
 
		xmlWriter.WriteAttributeString("ResourceBundleFileExtension", thisWindow.resourceBundleFileExtension);
		xmlWriter.WriteAttributeString("SceneBundleFileExtension", thisWindow.sceneBundleFileExtension); 
		xmlWriter.WriteAttributeString("BuildTarget", thisWindow.buildTarget.ToString());
		xmlWriter.WriteAttributeString("PatchVersion", thisWindow.m_patchVersion.ToString());
		xmlWriter.WriteEndElement();
		 
		xmlWriter.WriteStartElement("ResourceAssetBundleInfos");
		WriteTotalAssetBundleInfo( xmlWriter, thisWindow.resourceAssetBundleInfos, false, thisWindow.resourceBundleFileExtension); 
		xmlWriter.WriteEndElement();
		
		xmlWriter.WriteStartElement("SceneAssetBundleInfos");
		WriteTotalAssetBundleInfo(xmlWriter, thisWindow.sceneAssetBundleInfos, true, thisWindow.sceneBundleFileExtension); 
		xmlWriter.WriteEndElement();		
		
		xmlWriter.WriteEndElement();
		xmlWriter.WriteEndDocument();
		xmlWriter.Flush();
		xmlWriter.Close();
	}

    private void WriteTotalAssetBundleInfo(System.Xml.XmlWriter xmlWriter, List<AssetBundleInfo> assetBundleInfos, bool isScene, string fileExt)
    {
		foreach(AssetBundleInfo assetBundleInfo in assetBundleInfos)
		{ 
			string assetBundleFolder 	= this.GetAssetBundleFolder(true); 
			string bundleExt 			= resourceBundleFileExtension;
			long  fileSize = 0;
			if(assetBundleInfo.isScene)
			{
				bundleExt = sceneBundleFileExtension;
			}  
			//fileSize
			string assetBundlePath = assetBundleFolder + "/" + assetBundleInfo.assetBundleFileName + bundleExt; 
			if (File.Exists(assetBundlePath))
			{
				FileInfo thisFileInfo = new FileInfo(assetBundlePath);
				fileSize = thisFileInfo.Length; 
			}
			 
			xmlWriter.WriteStartElement("AssetBundleInfo");
			xmlWriter.WriteAttributeString("PatchVersion", assetBundleInfo.myPatchVersion.ToString()); 
			xmlWriter.WriteAttributeString("Version", assetBundleInfo.version.ToString()); 
			xmlWriter.WriteAttributeString("AssetBundleFileName", assetBundleInfo.assetBundleFileName+fileExt);
			xmlWriter.WriteAttributeString("FileSize", fileSize.ToString());
			xmlWriter.WriteAttributeString("AssetPath", "http://urlFolder/"+assetBundleInfo.assetBundleFileName+fileExt); 
			xmlWriter.WriteEndElement();
			 
		} 
	}

	string GetAssetBundleDatabaseFileName()
	{
		return "LoAssetBundleDatabase" + ".txt";
	}
	void SeveAssetBundleDatabase()
	{
		string assetBundleFolder = this.GetAssetBundleFolder(true);
		string exportLocationFull = assetBundleFolder + "/" + GetAssetBundleDatabaseFileName();

		LoAssetBundleDatabase assetBundleDatabase = new LoAssetBundleDatabase();
		for(int i = 0; i < resourceAssetBundleInfos.Count; ++i)
		{
			AssetBundleInfo assetBundleInfo = resourceAssetBundleInfos[i];

			foreach (KeyValuePair<int, Object> objPair in assetBundleInfo.myObjects)
			{
				LoAssetBundleDatabase.BundleDatabaseInfo bundleDatabaseInfo = new LoAssetBundleDatabase.BundleDatabaseInfo();

				string assetPath = AssetDatabase.GetAssetPath(objPair.Value);
				string []splitPath = assetPath.Split('.');
				string []splitResourcesPath = splitPath[0].Split(new char[]{'/', '\\'});
				List<string> loadResourcePathList = new List<string>();
				bool bAddPath = false;
				for(int j = 0; j < splitResourcesPath.Length; ++j)
				{
					if(bAddPath)
					{
						loadResourcePathList.Add(splitResourcesPath[j]);
					}
					string splitResourcesPathToLower = splitResourcesPath[j].ToLower();
					if(splitResourcesPathToLower == "resources")
					{
						bAddPath = true;
					}
				}
				if(loadResourcePathList.Count > 0)
				{
					assetPath = "";
					for(int j = 0; j < loadResourcePathList.Count; ++j)
					{
						assetPath += loadResourcePathList[j];
						if(j != loadResourcePathList.Count-1)
						{
							assetPath += "/";
						}
					}
				}
				else
				{
					assetPath = "";
					for(int j = 0; j < splitPath.Length; ++j)
					{
						assetPath += splitPath[j];
						if(j != splitPath.Length-1)
						{
							assetPath += "/";
						}
					} 
				}
				bundleDatabaseInfo.m_assetbundleObjectName 	= objPair.Value.name;
				bundleDatabaseInfo.m_bundleName 			= assetBundleInfo.assetBundleFileName;
				bundleDatabaseInfo.m_type 					= objPair.Value.GetType();
				bundleDatabaseInfo.m_version 				= assetBundleInfo.version;
				assetBundleDatabase.AddResourceAssetBundleInfo(assetPath, bundleDatabaseInfo);
			} 
			//resourceAssetBundleInfos[i].;
		}
		 
		for(int i = 0; i < sceneAssetBundleInfos.Count; ++i)
		{
			AssetBundleInfo assetBundleInfo = sceneAssetBundleInfos[i];
			
			foreach (KeyValuePair<int, Object> objPair in assetBundleInfo.myObjects)
			{
				LoAssetBundleDatabase.BundleDatabaseInfo bundleDatabaseInfo = new LoAssetBundleDatabase.BundleDatabaseInfo();
				 
				AssetDatabase.GetAssetOrScenePath(objPair.Value);
				string assetPath = AssetDatabase.GetAssetPath(objPair.Value);
				bundleDatabaseInfo.m_assetbundleObjectName 	= objPair.Value.name;
				bundleDatabaseInfo.m_bundleName 			= assetBundleInfo.assetBundleFileName;
				bundleDatabaseInfo.m_type 					= objPair.Value.GetType();
				bundleDatabaseInfo.m_version 				= assetBundleInfo.version;
				assetBundleDatabase.AddSceneAssetBundleInfo(assetPath, bundleDatabaseInfo);
			} 
			//resourceAssetBundleInfos[i].;
		}

		assetBundleDatabase.SaveFileCsv(exportLocationFull);
	}

	void SavePatchList()
	{
		string assetBundleFolder 	= this.GetAssetBundleFolder(true);
		string exportLocationFull 	= assetBundleFolder + "/" + "PatchList" + ".txt";
		LoPatchList l_patchList = new LoPatchList();

		string l_assetBundleDatabaseFileName = GetAssetBundleDatabaseFileName();
		if(string.IsNullOrEmpty(l_assetBundleDatabaseFileName) == false)
		{
			LoPatchList.LoPatchListInfo l_patchListInfo = new LoPatchList.LoPatchListInfo();
			l_patchListInfo.m_file					= l_assetBundleDatabaseFileName;
			l_patchListInfo.m_url_path				= this.m_downURL + l_assetBundleDatabaseFileName;
			l_patchListInfo.m_version				= this.m_patchVersion;
			l_patchListInfo.m_asset_bundle_type		= "resource_data_base";
			l_patchList.m_assetbundleResourceDatabaseList.Add(l_patchListInfo);
		}
  
		for(int i = 0; i < resourceAssetBundleInfos.Count; ++i)
		{
			AssetBundleInfo assetBundleInfo = resourceAssetBundleInfos[i];
			LoPatchList.LoPatchListInfo l_patchListInfo = new LoPatchList.LoPatchListInfo();
			l_patchListInfo.m_file					= assetBundleInfo.assetBundleFileName + this.resourceBundleFileExtension;
			l_patchListInfo.m_url_path				= this.m_downURL + assetBundleInfo.assetBundleFileName + this.resourceBundleFileExtension;
			l_patchListInfo.m_version				= assetBundleInfo.version;
			l_patchListInfo.m_asset_bundle_type		= "resource";
			l_patchList.m_patchInfoList.Add(l_patchListInfo);
		}
 
		for(int i = 0; i < sceneAssetBundleInfos.Count; ++i)
		{
			AssetBundleInfo assetBundleInfo = sceneAssetBundleInfos[i];
			LoPatchList.LoPatchListInfo l_patchListInfo = new LoPatchList.LoPatchListInfo();
			l_patchListInfo.m_file					= assetBundleInfo.assetBundleFileName + this.sceneBundleFileExtension;
			l_patchListInfo.m_url_path				= this.m_downURL + assetBundleInfo.assetBundleFileName + this.sceneBundleFileExtension;
			l_patchListInfo.m_version				= assetBundleInfo.version;
			l_patchListInfo.m_asset_bundle_type		= "scene";
			l_patchList.m_patchInfoList.Add(l_patchListInfo);
		}
		l_patchList.SaveCSV(exportLocationFull);
	}
}
