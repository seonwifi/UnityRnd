// Pre-Integrated Skin Shader for Unity3D
//  
// Author:
//       Maciej Kacper Jagiełło <maciej@jagiello.it>
// 
// Copyright (c) 2013 Maciej Kacper Jagiełło
// 
// This file is provided under standard Unity Asset Store EULA
// http://unity3d.com/company/legal/as_terms

Shader "Skin/PreIntegratedSkinShaderV1.1_SM5" {
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
		_DisplacementScale ("Tessellation Displacement Scale", Float) = 0.0
		_DisplacementOffset ("Tessellation Displacement Offset", Float) = 0.0
		_EdgeLength ("Tessellation Edge length", Range(2,50)) = 5
		_Phong ("Tessellation Phong Strengh", Range(0,1)) = 0.5
        _TessDivisions ("Tessellation Subdivision Factor", Range(1,32)) = 8
        _TessRadius ("Tessellation Outline Radius", Range(-1,1)) = 0.25
        _TessCullAngle ("Tessellation Backface Cull Angle", Range(0.5,2)) = 1
		_DispTex ("Tessellation Displacement (R)", 2D) = "black" {}
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
		LOD 600
		
		CGPROGRAM
			#pragma surface surf Skin addshadow fullforwardshadows nolightmap nodirlightmap exclude_path:prepass vertex:dispSkin tessellate:tessSkin tessphong:_Phong  
			#pragma target 5.0
			#pragma glsl

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
						
			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _SkinMask;
			sampler2D _TranslucencyMap;
			sampler2D _SpecularMap;
			sampler2D _GlossinessMap;
			sampler2D _DepthMap;
			sampler2D _DispTex;

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
	Fallback "Skin/PreIntegratedSkinShaderV1.1_SM3"
}
