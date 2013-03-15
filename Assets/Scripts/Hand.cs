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
	
	private Mesh _mesh;
	private Matrix4x4 depthToColorRigid;
	private Transform [] fingers;
    
	void Awake(){
		_mesh = new Mesh();
		
		depthToColorRigid = new Matrix4x4();
		
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
		
		fingers = new Transform[5];
		for(int i = 0; i < 5; ++i){
			fingers[i] = ((GameObject)Instantiate(fingerPrefab)).transform;
			fingers[i].renderer.material = fingerMat;
		}
		//readMeshFromFile();
	}
	
	void Update () {
		if(HandInput.Detected){
			FingerTip f = HandInput.getFingerTip();
			for(int i = 0; i < 5; ++i){
				if(f.fingerStatus[i]){
					Vector3 v = f.fingerPos[i];
					v = depthToColorRigid.MultiplyPoint3x4(v);
					fingers[i].position = new Vector3(v.x, v.y, v.z);
				}
			}
			drawMesh();
		}
		else{
			// If the hand cannot be detected, setting fingers' position to the origin
			for(int i = 0; i < fingers.Length; ++i){
				fingers[i].position = Vector3.zero;
			}
		}
		//Graphics.DrawMesh(_mesh, depthToColorRigid, HandMaterial, 1);
	}
	
	public Mesh HandMesh{
		get{
			return _mesh;
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
		Graphics.DrawMesh(_mesh, depthToColorRigid, HandMaterial, 1);
	}
	
	private void readMeshFromFile(){
		_mesh.Clear();
		string [] lines = System.IO.File.ReadAllLines(@".\HandMesh.obj");
		List<float> val = new List<float>();
		List<Vector3> vertex = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<int> triangles = new List<int>();
		foreach(string line in lines){
			val.Clear();
			string[] ss = line.Split(' ');
			if(ss[0] == "v"){
				Vector3 v = new Vector3(float.Parse(ss[1]), float.Parse(ss[2]), float.Parse(ss[3]));
				vertex.Add(v);
			}
			else if(ss[0] == "vn"){
				Vector3 vn = new Vector3(float.Parse(ss[1]), float.Parse(ss[2]), float.Parse(ss[3]));
				normals.Add(vn);
			}
			else if(ss[0] == "f"){
				int a, b, c;
				a = int.Parse(ss[1]) - 1;
				b = int.Parse(ss[2]) - 1;
				c = int.Parse(ss[3]) - 1;
				triangles.Add(a);
				triangles.Add(b);
				triangles.Add(c);
			}
			
		}
		_mesh.vertices = vertex.ToArray();
		_mesh.normals = normals.ToArray();
		_mesh.triangles = triangles.ToArray();
	}
}
