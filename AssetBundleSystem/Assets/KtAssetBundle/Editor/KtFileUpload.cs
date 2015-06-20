using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.IO;
using System.Text; 
 

public class KtFileUpload : MonoBehaviour {

    private string m_LocalFileName = "C:/boot.ini";    
	private string m_URL = "http://192.168.178.29/php/upload.php"; 
	
	IEnumerator UploadFileCo(string localFileName, string uploadURL) 
	{      
		WWW localFile = new WWW("file:///" + localFileName); 
		yield return localFile;  
		if (localFile.error == null)
			Debug.Log("Loaded file successfully");
		else       
		{           
			Debug.Log("Open file error: "+localFile.error);   
			yield break; // stop the coroutine here20.   
		}         
		WWWForm postForm = new WWWForm();  
		// version 1        
		//postForm.AddBinaryData("theFile",localFile.bytes);25. 
		// version 2     
		postForm.AddBinaryData("theFile",localFile.bytes,localFileName,"text/plain");   
		WWW upload = new WWW(uploadURL,postForm);    
		yield return upload;     
		if (upload.error == null)   
			Debug.Log("upload done :" + upload.text);   
		else         
			Debug.Log("Error during upload: " + upload.error);
 
	} 
	
	void UploadFile(string localFileName, string uploadURL) 
	{    
		StartCoroutine(UploadFileCo(localFileName, uploadURL));
	} 
	
	void OnGUI() 
	{     
		GUILayout.BeginArea(new Rect(0,0,Screen.width,Screen.height));
		m_LocalFileName = GUILayout.TextField(m_LocalFileName);
		m_URL           = GUILayout.TextField(m_URL); 
		if (GUILayout.Button("Upload")) 
		{     
			UploadFile(m_LocalFileName,m_URL);
		}        GUILayout.EndArea();
	}
	public static string Create(string path, string url)
    {
        string authInfo = "msjtech" + ":" + "";
 
        string file = path;

        FileInfo fi = new FileInfo(file);
        int fileLength = (int)fi.Length;

        FileStream rdr = new FileStream(file, FileMode.Open); 
        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        httpWebRequest.Method = "POST";
        httpWebRequest.ContentType = "application/x-www-form-urlencoded";
        httpWebRequest.Accept = "application/xml";

        authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
        httpWebRequest.Headers["Authorization"] = "Basic " + authInfo;
        byte[] requestBytes = new byte[fileLength];

        int bytesRead = 0;
        httpWebRequest.ContentLength = requestBytes.Length;
        using (Stream requestStream = httpWebRequest.GetRequestStream())
        {
            while ((bytesRead = rdr.Read(requestBytes, 0, requestBytes.Length)) != 0)
            {
                requestStream.Write(requestBytes, 0, bytesRead);
                requestStream.Close();
            }
        }
        //READ RESPONSE FROM STREAM
        string responseData;
        using (StreamReader responseStream = new StreamReader(httpWebRequest.GetResponse().GetResponseStream()))
        {
            responseData = responseStream.ReadToEnd();
            responseStream.Close();
        }
        return responseData;
    }	
}

public class UploadFile
{
    public class TimeoutWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest webRequest = base.GetWebRequest(address);
			if(webRequest is HttpWebRequest)
			{
				HttpWebRequest httpWebRequest = webRequest as HttpWebRequest;
				//httpWebRequest.KeepAlive = false;
			}
            webRequest.Timeout = 100000000; 
            return webRequest;
        }
    }
 
   public  static void Upload(string filePath, string uploadUrl)
   {        
      // Create a new WebClient instance.
      //WebClient client = new TimeoutWebClient();

      //서버에 익명으로 파일을 올리는것은 허용되지 않으므로 NetworkCredential 객체를
      //이용하여 계정에 계정에 대한 정보를 입력 해야 한다.
		//Uri u = new Uri(uploadUrl);
     // client.Credentials =  new NetworkCredential("msjtech", "비밀번호");
   
      // File Uploaded using PUT method
		
   //   byte[] responseArray = client.UploadFile("http://www.angrypower.com/","PUT", "c:\\a.txt");
	 


   }
}
  
