// Pre-Integrated Skin Shader for Unity3D
//  
// Author:
//       Maciej Kacper Jagiełło <maciej@jagiello.it>
// 
// Copyright (c) 2013 Maciej Kacper Jagiełło
// 
// This file is provided under standard Unity Asset Store EULA
// http://unity3d.com/company/legal/as_terms

#include "UnityCG.cginc"

#if defined(ENABLE_DIFFUSE) || defined(ENABLE_SPECULAR)
sampler2D _LookupDiffuseSpec;
#endif

#ifdef ENABLE_SPECULAR
float _FresnelSkin;
float _FresnelMasked;
#endif

#ifdef ENABLE_DIFFUSE
float _ScatteringOffset;
float _ScatteringPower;
#endif
			
#ifdef ENABLE_BACK_RIM
float _BackRimStrength;
float _BackRimWidth;
float _BackRimSmoothness;
#endif

#ifdef ENABLE_FRONT_RIM
float _FrontRimStrength;
float _FrontRimWidth;
float _FrontRimSmoothness;
#endif

float _Bumpiness;

#ifdef ENABLE_SEPARATE_DIFFUSE_NORMALS
float _BumpinessDR;
float _BumpinessDG;
float _BumpinessDB;
#endif

float _SpecIntensity;
float _SpecRoughness;

fixed4 _Color;

#ifdef ENABLE_TRANSLUCENCY
float _TranslucencyOffset;
float _TranslucencyPower;
float _TranslucencyRadius;

fixed4 _TranslucencyColor;
sampler2D _LookupTranslucency;
#endif

#ifdef ENABLE_TESSELLATION
	float _DisplacementOffset;
	float _DisplacementScale;
	float _EdgeLength;
	float _Phong;
	float _TessDivisions;
	float _TessRadius;
	float _TessCullAngle;
#endif

struct SkinSurfaceOutput {
    half3 Albedo;
    half3 Normal;
    #ifdef ENABLE_DIFFUSE
	    #ifdef ENABLE_SEPARATE_DIFFUSE_NORMALS
	    half3 NormalBlue;
	    half3 NormalGreen;
	    half3 NormalRed;
	    #endif
	    half Scattering;
    #endif
    half3 Emission;
    half3 Specular;
    half Fresnel;
    half Gloss;
    half Alpha;
    #ifdef ENABLE_TRANSLUCENCY
    half3 Translucency;
    #endif
	#ifdef ENABLE_BACK_RIM
    half BackRimWidth;
    #endif
	#ifdef ENABLE_FRONT_RIM
    half FrontRimWidth;
    #endif
};     

inline fixed4 LightingSkin(SkinSurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten) {
	float NdotL = dot(s.Normal, lightDir); // light ramp
	#if defined(ENABLE_TRANSLUCENCY) || defined(ENABLE_BACK_RIM) || defined(ENABLE_FRONT_RIM)
	float NdotE = dot(s.Normal, viewDir); // faloff/rim
	#endif
	#ifdef ENABLE_SPECULAR
	float3 h = lightDir + viewDir; // Unnormalized half-way vector  
	float3 H = normalize(h);
	float NdotH = dot(s.Normal, H);
	float EdotH = dot(viewDir, H);
	#endif
	
	#ifdef ENABLE_DIFFUSE
		#ifdef ENABLE_SEPARATE_DIFFUSE_NORMALS
		half3 diffNdotL = 0.5 + 0.5 * half3(
			dot(s.NormalRed, lightDir),
			dot(s.NormalGreen, lightDir),
			dot(s.NormalBlue, lightDir));
		#else
		half diffNdotL = 0.5 + 0.5 * NdotL;
		#endif
			
		#ifdef DIRECTIONAL
			diffNdotL *= atten;
		#endif
	
		#ifdef ENABLE_SEPARATE_DIFFUSE_NORMALS
		half3 diff = 2.0 * half3(
			tex2D(_LookupDiffuseSpec, half2(diffNdotL.r, s.Scattering)).r,
			tex2D(_LookupDiffuseSpec, half2(diffNdotL.g, s.Scattering)).g,
			tex2D(_LookupDiffuseSpec, half2(diffNdotL.b, s.Scattering)).b
		);
		#else
		half3 diff = 2.0 * tex2D(_LookupDiffuseSpec, half2(diffNdotL, s.Scattering)).rgb;
		#endif
		
		#ifndef DIRECTIONAL
			diff *= atten;
		#endif
	#endif
	
	#ifdef ENABLE_SPECULAR
		// specular
		float PH = pow( 2.0*tex2D(_LookupDiffuseSpec,float2(NdotH,s.Gloss)).a, 10.0 );
		float exponential = pow(1.0 - EdotH, 5.0);
		float fresnelReflectance = exponential + s.Fresnel * (1.0 - exponential);  
	
		float frSpec = max( PH * fresnelReflectance / dot( h, h ), 0 );
		
		float3 specLevel = saturate(NdotL) * s.Specular * frSpec; // BRDF * dot(N,L) * rho_s
	#endif

	#ifdef ENABLE_TRANSLUCENCY
//    half3 translucency = s.Translucency * saturate((1-NdotL)*dot(s.Normal, (viewDir-lightDir)  ));
    half3 translucency = s.Translucency * saturate(NdotE - NdotL);
    #endif
    
	half4 c = _LightColor0.rgba;

    #ifdef ENABLE_FRONT_RIM
	half frim = (pow(saturate((1-NdotE)*(NdotL)) * s.FrontRimWidth, _FrontRimSmoothness)) * _FrontRimStrength;
	#endif
    #ifdef ENABLE_BACK_RIM
	half brim = (pow(saturate((1-NdotE)*(1-NdotL) * s.BackRimWidth), _BackRimSmoothness)) * _BackRimStrength;
	#endif
	
	c.rgb *= 
		s.Albedo * (
			#ifdef ENABLE_DIFFUSE
			diff
			#else
			0 // just so other terms compile
			#endif
			#ifdef ENABLE_TRANSLUCENCY
			+ translucency * atten.xxx
			#endif
		    #ifdef ENABLE_FRONT_RIM
			+ (frim * atten).xxx
			#endif
		)
		#ifdef ENABLE_SPECULAR
			+ (specLevel * atten.xxx)
		#endif
		
	    #ifdef ENABLE_BACK_RIM
		+ brim.xxx
		#endif
		;
	
	return c;
}

