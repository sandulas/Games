using UnityEngine;
using System.Collections;
using System;

namespace ThisProject
{
	public enum ItemShape { Rectangle, Circle, Triangle }
	public enum ItemMaterial { FixedMetal, Metal, Wood, Rubber, Ice }
	public class IntVector2
	{
		public int x, y;

		public IntVector2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	public class ItemFactory
	{
		enum ItemEffect { Solid, Ice }

		static Texture2D atlas1, atlas2;
		static Material atlas1Material, atlas2Material;
		static float PixelsPerUnit = 1536f / 10;
		static int circleSegments = 50;
		
		static int currentLayerPair;

		static ItemFactory()
		{
			atlas1 = (Texture2D)Resources.Load("Atlas1");
			atlas2 = (Texture2D)Resources.Load("Atlas2");

			atlas1Material = new Material(Shader.Find("Custom/UnlitTransparent"));
			atlas1Material.mainTexture = atlas1;

			atlas2Material = new Material(Shader.Find("Custom/UnlitTransparent"));
			atlas2Material.mainTexture = atlas2;

			currentLayerPair = 0;
		}

		public static GameObject CreateItem(ItemShape shape, ItemMaterial material)
		{
			ItemEffect effect;
			if (material == ItemMaterial.Ice) effect = ItemEffect.Ice;
			else effect = ItemEffect.Solid;

			GameObject obj, objEffect;

			switch (shape)
			{
				case ItemShape.Circle:
					obj = CreateCircle(0.5f, material);
					objEffect = CreateCircleEffect(0.5f, effect);
					break;
				case ItemShape.Rectangle:
					obj = CreateRectangle(1, 1, material);
					objEffect = CreateCircleEffect(0.5f, effect);
					break;
				default:
					obj = CreateCircle(0.5f, material);
					objEffect = CreateCircleEffect(0.5f, effect);
					break;
			}

			objEffect.transform.parent = obj.transform;

			obj.renderer.sortingLayerName = "Elements";
			obj.renderer.sortingOrder = currentLayerPair * 2;

			objEffect.renderer.sortingLayerName = "Elements";
			objEffect.renderer.sortingOrder = currentLayerPair * 2 + 1;

			currentLayerPair++;

			return obj;
		}

		private static GameObject CreateCircle(float radius, ItemMaterial itemMaterial)
		{
			Vector3[] vertices = new Vector3[circleSegments + 1];
			int[] triangles = new int[circleSegments * 3];
			Vector2[] uvs = new Vector2[circleSegments + 1];

			vertices[0] = new Vector3(0, 0, 0);

			Vector2 uvCenter;
			Material material;
			switch (itemMaterial)
			{
				case ItemMaterial.FixedMetal:
					uvCenter = TextureXY2UV(677, 1756);
					material = atlas1Material;
					break;
				case ItemMaterial.Ice:
					uvCenter = TextureXY2UV(677, 1254);
					material = atlas1Material;
					break;
				case ItemMaterial.Metal:
					uvCenter = TextureXY2UV(677, 250);
					material = atlas1Material;
					break;
				case ItemMaterial.Rubber:
					uvCenter = TextureXY2UV(677, 250);
					material = atlas2Material;
					break;
				case ItemMaterial.Wood:
					uvCenter = TextureXY2UV(677, 752);
					material = atlas1Material;
					break;
				default:
					uvCenter = TextureXY2UV(677, 1756);
					material = atlas1Material;
					break;
			}

			uvs[0] = new Vector2(uvCenter.x, uvCenter.y);

			double angle = 2 * Math.PI / circleSegments;

			for (int i = 0; i < circleSegments; i++)
			{
				vertices[i + 1] = new Vector3((float)(radius * Math.Cos(i * angle)), (float)(radius * Math.Sin(i * angle)), 0);

				uvs[i + 1] = new Vector2(uvCenter.x + radius * 2 * (float)(0.0375f * Math.Cos(i * angle)), uvCenter.y + radius * 2 * (float)(0.0375f * Math.Sin(i * angle)));

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
			
			return obj;
		}

		private static GameObject CreateCircleEffect(float radius, ItemEffect effect)
		{
			Vector3[] vertices = new Vector3[circleSegments * 2];
			int[] triangles = new int[circleSegments * 3 * 2];
			Vector2[] uvs = new Vector2[circleSegments * 2];

			int pixelInnerOffset, pixelOuterOffset;
			Vector2 uv0, uv1, uv2, uv3;

			switch (effect)
			{
				case ItemEffect.Ice:
					pixelInnerOffset = 15;
					pixelOuterOffset = 10;
					uv0 = TextureXY2UV(70, 2046);
					uv1 = TextureXY2UV(70, 2022);
					uv2 = TextureXY2UV(87, 2046);
					uv3 = TextureXY2UV(87, 2022);
					break;
				case ItemEffect.Solid:
					pixelInnerOffset = 9;
					pixelOuterOffset = 10;
					uv0 = TextureXY2UV(49, 2044);
					uv1 = TextureXY2UV(49, 2026);
					uv2 = TextureXY2UV(66, 2044);
					uv3 = TextureXY2UV(66, 2026);
					break;
				default:
					pixelInnerOffset = 9;
					pixelOuterOffset = 10;
					uv0 = TextureXY2UV(49, 2044);
					uv1 = TextureXY2UV(49, 2026);
					uv2 = TextureXY2UV(66, 2044);
					uv3 = TextureXY2UV(66, 2026);
					break;
			}

			float innerRadius = radius - pixelInnerOffset / PixelsPerUnit;
			float outerRadius = radius + pixelOuterOffset / PixelsPerUnit;

			double angle = 2 * Math.PI / circleSegments;

			for (int i = 0; i < circleSegments; i++)
			{
				vertices[i * 2] = new Vector3((float)(innerRadius * Math.Cos(i * angle)), (float)(innerRadius * Math.Sin(i * angle)), 0);
				vertices[i * 2 + 1] = new Vector3((float)(outerRadius * Math.Cos(i * angle)), (float)(outerRadius * Math.Sin(i * angle)), 0);


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

			return obj;
		}


		private static GameObject CreateRectangle(float width, float height, ItemMaterial itemMaterial)
		{
			Vector3[] vertices = {
														new Vector3(-width / 2, -height / 2, 0),
														new Vector3(-width / 2, height / 2, 0),
														new Vector3(width / 2, height / 2, 0),
														new Vector3(width / 2, -height / 2, 0),
													};
			int[] triangles = { 0, 1, 2, 2, 3, 0 };
			Vector2[] uvs = new Vector2[4];


			IntVector2 TextureXYTopLeft;
			Material material;
			
			switch (itemMaterial)
			{
				case ItemMaterial.FixedMetal:
					TextureXYTopLeft = new IntVector2(1, 1507);
					material = atlas1Material;
					break;
				case ItemMaterial.Ice:
					TextureXYTopLeft = new IntVector2(1, 1005);
					material = atlas1Material;
					break;
				case ItemMaterial.Metal:
					TextureXYTopLeft = new IntVector2(1, 1);
					material = atlas1Material;
					break;
				case ItemMaterial.Rubber:
					TextureXYTopLeft = new IntVector2(1, 1);
					material = atlas2Material;
					break;
				case ItemMaterial.Wood:
					TextureXYTopLeft = new IntVector2(1, 503);
					material = atlas1Material;
					break;
				default:
					TextureXYTopLeft = new IntVector2(1, 1);
					material = atlas1Material;
					break;
			}

			uvs[0] = TextureXY2UV(TextureXYTopLeft.x, (int)(TextureXYTopLeft.y + height * PixelsPerUnit - 1));
			uvs[1] = TextureXY2UV(TextureXYTopLeft.x, TextureXYTopLeft.y);
			uvs[2] = TextureXY2UV((int)(TextureXYTopLeft.x + width * PixelsPerUnit - 1), TextureXYTopLeft.y);
			uvs[3] = TextureXY2UV((int)(TextureXYTopLeft.x + width * PixelsPerUnit - 1), (int)(TextureXYTopLeft.y + height * PixelsPerUnit - 1));

			Mesh mesh = new Mesh();

			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uvs;

			GameObject obj = new GameObject();
			obj.AddComponent<MeshRenderer>();
			obj.AddComponent<MeshFilter>().mesh = mesh;

			obj.renderer.material = material;

			return obj;
		}

		
		private static Vector2 TextureXY2UV(int x, int y)
		{
			return new Vector2((float)x / 2047, (float)(2047 - y) / 2047);
		}

		private static Vector2 TextureXY2UV(IntVector2 TextureXY)
		{
			return new Vector2((float)TextureXY.x / 2047, (float)(2047 - TextureXY.y) / 2047);
		}




		//OLD
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
	}
}