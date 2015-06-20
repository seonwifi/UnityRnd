using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
 
[AddComponentMenu("Wo/Fx/WoTrailLauncher")]
public class WoTrailLauncher : MonoBehaviour
{  
	public enum TrailAxis
	{
		X = 0,
		Y,
		Z,
	}

	public bool 		m_autoPlay				= true; 
	public bool			m_lookCamera 			= true;
	public float		m_TrailLifeTime			= -1;
	public float		m_SectionLifeTime		= -1;
	public float		m_maxTrailLength		= 10; 
	public Material 	m_material				= null;
	public Color 	  []m_Colors 				= new Color[2]{Color.white, Color.white}; 
	public float 		heightStart				= 2.0f;
	public float 		heightEnd				= 2.0f;

	//public float	 	m_autoPlayDelayTime 	= 0;  
	public Vector3		m_heightDir				= Vector3.right;
	public float		m_emitOnfadeTime		= 0;
	public float		m_emitOffFadeTime		= 0.25f;
	public int			m_lineQuality			= 0;
	public bool			m_UVRatio				= false;
	public float		m_rightRatio			= 0.5f;
	public float		m_leftRatio				= 0.5f;  
	Transform 			m_target				= null; 
	WoTrail				m_trail = null;
	bool				m_inited = false;
	void Awake() 
	{   
 
	}

	void Start()
	{ 
		if(m_autoPlay == true)
		{
			Begin();
		}
	}
 
	void Init()
	{
		if(m_inited)
			return;
		if(m_target == null)
			m_target = this.transform;
		m_heightDir.Normalize();
		GameObject _trailObject = new GameObject("Trail");
		_trailObject.transform.parent = null;
		_trailObject.transform.position = Vector3.zero;
		_trailObject.transform.rotation = Quaternion.identity;
		_trailObject.transform.localScale = Vector3.one;
		m_trail = _trailObject.AddComponent<WoTrail>();  
		m_trail.m_lookCamera			= m_lookCamera;
		m_trail.m_material 				= m_material;
		m_trail.m_Colors 	  			= m_Colors;
		m_trail.m_maxTrailLength		= m_maxTrailLength;
		m_trail.m_heightStart			= heightStart;
		m_trail.m_heightEnd 			= heightEnd;
		m_trail.m_target 				= m_target;
		m_trail.m_heightDir				= m_heightDir; 
		m_trail.m_interpolateSegnent    = m_lineQuality;
		m_trail.m_bUVRatio				= m_UVRatio;

		m_trail.m_rightRatio			= m_rightRatio;
		m_trail.m_leftRatio				= m_leftRatio;
		m_trail.m_SectionLifeTime		= m_SectionLifeTime;
		m_trail.m_TrailLifeTime			= m_TrailLifeTime;

		m_inited = true; 

	}

	void Begin()
	{ 
		Begin( m_target, m_emitOnfadeTime);
	}

	void Begin(Transform target)
	{ 
		Begin( target, m_emitOnfadeTime);
	}

	void Begin( float fadeTime)
	{ 
		Begin( m_target, fadeTime); 
	}

	void Begin(Transform target, float fadeTime)
	{
		m_target = target;
		if(m_target == null)
			m_target = this.transform;
		Init(); 
		m_trail.Begin(m_target, fadeTime); 
	}


	void OnDisable()
	{
		if(m_trail != null)
		{ 
			m_trail.FadeDestory(m_emitOffFadeTime); 
			m_trail = null;
			m_inited = false;
		}
	}
	void OnEnable()
	{
		if(m_autoPlay == true)
		{
			Begin();
		}
	}

	void OnDestroy()
	{
		if(m_trail != null)
		{ 
			m_trail.FadeDestory(m_emitOffFadeTime); 
			m_trail = null;
			m_inited = false;
		} 
	}
  
	public void EmitOn()
	{
		Begin(this.transform, m_emitOnfadeTime);
	}

	public void EmitOff()
	{
		if(m_trail != null)
		{ 
			m_trail.End(m_emitOffFadeTime); 
		}
	}
}

