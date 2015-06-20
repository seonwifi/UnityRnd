// Pre-Integrated Skin Shader for Unity3D
//  
// Author:
//       Maciej Kacper Jagiełło <maciej@jagiello.it>
// 
// Copyright (c) 2013 Maciej Kacper Jagiełło
// 
// This file is provided under standard Unity Asset Store EULA
// http://unity3d.com/company/legal/as_terms

using UnityEditor;
using UnityEngine;
 
using System.IO;
 
class GenerateTranslLookupTextureWizard : ScriptableWizard {
	public int width = 256;
	public int height = 256;
 
    [MenuItem ("GameObject/Pre-Integrated Skin Shader/Generate Lookup Texture - translucency")]
    static void CreateWizard () {
        ScriptableWizard.DisplayWizard<GenerateTranslLookupTextureWizard>("PreIntegrate Lookup Textures", "Create");
    }
 
	float Gaussian (float v, float r) {
		return 1.0f / Mathf.Sqrt(2.0f * Mathf.PI * v) * Mathf.Exp(-(r * r) / (2 * v));
	}
 
	Vector3 ScatterDepth (float dd) {
		// Values from GPU Gems 3 "Advanced Skin Rendering"
		// Originally taken from real life samples
		return Mathf.Exp(dd / 0.0064f) * new Vector3(0.233f, 0.455f, 0.649f) +
				Mathf.Exp(dd / 0.0484f) * new Vector3(0.100f, 0.336f, 0.344f) +
				Mathf.Exp(dd / 0.1870f) * new Vector3(0.118f, 0.198f, 0.000f) +
				Mathf.Exp(dd / 0.5670f) * new Vector3(0.113f, 0.007f, 0.007f) +
				Mathf.Exp(dd / 1.9900f) * new Vector3(0.358f, 0.004f, 0.00001f) +
				Mathf.Exp(dd / 7.4100f) * new Vector3(0.078f, 0.000f, 0.00001f);
	}		
 
    void OnWizardCreate () {
		string path = Application.dataPath + "/PreIntegratedSkinShader V1.1/skin_lookup_translucency.png";
		string pathRel = "Assets/PreIntegratedSkinShader V1.1/skin_lookup_translucency.png";
		
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
		try {
			for (int j = 0; j < height; ++j) {
				float radius = j / (float) height;
				for (int i = 0; i < width; ++i) {
					float depth = 1f - (i / (float) width);
					float scale = depth / radius;
					float d = scale * depth;					
					
				    float dd = -d * d;
					
					Vector3 profile = ScatterDepth(dd);
					
					
					texture.SetPixel(i, j, new Color(profile.x,profile.y,profile.z));
				}
				
				float progress = (float)j / (float)height;
				bool canceled = EditorUtility.DisplayCancelableProgressBar("generating lookup texture", "", progress);
				if (canceled)
					return;				
			}
				
			texture.Apply();

			byte[] bytes = texture.EncodeToPNG();
			File.WriteAllBytes(path, bytes);
		} finally {
			DestroyImmediate(texture);
			EditorUtility.ClearProgressBar();
		}
		
		// not set import settings for the texture
		// It needs to be clamped and it shouldn't be compressed.
		TextureImporter textureImporter = TextureImporter.GetAtPath(pathRel) as TextureImporter;
		textureImporter.textureFormat = TextureImporterFormat.ARGB32;
		textureImporter.textureType = TextureImporterType.Advanced;
		textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        textureImporter.mipmapEnabled = false;
        textureImporter.linearTexture = true;
		textureImporter.filterMode = FilterMode.Bilinear;
		textureImporter.wrapMode = TextureWrapMode.Clamp;
        textureImporter.maxTextureSize = Mathf.Max(width, height);
        AssetDatabase.ImportAsset(pathRel, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
    }
 
    void OnWizardUpdate () {
        helpString = "Press Create to create lookup textures. You have to set wrap mode to clamp manually for correct results.";
    }
}