using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This class asks Iisu SDK for hand data.
// 3D positions (Fingers, HandMesh)are all in the depth camera's coordinate system,
// and they need to transformed into the color camera's coordinate system using depthToColorRigid
public class Hand : MonoBehaviour {
	public HandIisuInput HandInput;
	public Color HandColor;
	public GameObject fingerPrefab;
	public Material fingerMat;
	public Material HandMaterial;
	public GameObject meshPrefab;
	public static Matrix4x4 depthToColorRigid;
	
	private Mesh _mesh;
	private Transform [] fingers;
	private Transform handMesh;
    
	void Awake(){
		depthToColorRigid = new Matrix4x4();
		
		depthToColorRigid.m00 = 0.999871f;
		//depthToColorRigid.m00 = 1.2f;
		depthToColorRigid.m01 = -0.001319f;
		depthToColorRigid.m02 = -0.0159946f;
		depthToColorRigid.m03 = 0.024492f;
		
		depthToColorRigid.m10 = 0.00120216f;
		depthToColorRigid.m11 = -0.999973f;
		//depthToColorRigid.m11 = -1.2f;
		depthToColorRigid.m12 = -0.00728971f;
		depthToColorRigid.m13 = -0.000508f;
		
		depthToColorRigid.m20 = 0.0160038f;
		depthToColorRigid.m21 = 0.00726954f;
		depthToColorRigid.m22 = 0.999846f;
		//depthToColorRigid.m22 = 1.2f;
		depthToColorRigid.m23 = -0.000863f;
		depthToColorRigid.SetRow(3, Vector4.zero);
		depthToColorRigid.m33 = 1f;
		
		_mesh = new Mesh();
		handMesh = ((GameObject)Instantiate(meshPrefab)).transform;
		handMesh.renderer.enabled = false;
		handMesh.GetComponent<Renderer>().material = HandMaterial;

		
		fingers = new Transform[5];
		for(int i = 0; i < 5; ++i){
			fingers[i] = ((GameObject)Instantiate(fingerPrefab)).transform;
			fingers[i].position = new Vector3(10, 0, 0);
			fingers[i].renderer.material = fingerMat;
			fingers[i].renderer.enabled = false;
		}
	}
	
	/*void Update () {
		if(HandInput.Detected){
			FingerTip f = HandInput.getFingerTip();
			for(int i = 0; i < 5; ++i){
				if(f.fingerStatus[i]){
					fingers[i].renderer.enabled = true;
					Vector3 v = f.fingerPos[i];
					v = depthToColorRigid.MultiplyPoint3x4(v);
					fingers[i].position = new Vector3(v.x, v.y, v.z);
				}
				else{
					fingers[i].renderer.enabled = false;
				}
			}
			drawMesh();
		}
		else{
			// If the hand cannot be detected, setting fingers' position to the origin
			for(int i = 0; i < fingers.Length; ++i){
				fingers[i].renderer.enabled = false;
			}
		}
	}*/
	
	public void draw(){
		if(HandInput.Detected){
			FingerTip f = HandInput.getFingerTip();
			for(int i = 0; i < 5; ++i){
				if(f.fingerStatus[i]){
					//fingers[i].renderer.enabled = true;
					Vector3 v = f.fingerPos[i];
					v = depthToColorRigid.MultiplyPoint3x4(v);
					fingers[i].position = new Vector3(v.x, v.y, v.z);
				}
				else{
					//fingers[i].renderer.enabled = false;
					fingers[i].position = new Vector3(10, 0, 0);
				}
			}
			handMesh.renderer.enabled = true;
			drawMesh();
		}
		else{
			// If the hand cannot be detected, setting fingers' position to the origin
			for(int i = 0; i < fingers.Length; ++i){
				//fingers[i].renderer.enabled = false;
				fingers[i].position = new Vector3(10, 0, 0);
			}
			handMesh.renderer.enabled = false;
		}
	}

	private void drawMesh(){
		HandMeshInfo hmi = HandInput.GetHandMesh();
		_mesh.Clear();
		if(hmi.Vertices.Length == hmi.Normals.Length){
			_mesh.vertices = hmi.Vertices;
			_mesh.triangles = hmi.Triangles;
			_mesh.normals = hmi.Normals;
		}
		handMesh.GetComponent<MeshFilter>().mesh = _mesh;
		//Graphics.DrawMesh(_mesh, depthToColorRigid, HandMaterial, 1);
	}
	
}
