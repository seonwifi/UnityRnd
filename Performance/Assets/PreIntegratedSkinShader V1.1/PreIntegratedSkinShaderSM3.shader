// Pre-Integrated Skin Shader for Unity3D
//  
// Author:
//       Maciej Kacper Jagiełło <maciej@jagiello.it>
// 
// Copyright (c) 2013 Maciej Kacper Jagiełło
// 
// This file is provided under standard Unity Asset Store EULA
// http://unity3d.com/company/legal/as_terms

Shader "Skin/PreIntegratedSkinShaderV1.1_SM3" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_BackRimStrength ("Back Rim Strength", Range(0,1)) = 0.75
		_BackRimSmoothness ("Back Rim Smoothness", Range(20,1)) = 5.0
		_BackRimWidth ("Back Rim Width", Range(0,2)) = 0.0
		_FrontRimStrength ("Front Rim Strength", Range(0,100)) = 20.0
		_FrontRimSmoothness ("Front Rim Smoothness", Range(20,1)) = 2.0
		_FrontRimWidth ("Front Rim Width", Range(0,1)) = 0.5
		_MainTex ("Diffuse Map(RGB)", 2D) = "white" {}
		_BumpinessDR ("Diffuse Bumpiness R", Range(0,2)) = 0.1
		_BumpinessDG ("Diffuse Bumpiness G", Range(0,2)) = 0.6
		_BumpinessDB ("Diffuse Bumpiness B", Range(0,2)) = 0.7
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_FresnelSkin ("Skin Fresnel Value", Float) = 0.028
		_FresnelMasked ("Masked Fresnel Value", Float) = 0.01
		_Bumpiness ("Specular Bumpiness", Range(0,2)) = 0.9
		_SpecIntensity ("Specular Intensity", Range(0,10)) = 1.0
		_SpecularMap ("Specular (RGB)", 2D) = "white" {}
		_SpecRoughness ("Specular Roughness", Range(0.5,1)) = 0.7
		_GlossinessMap ("Glossiness (R)", 2D) = "white" {}
		_SkinMask ("Skin Mask(R)", 2D) = "white" {}
		_ScatteringOffset ("Scattering Boost", Range(0,1)) = 0.0
		_ScatteringPower ("Scattering Power", Range(0,2)) = 1.0  
		_TranslucencyOffset ("Translucency Offset", Range(0,1)) = 0.0
		_TranslucencyPower ("Translucency Power", Range(0,10)) = 1
		_TranslucencyRadius ("Translucency Radius", Range(0,1)) = 1
		_DepthMap ("Depth (R)", 2D) = "white" {}
		_TranslucencyColor ("Translucency Tint Color", Color) = (1,1,1,1)
		_TranslucencyMap ("Translucency Color(RGB)", 2D) = "white" {}
		_LookupTranslucency("Lookup: Translucency (RGB)", 2D) = "white" {}
		_LookupDiffuseSpec ("Lookup Map: Diffuse Falloff(RGB) Specular(A)", 2D) = "gray" {}
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
			
			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _SkinMask;
			sampler2D _TranslucencyMap;
			sampler2D _SpecularMap;
			sampler2D _GlossinessMap;
			sampler2D _DepthMap;

			struct Input {
				// Only using uvs of diffuse map for everything (another SM2 limitation).
				// This should not be a problem in most cases.
				float2 uv_MainTex;
			};
			
			#include "PreIntegratedSkinShaderCommon.cginc"
			
			void surf(Input IN, inout SkinSurfaceOutput o) {
				float2 uv = IN.uv_MainTex; // only using uvs of diffuse map
					
				fixed3 albedo = tex2D(_MainTex, uv).rgb * _Color.rgb;
				float3 specular = tex2D(_SpecularMap, uv).rgb * _SpecIntensity;
				float gloss = tex2D(_GlossinessMap, uv).r * _SpecRoughness;
				float depth = tex2D(_DepthMap, uv).r;
				float mask = tex2D(_SkinMask, uv).r * _Color.a;
				half3 translucencyColor = tex2D(_TranslucencyMap, uv).rgb * _TranslucencyColor.rgb;
				
				surfSkin(o, uv, _BumpMap, albedo, depth, specular, gloss, mask, translucencyColor);
			}
		ENDCG
	}
	Fallback "Skin/PreIntegratedSkinShaderV1.1_SM2_2Pass"
}