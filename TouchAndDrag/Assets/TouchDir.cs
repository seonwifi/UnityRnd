using UnityEngine;
using System.Collections;
 
// 0~360
public class TouchDir
{
	public float myDockAngle = 0;
 	public float myAngleRange = 0;
	
	public bool IsRangeIn(float yourAngle)
	{
		float rightRange  = myDockAngle + myAngleRange;
		if(rightRange > 360)
		{
	   		if(yourAngle >= myDockAngle && yourAngle < 360)
			{
				return true;
			} 
	   		if(yourAngle >= myDockAngle && yourAngle < 360)
			{
				return true;
			} 			
		}
		else
		{
	   		if(yourAngle >= myDockAngle && yourAngle < rightRange)
			{
				return true;
			} 
		} 
		return false;
	}
}


public class TouchDir360
{
	TouchDir []touchDirs;
	
	public void CreateDir(int count, float firstAngle)
	{
		touchDirs = new TouchDir[count];
		float angleRange = 360.0f/(float)count;
		
		for(int i = 0; i < count; ++i)
		{
			touchDirs[i].myDockAngle = firstAngle;
			touchDirs[i].myAngleRange = angleRange;
			firstAngle += angleRange; 
			if(firstAngle > 360)
			{
				firstAngle = firstAngle%360;
			}
		}
	}
	
	public void CheckDir(float angle)
	{
		
	}
}