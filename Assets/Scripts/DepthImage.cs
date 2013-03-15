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
using Iisu.Data;
using System.Runtime.InteropServices;

public class DepthImage : MonoBehaviour
{
    public IisuInputProvider IisuInput;
    public Texture2D depthTexture;
	public float NormalizedXCoordinate;
	public float NormalizedYCoordinate;
	public float NormalizedWidth;
	
	private float _heightWidthRatio;
	private float _timer;
	
	private byte [] depthImageRaw;
	private Color [] depthImage;
	
	private float [] depthData;
	
    void Awake(){
		_timer = 0;
		_heightWidthRatio = 120f/160f;
		depthTexture = new Texture2D(320, 240, TextureFormat.ARGB32, false);
		depthData = new float[320 * 240];
    }
	
	/// <summary>
	/// We get the depth image from iisu, which is in a 16bit grey image format 
	/// The image is converted by the ImageConvertor class to a Unity image, and then applied to the 2D GUI texture
	/// </summary>	
    void Update(){
		if(_timer >= 0.0333f){
			_timer = 0;
			//_imageConvertor.generateHandMask(IisuInput.DepthMap, IisuInput.LabelImage, ref DepthMap, IisuInput.Hand1Label, IisuInput.Hand2Label);
			generateNewDepthTexture();
		}
		else{
			_timer += Time.deltaTime;
		}
    }
	
	void OnGUI()
	{
		if(depthTexture != null)
		{
			GUI.DrawTexture(new Rect((Screen.width) * (NormalizedXCoordinate + NormalizedWidth), 
			                      	 Screen.height * NormalizedYCoordinate + Screen.width * NormalizedWidth * _heightWidthRatio, 
			                         Screen.width * NormalizedWidth,
			                         Screen.width * NormalizedWidth * _heightWidthRatio), depthTexture);
		}
	}
	
	private void generateNewDepthTexture(){
		IImageData cameraDepthImage = IisuInput.DepthMap;	
		
		if(depthImage == null || depthImage.Length != cameraDepthImage.ImageInfos.BytesRaw / 2){
			depthImage = new Color[320 * 240];
			depthImageRaw = new byte[cameraDepthImage.ImageInfos.BytesRaw];
		}
		
        uint byte_size = (uint)cameraDepthImage.ImageInfos.BytesRaw;
        Marshal.Copy(cameraDepthImage.Raw, depthImageRaw, 0, (int)byte_size);
        
		for(int y = 0; y < 240; ++y){
			for(int x = 0; x < 320; ++x){
				int pos = y * 320+ x;
				int pos1 = (239 - y) * 320 + x;
				ushort value = (ushort)(depthImageRaw[pos * 2] + (depthImageRaw[pos * 2 + 1] << 8));	
				depthData[pos1] = value;
				value = (ushort)(value * 255 / (1000f));
            	if (value > 255) 
					value = 255;
				depthImage[pos1] = new Color(value / 255f, value / 255f, value / 255f, 1);
			}
		}
		depthTexture.SetPixels(depthImage);
        depthTexture.Apply();
	}
	
	public Color [] DepthColorData{
		get{
			return depthImage;
		}
	}
	
	public float[] DepthData{
		get{
			return depthData;
		}
	}
}
