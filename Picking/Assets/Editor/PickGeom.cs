using UnityEngine;
using System.Collections;
using UnityEditor;


public class PickGeom : EditorWindow
{
	[MenuItem("Pick/Test")]
	static void OnOpen()
	{
		PickGeom pickGeom = (PickGeom)EditorWindow.GetWindow(typeof(PickGeom));
		pickGeom.title = "test"; 
	}
	private void OnEnable( )
	{
		 SceneView.onSceneGUIDelegate += new SceneView.OnSceneFunc(OnSceneGUI);
	}
   	void OnSceneGUI(SceneView sceneView)
	{
		
		Ray mouseRay = sceneView.camera.ScreenPointToRay(new Vector3(Event.current.mousePosition.x,  sceneView.position.height - Event.current.mousePosition.y , 0.5f));
 		/*
		GameObject gameObject = HandleUtility.PickGameObject(Event.current.mousePosition, true);
		if(gameObject != null)
		{
			Handles.SphereCap(0 , gameObject.transform.position, Quaternion.identity, 0.5f);
		}//*/
		
		RaycastHit hit;
		if(Physics.Raycast(mouseRay, out hit) == true)
		{
			
			
		}
		
		if (Event.current.type == EventType.mouseMove)
		{
			MeshFilter m;
		 	
			/*
			RaycastHit hit;
			if(Physics.Raycast(mouseRay, out hit) == true)
			{
				 
				//
				
			}//*/
		}
 
		
		
	}
}
