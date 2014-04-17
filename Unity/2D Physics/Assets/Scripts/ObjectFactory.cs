using UnityEngine;
using System.Collections;

public class ObjectFactory
{
	private static Texture2D atlas1;
	private static Material dynamicMaterial;

	static ObjectFactory()
	{
		atlas1 = (Texture2D)Resources.Load("Atlas1");
		dynamicMaterial = new Material(Shader.Find("Unlit/Transparent"));
		dynamicMaterial.mainTexture = atlas1;
	}

	public static void CreateDiskMesh(int segments, Material material)
	{
		Debug.Log("Create Disk Mesh");

		Mesh mesh = new Mesh();

		Vector3[] vertices =
		{
			new Vector3(-2, -2, 0),
			new Vector3(0, 2, 0),
			new Vector3(2, -2, 0)

		};

		int[] triangles = { 0, 1, 2 };
		Vector2[] uvs = 
		{
			new Vector2(0, 0.5f),
			new Vector2(0.25f, 1),
			new Vector2(0.5f, 0.5f),
		};

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;

		GameObject obj = new GameObject();
		obj.AddComponent<MeshRenderer>();
		obj.AddComponent<MeshFilter>().mesh = mesh;

		obj.renderer.material = dynamicMaterial;
	}
}
