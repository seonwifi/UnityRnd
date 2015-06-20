// Pre-Integrated Skin Shader for Unity3D
//  
// Author:
//       Maciej Kacper Jagiełło <maciej@jagiello.it>
// 
// Copyright (c) 2013 Maciej Kacper Jagiełło
// 
// This file is provided under standard Unity Asset Store EULA
// http://unity3d.com/company/legal/as_terms

// This is an example of how to make a customized and more efficient version of the skin shader.
//
// It differs from the default implementation in:
// 1. It's implemented as a single shader with subshader fallbacks in the same file to limit clutter.
// 2. Specular, Glossiness and Depth maps are combined into one texture and skin mask is in alpha of diffuse texture.
//    This way the shader is a little faster, but you have to make the combined textures.
//    See the forum thread for more resources.
// 3. It does not support RGB specular map.
//    Colored specular map has little to offer visually. Turning it off saves two texture channels (but no ALU instructions).
// 4. Translucency color map is disabled
//
// Besides combining to one shader it's how the version 1.0 worked.
//
// See the manual for details on customization options.

Shader "Skin/PreIntegratedSkinShaderV1.1_combined" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_BackRimStrength ("Back Rim Strength", Range(0,1)) = 0.75
		_BackRimSmoothness ("Back Rim Smoothness", Range(20,1)) = 5.0
		_BackRimWidth ("Back Rim Width", Range(0,2)) = 0.0
		_FrontRimStrength ("Front Rim Strength", Range(0,100)) = 20.0
		_FrontRimSmoothness ("Front Rim Smoothness", Range(20,1)) = 2.0
		_FrontRimWidth ("Front Rim Width", Range(0,1)) = 0.5
		_MainTex ("Diffuse Map(RGB) Skin Mask(A)", 2D) = "white" {}
		_BumpinessDR ("Diffuse Bumpiness R", Range(0,2)) = 0.1
		_BumpinessDG ("Diffuse Bumpiness G", Range(0,2)) = 0.6
		_BumpinessDB ("Diffuse Bumpiness B", Range(0,2)) = 0.7
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_FresnelSkin ("Skin Fresnel Value", Float) = 0.028
		_Bumpiness ("Specular Bumpiness", Range(0,2)) = 0.9
		_SpecIntensity ("Specular Intensity", Range(0,10)) = 1.0
		_SpecRoughness ("Specular Roughness", Range(0.5,1)) = 0.7
		_SpecGlosDepthMap ("Specular (R) Glosiness(G) Depth (B)", 2D) = "white" {}		
		_DisplacementScale ("Tessellation Displacement Scale", Float) = 0.0
		_DisplacementOffset ("Tessellation Displacement Offset", Float) = 0.0
		_EdgeLength ("Tessellation Edge length", Range(2,50)) = 5
		_Phong ("Tessellation Phong Strengh", Range(0,1)) = 0.5
        _TessDivisions ("Tessellation Subdivision Factor", Range(1,32)) = 8
        _TessRadius ("Tessellation Outline Radius", Range(-1,1)) = 0.25
        _TessCullAngle ("Tessellation Backface Cull Angle", Range(0.5,2)) = 1		
		_DispTex ("Tessellation Displacement (R)", 2D) = "black" {}
		_ScatteringOffset ("Scattering Boost", Range(0,1)) = 0.0
		_ScatteringPower ("Scattering Power", Range(0,2)) = 1.0  
		_TranslucencyOffset ("Translucency Offset", Range(0,1)) = 0.0
		_TranslucencyPower ("Translucency Power", Range(0,10)) = 1
		_TranslucencyRadius ("Translucency Radius", Range(0,1)) = 1	
		_TranslucencyColor ("Translucency Tint Color", Color) = (1,1,1,1)
		_LookupTranslucency("Lookup: Translucency (RGB)", 2D) = "white" {}
		_LookupDiffuseSpec ("Lookup Map: Diffuse Falloff(RGB) Specular(A)", 2D) = "gray" {}
	}
		
	Category {
		Tags {
			"RenderType"="Opaque"
		}
		CGINCLUDE
			// included in all subshaders
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _SpecGlosDepthMap;
			sampler2D _DispTex;
//			sampler2D _TranslucencyMap;
		ENDCG
		SubShader {
			Tags { "RenderType"="Opaque" }
			LOD 600
			
			CGPROGRAM
				#pragma surface surf Skin addshadow fullforwardshadows nolightmap nodirlightmap exclude_path:prepass vertex:dispSkin tessellate:tessSkin tessphong:_Phong  
				#pragma target 5.0
				
				// currently unity supports SM5 only on dx11, no tessellation goodness on opengl
				// I'm exluding other renderers (in particular d3d11_9x), there the shader should fallback on SM3 version.
				#pragma only_renderers d3d11
				
				#define ENABLE_TRANSLUCENCY 1
				#define ENABLE_BACK_RIM 1
				#define ENABLE_FRONT_RIM 1
				#define ENABLE_DIFFUSE 1
				#define ENABLE_SPECULAR 1
				#define ENABLE_SEPARATE_DIFFUSE_NORMALS 1
				#define ENABLE_TESSELLATION 1
				#define NO_TRANSLUCENCY_LOOKUP 1
				
				struct Input {
					// Only using uvs of diffuse map for everything (another SM2 limitation).
					// This should not be a problem in most cases.
					float2 uv_MainTex;
				};
				
				#include "PreIntegratedSkinShaderCommon.cginc"
				
				void surf(Input IN, inout SkinSurfaceOutput o) {
					float2 uv = IN.uv_MainTex; // only using uvs of diffuse map
					
					fixed4 main = tex2D(_MainTex, uv).rgba * _Color.rgba;
					fixed3 sgd = tex2D(_SpecGlosDepthMap, uv).rgb;
					
					fixed3 albedo = main.rgb;
					float3 specular = (sgd.x * _SpecIntensity).xxx;
					float gloss = sgd.y * _SpecRoughness;
					float depth = sgd.z;
					float mask = main.a;
//					half3 translucencyColor = tex2D(_TranslucencyMap, uv).rgb * TranslucencyColor.rgb;
					half3 translucencyColor = _TranslucencyColor.rgb;
				
					surfSkin(o, uv, _BumpMap, albedo, depth, specular, gloss, mask, translucencyColor);
				}
			ENDCG
		}
		SubShader {
			Tags { "RenderType"="Opaque" }
			LOD 500
			
			CGPROGRAM
				#pragma surface surf Skin addshadow fullforwardshadows nolightmap nodirlightmap exclude_path:prepass
				#pragma target 3.0
				#pragma exclude_renderers flash
				#pragma glsl
				
				#define ENABLE_TRANSLUCENCY 1
				#define ENABLE_BACK_RIM 1
				#define ENABLE_FRONT_RIM 1
				#define ENABLE_DIFFUSE 1
				#define ENABLE_SPECULAR 1
				#define ENABLE_SEPARATE_DIFFUSE_NORMALS 1
				#define NO_TRANSLUCENCY_LOOKUP 1
				
				#include "PreIntegratedSkinShaderCommon.cginc"
				
				struct Input {
					// Only using uvs of diffuse map for everything (another SM2 limitation).
					// This should not be a problem in most cases.
					float2 uv_MainTex;
				};
				
				void surf (Input IN, inout SkinSurfaceOutput o) {
					float2 uv = IN.uv_MainTex; // only using uvs of diffuse map
					
					fixed4 main = tex2D(_MainTex, uv).rgba * _Color.rgba;
					fixed3 sgd = tex2D(_SpecGlosDepthMap, uv).rgb;
					
					fixed3 albedo = main.rgb;
					float3 specular = (sgd.x * _SpecIntensity).xxx;
					float gloss = sgd.y * _SpecRoughness;
					float depth = sgd.z;
					float mask = main.a;
//					half3 translucencyColor = tex2D(_TranslucencyMap, uv).rgb * TranslucencyColor.rgb;
					half3 translucencyColor = _TranslucencyColor.rgb;
				
					surfSkin(o, uv, _BumpMap, albedo, depth, specular, gloss, mask, translucencyColor);
				}	
			ENDCG
		}
		SubShader {
			Tags { "RenderType"="Opaque" }
			LOD 400
			
			CGPROGRAM
				#pragma surface surf Skin nodirlightmap nolightmap  exclude_path:prepass  addshadow approxview
				#pragma exclude_renderers flash
				#pragma target 2.0
				#pragma glsl
				
				#define ENABLE_DIFFUSE 1
				#define ENABLE_SPECULAR 1
				#define ENABLE_MASK 1
	
				struct Input {
					// Only using uvs of diffuse map for everything (another SM2 limitation).
					// This should not be a problem in most cases.
					float2 uv_MainTex;
				};
				
				#include "PreIntegratedSkinShaderCommon.cginc"
				
				void surf(Input IN, inout SkinSurfaceOutput o) {
					float2 uv = IN.uv_MainTex; // only using uvs of diffuse map
					
					fixed4 main = tex2D(_MainTex, uv).rgba * _Color.rgba;
					fixed3 sgd = tex2D(_SpecGlosDepthMap, uv).rgb;
					
					fixed3 albedo = main.rgb;
					float3 specular = (sgd.x * _SpecIntensity).xxx;
					float gloss = sgd.y * _SpecRoughness;
					float depth = sgd.z;
					float mask = main.a;
					
					half3 translucencyColor = 0;
					
					surfSkin(o, uv, _BumpMap, albedo, depth, specular, gloss, mask, translucencyColor);
				}
			ENDCG
			
			CGPROGRAM
				#pragma surface surf Skin nodirlightmap nolightmap exclude_path:prepass decal:add noambient
				#pragma exclude_renderers flash
				#pragma target 2.0
				#pragma glsl
				
				#define ENABLE_TRANSLUCENCY 1
				#define ENABLE_BACK_RIM 1
				#define ENABLE_FRONT_RIM 1
				#define ENABLE_MASK 1
	
				struct Input {
					// Only using uvs of diffuse map for everything (another SM2 limitation).
					// This should not be a problem in most cases.
					float2 uv_MainTex;
				};
				
				#include "PreIntegratedSkinShaderCommon.cginc"
				
				void surf(Input IN, inout SkinSurfaceOutput o) {
					float2 uv = IN.uv_MainTex; // only using uvs of diffuse map
	
					fixed4 main = tex2D(_MainTex, uv).rgba * _Color.rgba;
					fixed3 sgd = tex2D(_SpecGlosDepthMap, uv).rgb;
					
					fixed3 albedo = main.rgb;
					float3 specular = 0;
					float gloss = 0;
					float depth = sgd.z;
					float mask = main.a;
//					half3 translucencyColor = tex2D(_TranslucencyMap, uv).rgb * _TranslucencyColor.rgb;
					half3 translucencyColor = _TranslucencyColor.rgb;
					
					surfSkin(o, uv, _BumpMap, albedo, depth, specular, gloss, mask, translucencyColor);
				}
			ENDCG
		}
	}
	Fallback "VertexLit"
}
