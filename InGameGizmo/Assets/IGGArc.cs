using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class IGGArc : MonoBehaviour {
 
	float		m_xSize = 1; 
	float		m_zSize = 2; 
	float		m_angle = 45;
	Mesh 		m_Mesh;
   
    //int l_vertCnt = 3;
    Vector3[] 	l_vertices 		= new Vector3[7];
    Vector2[] 	l_uv 			= new Vector2[7];
	Color[] 	l_colors 		= new Color[7]; 
    int[] 		m_triIds    	= new int[3];
	float		m_angleRangeDot = 0; 
	List<int>   m_tris			= new List<int>();
	Vector3     m_leftDir		= Vector3.forward;
	Vector3     m_rightDir		= Vector3.forward;
	// Use this for initialization
	void Start ()
	{  
		m_Mesh 		= gameObject.GetComponent<MeshFilter>().mesh; 
		SetAngle (0);
	}
	
 	enum eRectEdge
	{
		LeftBottom = 0,
		LeftTop, 
 		RightTop,
		RightBottom,
		Center, 
	}
	
	
 	bool CheckRange0(ref int []triIDs)
    { 
		Vector3 dir1 = l_vertices[triIDs[1]] - l_vertices[triIDs[0]]; //left
		Vector3 dir2 = l_vertices[triIDs[2]] - l_vertices[triIDs[0]]; //right
		dir1.Normalize();
		dir2.Normalize();
		float dot = Vector3.Dot(Vector3.forward, dir2);
		if(dot < m_angleRangeDot)
		{
			return false;
		} 
		
		dot = Vector3.Dot(Vector3.forward, dir1);
		if(dot < m_angleRangeDot)
		{
			float helfX = m_xSize*0.5f; 
			float scale = helfX/Mathf.Abs(m_leftDir.x);			
			l_vertices[triIDs[1]] 	= m_leftDir;
			l_vertices[triIDs[1]].x = m_leftDir.x*scale;
			l_vertices[triIDs[1]].z = m_leftDir.z*scale; 
			float zLength 			= l_vertices[triIDs[1]].z - l_vertices[triIDs[2]].z;
			zLength 				= 1-Mathf.Abs(zLength/m_zSize); 
			l_uv[triIDs[1]].y 		= zLength;
		}
		
		return true;
	}
	
 	bool CheckRange1(ref int []triIDs, float size, float left, float right)
    {
		float helfSize = m_zSize*0.5f; 
		
		Vector3 dir1 = l_vertices[triIDs[1]] - l_vertices[triIDs[0]]; //left
		Vector3 dir2 = l_vertices[triIDs[2]] - l_vertices[triIDs[0]]; //right
		dir1.Normalize();
		dir2.Normalize(); 
		//right
		float dot = Vector3.Dot(Vector3.forward, dir2);
		if(dot < m_angleRangeDot)
		{
			//vertex
 			
			float scale = helfSize/Mathf.Abs(m_rightDir.z);			
			l_vertices[triIDs[2]] 	= m_rightDir;
			l_vertices[triIDs[2]].x = m_rightDir.x*scale;
			l_vertices[triIDs[2]].z = m_rightDir.z*scale; 
			//uv
			float zLength 			= l_vertices[triIDs[2]].x - left;
			zLength 				= Mathf.Abs(zLength/size); 
			l_uv[triIDs[2]].x 		= zLength;
		} 
		 
		dot = Vector3.Dot(Vector3.forward, dir1);
		if(dot < m_angleRangeDot)
		{
			//vertex 
			float scale = helfSize/Mathf.Abs(m_leftDir.z);		
			l_vertices[triIDs[1]] 	= m_leftDir;
			l_vertices[triIDs[1]].x = m_leftDir.x*scale;
			l_vertices[triIDs[1]].z = m_leftDir.z*scale; 
			float zLength 			= (l_vertices[triIDs[1]].x-left);
			zLength 				= Mathf.Abs(zLength/size); 
			l_uv[triIDs[1]].x 		= zLength;
		}
  		return true;
	}
 	bool CheckRange2(ref int []triIDs, Vector3 myPoint)
    { 
		Vector3 dir1 = myPoint - l_vertices[triIDs[0]]; //right 
		dir1.Normalize(); 
		float dot = Vector3.Dot(Vector3.forward, dir1);
		if(dot < m_angleRangeDot)
		{
			return false;
		} 
		
		dot = Vector3.Dot(Vector3.forward, dir1);
		if(dot < m_angleRangeDot)
		{
			float helfX = m_xSize*0.5f; 
			float scale = helfX/Mathf.Abs(m_leftDir.x);			
			l_vertices[triIDs[1]] 	= m_leftDir;
			l_vertices[triIDs[1]].x = m_leftDir.x*scale;
			l_vertices[triIDs[1]].z = m_leftDir.z*scale; 
			float zLength 			= l_vertices[triIDs[1]].z - l_vertices[triIDs[2]].z;
			zLength 				= 1-Mathf.Abs(zLength/m_zSize); 
			l_uv[triIDs[1]].y 		= zLength;
		}
		
		return true;
	}	
	
    void CreateMesh()
    {   
		m_tris.Clear();
		float left 		= -m_xSize*0.5f;
		float right 	= m_xSize*0.5f;
		float top 		= m_zSize*0.5f;
		float bottom 	= -m_zSize*0.5f;
		
		l_vertices[0] = new Vector3(left, 0, bottom);
		l_vertices[1] = new Vector3(left, 0, top);
		l_vertices[2] = new Vector3(right, 0, top);
		l_vertices[3] = new Vector3(right, 0, bottom);
		l_vertices[4] = new Vector3(0,0,0);
		
		float uvCooldV = 1; 
        l_uv[0] = new Vector2(0, 0);
        l_uv[1] = new Vector2(0, uvCooldV);
        l_uv[2] = new Vector2(1, uvCooldV);
        l_uv[3] = new Vector2(1, 0);
		l_uv[4] = new Vector2(0.5f, 0.5f);
 
        l_colors[0] = Color.red;
        l_colors[1] = Color.red;
        l_colors[2] = Color.red;
        l_colors[3] = Color.red;
		l_colors[4] = Color.red;
		
		bool _0True = false;
		m_triIds[0] = 4;
		m_triIds[1] = 0;
		m_triIds[2] = 1;
		_0True = CheckRange0(ref m_triIds);
		if(_0True)
		{
			m_tris.Add(m_triIds[0]);
			m_tris.Add(m_triIds[1]);
			m_tris.Add(m_triIds[2]);
		}
		
		m_triIds[0] = 4;
		m_triIds[1] = 1;
		m_triIds[2] = 2;		
  		if(CheckRange1(ref m_triIds, m_xSize, left, right))
		{
			m_tris.Add(m_triIds[0]);
			m_tris.Add(m_triIds[1]);
			m_tris.Add(m_triIds[2]);
		}
		
		if(_0True == true)
		{
			//vertex
			l_vertices[3] 	= l_vertices[0];
			l_vertices[3].x *= -1;
			// uv
			l_uv[3].y 		= l_uv[0].y;
			
			//tri
			m_tris.Add(4);
			m_tris.Add(2);
			m_tris.Add(3);	 			
		}
		
        // Assign to mesh	
        m_Mesh.vertices 	= l_vertices;
        m_Mesh.colors 		= l_colors;
        m_Mesh.uv 			= l_uv;
        m_Mesh.triangles 	= m_tris.ToArray();
 
    }
 
	float testAngle = 0;
	void Update()
	{
		testAngle += Time.deltaTime*180;
		testAngle = testAngle%360; 
		testAngle = Mathf.Round(testAngle);
		SetAngle (testAngle);
	}
	
	
	void SetAngle (float angle)
	{
 
		float angleRad = angle*Mathf.Deg2Rad;
		m_angleRangeDot = Mathf.Cos(angleRad*0.5f);
		float leftAngle = -angle*0.5f;
		float rightAngle = angle*0.5f;
		Matrix4x4 mLeft = Matrix4x4.identity;
		Matrix4x4 mRight = Matrix4x4.identity;
		mLeft.SetTRS(new Vector3(0,0,0), Quaternion.Euler(0,leftAngle,0), new Vector3(1, 1, 1));
		mRight.SetTRS(new Vector3(0,0,0), Quaternion.Euler(0,rightAngle,0), new Vector3(1, 1, 1));
		m_leftDir 	= mLeft.MultiplyPoint(Vector3.forward);
		m_rightDir 	= mRight.MultiplyPoint(Vector3.forward);
		CreateMesh(); 
	}
 
	//cross point 2Line 
	 bool GetIntersectPoint(ref Vector2 outCrossPoint, ref Vector2 AP1,ref Vector2 AP2,ref Vector2 BP1,ref Vector2 BP2) 

	 {

		 float t; 

		 float under = (BP2.y-BP1.y)*(AP2.x-AP1.x)-(BP2.x-BP1.x)*(AP2.y-AP1.y);

		 if(under==0) return false;  
		
		 t = (BP2.x-BP1.x)*(AP1.y-BP1.y) - (BP2.y-BP1.y)*(AP1.x-BP1.x); 

		 t = t/under;

		 outCrossPoint.x = AP1.x + t * (float)(AP2.x-AP1.x);

		 outCrossPoint.y = AP1.y + t * (float)(AP2.y-AP1.y);

		 return true;

	 }

	

  	 //Range cross point 2Line 

	 bool GetIntersectPointRange(ref Vector2 outCrossPoint, ref Vector2 AP1,ref Vector2 AP2,ref Vector2 BP1,ref Vector2 BP2) 

	 {

		 float t;

		 float s; 

		 float under = (BP2.y-BP1.y)*(AP2.x-AP1.x)-(BP2.x-BP1.x)*(AP2.y-AP1.y);

		 if(under==0) return false; 
 
		 float _t = (BP2.x-BP1.x)*(AP1.y-BP1.y) - (BP2.y-BP1.y)*(AP1.x-BP1.x);

		 float _s = (AP2.x-AP1.x)*(AP1.y-BP1.y) - (AP2.y-AP1.y)*(AP1.x-BP1.x); 
 
		 t = _t/under;

		 s = _s/under; 
 
		 if(t<0.0 || t>1.0 || s<0.0 || s>1.0) return false;

		 if(_t==0 && _s==0) return false; 
 
		 outCrossPoint.x = AP1.x + t * (float)(AP2.x-AP1.x);

		 outCrossPoint.y = AP1.y + t * (float)(AP2.y-AP1.y);

		 return true;

	 }	
}
