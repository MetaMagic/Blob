using UnityEngine;
using System.Collections;
using Iisu;
using IisuUnity;

public class HandMeshInfo
{
	public Vector3[] Vertices;
	public Vector3[] Normals;
	public int[] Triangles;
	public float[] Intensities;
}

public class FingerTip{
	public Vector3 [] fingerPos;
	public bool [] fingerStatus;
}

public class HandIisuInput : MonoBehaviour {
	public IisuInputProvider InputProvider;
	public int HandNumber;
	
	private IDataHandle<int> _handDetected;
	
	private IDataHandle<Iisu.Data.Vector3[]> _meshPoints;
	private IDataHandle<Iisu.Data.Vector3[]> _meshNormals;
	private IDataHandle<float[]> _meshIntensities;
	private IDataHandle<Iisu.Data.Vector3i[]> _meshTriangles;
	
	private Iisu.Data.Vector3[] _meshPointsValue;
	private Iisu.Data.Vector3[] _meshNormalsValue;
	private Iisu.Data.Vector3i[] _meshTrianglesValue;
	
	private IDataHandle<Iisu.Data.Vector3> _palmPos;
	private IDataHandle<Iisu.Data.Vector3> _palmNorm;
	
	private IDataHandle<Iisu.Data.Vector3[]> fingerTipPosition;
	private IDataHandle<int[]> fingerTipStatus;
	
	
	void Start()
	{
		_handDetected = InputProvider.Device.RegisterDataHandle<int>("CI.HAND" + HandNumber + ".Status");
		
		_meshPoints = InputProvider.Device.RegisterDataHandle<Iisu.Data.Vector3[]>("CI.HAND" + HandNumber + ".MESH.Points3D");
		_meshNormals = InputProvider.Device.RegisterDataHandle<Iisu.Data.Vector3[]>("CI.HAND" + HandNumber + ".MESH.Normals");
		_meshIntensities = InputProvider.Device.RegisterDataHandle<float[]>("CI.HAND" + HandNumber + ".MESH.Intensities");
		_meshTriangles = InputProvider.Device.RegisterDataHandle<Iisu.Data.Vector3i[]>("CI.HAND" + HandNumber + ".MESH.Triangles");
	
		_palmPos = InputProvider.Device.RegisterDataHandle<Iisu.Data.Vector3>("CI.HAND" + HandNumber + ".PalmPosition3D");
		_palmNorm = InputProvider.Device.RegisterDataHandle<Iisu.Data.Vector3>("CI.HAND" + HandNumber + ".PalmNormal3D");
		
		fingerTipPosition = InputProvider.Device.RegisterDataHandle<Iisu.Data.Vector3[]>("CI.HAND" + HandNumber + ".FingerTipPositions3D");
		fingerTipStatus = InputProvider.Device.RegisterDataHandle<int[]>("CI.HAND" + HandNumber + ".FingerStatus");
	}
	
	public Vector3 GetPalmPos(){
		return new Vector3(-_palmPos.Value.X, _palmPos.Value.Z, _palmPos.Value.Y);
	}
	public Vector3 GetPalmNorm(){
		return new Vector3(-_palmNorm.Value.X, _palmNorm.Value.Z, _palmNorm.Value.Y);
	}
	public FingerTip getFingerTip(){
		FingerTip f = new FingerTip();
		f.fingerPos = new Vector3[5];
		f.fingerStatus = new bool[5];
		int[] fingerStatus = fingerTipStatus.Value;
		for(int i = 0; i < fingerStatus.Length; ++i){
			f.fingerStatus[i] = (fingerStatus[i] >= 1);
			Iisu.Data.Vector3 v = fingerTipPosition.Value[i];
			f.fingerPos[i] = new Vector3(v.X, v.Y, v.Z);
		}
		return f;
	}
	
	public HandMeshInfo GetHandMesh(){
		_meshPointsValue = _meshPoints.Value;
		_meshNormalsValue = _meshNormals.Value;
		_meshTrianglesValue = _meshTriangles.Value;
		
		HandMeshInfo hmi = new HandMeshInfo();
		
		//-----Vertices-----
		Vector3[] vertices = new Vector3[_meshPointsValue.Length];
		for(int i=0; i<vertices.Length; ++i){
			vertices[i] = new Vector3(_meshPointsValue[i].X, _meshPointsValue[i].Y, _meshPointsValue[i].Z);
		}
		hmi.Vertices = vertices;
		
		//-----Normals-----
		Vector3[] normals = new Vector3[_meshNormalsValue.Length];
		for(int i=0; i<normals.Length; ++i){
			normals[i] = new Vector3(_meshNormalsValue[i].X, _meshNormalsValue[i].Y, _meshNormalsValue[i].Z);
		}
		hmi.Normals = normals;
		
		//-----Triangles-----
		int[] triangles = new int[_meshTrianglesValue.Length * 3];
		for(int i=0; i<_meshTrianglesValue.Length; ++i){
			triangles[i*3] = _meshTrianglesValue[i].X;	
			triangles[(i*3) + 1] = _meshTrianglesValue[i].Y;
			triangles[(i*3) + 2] = _meshTrianglesValue[i].Z;
		}
		hmi.Triangles = triangles;
		hmi.Intensities = _meshIntensities.Value;
		return hmi;
	}
	
	public bool Detected
	{
		get
		{
			return _handDetected.Value >= 1;	
		}
	}
	

}

