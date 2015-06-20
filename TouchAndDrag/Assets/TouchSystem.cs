using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchSystem : MonoBehaviour {
	
	public class TouchInfo
	{
		public TouchInfo()
		{
			Clear();
		}
		public int 		id = 0;
		public Vector2 	firstPos;
		public Vector2 	firstFromMoved;	
		public Vector2 	currentPos;	
		public float	firstFromTotalTime = 0;
		public float	touchSpeed		   = 0;
		public void Clear()
		{
			id 					= -1;
			firstPos 			= Vector2.zero;
			firstFromMoved 		= Vector2.zero;			
			firstFromTotalTime 	= 0;
			touchSpeed			= 0;
		}
	}
	
	public int totalFingerCount = 2;
	List<TouchInfo> m_TouchInfos = new List<TouchInfo>();
	// Use this for initialization
	void Start () 
	{	
		for(int i = 0; i < totalFingerCount; ++i)
		{
			TouchInfo touchInfo = new TouchInfo();
			m_TouchInfos.Add(touchInfo);
		}
		
	}
 
	// Update is called once per frame
	void Update ()
	{
		for(int i = 0; i < m_TouchInfos.Count; ++i)
		{
			if(m_TouchInfos[i].id >= 0)
			{
				m_TouchInfos[i].firstFromTotalTime += Time.smoothDeltaTime;
			}
		}
		
		TouchInfo touchInfo = null;
		for(int i = 0; i < Input.touchCount; ++i)
		{
			if(Input.touches[i].fingerId >= totalFingerCount)
			{
				continue;
			}			
			
			switch(Input.touches[i].phase)
			{
			case TouchPhase.Began:
				{ 
					touchInfo 				= m_TouchInfos[Input.touches[i].fingerId];
				    touchInfo.id 			= Input.touches[i].fingerId;
					touchInfo.firstPos 		= Input.touches[i].position;
					touchInfo.currentPos    = Input.touches[i].position;
					OnTouchBegin(ref touchInfo, ref Input.touches[i]);
				}
				break;
			case TouchPhase.Ended:
				{
					touchInfo 				 	= m_TouchInfos[Input.touches[i].fingerId];
					touchInfo.currentPos    	= Input.touches[i].position;
					OnTouchEnded( ref touchInfo, ref Input.touches[i]);
				
					m_TouchInfos[Input.touches[i].fingerId].Clear();
				} 
				break;
			case TouchPhase.Moved:
				{
				
					touchInfo 				 	= m_TouchInfos[Input.touches[i].fingerId];
					touchInfo.firstFromMoved 	= Input.touches[i].position - touchInfo.firstPos; 
					touchInfo.currentPos    	= Input.touches[i].position;
					if(m_TouchInfos[i].firstFromTotalTime > 0)
					{
						touchInfo.touchSpeed        = touchInfo.firstFromMoved.magnitude/m_TouchInfos[i].firstFromTotalTime;
					}
					else
					{
						touchInfo.touchSpeed = 0;
					}
					OnTouchMoved(ref touchInfo, ref Input.touches[i]);
				}
					
				break;
			case TouchPhase.Canceled:
				{
					touchInfo 				 	= m_TouchInfos[Input.touches[i].fingerId];  
					touchInfo.currentPos    	= Input.touches[i].position;
					OnTouchCanceled(ref touchInfo, ref Input.touches[i]);
				}
				break;
			case TouchPhase.Stationary:
				{
					touchInfo 				 	= m_TouchInfos[Input.touches[i].fingerId]; 
					touchInfo.currentPos    	= Input.touches[i].position;
					OnTouchStationary(ref touchInfo, ref Input.touches[i]);
				}
				break;				
			}
			
		}
	}
	//Begin
	protected virtual void OnTouchBegin(ref TouchInfo touchInfo, ref Touch touch)
	{ 
		Debug.Log("TouchBegin : " + touchInfo.id.ToString());
		
	}
	//Ended
	protected virtual void OnTouchEnded(ref TouchInfo touchInfo, ref Touch touch)
	{
		Debug.Log("TouchEnded : " + touchInfo.id.ToString());
	}
	//Moved
	protected virtual void OnTouchMoved(ref TouchInfo touchInfo, ref Touch touch)
	{
		Debug.Log("TouchMoved : " + touchInfo.id.ToString());
	}	
	//Canceled
	protected virtual void OnTouchCanceled(ref TouchInfo touchInfo, ref Touch touch)
	{
		Debug.Log("TouchCanceled : " + touchInfo.id.ToString());
	}
	//Stationary
	protected virtual void OnTouchStationary(ref TouchInfo touchInfo, ref Touch touch)
	{
		Debug.Log("TouchStationary : " + touchInfo.id.ToString());
	}	
	string touchTextMonitoring;
	protected virtual void OnGUI()
	{
		touchTextMonitoring = "";
		touchTextMonitoring += "\nScreen Width: " + Screen.width.ToString();
		touchTextMonitoring += "\nScreen Height: "+ Screen.height.ToString();
		touchTextMonitoring += "\n\n\n";
		for(int i = 0; i < m_TouchInfos.Count; ++i)
		{
			if(m_TouchInfos[i].id >= 0)
			{ 
 				touchTextMonitoring += "\ntouchID: " + m_TouchInfos[i].id.ToString();
				touchTextMonitoring += "	\ntouch First Position: " + m_TouchInfos[i].firstPos.ToString();
				touchTextMonitoring += "	\ntouch Current Position: " + m_TouchInfos[i].currentPos.ToString();
				touchTextMonitoring += "	\ntouch First From Moved: " + m_TouchInfos[i].firstFromMoved.ToString();
				touchTextMonitoring += "	\ntouch First From Speed: " + m_TouchInfos[i].touchSpeed.ToString();
			}
		}		
		GUI.Label(new Rect(0, 0, 1000, 1000), touchTextMonitoring); 
	}
}





































