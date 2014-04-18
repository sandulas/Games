using UnityEngine;
using System.Collections;
using System;

public class ObjectFactory
{
	private static Texture2D atlas1;
	private static Material material;

	static ObjectFactory()
	{
		atlas1 = (Texture2D)Resources.Load("Atlas1");
		material = new Material(Shader.Find("Custom/UnlitTransparent"));
		material.mainTexture = atlas1;
	}

	public static void CreateDiskMesh(int segments)
	{
		Mesh mesh = new Mesh();

		//Vertices
		Vector3[] vertices = new Vector3[segments + 1];		
		int[] triangles = new int[segments * 3];
		Vector2[] uvs = new Vector2[segments + 1];

		vertices[0] = new Vector3(0, 0, 0);

		float uvCenterX = 677f / 2048;
		//float uvCenterY = 290f / 2048;
		float uvCenterY = 1797f / 2048;

		Debug.Log("uv center: " + uvCenterX + ", " + uvCenterY);

		uvs[0] = new Vector2(uvCenterX, uvCenterY);

		double angle = 2 * Math.PI / segments;

		for (int i = 0; i < segments; i++)
		{
			//Vertices
			vertices[i + 1] = new Vector3((float)(0.5f * Math.Cos(i * angle)), (float)(0.5f * Math.Sin(i * angle)), 0);


			//UVs
			uvs[i + 1] = new Vector2(uvCenterX + (float)(0.0375f * Math.Cos(i * angle)), uvCenterY + (float)(0.0375f * Math.Sin(i * angle)));

			//Triangles
			triangles[i * 3] = 0;
	
			if (i < segments - 1)
				triangles[i * 3 + 1] = i + 2;
			else
				triangles[i * 3 + 1] = 1;

			triangles[i * 3 + 2] = i + 1;

			//UVs
			//if (i % 2 == 0) uvs[i + 1] = new Vector2(0.05f, 0.1f);
			//else uvs[i + 1] = new Vector2(0.1f, 0.1f);
		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;

		GameObject obj = new GameObject();
		obj.AddComponent<MeshRenderer>();
		obj.AddComponent<MeshFilter>().mesh = mesh;

		obj.renderer.material = material;
	}
}
