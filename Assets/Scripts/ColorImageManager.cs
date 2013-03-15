using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Iisu.Data;

public class ColorImageManager : MonoBehaviour {
    public IisuInputProvider IisuInput;
    public HandIisuInput handInput;

	private Texture2D colorTexture;
	private byte [] colorImageRaw;
	private Color32 [] colorImage;
	
	private float timer;
	
	void Awake(){
		timer = 0f;
	}
	// Use this for initialization
	void Start () {
		colorTexture = new Texture2D(640, 480, TextureFormat.ARGB32, false);
		GetComponent<GUITexture>().texture = colorTexture;
		//readFromOutputImage();
	}
	
	// Update is called once per frame
	void Update () {
		//if(timer >= 0.0001){
			timer = 0f;
			generateNewColorTexture();	
			// Setting the color image from the camera to be a GUITexture
			//GetComponent<GUITexture>().texture = colorTexture;
		/*}
		else{
			timer += Time.deltaTime;
		}*/
	}
	
	private void generateNewColorTexture(){
		IImageData cameraColorImage = IisuInput.ColorImage;	
		if(cameraColorImage == null){
			return;
		}
		
		if(colorImage == null || colorImage.Length != cameraColorImage.ImageInfos.BytesRaw / 2){
			colorImage = new Color32[640 * 480];
			colorImageRaw = new byte[cameraColorImage.ImageInfos.BytesRaw];
		}
		
        uint byte_size = (uint)cameraColorImage.ImageInfos.BytesRaw;
        Marshal.Copy(cameraColorImage.Raw, colorImageRaw, 0, (int)byte_size);
        
		for(int i = 0; i < 480; ++i){
			for(int j = 0; j < 640; ++j){
				int pos = i * 640 + j;
				int pos1 = (479 - i) * 640 + j;
				colorImage[pos1].r = colorImageRaw[pos * 4+2];
            	colorImage[pos1].g = colorImageRaw[pos * 4+1];
            	colorImage[pos1].b = colorImageRaw[pos * 4];
            	colorImage[pos1].a = 255;
			}
		}
        /*for(int pos = 0; pos < colorImage.Length; ++ pos){
			colorImage[pos].r = colorImageRaw[pos * 4+2];
            colorImage[pos].g = colorImageRaw[pos * 4+1];
            colorImage[pos].b = colorImageRaw[pos * 4];
            colorImage[pos].a = 255;
		}*/
		
		colorTexture.SetPixels32(colorImage);
        colorTexture.Apply();
	}
	
	private void readFromOutputImage(){
		string [] lines = System.IO.File.ReadAllLines(@".\ColorImage.txt");
		List<int> val = new List<int>();
		
		Color [] image = new Color[640 * 480];
		foreach(string s in lines){
			val.Clear();
			string[] ss = s.Split(' ');
			for(int i = 0; i < ss.Length; ++i){
				val.Add(int.Parse(ss[i]));
			}
			if(val.Count != 6){
				Debug.LogError("Reading error");
			}
			image[val[0] * 640 + val[1]] = new Color(val[2] / 255f, val[3] / 255f, val[4] / 255f, val[5] / 255f);
		}
		colorTexture.SetPixels(image);
		colorTexture.Apply();
		GetComponent<GUITexture>().texture = colorTexture;
	}
	
	public Color32 [] ColorImage{
		get{
			return colorImage;
		}
	}
}
