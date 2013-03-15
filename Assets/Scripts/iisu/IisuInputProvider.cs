/***************************************************************************************/
//
//  SoftKinetic iisu SDK code samples 
//
//  Project Name      : skeletonBubbleManSample
//  Revision          : 1.0
//  Description       : Tutorial on how to use the Skeleton and Bubbleman. 
//						It is recommended to use this sample to get started, 
//						as it covers the most common uses of iisu: skeleton, 
//						bubbleman and displaying the depthmap + usermask.
//
// DISCLAIMER
// All rights reserved to SOFTKINETIC INTERNATIONAL SA/NV (a company incorporated
// and existing under the laws of Belgium, with its principal place of business at
// Boulevard de la Plainelaan 15, 1050 Brussels (Belgium), registered with the Crossroads
// bank for enterprises under company number 0811 784 189 - “Softkinetic”)
//
// For any question about terms and conditions, please contact: info@softkinetic.com
// Copyright (c) 2007-2011 SoftKinetic SA/NV
//
/****************************************************************************************/

using UnityEngine;
using System.Collections;
using Iisu;
using IisuUnity;
using System;
using System.Collections.Generic;


/// <summary>
/// Takes care of the communication between iisu and the Unity application by providing
/// the necessary data from iisu
/// </summary>
/// 

public class IisuInputProvider : MonoBehaviour {
	
	//the IisuUnityBehaviour object handles the iisu device, including its update thread, and disposing.
	private IisuUnityBehaviour _iisuUnity;
	
	private IDataHandle<Iisu.Data.IImageData> _depthImage;
	private IDataHandle<int> _hand1HandID;
	private IDataHandle<int> _hand2HandID;
	
	private IDataHandle<Iisu.Data.IImageData> _labelImage;
	
	private IDataHandle<Iisu.Data.IImageData> _colorImage;
	private IDataHandle<Iisu.Data.IImageData> _depthToColor;
	//private IParameterHandle<Iisu.Data.Matrix4> _cameraToWorld;
	
	void Awake()
	{		
		//this has to be done first. Inside the IisuUnityBehaviour object, iisu is initialized, and the update thread for the current device (camera, movie) is started
		_iisuUnity = GetComponent<IisuUnityBehaviour>();
		_iisuUnity.Initialize();
		
		_depthImage = _iisuUnity.Device.RegisterDataHandle<Iisu.Data.IImageData>("SOURCE.CAMERA.DEPTH.Image");
		
		_hand1HandID = _iisuUnity.Device.RegisterDataHandle<int>("CI.HAND1.Label");
		_hand2HandID = _iisuUnity.Device.RegisterDataHandle<int>("CI.HAND2.Label");
		
		_labelImage = _iisuUnity.Device.RegisterDataHandle<Iisu.Data.IImageData>("CI.SceneLabelImage");
		
		_colorImage = _iisuUnity.Device.RegisterDataHandle<Iisu.Data.IImageData>("SOURCE.CAMERA.COLOR.Image");
		_depthToColor = _iisuUnity.Device.RegisterDataHandle<Iisu.Data.IImageData>("SOURCE.CAMERA.COLOR.REGISTRATION.UV.Image");
	}
	
	public Iisu.Data.IImageData depthToColor{
		get{
			return _depthToColor.Value;	
		}
	}
	
	public Iisu.Data.IImageData ColorImage
	{
		get
		{
			return _colorImage.Value;
		}
	}
	
	public IDevice Device
	{
		get
		{
			return _iisuUnity.Device;	
		}
	}
	
	public Iisu.Data.IImageData DepthMap
	{
		get
		{
			return _depthImage.Value;
		}
	}
	
	public int Hand1Label
	{
		get
		{
			return _hand1HandID.Value;
		}
	}
	
	public int Hand2Label
	{
		get
		{
			return _hand2HandID.Value;
		}
	}
	
	public Iisu.Data.IImageData LabelImage
	{
		get
		{
			return _labelImage.Value;	
		}
	}
		
}
