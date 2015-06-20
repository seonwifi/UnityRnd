using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
class TrailSection
{
	public Vector3 point;
	public Vector3 upDir;
	public float   space;
	public float   spwanTime = 0; 
	public TrailSection() 
	{
		
	}
	public TrailSection(ref Vector3 p, ref Vector3 up, ref float l_length, ref float l_spwanTime) 
	{
		point 		= p;
		upDir 		= up;
		space 		= l_length;
		spwanTime 	= l_spwanTime;
	}
}


public class WoTrail : MonoBehaviour
{  
	//public
	//public bool 		m_emit					= true;
	public bool			m_lookCamera 			= true;
	public Material 	m_material				= null;
	public Color 	  []m_Colors 				= new Color[2]{Color.white, Color.white};
	public float		m_maxTrailLength		= 10; 
	public float 		m_heightStart			= 2.0f;
	public float 		m_heightEnd				= 2.0f;
 
	public Transform 	m_target				= null; 
	public Vector3		m_heightDir				= Vector3.right;
	public int			m_interpolateSegnent	= 0;
	public bool			m_bUVRatio				= true;
	public float		m_minDistance 			= 0.1f;  

	public float		m_rightRatio			= 0.5f;
	public float		m_leftRatio				= 0.5f;
	public float		m_SectionLifeTime		= -1;
	public float		m_TrailLifeTime			= -1;

	//private
	private float		m_currentTrailLength	= 0; 
	private Mesh 		m_mesh 					= null;
	private Vector3[] 	m_vertices	 			= null;
	private Color[] 	m_colors 				= null;
	private Vector2[] 	m_uv 					= null;
	 
