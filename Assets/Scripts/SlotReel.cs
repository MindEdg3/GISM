using UnityEngine;
using System.Collections.Generic;

public class SlotReel : MonoBehaviour
{
	public float symbolSideSize = 0.125f;
	public Vector2 atlasSymbolsOffset = Vector2.zero;
	public Vector2 atlasSymbolsSize = Vector2.one;
	public int emptySymbols;
	public float maxRotationSpeed = 500f;
	public float slowdownSpeed = 50f;
	private ReelFaceCoords[] reelsUVs;
	private float currentSpeed;
	public float audioSlowDown;
	private MeshFilter mf;
	private int stepsToRoll;
	private int lastAddedIndex;
	private AudioSource myAudio;
	

	// Use this for initialization
	void Start ()
	{
		mf = transform.Find ("mesh").GetComponent<MeshFilter> ();
		myAudio = audio;
		reelsUVs = GetFaces ();
		RandomizeReelUVs ();
		SwitchReelFaces (0);
	}
	
	void Update ()
	{
		if (stepsToRoll > 0) {
			float reelRotation = transform.localRotation.eulerAngles.x - currentSpeed * Time.deltaTime;
			
			if (reelRotation < 0) {
				reelRotation = (reelRotation % 36) + 36;
				currentSpeed -= slowdownSpeed;
				myAudio.pitch -= audioSlowDown;
				
				SwitchReelFaces (lastAddedIndex);
				
				lastAddedIndex++;
				if (lastAddedIndex >= reelsUVs.Length) {
					lastAddedIndex = 0;
				}
				
				stepsToRoll--;
				if (stepsToRoll == 0) {
					myAudio.Stop ();
				}
			}
			transform.localRotation = Quaternion.Euler (reelRotation, 0f, 0f);
		}
	}
	
	public void Roll (int steps)
	{
		currentSpeed = slowdownSpeed * steps;
		audioSlowDown = 2f / steps;
		myAudio.pitch = 2.5f;
		myAudio.Play ();
		stepsToRoll = steps;
	}
	
	private ReelFaceCoords[] GetFaces ()
	{
		int atlasSymbolsHorizontally = (int)(atlasSymbolsSize.x / symbolSideSize);
		int atlasSymbolsVertically = (int)(atlasSymbolsSize.y / symbolSideSize);
		
		ReelFaceCoords[] ret = new ReelFaceCoords[atlasSymbolsHorizontally * atlasSymbolsVertically - emptySymbols];
		
		for (int i = 0; i <atlasSymbolsVertically; i++) {
			for (int j = 0; j <  atlasSymbolsHorizontally; j++) {
				int currentIndex = i * atlasSymbolsHorizontally + j;
				if (currentIndex >= ret.Length) {
					break;
				}
				
				ReelFaceCoords rfc = new ReelFaceCoords ();
				
				rfc.v0 = new Vector2 (atlasSymbolsOffset.x + j * symbolSideSize, 1 - (atlasSymbolsOffset.y + (i + 1) * symbolSideSize));
				rfc.v1 = new Vector2 (atlasSymbolsOffset.x + (j + 1) * symbolSideSize, 1 - (atlasSymbolsOffset.y + i * symbolSideSize));
				rfc.v2 = new Vector2 (atlasSymbolsOffset.x + (j + 1) * symbolSideSize, 1 - (atlasSymbolsOffset.y + (i + 1) * symbolSideSize));
				rfc.v3 = new Vector2 (atlasSymbolsOffset.x + j * symbolSideSize, 1 - (atlasSymbolsOffset.y + i * symbolSideSize));
				
				ret [currentIndex] = rfc;
			}
		}
		
		return ret;
	}

	public void RandomizeReelUVs ()
	{
		for (int i = 0; i < reelsUVs.Length; i++) {
			int randomIndex = Random.Range (0, reelsUVs.Length);
			
			ReelFaceCoords tempRFC = reelsUVs [i];
			reelsUVs [i] = reelsUVs [randomIndex];
			reelsUVs [randomIndex] = tempRFC;
		}
	}
	
