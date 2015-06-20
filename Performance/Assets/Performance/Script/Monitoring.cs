using UnityEngine;
using System.Collections;
using System.Diagnostics;
	
public class Monitoring : MonoBehaviour {
	
	Stopwatch m_stopWatchFrameCheck = new Stopwatch();
	public static double	m_frameTimeMilli = 0;
	public static double	m_frameTimeFrequency = 0;
	public static float     m_deltaTimeUnity = 0;
	public static float     m_deltaTimeUnitySmooth = 0;
	
	double	  m_FPSMilli = 0;
	double	  m_FPSFrequency = 0;
	float     m_FPSUnity = 0;
	float     m_FPSUnitySmooth = 0;
	
	enum eFrameMode
	{
		AllView		  = 0,
		UnityDeltaTime,
		UnitySmoothDeltaTime,
		MilliSecond,
		FrequencySecond
	}
	static eFrameMode m_eFrameMode = eFrameMode.AllView;
	
	public static float GetDeltaTime()
	{
 		switch(m_eFrameMode)
		{ 
		case eFrameMode.UnityDeltaTime: 
			return m_deltaTimeUnity;
			break;
		case eFrameMode.UnitySmoothDeltaTime: 
			return m_deltaTimeUnitySmooth;
			break;
		case eFrameMode.MilliSecond: 
			return (float)m_frameTimeMilli;
			break;
		case eFrameMode.FrequencySecond: 
			return (float)m_frameTimeFrequency;
			break;
		}
		return 0;
	}
	// Use this for initialization
	void Start () 
	{ 
		m_stopWatchFrameCheck.Start();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
	void LateUpdate() 
	{
		m_deltaTimeUnity = Time.deltaTime;
		m_FPSUnity		 = 1.0f/Time.deltaTime;
		
		m_deltaTimeUnitySmooth = Time.smoothDeltaTime;
		m_FPSUnitySmooth	   = 1.0f/Time.smoothDeltaTime;
		
 
		m_frameTimeFrequency = (double)m_stopWatchFrameCheck.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency; 
		m_FPSFrequency = (double)1.0/m_frameTimeFrequency;
		
		m_frameTimeMilli = ((double)m_stopWatchFrameCheck.ElapsedMilliseconds) / (double)1000.0;
		m_FPSMilli = (double)1.0/m_frameTimeMilli;
		
		m_stopWatchFrameCheck.Reset();
		m_stopWatchFrameCheck.Start();
	}	
	void OnGUI()
	{
		ModeCheck();
		string label1String = "";
		bool bframeTimeMilli = false;
		bool bframeTimeFrequency = false;
		bool bdeltaTimeUnity = false;
		bool bdeltaTimeUnitySmooth = false;
		
 		switch(m_eFrameMode)
		{
		case eFrameMode.AllView:
			 bframeTimeMilli = true;
			 bframeTimeFrequency = true;
			 bdeltaTimeUnity = true;
			 bdeltaTimeUnitySmooth = true;
			break;
		case eFrameMode.UnityDeltaTime:
			bdeltaTimeUnity = true;
			break;
		case eFrameMode.UnitySmoothDeltaTime:
			bdeltaTimeUnitySmooth = true;
			break;
		case eFrameMode.MilliSecond:
			bframeTimeMilli = true;
			break;
		case eFrameMode.FrequencySecond:
			 bframeTimeFrequency = true;
			break;
		}
		if(bframeTimeMilli)
		{
			label1String += "DeltaTime Stopwatch Milli : "        + m_frameTimeMilli.ToString()       + "\nFPS: " + m_FPSMilli.ToString(); 
		}
		if(bframeTimeFrequency)
		{
			label1String += "\n\nDeltaTime Stopwatch Frequency: " + m_frameTimeFrequency.ToString()   + "\nFPS: " + m_FPSFrequency.ToString(); 
		}
		if(bdeltaTimeUnity)
		{
			label1String += "\n\nDeltaTime UnityTime : " 		  + m_deltaTimeUnity.ToString()       + "\nFPS: " + m_FPSUnity.ToString(); 
		}
		if(bdeltaTimeUnitySmooth)
		{
			label1String += "\n\nDeltaTime UnityTimeSmooth : "    + m_deltaTimeUnitySmooth.ToString() + "\nFPS: " + m_FPSUnitySmooth.ToString(); 
		}		
		  
		GUI.Label(new Rect(0,0, 600, 600), label1String);
	}

	void ModeCheck()
	{
		Vector2 bottonSize = new Vector2(180, 30);
		//Screen.width;
		//Screen.height;
		float x = Screen.width - (bottonSize.x + 20);
		float y = Screen.height;
		float gap = 5;
		if(GUI.Button(new Rect(x, y -= 80, bottonSize.x, bottonSize.y), "AllView"))
		{
			m_eFrameMode = eFrameMode.AllView;
		}
		if(GUI.Button(new Rect(x, y -= (bottonSize.y+gap), bottonSize.x, bottonSize.y), "UnityDeltaTime"))
		{
			m_eFrameMode = eFrameMode.UnityDeltaTime;
		}
		if(GUI.Button(new Rect(x, y -= (bottonSize.y+gap), bottonSize.x, bottonSize.y), "UnitySmoothDeltaTime"))
		{
			m_eFrameMode = eFrameMode.UnitySmoothDeltaTime;
		}
		if(GUI.Button(new Rect(x, y -= (bottonSize.y+gap), bottonSize.x, bottonSize.y), "MilliSecond"))
		{
			m_eFrameMode = eFrameMode.MilliSecond;
		}
		if(GUI.Button(new Rect(x, y -= (bottonSize.y+gap), bottonSize.x, bottonSize.y), "FrequencySecond"))
		{
			m_eFrameMode = eFrameMode.FrequencySecond;
		}		
	}
}


