	private List<TrailSection> 		m_sections 						= new List<TrailSection>();
	private List<TrailSection> 		m_interpolatorSections 			= null;
	private TrailSection 			[]m_interpolatorSectionPoint 	= new TrailSection[4];
	//private int 					m_LastVertexPoint 				= 0;
	//private bool   					m_isStart 						= false;
	private float  					m_fadeColorScale 				= 1;
	private GameObject				m_trailObject 					= null;
	private float					m_totalSpace					= 0;
	private float					m_textureW 						= 0;
	private float					m_textureH 						= 0;
	//private bool					m_cpuOptimize 					= true;
	private float					m_fadeColorScaleSpeed      	 	= 0;
	private bool					m_isReady 						= false;
	void RenderReady()
	{ 
		if(m_isReady == true)
			return;
		m_isReady = true;
		m_vertices = new Vector3[100*2 ];
		m_colors   = new Color[100*2 ];
		m_uv       = new Vector2[100*2 ]; 

		m_minDistance = m_minDistance*m_minDistance; 

		m_heightDir.Normalize();
		m_trailObject = this.gameObject;
		m_trailObject.transform.parent = null;
		m_trailObject.transform.position = Vector3.zero;
		m_trailObject.transform.rotation = Quaternion.identity;
		m_trailObject.transform.localScale = Vector3.one;
		m_trailObject.AddComponent(typeof(MeshFilter));
		m_trailObject.AddComponent(typeof(MeshRenderer));
		m_trailObject.renderer.material = m_material;
		if(m_material != null)
		{
			m_textureW = m_material.mainTexture.width;
			m_textureH = m_material.mainTexture.height;
		}

		m_mesh 											= new Mesh();
		m_mesh.name 									= name + "TrailMesh";
		m_trailObject.GetComponent<MeshFilter>().mesh 	= m_mesh;
		if(m_interpolateSegnent > 0)
		{
			m_interpolatorSections = new List<TrailSection>();
		}

		if(m_target == null)
		{
			m_target = this.transform;
		}
		m_currentTrailLength = m_maxTrailLength;

		/*
		if(m_autoPlay == true)
		{
			yield return new WaitForSeconds(m_autoPlayDelayTime);
			Begin(m_target, 0);
		} //*/
		//yield return null;
	}
	Vector3 m_lastPosition = Vector3.zero;
	void UpdatePosition(Transform tr, Camera lookCamera)
	{  
		if(m_lookCamera)
		{
			if(lookCamera == null)
				return;
		}
  
		if(tr != null)
		{
			/*
			if(m_cpuOptimize == true)
			{
				if((m_lastPosition - tr.position).sqrMagnitude < m_minDistance)
				{
					m_lastPosition = tr.position;
					return;
				}
			}//*/

			if(m_interpolateSegnent > 0)
			{
				int segmentCount = m_interpolateSegnent+2;
				if(m_interpolatorSections.Count < 2)
				{
					m_interpolatorSections.Clear();
					TrailSection trailSection = new TrailSection(); 
					trailSection.upDir =  tr.TransformDirection(m_heightDir);
					trailSection.point =  tr.position;
					trailSection.space = 0;
					trailSection.spwanTime = Time.time; 

					m_interpolatorSections.Add(trailSection);
					trailSection = new TrailSection(); 
					trailSection.upDir =  tr.TransformDirection(m_heightDir);
					trailSection.point =  tr.position;
					trailSection.space =0;
					trailSection.spwanTime = Time.time; 

					m_interpolatorSections.Add(trailSection);
					//InterpolateNewHip(m_interpolatorSections[1], m_interpolatorSections[0], segmentCount);
 
					return;
				}
				else
				{ 
					m_interpolatorSections[0].upDir =  tr.TransformDirection(m_heightDir);
					m_interpolatorSections[0].point =  tr.position;
					m_interpolatorSections[0].space = (m_interpolatorSections[0].point - m_interpolatorSections[1].point).magnitude;
					m_interpolatorSections[0].spwanTime = Time.time; 

					if((m_interpolatorSections[1].point - m_interpolatorSections[0].point).sqrMagnitude > m_minDistance)
					{
						TrailSection trailSection = new TrailSection(); 
						trailSection.upDir = tr.TransformDirection(m_heightDir);
						trailSection.point = tr.position;
						trailSection.space = m_interpolatorSections[0].space; 
						trailSection.spwanTime = Time.time; 

						m_interpolatorSections[0].space = 0;
						m_interpolatorSections.Insert(1, trailSection);
					
						if(m_interpolatorSections.Count > 4)
						{
							InterpolateNewHip(m_interpolatorSections[1], m_interpolatorSections[0], segmentCount);
							m_interpolatorSectionPoint[0] = m_interpolatorSections[4];
							m_interpolatorSectionPoint[1] = m_interpolatorSections[3];
							m_interpolatorSectionPoint[2] = m_interpolatorSections[2];
							m_interpolatorSectionPoint[3] = m_interpolatorSections[1];
							Interpolate2(m_interpolatorSectionPoint, segmentCount, segmentCount);
						}

						/*
						if(m_interpolatorSections.Count == 4)
						{
							m_interpolatorSectionPoint[0] = m_interpolatorSections[3];
							m_interpolatorSectionPoint[1] = m_interpolatorSections[3];
							m_interpolatorSectionPoint[2] = m_interpolatorSections[2];
							m_interpolatorSectionPoint[3] = m_interpolatorSections[1];
							Interpolate2(m_interpolatorSectionPoint, segmentCount*3, segmentCount);
						}
						else if(m_interpolatorSections.Count > 4)
						{
							m_interpolatorSectionPoint[0] = m_interpolatorSections[4];
							m_interpolatorSectionPoint[1] = m_interpolatorSections[3];
							m_interpolatorSectionPoint[2] = m_interpolatorSections[2];
							m_interpolatorSectionPoint[3] = m_interpolatorSections[1];
							Interpolate2(m_interpolatorSectionPoint, segmentCount*3, segmentCount);
						}//*/

						if(m_interpolatorSections.Count > 5)
						{
							m_interpolatorSections.RemoveRange(5, m_interpolatorSections.Count-5);
						}
					}
					/*
					if(m_interpolatorSections.Count == 2)
					{
						m_interpolatorSectionPoint[0] = m_interpolatorSections[1];
						m_interpolatorSectionPoint[1] = m_interpolatorSections[1];
						m_interpolatorSectionPoint[2] = m_interpolatorSections[0];
						m_interpolatorSectionPoint[3] = m_interpolatorSections[0];
						Interpolate2(m_interpolatorSectionPoint, segmentCount, segmentCount);
					}
					else if(m_interpolatorSections.Count == 3)
					{
						m_interpolatorSectionPoint[0] = m_interpolatorSections[2];
						m_interpolatorSectionPoint[1] = m_interpolatorSections[2];
						m_interpolatorSectionPoint[2] = m_interpolatorSections[1];
						m_interpolatorSectionPoint[3] = m_interpolatorSections[0];
						Interpolate2(m_interpolatorSectionPoint, segmentCount*2, segmentCount);

						m_interpolatorSectionPoint[0] = m_interpolatorSections[2];
						m_interpolatorSectionPoint[1] = m_interpolatorSections[1];
						m_interpolatorSectionPoint[2] = m_interpolatorSections[0];
						m_interpolatorSectionPoint[3] = m_interpolatorSections[0];
						Interpolate2(m_interpolatorSectionPoint, segmentCount, segmentCount);
					}
					else if(m_interpolatorSections.Count >= 4)
					{
						m_interpolatorSectionPoint[0] = m_interpolatorSections[3];
						m_interpolatorSectionPoint[1] = m_interpolatorSections[2];
						m_interpolatorSectionPoint[2] = m_interpolatorSections[1];
						m_interpolatorSectionPoint[3] = m_interpolatorSections[0];
						Interpolate2(m_interpolatorSectionPoint, segmentCount*2, segmentCount); 

						m_interpolatorSectionPoint[0] = m_interpolatorSections[2];
						m_interpolatorSectionPoint[1] = m_interpolatorSections[1];
						m_interpolatorSectionPoint[2] = m_interpolatorSections[0];
						m_interpolatorSectionPoint[3] = m_interpolatorSections[0];
						Interpolate2(m_interpolatorSectionPoint, segmentCount, segmentCount);
					} 
					else
					{
						return;
					}//*/
				}

				//m_interpolatorSections.Add
			}
			else
			{
				if(m_sections.Count < 2)
				{
					TrailSection trailSection = new TrailSection(); 
					trailSection.upDir =  tr.TransformDirection(m_heightDir);
					trailSection.point =  tr.position;
					trailSection.space = 0;
					trailSection.spwanTime = Time.time; 

					m_sections.Insert(0, trailSection);
					
					trailSection = new TrailSection(); 
					trailSection.upDir =  tr.TransformDirection(m_heightDir);
					trailSection.point =  tr.position;
					trailSection.space = 0;
					trailSection.spwanTime = Time.time; 

					m_sections.Insert(1, trailSection);
					m_totalSpace = 0;
					return;
				}
				
				m_sections[0].upDir =  tr.TransformDirection(m_heightDir);
				m_sections[0].point =  tr.position;
				m_sections[0].space = (tr.position - m_sections[1].point).magnitude;
				m_sections[0].spwanTime = Time.time; 
				if((m_sections[1].point - tr.position).sqrMagnitude > m_minDistance)
				{
					TrailSection trailSection = new TrailSection();
					Vector3 upDir = tr.TransformDirection(m_heightDir);
					
					trailSection.point = tr.position;
					trailSection.upDir = upDir;
					trailSection.space = m_sections[0].space;
					trailSection.spwanTime = Time.time; 
					m_totalSpace += trailSection.space;
					m_sections[0].space  = 0;
					m_sections.Insert(1, trailSection); 
				}
			} 
		}

		//if(sections.Count < 2)
			//return;
 
		if(m_SectionLifeTime >= 0)
		{
			float currentTime = Time.time;
			int timeedClipArray = -1;
			for(int i = m_sections.Count-1; i > -1; --i)
			{ 
				float timeGap = Time.time - m_sections[i].spwanTime;
				if(timeGap < m_SectionLifeTime)
				{
					if(i+1 < m_sections.Count)
					{
						float time_i_ip1_Gap = m_sections[i].spwanTime - m_sections[i+1].spwanTime;
						if(timeGap > 0)
						{
							float gap = m_SectionLifeTime - timeGap;
							gap = gap/time_i_ip1_Gap;
							Vector3 dir  = m_sections[i+1].point - m_sections[i].point;
							dir *= gap; 
							m_sections[i+1].point = m_sections[i].point + dir;
							timeedClipArray = i+2;
						}
						else
						{
							timeedClipArray = i+1;
						} 
					}
					else
					{
						timeedClipArray = i+1;
					}
					break;
				}
				else
				{
					timeedClipArray = i;
				}
			}

			if(timeedClipArray >= 0 && timeedClipArray < m_sections.Count)
			{  
				m_sections.RemoveRange(timeedClipArray, m_sections.Count - timeedClipArray); 
				m_totalSpace = 0;
				for(int i = 1; i< m_sections.Count-1;++i)
				{
					m_totalSpace += m_sections[i].space;
				}
			}
		}

		int   clipLine = -1;
		float lineLength = 0;
		//clip tail
		if(m_interpolateSegnent > 0)
		{  
			float beforLineLength = 0;

			for(int i = 0; i< m_sections.Count-1;++i)
			{
				Vector3 dir = m_sections[i+1].point - m_sections[i].point;
				m_sections[i].space = dir.magnitude;
				lineLength += m_sections[i].space;
				if(lineLength > m_currentTrailLength)
				{  
					m_sections[i].space = m_currentTrailLength - beforLineLength;
					dir.Normalize();
					dir *= m_sections[i].space;
					m_sections[i+1].point = m_sections[i].point + dir;
					m_sections[i+1].space = 0;
					clipLine = i+2;
					break;
				}
				beforLineLength = lineLength;
			} 
		}
		else
		{ 
			if(m_sections.Count > 0)
			{
				float totalSpace = m_totalSpace + m_sections[0].space;
				if(totalSpace > m_currentTrailLength)
				{ 
					float clipGap = totalSpace - m_currentTrailLength;
					float tailLength = 0;
					for(int i = m_sections.Count-2; i > -1; --i)
					{ 
						tailLength += m_sections[i].space;
						if(tailLength > clipGap)
						{ 
							Vector3 dir  = m_sections[i+1].point - m_sections[i].point;
							dir.Normalize();
							float newlength = tailLength - clipGap;
							dir *= newlength;
							m_sections[i].space = newlength;
							m_sections[i+1].point = m_sections[i].point + dir;
							clipLine = i+2;
							break;
						} 
					} 
					totalSpace = m_currentTrailLength;
					m_totalSpace = m_currentTrailLength - m_sections[0].space;
				}
				lineLength = totalSpace;
			} 
		}
		

		if(clipLine >= 0 && clipLine < m_sections.Count)
		{  
			m_sections.RemoveRange(clipLine, m_sections.Count- clipLine);
			
		}
		/*
		if(sections.Count > m_lerpSegmentCount)
		{
			sections.RemoveAt(sections.Count-1);
		}//*/
		
		if(m_sections.Count*2 > m_vertices.Length)
		{
			m_vertices = new Vector3[m_sections.Count*2 ];
			m_colors   = new Color[m_sections.Count*2 ];
			m_uv       = new Vector2[m_sections.Count*2 ]; 
		}

		if(m_sections.Count > 1)
		{
			m_mesh.Clear();
			Vector3 sidePoint1;
			Vector3 sidePoint2;
 
			float mylineLength = 0;
			Vector3 upBeforUped = Vector3.zero; 
			for(int i = 0; i< m_sections.Count;++i)
			{
				TrailSection curTrailSection = m_sections[i];
				float TexCoordV = 0;  
				TexCoordV = mylineLength/lineLength; 


				//vertex
				float height = Mathf.Lerp(m_heightStart, m_heightEnd, TexCoordV);
  
				if(m_lookCamera == true)
				{
					//
					Vector3 currentToCameraDir = lookCamera.transform.position - m_sections[i].point;
					currentToCameraDir.Normalize();

					Vector3 up;

					if(i == 0)
					{ 
						Vector3 currentToBeforDir = m_sections[i+1].point - m_sections[i].point;  
						currentToBeforDir.Normalize();
						if(Vector3.zero == currentToBeforDir)
						{ 
							if(m_sections.Count > 2)
							{
								currentToBeforDir = m_sections[i+2].point - m_sections[i].point;
								currentToBeforDir.Normalize();
							} 
						}

						Vector3 upBefor = Vector3.Cross(currentToCameraDir , currentToBeforDir);  
						upBefor.Normalize();
						up = upBefor; 
						upBeforUped = upBefor;
					}
					else if(i == m_sections.Count-1)
					{
						Vector3 currentToNextrDir = m_sections[i-1].point - m_sections[i].point;  
						
						currentToNextrDir.Normalize(); 
						
						Vector3 upNext = Vector3.Cross(currentToNextrDir, currentToCameraDir);  
						upNext.Normalize(); 
						up = upNext; 
					}
					else
					{
						Vector3 currentToNextrDir = m_sections[i-1].point - m_sections[i].point; 
						Vector3 currentToBeforDir = m_sections[i+1].point - m_sections[i].point; 
						
						currentToNextrDir.Normalize();
						currentToBeforDir.Normalize();
						
						
						Vector3 upNext = Vector3.Cross(currentToNextrDir, currentToCameraDir);
						Vector3 upBefor = Vector3.Cross(currentToCameraDir , currentToBeforDir); 
						
						upNext.Normalize();
						upBefor.Normalize();
						
						Vector3 rightNext  = Vector3.Cross(upNext , currentToCameraDir); 
						Vector3 rightBefor = Vector3.Cross(upBefor , currentToCameraDir); 
						
						float dot = Vector3.Dot(rightBefor, rightNext);  
						if(dot > 0)
						{
							//camera to cw direction
							up = upBefor + upNext;
						}
						else
						{
							up = upBefor + upNext;
							//up *= -1;
							//camera to ccw direction
						}
					}

					up.Normalize();
					sidePoint1 = m_sections[i].point  + up*height*m_rightRatio;
					sidePoint2 = m_sections[i].point - up*height*m_leftRatio;
				}
				else
				{  
					sidePoint1 = m_sections[i].point  + curTrailSection.upDir*height*m_rightRatio;
					sidePoint2 = m_sections[i].point - curTrailSection.upDir*height*m_leftRatio;
				}
 
				m_vertices[i*2]   = sidePoint1;
				m_vertices[i*2+1] = sidePoint2;

				if(m_bUVRatio == true)
				{
					float texRatio = m_textureW/m_textureH; 
					TexCoordV = (mylineLength/m_heightStart)*texRatio;
				}
 
				//uv
				//TexCoordV = (float)i/(float)(sections.Count-1);
				m_uv[i*2].x	= TexCoordV;
				m_uv[i*2].y	= 0;
				m_uv[i*2+1].x	= TexCoordV;
				m_uv[i*2+1].y	= 1;

				//color
				Color myColor = Color.white;
				
				if(m_Colors.Length > 0)
				{
					float myColorT = m_Colors.Length*((float)(i)/(float)m_sections.Count);  
					int ci = (int)myColorT;
					myColorT = myColorT - (float)ci;
					if(ci <m_Colors.Length -1)
					{
						Color beginColor = m_Colors[ci];
						Color endColor = m_Colors[ci+1];
						myColor = Color.Lerp(beginColor, endColor, myColorT);
					} 
					else
					{
						myColor = m_Colors[m_Colors.Length -1];
					}
				}

				// 
				float alphaScale = 1;
				if(m_SectionLifeTime > 0)
				{
					float timeGap = Time.time - m_sections[i].spwanTime;
					alphaScale = (m_SectionLifeTime  - timeGap)/m_SectionLifeTime;
					if(alphaScale < 0)
						alphaScale = 0;
				}

				myColor.a 		*= m_fadeColorScale*alphaScale;
				m_colors[i*2] 	= myColor;
				m_colors[i*2+1]	= myColor;

				mylineLength += m_sections[i].space;
			}  
			//System.
			//GCHandle.
			int []triIndexs = new int[(m_sections.Count-1)*2*3];
			
			for(int i = 0; i< m_sections.Count-1;++i)
			{
				
				triIndexs[i*6] 		= i*2;
				triIndexs[i*6 + 1] 	= i*2+2;
				triIndexs[i*6 + 2] 	= i*2+1; 
				
				triIndexs[i*6 + 3] 	= i*2+1;
				triIndexs[i*6 + 4] 	= i*2+2;
				triIndexs[i*6 + 5] 	= i*2+3;
				
			} 
  
			m_mesh.vertices 	= m_vertices;
			m_mesh.colors 		= m_colors;
			m_mesh.uv 			= m_uv;
			m_mesh.triangles 	= triIndexs;  
			
		}
		else
		{
 
			m_mesh.triangles 	= null;
		}
	}

