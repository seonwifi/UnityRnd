using UnityEngine;
using System.Collections;

public class Rotate2d  {

	//
	public static  void Rotate(ref float outx, ref float outy, float x, float y, float radAngle) 
	{
		outx = Mathf.Cos(radAngle)*x - Mathf.Sin(radAngle)*y;
		outy = Mathf.Sin(radAngle)*x + Mathf.Cos(radAngle)*y;
	}
	//
	public static  Vector2 Rotate(float x, float y, float radAngle) 
	{ 
		return new Vector2(Mathf.Cos(radAngle)*x - Mathf.Sin(radAngle)*y, Mathf.Sin(radAngle)*x + Mathf.Cos(radAngle)*y);
	}
	//
	public static void RotateInOut(ref float inoutx, ref float inouty, float radAngle) 
	{
		inoutx = Mathf.Cos(radAngle)*inoutx - Mathf.Sin(radAngle)*inouty;
		inouty = Mathf.Sin(radAngle)*inoutx + Mathf.Cos(radAngle)*inouty;
	}
}