inline void surfSkin (inout SkinSurfaceOutput o, float2 uv, sampler2D bumpMap, float3 albedo, float depth, float3 specular, float gloss, float mask, half3 translucencyColor) {
	#if defined(ENABLE_DIFFUSE) && defined(ENABLE_SEPARATE_DIFFUSE_NORMALS)
		float3 normalHigh = UnpackNormal(tex2D(_BumpMap, uv));
		
		#ifndef NOGLSL
			// use mipmapping as "free" smoothed normals
			float3 normalLow = UnpackNormal(tex2Dbias(_BumpMap, float4(uv, 0, 3)));
		#else
			// ARB target: fallback to smoothing to fully perpendicular normal instead of low res mipmap.
			// This could result in differences if normal map contains larger features, not just small bumps.
			float3 normalLow = half3(0,0,1);
		#endif
		
		o.Normal = normalize(lerp(normalLow, normalHigh, _Bumpiness));

		float3 diffuseBumpiness = float3(_BumpinessDR, _BumpinessDG, _BumpinessDB);
		
		diffuseBumpiness = lerp(_Bumpiness.xxx, diffuseBumpiness, mask);
		
		o.NormalRed = normalize(lerp(normalLow, normalHigh, diffuseBumpiness.r));
		o.NormalGreen = normalize(lerp(normalLow, normalHigh, diffuseBumpiness.g));
		o.NormalBlue = normalize(lerp(normalLow, normalHigh, diffuseBumpiness.b));
	#else
		#ifndef NOGLSL
			o.Normal = UnpackNormal(tex2Dbias(_BumpMap, float4(uv, 0, (1-_Bumpiness)*3)));
		#else
			o.Normal = normalize(lerp(half3(0,0,1),UnpackNormal(tex2D(_BumpMap, uv)), _Bumpiness));
		#endif
	#endif
	
	#ifdef ENABLE_SPECULAR
		o.Specular = specular;
		o.Gloss = gloss;
		o.Fresnel = lerp(_FresnelMasked, _FresnelSkin, mask);
	#endif
	
	#ifdef ENABLE_DIFFUSE
		o.Scattering = saturate((depth + _ScatteringOffset) * _ScatteringPower);
		o.Scattering *= mask;
	#endif
					
	o.Albedo = albedo;
	
	#if defined(ENABLE_BACK_RIM) || defined(ENABLE_FRONT_RIM)
		half rimSpread = 1 - depth*0.8;
		rimSpread *= mask;
		#ifdef ENABLE_BACK_RIM
		    o.BackRimWidth = _BackRimWidth * rimSpread;
	    #endif
		#ifdef ENABLE_FRONT_RIM
    		o.FrontRimWidth = _FrontRimWidth * rimSpread;
	    #endif    	
	#endif

	#ifdef ENABLE_TRANSLUCENCY
		#ifndef NO_TRANSLUCENCY_LOOKUP
			half3 translucencyProfile = tex2D(_LookupTranslucency, half2(depth - _TranslucencyOffset, _TranslucencyRadius)).rgb;
		#else
		    // Using lookup texture can actually be slower on newer hardware so I left the possibility to
		    // calculate in real time.
		    // On my GTX460 it's faster, but the difference is barely measurable.
			float depthOffset = 1 - depth + _TranslucencyOffset;
			float scale = depthOffset / _TranslucencyRadius;
			float d = scale * depthOffset;					
		    float dd = -d * d;
		    half3 translucencyProfile =
		    				 float3(0.233, 0.455, 0.649)   * exp(dd / 0.0064) +
		                     float3(0.100, 0.336, 0.344)   * exp(dd / 0.0484) +
		                     float3(0.118, 0.198, 0.000)   * exp(dd / 0.1870) +
		                     float3(0.113, 0.007, 0.007)   * exp(dd / 0.5670) +
		                     float3(0.358, 0.004, 0.00001) * exp(dd / 1.9900) +
		                     float3(0.078, 0.000, 0.00001) * exp(dd / 7.4100);
		#endif

	    o.Translucency = _TranslucencyPower * translucencyProfile * translucencyColor;
		o.Translucency *= mask;
	#endif

	// alpha blending is off
	o.Alpha = 1;

	// skin is not glowing, normally...
	o.Emission = 0;
}