	void InterpolateNewHip(TrailSection begin, TrailSection end, int segment) 
	{ 
		for(int i = 0; i < segment-1; ++i)
		{
			TrailSection trailSection = new TrailSection(); 
			trailSection.point = begin.point;
			trailSection.upDir = begin.upDir; 
			m_sections.Insert(0, trailSection);
		}

		TrailSection trailSection2 = new TrailSection(); 
		trailSection2.point = end.point;
		trailSection2.upDir = end.upDir;
		m_sections.Insert(0, trailSection2);
	}

	void Interpolate2(TrailSection [] trailSections, int refrashId, int segment) 
	{
		int refrashId2 = refrashId;
		int loopSegment = segment;//-1;
		float s = 0;
		for(int i = 0 ; i < loopSegment; ++i)
		{
			--refrashId;
			if(refrashId < m_sections.Count)
			{
				s = (float)i/(float)loopSegment;
				TrailSection trailSection = m_sections[refrashId]; 

				trailSection.point = CatmullRom(trailSections[0].point,
				                                            trailSections[1].point,
				                                            trailSections[2].point,
				                                            trailSections[3].point,
				                                            (float)i, (float)loopSegment);
	  
				trailSection.upDir  = Vector3.Slerp(trailSections[1].upDir, trailSections[2].upDir, s);  
				trailSection.spwanTime = trailSections[1].spwanTime + 
							(trailSections[2].spwanTime - trailSections[1].spwanTime)*s;
				//sections[refrashId] = trailSection; 
			} 
		}
		--refrashId;
		/*
		if(refrashId < m_sections.Count)
		{
			TrailSection trailSection2 = m_sections[refrashId]; 
			trailSection2.point = trailSections[2].point;
			trailSection2.upDir = trailSections[2].upDir; 
			trailSection2.spwanTime = trailSections[2].spwanTime;
			//sections[refrashId] = trailSection2;
		} //*/
 
		//
		//Interpolate.CatmullRom(basePoints, subdivisions, false);
	}
 
