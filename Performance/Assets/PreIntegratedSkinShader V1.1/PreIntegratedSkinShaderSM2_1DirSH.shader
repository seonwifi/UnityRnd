// Pre-Integrated Skin Shader for Unity3D
//  
// Author:
//       Maciej Kacper Jagiełło <maciej@jagiello.it>
// 
// Copyright (c) 2013 Maciej Kacper Jagiełło
// 
// This file is provided under standard Unity Asset Store EULA
// http://unity3d.com/company/legal/as_terms

// fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Skin/PreIntegratedSkinShaderV1.1_SM2_1DirSH" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_BackRimStrength ("Back Rim Strength", Range(0,1)) = 0.75
		_BackRimSmoothness ("Back Rim Smoothness", Range(20,1)) = 5.0
		_BackRimWidth ("Back Rim Width", Range(0,2)) = 0.0
		_FrontRimStrength ("Front Rim Strength", Range(0,100)) = 20.0
		_FrontRimSmoothness ("Front Rim Smoothness", Range(20,1)) = 2.0
		_FrontRimWidth ("Front Rim Width", Range(0,1)) = 0.5
		_MainTex ("Diffuse Map(RGB)", 2D) = "white" {}
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
		_DepthMap ("Depth (R)", 2D) = "white" {}
		_LookupDiffuseSpec ("Lookup Map: Diffuse Falloff(RGB) Specular(A)", 2D) = "gray" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 250
		
		CGPROGRAM
			#pragma surface surf Skin approxview halfasview noforwardadd nodirlightmap nolightmap exclude_path:prepass
			#pragma exclude_renderers flash
			#pragma target 2.0
			#pragma glsl
			
			#define ENABLE_DIFFUSE 1
			#define ENABLE_SPECULAR 1

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
				half3 translucencyColor = 0;
				
				surfSkin(o, uv, _BumpMap, albedo, depth, specular, gloss, mask, translucencyColor);
			}
		ENDCG
	}
	
	Fallback "VertexLit"
}
