using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SystemProfiler : MonoBehaviour {


	Vector2 scrollPos = new Vector2(0,0);
	public static SystemProfiler StartProfile()
	{
		SystemProfiler systemProfiler = null;
		GameObject go = GameObject.Find("SystemProfiler");
		if(go != null)
		{
			return go.GetComponent<SystemProfiler>(); 
		}
		else
		{
			systemProfiler  = new GameObject("SystemProfiler").AddComponent<SystemProfiler>();
		}

		return systemProfiler;
	}
	public static void StopProfile()
	{
		GameObject go = GameObject.Find("SystemProfiler");
		if(go != null)
		{
			 GameObject.Destroy(go);
		}
	}
	public static SystemProfiler PauseProfile()
	{
		GameObject go = GameObject.Find("SystemProfiler");
		if(go != null)
		{
			return go.GetComponent<SystemProfiler>(); 
		} 
		return null;
	}
	public enum eSizeByte
	{
		Byte = 0,
		KByte,
		MByte,
		GByte, 
	}	
	
	public bool 		m_isCashingProfile 		= true;
	public bool 		m_isSystemInfoProfile 	= true;
	public bool 		m_isMemoryProfile 		= true;
	public eSizeByte 	m_sizeByte 			    = eSizeByte.MByte;

	void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
	}
	void OnGUI()
	{  
 
 		float starty = Screen.height-50;
		m_isCashingProfile = GUI.Toggle(new Rect(Screen.width-120, starty,100,35), m_isCashingProfile, "Cashing");
		m_isSystemInfoProfile  = GUI.Toggle(new Rect(Screen.width-120, starty -= 40,100,35),  m_isSystemInfoProfile, "SystemInfo");
		m_isMemoryProfile = GUI.Toggle(new Rect(Screen.width-120, starty -= 40,100,35), m_isMemoryProfile, "Memory");
		 
		long kb = 1024; 
		long mb = kb*1024; 
		long gb = mb*1024; 
		string report = "Profile";
		if(m_isCashingProfile)
		{
			report += "\n\n" + "Cashing";
			report += "\n" + MakeStringCashing();
		}
		
		if(m_isSystemInfoProfile)
		{
			report += "\n\n" + "SystemInfo";
			report += "\n"+ MakeStringSystemInfo();
		}
		
		if(m_isMemoryProfile)
		{
			report += "\n\n" + "Memory";
			report += "\n"+ MakeStringProfiler(); 
		}
		 
		//scrollPos = GUI.BeginScrollView( new Rect(10,30,Screen.width-120,Screen.height), scrollPos, new Rect(10,30,Screen.width-120,Screen.height));
 		scrollPos = GUI.BeginScrollView(new Rect(10,30,Screen.width-60,Screen.height - 150), scrollPos, new Rect(10,30,Screen.width-80,Screen.height*3));
 		 
		GUI.Label(new Rect(10,30,Screen.width-60,Screen.height*2), report); 
		   
 		GUI.EndScrollView(); 
		 
	}
	string MakeStringCashing()
	{
		string report = ""; 
		long kb = 1024; 
		long mb = kb*1024; 
		long gb = mb*1024; 
 
		long spaceFree     = Caching.spaceFree & 0xffffffff;
		double freeSpacekb = (double)spaceFree/(double)(kb);
		double freeSpacemb = (double)spaceFree/(double)(mb);
		double freeSpacegb = (double)spaceFree/(double)(gb);
		if(m_sizeByte == eSizeByte.Byte)
			report += " Cach FreeSpaceByte: 	" + spaceFree.ToString() +" Byte";
		if(m_sizeByte == eSizeByte.KByte)
			report += " Cach FreeSpaceKB: 	" + freeSpacekb.ToString("0.0000") +" KByte";
		if(m_sizeByte == eSizeByte.MByte)
			report += "\n Cach FreeSpaceMB: 	" + freeSpacemb.ToString("0.000") +" MByte";
		if(m_sizeByte == eSizeByte.GByte)
			report += "\n Cach FreeSpaceGB: 	" + freeSpacegb.ToString("0.000") +" GByte";
 		
		long bit32Max = 0xffffffff;
		long maximumAvailableDiskSpace = Caching.maximumAvailableDiskSpace & 0xffffffff; 
		
		double maximumAvailableDiskSpacekb = (double)maximumAvailableDiskSpace/(double)(kb);
		double maximumAvailableDiskSpacemb = (double)maximumAvailableDiskSpace/(double)(mb);
		double maximumAvailableDiskSpacegb = (double)maximumAvailableDiskSpace/(double)(gb);
 		if(m_sizeByte == eSizeByte.Byte)
			report += "\n Cach MaximumAvailableDiskSpaceByte: 	" + maximumAvailableDiskSpace.ToString() +" Byte";
		if(m_sizeByte == eSizeByte.KByte)
			report += "\n Cach MaximumAvailableDiskSpaceKB: 	" + maximumAvailableDiskSpacekb.ToString("0.0000") +" KByte";
		if(m_sizeByte == eSizeByte.MByte)
			report += "\n Cach MaximumAvailableDiskSpaceMB: 	" + maximumAvailableDiskSpacemb.ToString("0.000") +" MByte";
		if(m_sizeByte == eSizeByte.GByte)
 			report += "\n Cach MaximumAvailableDiskSpaceGB: 	" + maximumAvailableDiskSpacegb.ToString("0.000") +" GByte";
		 
		long cachUse = maximumAvailableDiskSpace - spaceFree;
		double cachUsekb = cachUse/(double)(kb);
		double cachUsemb = cachUse/(double)(mb);
		double cachUsegb = cachUse/(double)(gb);
		if(m_sizeByte == eSizeByte.Byte)
			report += "\n Cach Use Byte: 	" + cachUse.ToString() +" Byte";
		if(m_sizeByte == eSizeByte.KByte)
 			report += "\n Cach UseKB: 	" + cachUsekb.ToString("0.0000") +" KByte";
		if(m_sizeByte == eSizeByte.MByte)
			report += "\n Cach UseMB: 	" + cachUsemb.ToString("0.000") +" MByte";
		if(m_sizeByte == eSizeByte.GByte)
			report += "\n Cach UseGB: 	" + cachUsegb.ToString("0.000") +" GByte";	
		return report;
	}
	
	string MakeStringSystemInfo()
	{
		string systemInfo = "";
		systemInfo += " deviceModel: 	" + 					SystemInfo.deviceModel 					;//+ "		;;;(The model of the device)";
		systemInfo += "\n" + " deviceName: 	" + 				SystemInfo.deviceName 					;//+ "		;;;(The user defined name of the device)";;;;
		systemInfo += "\n" + " deviceType: 	" + 				SystemInfo.deviceType.ToString() 		;//+ "		;;;(the kind of device the application is running on)";
		systemInfo += "\n" + " deviceUniqueIdentifier: 	" + 	SystemInfo.deviceUniqueIdentifier		;//+ "		;;;(A unique device identifier. It is guaranteed to be unique for every device)";
		systemInfo += "\n" + " graphicsDeviceID: 	" + 		SystemInfo.graphicsDeviceID.ToString() ;//	+ "		;;;(The identifier code of the graphics device)";
		systemInfo += "\n" + " graphicsDeviceName: 	" + 		SystemInfo.graphicsDeviceName			;//+ "		;;;(The name of the graphics device)";
		systemInfo += "\n" + " graphicsDeviceVendor: 	" + 	SystemInfo.graphicsDeviceVendor 		;//+ "		;;;(The vendor of the graphics device)";
		systemInfo += "\n" + " graphicsDeviceVendorID: 	" + 	SystemInfo.graphicsDeviceVendorID	 	;//+ "		;;;(The identifier code of the graphics device vendor)";
		systemInfo += "\n" + " graphicsDeviceVersion: 	" + 	SystemInfo.graphicsDeviceVersion 		;//+ "		;;;(The graphics API version supported by the graphics device)";
		systemInfo += "\n" + " graphicsMemorySize: 	" + 		SystemInfo.graphicsMemorySize	 		;//+ "		;;;(Amount of video memory present)";
		systemInfo += "\n" + " graphicsPixelFillrate: 	" + 	SystemInfo.graphicsPixelFillrate 		;//+ "		;;;(Approximate pixel fill-rate of the graphics device)";
		systemInfo += "\n" + " graphicsShaderLevel: 	" + 		SystemInfo.graphicsShaderLevel 			;//+ "		;;;(Graphics device shader capability level)";
		systemInfo += "\n" + " npotSupport: 	" + 				SystemInfo.npotSupport 					;//+ "		;;;(What NPOTSupport support does GPU provide?)";
		systemInfo += "\n" + " operatingSystem:	 " + 			SystemInfo.operatingSystem 				;//+ "		;;;(Operating system name with version)";
		systemInfo += "\n" + " processorCount: 	" + 			SystemInfo.processorCount 				;//+ "		;;;(Number of processors present)";
		systemInfo += "\n" + " processorType: 	" + 			SystemInfo.processorType 				;//+ "		;;;(Processor namet)";
		systemInfo += "\n" + " supportedRenderTargetCount: 	"+SystemInfo.supportedRenderTargetCount 	;//+ "		;;;(How many simultaneous render targets (MRTs) are supported?)";
		systemInfo += "\n" + " supports3DTextures: 	" + 		SystemInfo.supports3DTextures 			;//+ "		;;;(Are 3D (volume) textures supported?)";
		systemInfo += "\n" + " supportsAccelerometer: 	" + 	SystemInfo.supportsAccelerometer 		;//+ "		;;;(Is an accelerometer available on the device?)";
		systemInfo += "\n" + " supportsComputeShaders: 	" + 	SystemInfo.supportsComputeShaders 		;//+ "		;;;(Are compute shaders supported?)";
		systemInfo += "\n" + " supportsGyroscope: 	" + 		SystemInfo.supportsGyroscope 			;//+ "		;;;(Is a gyroscope available on the device?)";
		systemInfo += "\n" + " supportsImageEffects: 	" + 	SystemInfo.supportsImageEffects 		;//+ "		;;;(Are image effects supported? )";
		systemInfo += "\n" + " supportsInstancing: 	" + 		SystemInfo.supportsInstancing 			;//+ "		;;;(Is GPU draw call instancing supported? )"; 
		systemInfo += "\n" + " supportsLocationService: 	" + 	SystemInfo.supportsLocationService 		;//+ "		;;;(Is the device capable of reporting its location?)";
		systemInfo += "\n" + " supportsRenderTextures: 	" + 	SystemInfo.supportsRenderTextures 		;//+ "		;;;(Are render textures supported?)"; 
		systemInfo += "\n" + " supportsShadows: 	" + 			SystemInfo.supportsShadows 				;//+ "		;;;(Are built-in shadows supported? )";
		systemInfo += "\n" + " supportsStencil: 	" + 			SystemInfo.supportsStencil 				;//+ "		;;;(Is the stencil buffer supported?)";
 		systemInfo += "\n" + " supportsVertexPrograms: 	" + 	SystemInfo.supportsVertexPrograms 		;//+ "		;;;(Does this machine support vertex programs?)";
		systemInfo += "\n" + " supportsVibration: 	" + 		SystemInfo.supportsVibration 			;//+ "		;;;(Is the device capable of providing the user haptic feedback by vibration?)";
		systemInfo += "\n" + " systemMemorySize: 	" + 		SystemInfo.systemMemorySize 			;//+ "		;;;(Amount of system memory present)"; 
		return systemInfo;
	}
	string MakeStringProfiler()
	{ 
		string profiler = ""; 
		long kb = 1024; 
		long mb = kb*1024;  
		
		long Byte = 0;
		double KByte = 0;
		double MByte = 0;
  
		Byte = (long)Profiler.GetMonoHeapSize();
		KByte = decimal.ToDouble(Profiler.GetMonoHeapSize())/(double)(kb);
		MByte = decimal.ToDouble(Profiler.GetMonoHeapSize())/(double)(mb);  
		if(m_sizeByte == eSizeByte.Byte)
			profiler += " MonoHeapSize Byte: 	" 				+ Byte.ToString() + " Byte";
		if(m_sizeByte == eSizeByte.KByte)
			profiler += "\n" + " MonoHeapSize KByte: 	" 				+ KByte.ToString("0.000") + " KByte"; 
		if(m_sizeByte == eSizeByte.MByte)
			profiler += "\n" + " MonoHeapSize MByte: 	" 				+ MByte.ToString("0.000")+ " MByte"; 
 
		Byte =  (long)Profiler.GetMonoUsedSize();
		KByte = (double)Profiler.GetMonoUsedSize()/(double)(kb);
		MByte = (double)Profiler.GetMonoUsedSize()/(double)(mb); 
		if(m_sizeByte == eSizeByte.Byte)
			profiler += "\n" + " MonoUsedSize Byte: 		" 				+ Byte.ToString() + " Byte";
		if(m_sizeByte == eSizeByte.KByte)
			profiler += "\n" + " MonoUsedSize KByte: 	" 				+ KByte.ToString("0.000") + " KByte"; 
		if(m_sizeByte == eSizeByte.MByte)
			profiler += "\n" + " MonoUsedSize MByte: 	" 				+ MByte.ToString("0.000")+ " MByte"; 
 
		Byte = (long)Profiler.GetTotalAllocatedMemory();
		KByte = (double)Profiler.GetTotalAllocatedMemory()/(double)(kb);
		MByte = (double)Profiler.GetTotalAllocatedMemory()/(double)(mb); 
		if(m_sizeByte == eSizeByte.Byte)
			profiler += "\n" + " TotalAllocatedMemory Byte: 		" 		+ Byte.ToString() + " Byte";
		if(m_sizeByte == eSizeByte.KByte)
			profiler += "\n" + " TotalAllocatedMemory KByte: 	" 		+ KByte.ToString("0.000") + " KByte"; 
		if(m_sizeByte == eSizeByte.MByte)
			profiler += "\n" + " TotalAllocatedMemory MByte: 	" 		+ MByte.ToString("0.000")+ " MByte"; 
 
		Byte = (long)Profiler.GetTotalUnusedReservedMemory();
		KByte = (double)Profiler.GetTotalUnusedReservedMemory()/(double)(kb);
		MByte = (double)Profiler.GetTotalUnusedReservedMemory()/(double)(mb); 
		if(m_sizeByte == eSizeByte.Byte)
			profiler += "\n" + " TotalUnusedReservedMemory Byte: 	" 		+ Byte.ToString() + " Byte";
		if(m_sizeByte == eSizeByte.KByte)
			profiler += "\n" + " TotalUnusedReservedMemory KByte: 	" 		+ KByte.ToString("0.000") + " KByte"; 
		if(m_sizeByte == eSizeByte.MByte)		
			profiler += "\n" + " TotalUnusedReservedMemory MByte: 	" 		+ MByte.ToString("0.000")+ " MByte"; 
 
		Byte = (long)Profiler.GetTotalReservedMemory();
		KByte = (double)Profiler.GetTotalReservedMemory()/(double)(kb);
		MByte = (double)Profiler.GetTotalReservedMemory()/(double)(mb);
		if(m_sizeByte == eSizeByte.Byte)
			profiler += "\n" + " TotalReservedMemory Byte: 	" 		+ Byte.ToString() + " Byte";
		if(m_sizeByte == eSizeByte.KByte)
			profiler += "\n" + " TotalReservedMemory KByte: 	" 		+ KByte.ToString("0.000") + " KByte"; 
		if(m_sizeByte == eSizeByte.MByte)
			profiler += "\n" + " TotalReservedMemory MByte: 	" 		+ MByte.ToString("0.000")+ " MByte"; 
 
		Byte = (long)Profiler.usedHeapSize;
		KByte = (double)Profiler.usedHeapSize/(double)(kb);
		MByte = (double)Profiler.usedHeapSize/(double)(mb);
		if(m_sizeByte == eSizeByte.Byte)
			profiler += "\n" + " usedHeapSize Byte: 	" 				+ Byte.ToString() + " Byte";
		if(m_sizeByte == eSizeByte.KByte)
			profiler += "\n" + " usedHeapSize KByte: 	" 				+ KByte.ToString("0.000") + " KByte"; 
		if(m_sizeByte == eSizeByte.MByte) 
			profiler += "\n" + " usedHeapSize MByte: 	" 				+ MByte.ToString("0.000")+ "  MByte"; 
		  
 		return profiler;
	}
}