	public void  LateUpdate()
	{ 
 
		//begin
		if(m_fadeColorScaleSpeed > 0)
		{
			if(m_fadeColorScale >= 1)
			{
				m_fadeColorScale = 1;
				m_currentTrailLength = m_maxTrailLength;
			}
			else
			{
				m_fadeColorScale += m_fadeColorScaleSpeed  * Time.smoothDeltaTime;
				m_currentTrailLength = m_maxTrailLength*m_fadeColorScale;
			}
		}
		//end
		else if(m_fadeColorScaleSpeed < 0)
		{ 
			if(m_fadeColorScale <= 0)
			{
				this.enabled = false;
				this.renderer.enabled = false;
				m_fadeColorScale = 0;
				//m_isStart 	= false;
				//m_emit 		= false;
				m_target 	= null;
				m_currentTrailLength = 0;
				ClearTrail();
				if(m_fadeDestroy)
				{
					Destroy(this.gameObject);
				}
				//
			}
			else
			{ 
				m_fadeColorScale += m_fadeColorScaleSpeed * Time.smoothDeltaTime;
				m_currentTrailLength = m_maxTrailLength*m_fadeColorScale;
			} 
		}

		UpdatePosition(this.m_target, Camera.main);  
	}
	/*
	public void  UpdateTrail()
	{ 
		if(m_isStart == true)
		{ 
			UpdatePosition(this.m_target); 
			
		} 
	}//*/
	
