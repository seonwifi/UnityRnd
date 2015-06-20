using UnityEngine;
using System.Collections;

public class TouchDirection : TouchSystem {
	
	float directionLength = 50;
	public enum eDirection8
	{
		top = 0,
		right,
		bottom,
		left,
		top_right,
		right_bottom,
		left_bottom,
		left_top,
		non,
	}
	eDirection8 m_myDir = eDirection8.non;

	
	//Begin
	protected override void OnTouchBegin(ref TouchInfo touchInfo, ref Touch touch)
	{ 
		Debug.Log("TouchBegin : " + touchInfo.id.ToString());
		
	}
	//Ended
	protected override void OnTouchEnded(ref TouchInfo touchInfo, ref Touch touch)
	{
		Debug.Log("TouchEnded : " + touchInfo.id.ToString());
	}
	//Moved
	protected override  void OnTouchMoved(ref TouchInfo touchInfo, ref Touch touch)
	{
		float length = touchInfo.firstFromMoved.magnitude;
		if(length > directionLength)
		{
			Vector2 dir = touchInfo.firstFromMoved;
			dir.Normalize();		
			
			if(dir.x > 0)
			{
				//1 (x+, y+)
				if(dir.y > 0)
				{
					Vector2 Axis = new Vector2(0, 1);
					int id = GetDirId(ref dir, ref Axis); 
					if(id == 0)
					{
						m_myDir = eDirection8.top;
					}
					else if(id == 1) 
					{
						m_myDir = eDirection8.top_right;
					}
					else
					{
						m_myDir = eDirection8.right; 
					}				
				}	
				else
				{
					Vector2 Axis = new Vector2(1, 0);
					int id = GetDirId(ref dir, ref Axis); 
					if(id == 0)
					{
						m_myDir = eDirection8.right;
					}
					else if(id == 1) 
					{
						m_myDir = eDirection8.right_bottom;
					}
					else
					{
						m_myDir = eDirection8.bottom; 
					}
				}
			}
			else 
			{
				if(dir.y < 0)
				{
					Vector2 Axis = new Vector2(0, -1);
					int id = GetDirId(ref dir, ref Axis); 
					if(id == 0)
					{
						m_myDir = eDirection8.bottom;
					}
					else if(id == 1) 
					{
						m_myDir = eDirection8.left_bottom;
					}
					else
					{
						m_myDir = eDirection8.left; 
					}
				}	
				else
				{
					Vector2 Axis = new Vector2(-1, 0);
					int id = GetDirId(ref dir, ref Axis); 
					if(id == 0)
					{
						m_myDir = eDirection8.left;
					}
					else if(id == 1) 
					{
						m_myDir = eDirection8.left_top;
					}
					else
					{
						m_myDir = eDirection8.top; 
					}
				}
			}
			//Vector2.Dot(dir, );
		}
	}	
	int GetDirId(ref Vector2 dir, ref Vector2 Axis)
	{ 
		float dot = Vector2.Dot(Axis, dir);
		float angle = Mathf.Acos(dot)*Mathf.Rad2Deg;
		if(angle < 22.5f)
		{
			return 0;
		}
		else if(angle < 67.5f) 
		{
			return 1;
		} 
		return 2;
	}
	
	//Canceled
	protected override  void OnTouchCanceled(ref TouchInfo touchInfo, ref Touch touch)
	{
		Debug.Log("TouchCanceled : " + touchInfo.id.ToString());
	}
	//Stationary
	protected override  void OnTouchStationary(ref TouchInfo touchInfo, ref Touch touch)
	{
		Debug.Log("TouchStationary : " + touchInfo.id.ToString());
	}	
	
	Vector3 myTargetDir;
	float   myAngleRange = 25;
	protected override bool CheckDir(Vector2 touchDir)
	{
		float dot = Vector2.Dot(Axis, dir);
		float angle = Mathf.Acos(dot)*Mathf.Rad2Deg;
 		if(angle < myAngleRange)
		{
			return true;
		}
		return false;
	}
	
	protected override void OnGUI()
	{
		base.OnGUI();
		string smyDir =  m_myDir.ToString();
		GUI.Label(new Rect(300, 100, 1000, 1000), smyDir); 
	}
}
