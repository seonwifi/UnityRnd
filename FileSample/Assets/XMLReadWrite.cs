using UnityEngine;
using System.Collections;
using System.Xml;

public class XMLReadWrite
{
	public static bool LodXml(string filePath)
	{
		XmlDocument xmlDoc = new XmlDocument();
		try
		{
			xmlDoc.Load(filePath);
			
			XmlElement root = xmlDoc.DocumentElement; 
 			Debug.Log(root.Name); 
			root = root.SelectSingleNode("boolHasChild") as XmlElement;
			foreach(XmlNode node in root.ChildNodes)
			{   
				 XmlElement xmlElement = node as XmlElement;
				if(xmlElement != null)
				{ 
					Debug.Log("NodeName: " + node.Name + ", NodeValue: " + node.InnerText);
					string attributeValue = xmlElement.GetAttribute("name");
					Debug.Log("attributeValue: " + attributeValue);
					 
				} 
			}
			 XmlElement hasChild = root.SelectSingleNode("boolHasChild") as XmlElement;
			/*
			foreach(XmlNode node in root.ChildNodes)
			{   
				 XmlElement xmlElement = node as XmlElement;
				if(xmlElement != null)
				{
					Debug.Log("NodeName: " + node.Name + ", NodeValue: " + node.InnerText);
					string attributeValue = xmlElement.GetAttribute("name");
					Debug.Log("attributeValue: " + attributeValue);
					 
				} 
			}//*/
 			//root.get
 			//root.FirstChild.
			//System.Xml.XmlNode
		}
		catch(XmlException e)
		{
			Debug.Log("xml Read Error: " + e.Message);
			return false;
		}
 		return true;
	}
	
	public static bool SaveXml(string filePath)
	{ 
		try
		{	 
			XmlWriterSettings settings 	= new XmlWriterSettings();
			settings.Indent 			= true;
			settings.IndentChars 		= "    "; //  4개의 공백 문자
			settings.NewLineChars 		= System.Environment.NewLine;
			settings.NewLineHandling 	= NewLineHandling.Replace;
			settings.NewLineOnAttributes = false;
 
			System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(filePath, settings);
			xmlWriter.WriteStartDocument();
		 
			xmlWriter.WriteStartElement("root");
			xmlWriter.Settings.NewLineOnAttributes = true;
 
 
			xmlWriter.WriteAttributeString("data", "value"); 
			xmlWriter.WriteAttributeString("data2", "value2");  
  
			xmlWriter.WriteAttributeString("data","ns", "value3");
			xmlWriter.WriteAttributeString("data","ns2", "value3");
 
 
			xmlWriter.WriteString("value");
			xmlWriter.WriteValue(5.0f);
			xmlWriter.WriteEndElement(); 
			xmlWriter.WriteEndDocument();
			
			xmlWriter.Flush();
			xmlWriter.Close();
			
			 
		}
 		catch(XmlException e)
		{
			Debug.Log("xml Read Error: " + e.Message);
			return false;
		}
		return true;
	}
}