	public void Begin()
	{ 
		Begin( m_target, 0);
	}
	public void Begin(Transform target)
	{ 
		Begin( target, 0);
	}
	public void Begin( float fadeTime)
	{ 
		Begin( m_target, fadeTime); 
	}
	public void Begin(Transform target, float fadeTime)
	{
		this.ClearTrail();
		RenderReady();
		this.m_target = target;
		this.enabled 		  = true;
		this.renderer.enabled = true;
		//m_isStart = true;
	 
		if(fadeTime > 0)
		{
			m_fadeColorScale 		= 0;
			m_fadeColorScaleSpeed 	= 1/fadeTime;
			m_currentTrailLength    = 0;
		}
		else
		{
			m_fadeColorScale		= 1;
			m_fadeColorScaleSpeed   = 0;
			m_currentTrailLength    = m_maxTrailLength;
		}
	}
	public void End(float fadeTime)
	{ 
		if(fadeTime > 0)
		{
			m_fadeColorScaleSpeed   = -1/fadeTime;

		}
		else
		{ 
			ClearTrail();
			//m_emit		= false;
			this.enabled 		  = false;
			this.renderer.enabled = false;
			//m_isStart 	= false; 
			m_target 	= null;
		}
	}
	bool m_fadeDestroy = false;
	public void FadeDestory(float fadeTime)
	{
		//m_target 		= null; 
		m_fadeDestroy 	= true;
		if(fadeTime > 0)
		{
			m_fadeColorScaleSpeed   = -1/fadeTime;;
		}
		else
		{ 
			ClearTrail();
			//m_emit		= false;
			this.enabled 		  = false;
			this.renderer.enabled = false;
			//m_isStart 	= false; 
			m_target 	= null;
			Destroy(this.gameObject);

		}

	}

