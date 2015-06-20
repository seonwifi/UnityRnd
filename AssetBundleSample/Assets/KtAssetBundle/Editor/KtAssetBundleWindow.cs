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
	}
	public class AssetBundleInfo
	{ 
		public bool						isScene = false;
		public AssetBundleInfoResourceEnum	resourceEnum = null;
		public string 					sourceFolder 			= "Assets"; 
		public int      				version					= 1;
		public string   				assetBundleFileName 	= "AssetBundleFileName";
		public string   				assetFileExt 			= ".unity";
		public System.Type   			assetType 				=  typeof(UnityEngine.Object);
		public bool  					uiToggleGroup 			= true;  
		//public bool  					uiToggleAssetObjectView	= true;
	
		public Dictionary<int, Object>  myObjects 				= new Dictionary<int, Object>();
		public UnityEngine.Object 		dragdFolderObject 		= null;
		public bool		 				uiEditingData			= false;
		public void     SetModeResource()
		{
			assetFileExt 		= ".unity";
			assetType 			=  typeof(UnityEngine.GameObject);
			isScene             = false;
			resourceEnum 		= new AssetBundleInfoResourceEnum();
		}
		public void     SetModeScene()
		{
			assetFileExt 		= ".unity";
			assetType 			=  typeof(UnityEngine.Object);
			isScene             = true;
			resourceEnum		= null;
		}		
		public List<TypeExt> GetTypeExt()
		{
			List<TypeExt> typeExts  = new List<TypeExt>();
			if(isScene)
			{
				TypeExt typeExt = new TypeExt(".prefab", typeof(UnityEngine.Object));
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
						TypeExt typeExt = new TypeExt(".png", typeof(UnityEngine.Texture2D));
						typeExts.Add(typeExt);
					}
					
					if(resourceEnum.animationFile)
					{
						TypeExt typeExt = new TypeExt(".anim", typeof(UnityEngine.Animation));
						typeExts.Add(typeExt);
					}
					 
					if(resourceEnum.textFile)
					{
						TypeExt typeExt = new TypeExt(".txt", typeof(UnityEngine.TextAsset));
						typeExts.Add(typeExt);
					}					
 
					//resourceEnum.filterFile
				}
			}
			return typeExts;
		}
	}
	
    private AssetBundleContent contentWindow;
 
    public string 		exportLocation 				= "/../AssetBundles/";
    public string 		resourceBundleFileExtension	= ".unity3d"; 
	public string 		sceneBundleFileExtension	= ".scene3d"; 
    public bool 		optionalSettings 			= false;
    public BuildTarget 	buildTarget 				= BuildTarget.Android;
	
	public bool addFolderBuildTarget = true;
    //BuildAssetBundleOptions
    public bool buildAssetBundleOptions = true;
    public bool collectDependencies = true;
    public bool completeAssets = true;
    public bool disableWriteTypeTree = false;
    public bool deterministicAssetBundle = false;
    public bool uncompressedAssetBundle = false; 
	
    public Dictionary<string, int> bundleVersions = new Dictionary<string, int>();
    public Dictionary<string, List<string>> bundleContents = new Dictionary<string, List<string>>();
    public Dictionary<string, float> bundleFileSizes = new Dictionary<string, float>();
	
	//The position of the scrollview
	private Vector2 scrollPosition = Vector2.zero;
	
	//The undo manager
	//HOEditorUndoManager undoManager;
	
	public List<AssetBundleInfo> resourceAssetBundleInfos = new List<AssetBundleInfo>();
	public List<AssetBundleInfo> sceneAssetBundleInfos = new List<AssetBundleInfo>();
	
	class Directory : UnityEngine.Object
	{
		
	}
	
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
		if(GUILayout.Button("Open", GUILayout.Width(50)))
		{
			
		}
		EditorGUILayout.EndHorizontal();
		
		 addFolderBuildTarget	= EditorGUILayout.Toggle("Add Folder Build Target", addFolderBuildTarget);
		
        exportLocation 					= EditorGUILayout.TextField("Export folder", exportLocation);
        resourceBundleFileExtension 	= EditorGUILayout.TextField("Bundle Resource file ext.", resourceBundleFileExtension); 
		sceneBundleFileExtension 		= EditorGUILayout.TextField("Bundle Scene file ext.", sceneBundleFileExtension); 

        buildAssetBundleOptions 	= EditorGUILayout.BeginToggleGroup("BuildAssetBundleOptions", buildAssetBundleOptions);
        collectDependencies 		= EditorGUILayout.Toggle("CollectDependencies", collectDependencies);
        completeAssets 				= EditorGUILayout.Toggle("CompleteAssets", completeAssets);
        disableWriteTypeTree 		= EditorGUILayout.Toggle("DisableWriteTypeTree", disableWriteTypeTree);
        deterministicAssetBundle 	= EditorGUILayout.Toggle("DeterministicAssetBundle", deterministicAssetBundle);
        uncompressedAssetBundle 	= EditorGUILayout.Toggle("UncompressedAssetBundle", uncompressedAssetBundle);
        EditorGUILayout.EndToggleGroup();

        optionalSettings 	= EditorGUILayout.BeginToggleGroup("Optional Settings", optionalSettings);
        buildTarget 		= (BuildTarget)EditorGUILayout.EnumPopup("Build Target", buildTarget);
        EditorGUILayout.EndToggleGroup();
		
		//undoManager.CheckDirty();
  
		if (GUILayout.Button("Add New Resource AssetBundle"))
        {
			AssetBundleInfo assetBundleInfo = new AssetBundleInfo(); 
			assetBundleInfo.SetModeResource();
			assetBundleInfo.assetBundleFileName = GetNewAssetBundleFileName(false);
			resourceAssetBundleInfos.Add(assetBundleInfo);
		}
  
		OnGUIAssetBundleInfo(resourceAssetBundleInfos);
		
 		if (GUILayout.Button("Add New Scene AssetBundle"))
        {
			AssetBundleInfo assetBundleInfo = new AssetBundleInfo(); 
			assetBundleInfo.SetModeScene();
			assetBundleInfo.assetBundleFileName = GetNewAssetBundleFileName(true);
			sceneAssetBundleInfos.Add(assetBundleInfo);
		}
		 
		OnGUIAssetBundleInfo(sceneAssetBundleInfos);
		
 
		EditorGUILayout.EndScrollView();
		
 
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
	void OnGUIAssetBundleInfo(List<AssetBundleInfo> assetBundleInfos)
    {
		List<AssetBundleInfo> removes = new List<AssetBundleInfo>();
		foreach(AssetBundleInfo assetBundleInfo in assetBundleInfos)
		{   
			GUIContent content = new GUIContent();
			string currentBundleFileExtension = this.resourceBundleFileExtension;
			if(assetBundleInfo.isScene)
			{
				currentBundleFileExtension = this.sceneBundleFileExtension;
			} 
			
			content.text = assetBundleInfo.assetBundleFileName + currentBundleFileExtension;
			content.tooltip = "에셋번들을 만듭니다.";
			
			assetBundleInfo.uiToggleGroup = EditorGUILayout.BeginToggleGroup(content, assetBundleInfo.uiToggleGroup); 
			if(assetBundleInfo.uiToggleGroup)
			{  
  
				EditorGUILayout.BeginHorizontal();
				  
				GUILayout.Label ("Create", EditorStyles.boldLabel, GUILayout.Width(55));
				string createButtonName = assetBundleInfo.assetBundleFileName + currentBundleFileExtension + ")";
 
				if (GUILayout.Button(createButtonName))
				{
					string assetBundleFolder = this.GetAssetBundleFolder(true);
					string exportLocationFull = assetBundleFolder + "/" + assetBundleInfo.assetBundleFileName + currentBundleFileExtension;
					List<TypeExt> typeExts = assetBundleInfo.GetTypeExt();
					if(assetBundleInfo.isScene)
					{ 
						KtCreateAssetBundles.BuildAssetBundleSceneFromDictionary(this, assetBundleInfo.myObjects, exportLocationFull);  
					}
					else
					{
						KtCreateAssetBundles.BuildAssetBundleFromDictionary(this, assetBundleInfo.myObjects, exportLocationFull); 
					} 
				} 
 
				if (GUILayout.Button("Delete Setting"))
				{
					removes.Add(assetBundleInfo);
				}
				
				EditorGUILayout.EndHorizontal();
				if(assetBundleInfo.resourceEnum != null)
				{   
					assetBundleInfo.resourceEnum.filterFile		= EditorGUILayout.BeginToggleGroup("Filter", assetBundleInfo.resourceEnum.filterFile);
					if(assetBundleInfo.resourceEnum.filterFile)
					{
						assetBundleInfo.resourceEnum.prefab 		= EditorGUILayout.Toggle("	Prefab (.prefab)	", assetBundleInfo.resourceEnum.prefab);
	 					assetBundleInfo.resourceEnum.materialFile 	= EditorGUILayout.Toggle("	MaterialFile (.mat)	", assetBundleInfo.resourceEnum.materialFile);
						assetBundleInfo.resourceEnum.imageFile 		= EditorGUILayout.Toggle("	ImageFile (.png)	", assetBundleInfo.resourceEnum.imageFile);
						assetBundleInfo.resourceEnum.animationFile 	= EditorGUILayout.Toggle("	AnimationFile (.animation)	", assetBundleInfo.resourceEnum.animationFile);
						assetBundleInfo.resourceEnum.textFile 		= EditorGUILayout.Toggle("	TextFile(.txt)	", assetBundleInfo.resourceEnum.textFile); 
					} 
					EditorGUILayout.EndToggleGroup();
					//assetBundleInfo.resourceEnum.anythingFile	= EditorGUILayout.Toggle("AllFile	", assetBundleInfo.resourceEnum.anythingFile); 
				}
				
				//version
				assetBundleInfo.version 			= EditorGUILayout.IntField("Version", assetBundleInfo.version); 
				assetBundleInfo.assetBundleFileName = EditorGUILayout.TextField("Create FileName", assetBundleInfo.assetBundleFileName);

				Object objMultiSelect = EditorGUILayout.ObjectField("Drag Here->", null, typeof(UnityEngine.Object), true);
 				if(objMultiSelect != null)
				{
					Object []newObjMultiSelects = Selection.GetFiltered(typeof(UnityEngine.Object),SelectionMode.DeepAssets);
					bool findObj = false;
					 
					foreach(Object newObjMultiSelect in newObjMultiSelects)
					{
						if(newObjMultiSelect.GetInstanceID() == objMultiSelect.GetInstanceID())
						{
							findObj = true;
							break;
						}
					}
					
					if(findObj)
					{
						foreach(Object newObjMultiSelect in newObjMultiSelects)
						{
							List<TypeExt> typeExts = assetBundleInfo.GetTypeExt(); 
							bool isTypeOK = true;
							foreach(TypeExt typeExt in typeExts)
							{
								if(!isWrite(newObjMultiSelect, typeExt.ext))
								{
									isTypeOK = false;
									break;
								}
							}
							
							if(isTypeOK)
							{
								assetBundleInfo.myObjects[newObjMultiSelect.GetInstanceID()] = newObjMultiSelect;
							} 
							else 
							{
								string outPath = "";
								if(isFolder( newObjMultiSelect,ref outPath))
								{ 
									GetFolderObj(outPath, assetBundleInfo.myObjects, assetBundleInfo.assetFileExt,typeof(UnityEngine.Object));
								} 
							}
						}
					} 
					
					if(isWrite(objMultiSelect, assetBundleInfo.assetFileExt))
					{
						assetBundleInfo.myObjects[objMultiSelect.GetInstanceID()] = objMultiSelect;
					} 
					else 
					{
						string outPath = "";
						if(isFolder( objMultiSelect,ref outPath))
						{
							
							GetFolderObj(outPath, assetBundleInfo.myObjects, assetBundleInfo.assetFileExt,typeof(UnityEngine.Object));
						} 
					}
				}
				 
				assetBundleInfo.uiEditingData = EditorGUILayout.BeginToggleGroup("Editing Datas", assetBundleInfo.uiEditingData);
 
				  
				if(assetBundleInfo.myObjects != null)
				{
					List<UnityEngine.Object> removeObjects = new List<UnityEngine.Object>();
					
					foreach(KeyValuePair<int, Object>  keyValuePair in assetBundleInfo.myObjects)
					{
						EditorGUILayout.BeginHorizontal(); 
						Object obj = keyValuePair.Value;
						EditorGUILayout.ObjectField("		Data", obj, typeof(UnityEngine.Object), true);
						
						if(GUILayout.Button("X", GUILayout.Width(25)))
						{
							removeObjects.Add(keyValuePair.Value);
						}
						EditorGUILayout.EndHorizontal();
					}
					
					foreach(UnityEngine.Object removeObj in removeObjects)
					{ 
						assetBundleInfo.myObjects.Remove(removeObj.GetInstanceID());
					}
				} 
				
				//EditorGUILayout.EndToggleGroup();
				
				GUILayout.Label ("---------------------------------------------");
				EditorGUILayout.EndToggleGroup();
			} 
		
			EditorGUILayout.EndToggleGroup();
		}
		foreach(AssetBundleInfo assetBundleInfo in removes)
		{
			assetBundleInfos.Remove(assetBundleInfo);
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
    private void CreateContentWindow()
    {
        if (contentWindow == null)
        {
            contentWindow = AssetBundleContent.CreateContentWindow();
        }
    }
    private static string GetXmlFileName()
    {
		string assetBundleEditorOption = "AssetBundleEditorOption.xml";
		string pathAssetBundleEditorOption = Application.dataPath + "/KtAssetBundle/" + assetBundleEditorOption;
		return pathAssetBundleEditorOption;
	}
    private static void ReadEditorPrefs(KtAssetBundleWindow thisWindow)
    {      
 		System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
		string assetBundleEditorOption = "AssetBundleEditorOption.xml";
		string pathAssetBundleEditorOption = GetXmlFileName();
		
		try
		{
			xmlDocument.Load(pathAssetBundleEditorOption);
			XmlElement exportSetting = xmlDocument.DocumentElement.SelectSingleNode("ExportSetting") as XmlElement;
			if(exportSetting != null)
			{    
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
				ReadEditorAssetBundleInfo(thisWindow, assetBundleInfos, thisWindow.resourceAssetBundleInfos);
			}
			
			assetBundleSettings = xmlDocument.DocumentElement.SelectSingleNode("SceneAssetBundleInfos") as XmlElement; 
			if(assetBundleSettings != null)
			{ 
				thisWindow.sceneAssetBundleInfos.Clear();
				XmlNodeList assetBundleInfos = assetBundleSettings.SelectNodes("AssetBundleInfo");
				ReadEditorAssetBundleInfo(thisWindow, assetBundleInfos, thisWindow.sceneAssetBundleInfos);
			}
		}
		catch(System.Xml.XmlException e)
		{
			Debug.LogError("Xml Load Error: " + pathAssetBundleEditorOption);
			Debug.LogError(e.Message);
		}
		
		//xmlDocument.
    }
    private static void ReadEditorAssetBundleInfo(KtAssetBundleWindow thisWindow, XmlNodeList xmlAssetBundleInfos, List<AssetBundleInfo> assetBundleInfos)
    {
		foreach(XmlNode xmlNode in xmlAssetBundleInfos)
		{
			XmlElement assetBundleSettingInfo = xmlNode as XmlElement;
			if(assetBundleSettingInfo != null)
			{ 
				AssetBundleInfo	assetBundleInfo 		= new AssetBundleInfo();  
				string temp; 
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
				temp = assetBundleSettingInfo.GetAttribute("AssetFolder"); 
				if(!string.IsNullOrEmpty(temp))
				{
					assetBundleInfo.sourceFolder = temp;
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
		xmlWriter.WriteAttributeString("ExportFolder", thisWindow.exportLocation);
		xmlWriter.WriteAttributeString("ResourceBundleFileExtension", thisWindow.resourceBundleFileExtension);
		xmlWriter.WriteAttributeString("SceneBundleFileExtension", thisWindow.sceneBundleFileExtension);
		
		xmlWriter.WriteAttributeString("BuildTarget", thisWindow.buildTarget.ToString());
		xmlWriter.WriteEndElement();
		 
		xmlWriter.WriteStartElement("ResourceAssetBundleInfos");
		WriteEditorAssetBundleInfo(thisWindow, xmlWriter, thisWindow.resourceAssetBundleInfos); 
		xmlWriter.WriteEndElement();
		xmlWriter.WriteStartElement("SceneAssetBundleInfos");
		WriteEditorAssetBundleInfo(thisWindow, xmlWriter, thisWindow.sceneAssetBundleInfos); 
		xmlWriter.WriteEndElement();		
		
		xmlWriter.WriteEndElement();
		xmlWriter.WriteEndDocument();
		xmlWriter.Flush();
		xmlWriter.Close();
    }
    private static void WriteEditorAssetBundleInfo(KtAssetBundleWindow thisWindow, System.Xml.XmlWriter xmlWriter, List<AssetBundleInfo> assetBundleInfos)
    {
		foreach(AssetBundleInfo assetBundleInfo in assetBundleInfos)
		{ 
			xmlWriter.WriteStartElement("AssetBundleInfo");
			xmlWriter.WriteAttributeString("Version", assetBundleInfo.version.ToString()); 
			xmlWriter.WriteAttributeString("AssetBundleFileName", assetBundleInfo.assetBundleFileName);
			xmlWriter.WriteAttributeString("AssetFolder", assetBundleInfo.sourceFolder);
			xmlWriter.WriteEndElement();
		} 
	}
	
    public static void ClearPreferences(KtAssetBundleWindow thisWindow)
    { 
        thisWindow.exportLocation = "AssetBundles";
        thisWindow.resourceBundleFileExtension = ".unity3d";
		 thisWindow.sceneBundleFileExtension = ".scene3d";

        thisWindow.optionalSettings = false;
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
    }

    //Show window
    [MenuItem("KtAssets/Asset Bundle Creator")]
    public static void ShowWindow()
    {
        KtAssetBundleWindow thisWindow = (KtAssetBundleWindow)EditorWindow.GetWindow(typeof(KtAssetBundleWindow));
        thisWindow.title = "KtBundle Creator"; 
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
		string []splitedApplicationContentsPath = Application.persistentDataPath.Split(new char[2]{'\\','/'}, System.StringSplitOptions.RemoveEmptyEntries);
		fileName = splitedApplicationContentsPath[splitedApplicationContentsPath.Length-1];
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
	
	bool isWrite(Object obj, string ext)
	{ 
		string objPath = AssetDatabase.GetAssetOrScenePath(obj);
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
}