#ifdef ENABLE_TESSELLATION
	#ifndef UNITY_SHADER_VARIABLES_INCLUDED
		// On Unity3 use a different appdata struct to avoid compilation error.
		// This way instead of failing completely it compiles with warnings and fallbacks to SM3 as expected.
		// To detect if running on Unity4 I test the presence of UnityShaderVariables.cginc file.
		// If you have a less hackish way let me know, I couldn't make it work with SHADER_API_D3D11 or UNITY_CAN_COMPILE_TESSELLATION.
		
        void dispSkin(inout appdata_full v) {
             // do nothing
        }
    #else
		// Unity version >= 4.0, compile normally with tessellation
		
    	#include "Tessellation.cginc"
        
		void dispSkin(inout appdata_tan v) {
            float displacement = (tex2Dlod(_DispTex, float4(v.texcoord.xy,0,0)).r - _DisplacementOffset) * _DisplacementScale;
            v.vertex.xyz += v.normal * displacement;
        }
        
		float4 tessSkin(appdata_tan v0, appdata_tan v1, appdata_tan v2) {
			float3 objSpaceCameraPos = mul(_World2Object, float4(_WorldSpaceCameraPos.xyz, 1)).xyz * unity_Scale.w;
			float3 viewDir0 = normalize(objSpaceCameraPos.xyz - v0.vertex.xyz);
			float3 viewDir1 = normalize(objSpaceCameraPos.xyz - v1.vertex.xyz);
			float3 viewDir2 = normalize(objSpaceCameraPos.xyz - v2.vertex.xyz);
			
			float EdotN0 = dot(viewDir0, v0.normal);
			float EdotN1 = dot(viewDir1, v1.normal);
			float EdotN2 = dot(viewDir2, v2.normal);
		
			float3 angles = 0.5 + -0.5 * float3(
				EdotN1 + EdotN2,
				EdotN2 + EdotN0,
				EdotN0 + EdotN1
			);
							
			float4 tess;
		
			tess.xyz = 1 + saturate(_TessRadius + angles.xyz) * _TessDivisions;
			
			// we're inside shadow caster, don't cull based on camera angle
			#if !defined (SHADOWS_DEPTH) && !defined (SHADOWS_CUBE)
				if (angles.x > _TessCullAngle)
					tess.x = 0;
				if (angles.y > _TessCullAngle)
					tess.y = 0;
				if (angles.z > _TessCullAngle)
					tess.z = 0;
			#endif
		
			float3 pos0 = mul(_Object2World,v0.vertex).xyz;
			float3 pos1 = mul(_Object2World,v1.vertex).xyz;
			float3 pos2 = mul(_Object2World,v2.vertex).xyz;
			tess.x = min(tess.x, UnityCalcEdgeTessFactor (pos1, pos2, _EdgeLength));
			tess.y = min(tess.y, UnityCalcEdgeTessFactor (pos2, pos0, _EdgeLength));
			tess.z = min(tess.z, UnityCalcEdgeTessFactor (pos0, pos1, _EdgeLength));				
		
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			
			return tess;
		}
    #endif
#endif