	private void SwitchReelFaces (int firstFace)
	{
		Mesh mesh = mf.mesh;
		Vector2[] uvs = mesh.uv;
		int currentFace = firstFace;
		
		for (int i = 0; i < 16; i += 4) {
			uvs [i + 0] = reelsUVs [currentFace].v0;
			uvs [i + 1] = reelsUVs [currentFace].v1;
			uvs [i + 2] = reelsUVs [currentFace].v2;
			uvs [i + 3] = reelsUVs [currentFace].v3;
			
			currentFace ++;
			if (currentFace >= reelsUVs.Length) {
				currentFace = 0;
			}
		}
		mesh.uv = uvs;
		mf.mesh = mesh;
	}
	
	/*
	private Mesh GetReelMesh ()
	{
	int numberOfSymbols = 10;
	
		Mesh newMesh = new Mesh ();
		newMesh.name = "Slot reel Mesh";
		
		// Vertices
		Vector3[] vertices = new Vector3 [4 * 4];
		
		float rot = 0;
		for (int i = 0; i < vertices.Length; i+=4) {
			vertices [i] = Quaternion.Euler (rot, 0f, 0f) * new Vector3 (-1f, 1f, 0f);
			vertices [i + 1] = Quaternion.Euler (rot, 0f, 0f) * new Vector3 (1f, 1f, 0f);
			rot = ((i * 0.25f) + 1) * 360 / numberOfSymbols;
			vertices [i + 2] = Quaternion.Euler (rot, 0f, 0f) * new Vector3 (-1f, 1f, 0f);
			vertices [i + 3] = Quaternion.Euler (rot, 0f, 0f) * new Vector3 (1f, 1f, 0f);
		}
		
		newMesh.vertices = vertices;
		
		
		// UVs
		Queue<Vector2> tiles = new Queue<Vector2> ();
		
		for (int i = 0; i < atlasSymbolsHorizontally; i++) {
			for (int j = 0; j < atlasSymbolsHorizontally; j++) {
				tiles.Enqueue (new Vector2 (i * 1f / atlasSymbolsHorizontally, j * 1f / atlasSymbolsVertically));
				tiles.Enqueue (new Vector2 ((i + 1) * 1f / atlasSymbolsHorizontally, j * 1f / atlasSymbolsVertically));
				tiles.Enqueue (new Vector2 (i * 1f / atlasSymbolsHorizontally, (j + 1) * 1f / atlasSymbolsVertically));
				tiles.Enqueue (new Vector2 ((i + 1) * 1f / atlasSymbolsHorizontally, (j + 1) * 1f / atlasSymbolsVertically));
			}
		}
		
		Vector2[] uvs = new Vector2[4 * 4];
		
		for (int i = 0; i < uvs.Length; i += 4) {
			uvs [i + 0] = tiles.Dequeue ();
			uvs [i + 1] = tiles.Dequeue ();
			uvs [i + 2] = tiles.Dequeue ();
			uvs [i + 3] = tiles.Dequeue ();
		}
			
		newMesh.uv = uvs;
		
		
		// Normals
		Vector3[] normals = new Vector3 [4 * 4];
		
		for (int i = 0; i < normals.Length; i++) {
			normals [i] = Vector3.up;
		}
		
		newMesh.normals = normals;
		
		
		// Triangles
		int[] triangles = new int[4 * 6];
		
		for (int i = 0; i < triangles.Length; i+=6) {
			int firstVertex = 4 * i / 6;
			
			triangles [i + 0] = 2 + firstVertex;
			triangles [i + 1] = 1 + firstVertex;
			triangles [i + 2] = 0 + firstVertex;
			triangles [i + 3] = 3 + firstVertex;
			triangles [i + 4] = 1 + firstVertex;
			triangles [i + 5] = 2 + firstVertex;
		}
		
		newMesh.triangles = triangles;
		
		AssetDatabase.CreateAsset (newMesh, "Assets/MyMaterial.asset");
		AssetDatabase.SaveAssets ();
		
		return newMesh;
	}
	*/
}

[System.Serializable]
public class ReelFaceCoords
{
	public Vector2 v0;
	public Vector2 v1;
	public Vector2 v2;
	public Vector2 v3;
}