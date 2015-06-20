using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ActiveTest : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		for(int i = 0 ; i< transform.childCount; ++i)
		{
			transform.GetChild(i).gameObject.SetActive(false);	 
		} 	
	}
	int m_activeCount = 10;
	List<GameObject> Actives 	= new List<GameObject>();
	List<GameObject> OldActives = new List<GameObject>();
	int ArrayPoint = 0;
	// Update is called once per frame
	void Update () 
	{		
		int count = 0;
		
		for(int i = 0 ; i< transform.childCount; ++i)
		{
			if(ArrayPoint >= transform.childCount)
				ArrayPoint = 0;
			if(!transform.GetChild(ArrayPoint).gameObject.activeSelf)
			{
				transform.GetChild(ArrayPoint).gameObject.SetActive(true);
				Actives.Add(transform.GetChild(ArrayPoint).gameObject);
				count++;
				if(count == m_activeCount)
				{
					break;
				}
			}
			ArrayPoint++;
		} 
		
		for(int i = 0 ; i< OldActives.Count; ++i)
		{
			OldActives[i].SetActive(false);
		}
		OldActives.Clear();
		List<GameObject> temp = OldActives;
		OldActives  = Actives;
		Actives		= temp;		
	}
}