	 void ClearTrail() 
	{  
		this.m_sections.Clear();
		if(m_interpolatorSections != null)
			this.m_interpolatorSections.Clear();
		if (m_mesh != null) {
			m_mesh.Clear();
			m_sections.Clear();
		}
	} 
#if UNITY_EDITOR
	bool inited = false;
	void OnDrawGizmos()
	{  
		if(UnityEditor.EditorApplication.isPlaying)
			return;
		Camera Cameracurrent = UnityEditor.SceneView.currentDrawingSceneView.camera;
 
		if(Cameracurrent == null)
			return;
		if(inited == false)
		{
			m_material				= null;
			m_Colors 				= new Color[2]{Color.white, Color.white};
			m_maxTrailLength		= 10; 
			m_heightStart			= 2.0f;
			m_heightEnd				= 2.0f;
			//m_emit					= true; 
			m_target				= null; 
			m_heightDir				= Vector3.right;
			m_interpolateSegnent	= 0;
			m_bUVRatio				= true;
			m_minDistance 			= 0.1f;  
			
			m_currentTrailLength	= 0; 
  
 
			m_sections = new List<TrailSection>();
			m_interpolatorSections = null;
			m_interpolatorSectionPoint = new TrailSection[4];
			//m_LastVertexPoint 		= 0;
			//m_isStart 				= false;
			//this.enabled 			= false;
			m_fadeColorScale 		= 1;
			m_fadeColorScaleSpeed 	= 0;
			m_trailObject 			= null; 
			m_totalSpace			= 0;
			
			m_textureW = 0;
			m_textureH = 0;
			//m_cpuOptimize = true;


			m_vertices = new Vector3[100*2 ];
			m_colors   = new Color[100*2 ];
			m_uv       = new Vector2[100*2 ]; 
			m_minDistance = m_minDistance*m_minDistance; 
 
			
			m_heightDir.Normalize();
			m_trailObject = this.gameObject;
			m_trailObject.transform.parent = null;
			m_trailObject.transform.position = Vector3.zero;
			m_trailObject.transform.rotation = Quaternion.identity;
			m_trailObject.transform.localScale = Vector3.one;
			m_trailObject.AddComponent(typeof(MeshFilter));
			m_trailObject.AddComponent(typeof(MeshRenderer));
			m_trailObject.renderer.material = m_material;
			m_textureW = 512;
			m_textureH =512;
			m_mesh 											= new Mesh();
			m_mesh.name 									= name + "TrailMesh";
			m_trailObject.GetComponent<MeshFilter>().mesh 	= m_mesh;
			if(m_interpolateSegnent > 0)
			{
				m_interpolatorSections = new List<TrailSection>();
			}
			
			if(m_target == null)
			{
				m_target = this.transform;
			}
			m_currentTrailLength = m_maxTrailLength;


			Begin( this.transform);
			inited = true;
		}

		//this.UpdatePosition(this.transform, UnityEditor.SceneView.currentDrawingSceneView.camera);
 
		for(int i = 0; i < this.m_mesh.triangles.Length/3; ++i)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(m_mesh.vertices[m_mesh.triangles[i]], m_mesh.vertices[m_mesh.triangles[i+1]]);
			Gizmos.DrawLine(m_mesh.vertices[m_mesh.triangles[i]], m_mesh.vertices[m_mesh.triangles[i+2]]);
			Gizmos.DrawLine(m_mesh.vertices[m_mesh.triangles[i+1]], m_mesh.vertices[m_mesh.triangles[i+2]]); 
		}
		/*
		for(int i = 0; i < this.m_mesh.vertices.Length/4; ++i)
		{ 
			Gizmos.color = Color.red; 
			Gizmos.DrawLine(this.m_mesh.vertices[i], this.m_mesh.vertices[i+2]);
			Gizmos.DrawLine(this.m_mesh.vertices[i+1], this.m_mesh.vertices[i+3]);
		}//*/

	}
