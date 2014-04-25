﻿using UnityEngine;
using System.Collections;
using System;

namespace ThisProject
{
	public enum PhysicsMaterial { FixedMetal, Metal, Wood, Rubber, Ice }
	public enum Effects { Solid, Ice }

	public class ObjectFactory
	{
		private static Texture2D atlas1, atlas2;
		private static Material atlas1Material, atlas2Material;

		private static int circleSegments = 50;

		static ObjectFactory()
		{
			atlas1 = (Texture2D)Resources.Load("Atlas1");
			atlas2 = (Texture2D)Resources.Load("Atlas2");

			atlas1Material = new Material(Shader.Find("Custom/UnlitTransparent"));
			atlas1Material.mainTexture = atlas1;

			atlas2Material = new Material(Shader.Find("Custom/UnlitTransparent"));
			atlas2Material.mainTexture = atlas2;
		}

		public static void CreateCircle(float radius, PhysicsMaterial physicsMaterial)
		{
			//Vertices
			Vector3[] vertices = new Vector3[circleSegments + 1];
			int[] triangles = new int[circleSegments * 3];
			Vector2[] uvs = new Vector2[circleSegments + 1];

			vertices[0] = new Vector3(0, 0, 0);

			Vector2 uvCenter;
			Material material;
			switch (physicsMaterial)
			{
				case PhysicsMaterial.FixedMetal:
					uvCenter = TextureXYtoUV(677, 1756);
					material = atlas1Material;
					break;
				case PhysicsMaterial.Ice:
					uvCenter = TextureXYtoUV(677, 1254);
					material = atlas1Material;
					break;
				case PhysicsMaterial.Metal:
					uvCenter = TextureXYtoUV(677, 250);
					material = atlas1Material;
					break;
				case PhysicsMaterial.Rubber:
					uvCenter = TextureXYtoUV(677, 250);
					material = atlas2Material;
					break;
				case PhysicsMaterial.Wood:
					uvCenter = TextureXYtoUV(677, 752);
					material = atlas1Material;
					break;
				default:
					uvCenter = TextureXYtoUV(677, 1756);
					material = atlas1Material;
					break;
			}

			uvs[0] = new Vector2(uvCenter.x, uvCenter.y);

			double angle = 2 * Math.PI / circleSegments;

			for (int i = 0; i < circleSegments; i++)
			{
				//Vertices
				vertices[i + 1] = new Vector3((float)(radius * Math.Cos(i * angle)), (float)(radius * Math.Sin(i * angle)), 0);


				//UVs
				uvs[i + 1] = new Vector2(uvCenter.x + radius * 2 * (float)(0.0375f * Math.Cos(i * angle)), uvCenter.y + radius * 2 * (float)(0.0375f * Math.Sin(i * angle)));

				//Triangles
				triangles[i * 3] = 0;

				if (i < circleSegments - 1)
					triangles[i * 3 + 1] = i + 2;
				else
					triangles[i * 3 + 1] = 1;

				triangles[i * 3 + 2] = i + 1;
			}

			Mesh mesh = new Mesh();

			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uvs;

			GameObject obj = new GameObject();
			obj.AddComponent<MeshRenderer>();
			obj.AddComponent<MeshFilter>().mesh = mesh;

			obj.renderer.material = material;

			obj.renderer.sortingLayerName = "Elements";
			obj.renderer.sortingOrder = 1;
		}

