//
//  CreateAssetBundles.cs
//
//  Created by Niklas Borglund
//  Copyright (c) 2012 Cry Wolf Studios. All rights reserved.
//  crywolfstudios.net
//
// The class that does all the work in Bundle Creator. 
// It creates the asset bundles in the specified folder and
// stores the information in a control file and a contents file.

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class KtCreateAssetBundles
{
    public static string bundleControlFileName = "bundleControlFile.txt";
    public static string bundleContentsFileName = "bundleContents.txt";

    public static void ReadBundleControlFile(string path, Dictionary<string, int> bundleVersions)
    {
        if (bundleVersions == null)
        {
            bundleVersions = new Dictionary<string, int>();
        }
		else
		{    
			bundleVersions.Clear();	
		}

        if (File.Exists(path))
        {
            StreamReader streamReader = new StreamReader(path);
            string currentLine;
            while ((currentLine = streamReader.ReadLine()) != null)
            {
                if (currentLine.StartsWith("BundleName:"))
                {
                    string bundleName = currentLine.Substring("BundleName:".Length);
                    int bundleVersion = 0;

                    //if the bundlename is there, the versionNumber should be on the next line
                    //otherwise there's an error
                    string nextLine = streamReader.ReadLine();
                    if (nextLine != null && nextLine.StartsWith("VersionNumber:"))
                    {
                        bundleVersion = System.Convert.ToInt32(nextLine.Substring("VersionNumber:".Length));
                    }
                    else
                    {
                        Debug.LogError("CreateAssetBundles.cs: Error reading bundle control file! - Delete the current control file to start over");
                    }

                    bundleVersions.Add(bundleName, bundleVersion);
                }
            }

            streamReader.Close();
        }
    }
    public static void ReadBundleContentsFile(string path, Dictionary<string, List<string>> bundleContents)
    {
        if (bundleContents == null)
        {
            bundleContents = new Dictionary<string, List<string>>();
        }
		else
		{
			bundleContents.Clear();	
		}
        if (File.Exists(path))
        {
            StreamReader streamReader = new StreamReader(path);
            string currentLine;
            while ((currentLine = streamReader.ReadLine()) != null)
            {
                if (currentLine.StartsWith("BundleName:"))
                {
                    string bundleName = currentLine.Substring("BundleName:".Length);

                    int numberOfAssets = 0;
                    string nextLine = streamReader.ReadLine();
                    if (nextLine != null && nextLine.StartsWith("NumberOfAssets:"))
                    {
                        numberOfAssets = System.Convert.ToInt32(nextLine.Substring("NumberOfAssets:".Length));
                    }
                    else
                    {
                        Debug.LogError("CreateAssetBundles.cs: Error reading bundle contents file! - Delete the current control file to start over");
                        break;
                    }
                    List<string> assetsInBundle = new List<string>();
                    for (int i = 0; i < numberOfAssets; i++)
                    {
                        assetsInBundle.Add(streamReader.ReadLine());
                    }

                    bundleContents.Add(bundleName, assetsInBundle);
                }
            }

            streamReader.Close();
        }
    }
    static void WriteBundleControlFile(string path, Dictionary<string, int> bundleVersions, string exportPath)
    {
        StreamWriter streamWriter = new StreamWriter(path);
        foreach (KeyValuePair<string, int> bundleVersion in bundleVersions)
        {
            if (File.Exists(exportPath + bundleVersion.Key))
            {
                streamWriter.WriteLine("BundleName:" + bundleVersion.Key);
                streamWriter.WriteLine("VersionNumber:" + bundleVersion.Value.ToString());

                //For readability in the txt file, add an empty line
                streamWriter.WriteLine();
            }
        }
        streamWriter.Close();
    }
    static void WriteBundleContentsFile(string path, Dictionary<string, List<string>> bundleContents, string exportPath)
    {
        StreamWriter streamWriter = new StreamWriter(path);
        foreach (KeyValuePair<string, List<string>> asset in bundleContents)
        {
            if(File.Exists(exportPath + asset.Key))
            {
                //For readability in the txt file
                streamWriter.WriteLine("BundleName:" + asset.Key);
                streamWriter.WriteLine("NumberOfAssets:" + asset.Value.Count);
                foreach (string assetName in asset.Value)
                {
                    streamWriter.WriteLine(assetName);
                }
            }
        }
        streamWriter.Close();
    }
 
	//resource
    /// <summary>
    /// Helper function to build an asset bundle
    /// This will iterate through all the settings of the AssetBundleWindow and set them accordingly
    /// </summary>
    /// <param name="thisWindow"></param>
    /// <param name="toInclude"></param>
    /// <param name="bundlePath"></param>
    public static bool BuildAssetBundle(KtAssetBundleWindow thisWindow, List<Object> toInclude, string bundlePath)
    {
        BuildAssetBundleOptions buildAssetOptions = 0;
        if(thisWindow.buildAssetBundleOptions)
        {
            if(thisWindow.collectDependencies)
            {
                if(buildAssetOptions == 0)
                {
                    buildAssetOptions = BuildAssetBundleOptions.CollectDependencies;
                }
                else
                {
                    buildAssetOptions |= BuildAssetBundleOptions.CollectDependencies;
                }
            }
            if(thisWindow.completeAssets)
            {
                if(buildAssetOptions == 0)
                {
                    buildAssetOptions = BuildAssetBundleOptions.CompleteAssets;
                }
                else
                {
                    buildAssetOptions |= BuildAssetBundleOptions.CompleteAssets;
                }
            }
            if(thisWindow.disableWriteTypeTree)
            {
                if(buildAssetOptions == 0)
                {
                    buildAssetOptions = BuildAssetBundleOptions.DisableWriteTypeTree;
                }
                else
                {
                    buildAssetOptions |= BuildAssetBundleOptions.DisableWriteTypeTree;
                }

            }
            if(thisWindow.deterministicAssetBundle)
            {
                if(buildAssetOptions == 0)
                {
                    buildAssetOptions = BuildAssetBundleOptions.DeterministicAssetBundle;
                }
                else
                {
                    buildAssetOptions |= BuildAssetBundleOptions.DeterministicAssetBundle;
                }
            }
            if(thisWindow.uncompressedAssetBundle)
            {
                if(buildAssetOptions == 0)
                {
                    buildAssetOptions = BuildAssetBundleOptions.UncompressedAssetBundle;
                }
                else
                {
                    buildAssetOptions |= BuildAssetBundleOptions.UncompressedAssetBundle;
                }
            }
        } 
		else
		{
			if (buildAssetOptions == 0) //If it's still zero, set default values
			{
				Debug.LogWarning("No BuildAssetBundleOptions are set, reverting back to dependency tracking. If you want no dependency tracking uncheck the 'BuildAssetBundleOptions' && 'Optional Settings' toggles all together");
				buildAssetOptions = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets; 
			}
		}

		if (!BuildPipeline.BuildAssetBundle(null, toInclude.ToArray(), bundlePath, buildAssetOptions, thisWindow.buildTarget))
		{
			return false;
		}
		/*
        //If none of "BuildAssetBundleOptions" or "Optional Settings" are set, then create without dependency tracking
        if (!thisWindow.buildAssetBundleOptions && !thisWindow.optionalSettings)
        {
            if (!BuildPipeline.BuildAssetBundle(null, toInclude.ToArray(), bundlePath))
            {
                return false;
            }
        }
        else
        { 
              if (thisWindow.optionalSettings) //Support for different build targets
              {
                  if (!BuildPipeline.BuildAssetBundle(null, toInclude.ToArray(), bundlePath, buildAssetOptions, thisWindow.buildTarget))
                  {
                      return false;
                  }
              }
              else
              {
                  if (!BuildPipeline.BuildAssetBundle(null, toInclude.ToArray(), bundlePath, buildAssetOptions))
                  {
                      return false;
                  }
              }
        }//*/
        return true;
    }
  
	//resource
    public static List<Object> BuildAssetBundleFromDictionary(KtAssetBundleWindow thisWindow, Dictionary<int, Object> objs, string bundlePath)
    {
		List<Object> toInclude = new List<Object>();
		foreach(KeyValuePair<int, Object>  obj in objs)
		{
			toInclude.Add(obj.Value);   
		}
		if(toInclude.Count == 0)
		{
			EditorUtility.DisplayDialog("Fail Scene AssetBundle Create ", "No Have Datas (Look Datas)", "OK");
			return null;
		}	
 		if(BuildAssetBundle( thisWindow, toInclude, bundlePath))
		{
			return toInclude;
		}
		return null;
	}
	
	//scene
	public static bool BuildAssetBundleSceneFromDictionary(KtAssetBundleWindow thisWindow, Dictionary<int, Object> objs, string bundlePath)
    { 
		List<string> toInclude = new List<string>();
		foreach(KeyValuePair<int, Object>  obj in objs)
		{
			string path = AssetDatabase.GetAssetOrScenePath(obj.Value);  
			toInclude.Add(path); 
		}
		if(toInclude.Count == 0)
		{
			EditorUtility.DisplayDialog("Fail Scene AssetBundle Create ", "No Have Scene Files (Look Datas)", "OK");
			return false;
		}
		return BuildAssetBundleScene( thisWindow, toInclude, bundlePath); 
	}	
	//scene
	public static bool BuildAssetBundleScene(KtAssetBundleWindow thisWindow, List<string> toInclude, string bundlePath)
	{  
		BuildPipeline.BuildPlayer(toInclude.ToArray(),bundlePath, thisWindow.buildTarget, BuildOptions.BuildAdditionalStreamedScenes);
		//string result = BuildPipeline.BuildStreamedSceneAssetBundle(toInclude.ToArray(),bundlePath, thisWindow.buildTarget);
 		return true;
	}
}
 