#endif
	Vector3 Hermite(ref Vector3 v1, ref Vector3 t1, ref Vector3 v2, ref Vector3 t2, ref float s)
	{
 
		float s2 = s*s;
		float s3 = s2*s; 
		//(2v1 - 2v2 + t2 + t1)s3 + (3v2 - 3v1 - 2t1 - t2)s2 + t1s + v1
		return (2*s3 - 3*s2 + 1)*v1 + (-2*s3 + 3*s2)*v2 + (s3 - 2*s2 + s)*t1 + (s3 - s2)*t2; 
	} 

	static Vector3 CatmullRom(Vector3 previous, Vector3 start, Vector3 end, Vector3 next, 
	                          float elapsedTime, float duration)
	{
		// References used:
		// p.266 GemsV1
		//
		// tension is often set to 0.5 but you can use any reasonable value:
		// http://www.cs.cmu.edu/~462/projects/assn2/assn2/catmullRom.pdf
		//
		// bias and tension controls:
		// http://local.wasp.uwa.edu.au/~pbourke/miscellaneous/interpolation/
		
		float percentComplete = elapsedTime / duration;
		float percentCompleteSquared = percentComplete * percentComplete;
		float percentCompleteCubed = percentCompleteSquared * percentComplete;
		
		return previous * (-0.5f * percentCompleteCubed +
		                   percentCompleteSquared -
		                   0.5f * percentComplete) +
			start   * ( 1.5f * percentCompleteCubed +
			           -2.5f * percentCompleteSquared + 1.0f) +
				end     * (-1.5f * percentCompleteCubed +
				           2.0f * percentCompleteSquared +
				           0.5f * percentComplete) +
				next    * ( 0.5f * percentCompleteCubed -
				           0.5f * percentCompleteSquared);
	}
	/*
	// optional, use gizmos to draw the path in the editor
	void OnDrawGizmos() 
	{ 
		//path.
		IEnumerable<Vector3> smoothTip = NcInterpolate.NewCatmullRom(tipPoints, m_nSubdivisions, false);
		List<Vector3> smoothTipList = new List<Vector3>(smoothTip);
		Interpolate.NewCatmullRom(smoothTipList, 1000, false);
		
		if (path != null && path.Length >= 2) 
		{
			if(path[0] != null && path[1] != null)
			{
				for(int a = 0 ; a < path.Length-1; ++a)
				{
					Vector3 beforPos =  path[a].position;
					for(int  i= 1; i < betweenNodeCount; ++i)
					{
						float dot = Vector3.Dot(path[a].right, path[a+1].right);
						dot = Mathf.PI - Mathf.Acos(dot);
						dot /= Mathf.PI;
						float dist = ( path[a].position - path[a+1].position).magnitude*2;
						dist = dist*dot*normalLength;
						Vector3 v1 = path[a].position;//path[0].;
						Vector3 t1 = path[a].right*dist;
						Vector3 v2 = path[a+1].position;
						Vector3 t2 = path[a+1].right*dist;
						float    s = (float)i/(float)(betweenNodeCount-1); 
						Vector3 newPos = Hermite(ref v1,ref  t1,ref v2,ref t2, ref s);
						Gizmos.DrawLine(beforPos, newPos); 
						beforPos = newPos;
					}
				} 
			} 
		}
	}//*/
}

