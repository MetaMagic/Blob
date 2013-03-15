using UnityEngine;
using System.Collections;
using System.IO;

public class DebugOutput : MonoBehaviour {
	public Hand hand;
	public ColorImageManager colorImageManager;
	public DepthImage depthImageManager;

	// Use this for initialization
	void Start () {
		if(hand == null){
			Debug.LogError("Hand is null.");
		}
		if(colorImageManager == null){
			Debug.LogError("colorImageManager is null.");
		}
		if(depthImageManager == null){
			Debug.LogError("depthImage is null.");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("space")){
			outputToFile();
		}
	}
	
	private void outputToFile(){
		exportColorImageAsTxt();
		exportColorImageAsPNG();
		exportDepthImageAsTxt();
		exportDepthImageAsPNG();
		exportHandMesh();
	}
	
	private void exportDepthImageAsTxt(){
		float [] depthImage = depthImageManager.DepthData;	
		using(System.IO.StreamWriter depthImagefile = new System.IO.StreamWriter(@".\DepthImage.txt"))
		{
			for(int y = 0; y < 240; ++y){
				for(int x = 0; x < 320; ++x){
					int index = y * 320 + x;
					string s = (y.ToString() + " " + x.ToString()+ " " + depthImage[index].ToString());
					depthImagefile.WriteLine(s);
				}
			}
		}	
	}
	
	private void exportDepthImageAsPNG(){
		Color [] depthImage = depthImageManager.DepthColorData;	
		Texture2D texture = new Texture2D(320,240);
		texture.SetPixels(depthImage);
		texture.Apply();
		byte[] bytes = texture.EncodeToPNG();
		File.WriteAllBytes(@".\DepthImage.png", bytes);
	}
	
	private void exportColorImageAsPNG(){
		Color32 [] colorImage = colorImageManager.ColorImage;	
		Texture2D texture = new Texture2D(640,480);
		texture.SetPixels32(colorImage);
		texture.Apply();
		byte[] bytes = texture.EncodeToPNG();
		File.WriteAllBytes(@".\ColorImage.png", bytes);
	}
	
	private void exportColorImageAsTxt(){
		Color32 [] colorImage = colorImageManager.ColorImage;	
		using(System.IO.StreamWriter colorImagefile = new System.IO.StreamWriter(@".\ColorImage.txt"))
		{
			for(int y = 0; y < 480; ++y){
				for(int x = 0; x < 640; ++x){
					int index = y * 640 + x;
					string s = (y.ToString() + " " + x.ToString()+ " " + 
						colorImage[index].r.ToString() + " " + colorImage[index].g.ToString() + " " +
						colorImage[index].b.ToString() + " " + colorImage[index].a.ToString());
					colorImagefile.WriteLine(s);
				}
			}
		}
	}
	
	private void exportHandMesh(){
		Mesh handMesh = hand.HandMesh;
		Matrix4x4 depthToColorRigid = new Matrix4x4();
		
		depthToColorRigid.m00 = 0.999871f;
		depthToColorRigid.m01 = -0.001319f;
		depthToColorRigid.m02 = -0.0159946f;
		depthToColorRigid.m03 = 0.024492f;
		
		depthToColorRigid.m10 = 0.00120216f;
		depthToColorRigid.m11 = -0.999973f;
		depthToColorRigid.m12 = -0.00728971f;
		depthToColorRigid.m13 = -0.000508f;
		
		depthToColorRigid.m20 = 0.0160038f;
		depthToColorRigid.m21 = 0.00726954f;
		depthToColorRigid.m22 = 0.999846f;
		depthToColorRigid.m23 = -0.000863f;
		depthToColorRigid.SetRow(3, Vector4.zero);
		depthToColorRigid.m33 = 1f;
		using(System.IO.StreamWriter meshFile = new System.IO.StreamWriter(@".\HandMesh.obj"))
		{
			for(int i = 0; i < handMesh.vertices.Length; ++i){
				Vector3 vertex = handMesh.vertices[i];
				string v = "v " + vertex.x.ToString() + " " + vertex.y.ToString()
					+ " " + vertex.z.ToString();
				meshFile.WriteLine(v);
			}
			for(int i = 0; i < handMesh.normals.Length; ++i){
				string n = "vn " + handMesh.normals[i].x.ToString() + " " + handMesh.normals[i].y.ToString()
					+ " " + handMesh.normals[i].z.ToString();
				meshFile.WriteLine(n);
			}
			for(int i = 0; i < handMesh.triangles.Length; i += 3){
				string f = "f " + (handMesh.triangles[i] + 1).ToString() + " " + (handMesh.triangles[i + 1] + 1).ToString()
					+ " " + (handMesh.triangles[i+2] + 1).ToString();
				meshFile.WriteLine(f);	
			}
		}
	}
}
