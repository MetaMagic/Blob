using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Iisu.Data;

public struct RgbImage
{
	public IntPtr data;
	public int width, height;
};

public class ColorImageManager : MonoBehaviour {
    public IisuInputProvider IisuInput;
    public Hand hand1;
	public Hand hand2;

	private Texture2D colorTexture;
	private byte [] colorImageRaw;// A copy of SDK's raw data (BGR)
	private byte [] colorData; // Data with a correct order(RGBA)
	private Color32 [] colorImage;
	
	
	// We'll also pass native pointer to a texture in Unity.
	// The plugin will fill texture data from native code.
	[DllImport ("RenderingPlugin")]
	private static extern void SetTextureFromUnity (System.IntPtr textureId);
	
	[DllImport ("RenderingPlugin")]
	private static extern void SetTextureIdFromUnity (int textureId);
	
	[DllImport ("RenderingPlugin")]
	private static extern void SetColorDataFromUnity(ref RgbImage rgbImage);
	
	GCHandle rgbDataHandle;
	private RgbImage rgbImage;
	
	void Awake(){
		colorData = new byte[640 * 480 * 4];
		rgbImage = new RgbImage();
		rgbImage.width = 640;
		rgbImage.height = 480;
		
		rgbDataHandle = GCHandle.Alloc(colorData, GCHandleType.Pinned);
		rgbImage.data = rgbDataHandle.AddrOfPinnedObject();
		rgbDataHandle.Free();
	}
	
	// Use this for initialization
	IEnumerator Start (){
		CreateTextureAndPassToPlugin();
		yield return StartCoroutine("CallPluginAtEndOfFrames");
	}
	
	private void CreateTextureAndPassToPlugin()
	{
		// Create a texture
		colorTexture = new Texture2D(640, 480, TextureFormat.RGBA32,false);
		// Set point filtering just so we can see the pixels clearly
		colorTexture.filterMode = FilterMode.Point;
		// Call Apply() so it's actually uploaded to the GPU
		colorTexture.Apply();

		// Set texture onto our matrial
		GetComponent<GUITexture>().texture = colorTexture;

		// Pass texture pointer to the plugin
		//SetTextureFromUnity(colorTexture.GetNativeTexturePtr());
		SetTextureIdFromUnity(colorTexture.GetNativeTextureID()+1);
	}
	
	void Update(){
		if(Input.GetKey(KeyCode.Escape)){
			Debug.Log("esc");
			StopCoroutine("CallPluginAtEndOfFrames");
        	Application.Quit();
    	}
	}
	
	private IEnumerator CallPluginAtEndOfFrames ()
	{
		while (true) {
			// Wait until all frame rendering is done
			yield return new WaitForEndOfFrame();
			
			// Drawing hand meshes or fingers
			hand1.draw();
			hand2.draw();
			
			prepareColorRaw();
			// Set time for the plugin
			SetColorDataFromUnity (ref rgbImage);
			
			// Issue a plugin event with arbitrary integer identifier.
			// The plugin can distinguish between different
			// things it needs to do based on this ID.
			// For our simple plugin, it does not matter which ID we pass here.
			GL.IssuePluginEvent (1);
		}
	}
	
	private void prepareColorRaw(){
		if(IisuInput == null || IisuInput.ColorImage == null){
			return;
		}
		IImageData cameraColorImage = IisuInput.ColorImage;	
        rgbImage.data = cameraColorImage.Raw;
	}
}
