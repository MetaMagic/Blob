using UnityEngine;
using System.Collections;

// Using this class to initialize the maincamera's projection matrix
// The camera in Unity needs to be rotated by 90 degrees around X, and positioned at(0,0,0)
public class MainCamera : MonoBehaviour {
    private float [] colorFocalLength;
    private float [] colorPrincipalPoint;
    
	void Awake() {
		colorFocalLength = new float[2];
		colorFocalLength[0] = 583.078979f;
		colorFocalLength[1] = 596.203003f;
		colorPrincipalPoint = new float[2];
		colorPrincipalPoint[0] = 320f;
		colorPrincipalPoint[1] = 240f;
		Matrix4x4 colorIntrinsc = new Matrix4x4();
		colorIntrinsc = Matrix4x4.identity;
		colorIntrinsc.m00 = colorFocalLength[0];
		colorIntrinsc.m02 = colorPrincipalPoint[0];
		colorIntrinsc.m11 = colorFocalLength[1];
		colorIntrinsc.m12 = colorPrincipalPoint[1];
		
		// Geting the main camera
		Camera camera = GetComponent<Camera>();
		if(camera == null){
			Debug.LogError("camrea is null");
		}
		Matrix4x4 projMat = new Matrix4x4();
		projMat = Matrix4x4.zero;
		
		float width = 640f;//Screen.width;
		float height =  480f;//Screen.height;
		projMat.m00 = 2f * colorIntrinsc.m00 / width;
		projMat.m02 = (width - 2f * colorIntrinsc.m02) / width;
		
		projMat.m11 = 2f * colorIntrinsc.m11 / height;
		projMat.m12 = (height - 2f * colorIntrinsc.m12) / height;
		
		projMat.m22 = (-camera.farClipPlane - camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane);
		projMat.m23 = -2f * camera.farClipPlane * camera.nearClipPlane / (camera.farClipPlane - camera.nearClipPlane);
		
		projMat.m32 = -1f;
		
		camera.projectionMatrix = projMat;
	}
}