		public static void CreateCircleEffect(float radius, Effects effect)
		{
			//Vertices
			Vector3[] vertices = new Vector3[circleSegments * 2];
			int[] triangles = new int[circleSegments * 3 * 2];
			Vector2[] uvs = new Vector2[circleSegments * 2];

			int pixelInnerOffset, pixelOuterOffset;
			Vector2 uv0, uv1, uv2, uv3;

			switch (effect)
			{
				case Effects.Ice:
					pixelInnerOffset = 9;
					pixelOuterOffset = 15;
					uv0 = TextureXYtoUV(70, 2046);
					uv1 = TextureXYtoUV(70, 2022);
					uv2 = TextureXYtoUV(87, 2046);
					uv3 = TextureXYtoUV(87, 2022);
					break;
				case Effects.Solid:
					pixelInnerOffset = 9;
					pixelOuterOffset = 10;
					uv0 = TextureXYtoUV(49, 2044);
					uv1 = TextureXYtoUV(49, 2026);
					uv2 = TextureXYtoUV(66, 2044);
					uv3 = TextureXYtoUV(66, 2026);
					break;
				default:
					pixelInnerOffset = 9;
					pixelOuterOffset = 10;
					uv0 = TextureXYtoUV(49, 2044);
					uv1 = TextureXYtoUV(49, 2026);
					uv2 = TextureXYtoUV(66, 2044);
					uv3 = TextureXYtoUV(66, 2026);
					break;
			}

			float ppu = 10f / 1536;
			float innerRadius = radius - pixelInnerOffset * ppu;
			float outerRadius = radius + pixelOuterOffset * ppu;

			double angle = 2 * Math.PI / circleSegments;

			for (int i = 0; i < circleSegments; i++)
			{
				//Vertices
				vertices[i * 2] = new Vector3((float)(innerRadius * Math.Cos(i * angle)), (float)(innerRadius * Math.Sin(i * angle)), 0);
				vertices[i * 2 + 1] = new Vector3((float)(outerRadius * Math.Cos(i * angle)), (float)(outerRadius * Math.Sin(i * angle)), 0);


				//UVs
				if (i % 2 == 0)
				{
					uvs[i * 2] = uv0;
					uvs[i * 2 + 1] = uv1;
				}
				else
				{
					uvs[i * 2] = uv2;
					uvs[i * 2 + 1] = uv3;
				}

				//Triangles
				triangles[i * 6 + 1] = i * 2 + 1;
				triangles[i * 6 + 2] = i * 2;
				triangles[i * 6 + 5] = i * 2 + 1;

				if (i < circleSegments - 1)
				{
					triangles[i * 6] = i * 2 + 2;
					triangles[i * 6 + 3] = i * 2 + 2;
					triangles[i * 6 + 4] = i * 2 + 3;
				}
				else
				{
					triangles[i * 6] = 0;
					triangles[i * 6 + 3] = 0;
					triangles[i * 6 + 4] = 1;
				}
			}

			Mesh mesh = new Mesh();

			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uvs;

			GameObject obj = new GameObject();
			obj.AddComponent<MeshRenderer>();
			obj.AddComponent<MeshFilter>().mesh = mesh;

			obj.renderer.material = atlas1Material;

			obj.renderer.sortingLayerName = "Elements";
			obj.renderer.sortingOrder = 2;
		}

		public static void CreateTriangleMesh()
		{
			Mesh mesh = new Mesh();

			//Vertices
			Vector3[] vertices = 
		{
			new Vector3(0, 0, 0),
			new Vector3(0.2f, 2, 0),
			new Vector3(-0.2f, 2, 0)
		};

			//Triangles
			int[] triangles = { 0, 2, 1 };

			//UVs
			Vector2[] uvs = 
		{
			new Vector2(1676f / 2048, 313f / 2048),
			new Vector2(1696f / 2048, 474f / 2048),
			new Vector2(1656f / 2048, 474f / 2048)
		};

			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uvs;

			GameObject obj = new GameObject();
			obj.AddComponent<MeshRenderer>();
			obj.AddComponent<MeshFilter>().mesh = mesh;

			obj.renderer.material = atlas1Material;
		}

		public static void CreateTriangleMesh2()
		{
			Mesh mesh = new Mesh();

			//Vertices
			Vector3[] vertices = 
		{
			new Vector3(0, 0, 0),
			new Vector3(0.2f, 0.2f, 0),
			new Vector3(-0.2f, 0.2f, 0)
		};

			//Triangles
			int[] triangles = { 0, 2, 1 };

			//UVs
			Vector2[] uvs = 
		{
			new Vector2(1676f / 2048, 454f / 2048),
			new Vector2(1696f / 2048, 474f / 2048),
			new Vector2(1656f / 2048, 474f / 2048)
		};

			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uvs;

			GameObject obj = new GameObject();
			obj.AddComponent<MeshRenderer>();
			obj.AddComponent<MeshFilter>().mesh = mesh;

			obj.renderer.material = atlas1Material;

			obj.transform.localPosition = new Vector3(2, 0, 0);
		}

		private static Vector2 TextureXYtoUV(int x, int y)
		{
			return new Vector2((float)x / 2047, (float)(2047 - y) / 2047);
		}
	}
}