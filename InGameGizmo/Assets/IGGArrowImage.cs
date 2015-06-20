using UnityEngine;
using System.Collections;

public class IGGArrowImage : MonoBehaviour {
	
	public enum eArrowImageUV
	{
		Fill = 0,
		WrapWidthTo,
	}
  
	public eArrowImageUV m_arrowImageUV = eArrowImageUV.WrapWidthTo; 
	float		m_xSize = 1; 
	float		m_zSize = 1; 
	Mesh 		m_Mesh;
	Vector3 	m_startPos;
	Vector3 	m_endPos;
	Vector3 	m_Up = new Vector3(0,1,0);

    //int l_vertCnt = 3;
    Vector3[] 	l_vertices 	= new Vector3[4];
    Vector2[] 	l_uv 		= new Vector2[4];
    int[] 		l_triangles = new int[6];
    Color[] 	l_colors 	= new Color[4];
	// Use this for initialization
	void Start ()
	{
		m_startPos 	= new Vector3( 0,0,0) ;
		m_endPos 	= m_startPos + new Vector3( 0,0,20) ; 
		m_Mesh 		= gameObject.GetComponent<MeshFilter>().mesh;

		CreateMeshLine(); 
	}
	
	// Update is called once per frame
 
	void LateUpdate () 
	{
		if(m_arrowImageUV == eArrowImageUV.WrapWidthTo)
		{  
			float uvCooldV = 1;
			float texRatio = 1;
			if(this.renderer.material.mainTexture != null)
			{
				this.renderer.material.mainTexture.wrapMode = TextureWrapMode.Repeat;
			    texRatio = this.renderer.material.mainTexture.height/(float)this.renderer.material.mainTexture.width;
			} 
			if(transform.localScale.x > 0)
			{
				uvCooldV = transform.localScale.z/transform.localScale.x;
				uvCooldV = uvCooldV/texRatio;
				Vector2 mainTextureScale= this.renderer.material.mainTextureScale;
				mainTextureScale.y = uvCooldV;
				this.renderer.material.mainTextureScale = mainTextureScale;
			} 
		} 	
	}
    void CreateMeshLine()
    { 
        Vector3 l_start 	= new Vector3(0,0,0);//m_startPos;
        Vector3 l_end 		= new Vector3(0,0,m_zSize);

        Vector3 l_dir 		= l_end - l_start;
        float l_length 		= m_zSize;

        l_dir 				= l_dir.normalized; 
		Vector3 l_leftDir 	= Vector3.Cross(l_dir, m_Up);
		   
 
		l_vertices[0] = l_start + l_leftDir * m_xSize*0.5f;//left bottom
        l_vertices[1] = l_end 	+ l_leftDir * m_xSize*0.5f; //left top
		l_vertices[2] = l_end 	+ l_leftDir * m_xSize*-0.5f;//right top
		l_vertices[3] = l_start + l_leftDir * m_xSize*-0.5f;//right bottom
        
		float uvCooldV = 1; 
        l_uv[0] = new Vector2(0, 0);
        l_uv[1] = new Vector2(0, uvCooldV);
        l_uv[2] = new Vector2(1, uvCooldV);
        l_uv[3] = new Vector2(1, 0);
 
        l_colors[0] = Color.red;
        l_colors[1] = Color.red;
        l_colors[2] = Color.red;
        l_colors[3] = Color.red;
 
        l_triangles[0] = 0;
        l_triangles[1] = 1;
        l_triangles[2] = 2;
        l_triangles[3] = 2;
        l_triangles[4] = 3;
        l_triangles[5] = 0; 
		
        // Assign to mesh	
        m_Mesh.vertices = l_vertices;
        m_Mesh.colors = l_colors;
        m_Mesh.uv = l_uv;
        m_Mesh.triangles = l_triangles;
 
    }
    public void SetEndPos( Vector3 endPos)
    {
		float length = Vector3.Distance(this.transform.position, endPos);
		Vector3 scale = this.transform.localScale;
		scale.z = length;
		this.transform.localScale = scale;
 		this.transform.forward = endPos - this.transform.position;
	}
 
}
