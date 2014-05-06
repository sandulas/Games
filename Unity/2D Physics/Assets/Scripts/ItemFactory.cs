﻿using UnityEngine;
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

			Application.targetFrameRate = -1;
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
					objEffect = CreateRectangleEffect(1, 1, effect);
					break;
				case ItemShape.Triangle:
					obj = CreateTriangle(1, 1f, material);
					objEffect = CreateTriangleEffect(1, 1f, effect);
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

			IntVector2 TextureXYCenter;
			Material material;
			switch (itemMaterial)
			{
				case ItemMaterial.FixedMetal:
					TextureXYCenter = new IntVector2(677, 1756);
					material = atlas1Material;
					break;
				case ItemMaterial.Ice:
					TextureXYCenter = new IntVector2(1004, 1254);
					material = atlas1Material;
					break;
				case ItemMaterial.Metal:
					TextureXYCenter = new IntVector2(677, 250);
					material = atlas1Material;
					break;
				case ItemMaterial.Rubber:
					TextureXYCenter = new IntVector2(677, 250);
					material = atlas2Material;
					break;
				case ItemMaterial.Wood:
					TextureXYCenter = new IntVector2(800, 752);
					material = atlas1Material;
					break;
				default:
					TextureXYCenter = new IntVector2(677, 1756);
					material = atlas1Material;
					break;
			}

			uvs[0] = TextureXY2UV(TextureXYCenter);

			double angle = 2 * Math.PI / circleSegments;

			for (int i = 0; i < circleSegments; i++)
			{
				vertices[i + 1] = new Vector3((float)(radius * Math.Cos(i * angle)), (float)(radius * Math.Sin(i * angle)), 0);

				uvs[i + 1] = TextureXY2UV(TextureXYCenter.x + (int)(radius * PixelsPerUnit * Math.Cos(i * angle)), TextureXYCenter.y - (int)(radius * PixelsPerUnit * Math.Sin(i * angle)));

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
					uv0 = TextureXY2UV(70, 2046);  //bottom left
					uv1 = TextureXY2UV(70, 2022);  //top left
					uv2 = TextureXY2UV(87, 2046);  //bottom right
					uv3 = TextureXY2UV(87, 2022);  //top right
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

		private static GameObject CreateRectangleEffect(float width, float height, ItemEffect effect)
		{
			int pixelInnerOffset, pixelOuterOffset;
			Vector2 uv0, uv1, uv2, uv3;

			switch (effect)
			{
				case ItemEffect.Ice:
					pixelInnerOffset = 15;
					pixelOuterOffset = 10;
					uv0 = TextureXY2UV(70, 2046);  //bottom left
					uv1 = TextureXY2UV(70, 2022);  //top left
					uv2 = TextureXY2UV(87, 2046);  //bottom right
					uv3 = TextureXY2UV(87, 2022);  //top right
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

			//the mesh looks like 4 trapezoids with the small base inside, forming a rectangle
			Vector3[] vertices = {
		                         //top
		                         new Vector3(-width / 2 - pixelOuterOffset / PixelsPerUnit, height / 2 + pixelOuterOffset / PixelsPerUnit),
		                         new Vector3(width / 2 + pixelOuterOffset / PixelsPerUnit, height / 2 + pixelOuterOffset / PixelsPerUnit),
		                         new Vector3(width / 2 - pixelInnerOffset / PixelsPerUnit, height / 2 - pixelInnerOffset / PixelsPerUnit),
		                         new Vector3(-width / 2 + pixelInnerOffset / PixelsPerUnit, height / 2 - pixelInnerOffset / PixelsPerUnit),
		                         //right
		                         new Vector3(width / 2 - pixelInnerOffset / PixelsPerUnit, height / 2 - pixelInnerOffset / PixelsPerUnit),
		                         new Vector3(width / 2 + pixelOuterOffset / PixelsPerUnit, height / 2 + pixelOuterOffset / PixelsPerUnit),
		                         new Vector3(width / 2 + pixelOuterOffset / PixelsPerUnit, -height / 2 - pixelOuterOffset / PixelsPerUnit),
		                         new Vector3(width / 2 - pixelInnerOffset / PixelsPerUnit, -height / 2 + pixelInnerOffset / PixelsPerUnit),
		                         //bottom
		                         new Vector3(-width / 2 + pixelInnerOffset / PixelsPerUnit, -height / 2 + pixelInnerOffset / PixelsPerUnit),
		                         new Vector3(width / 2 - pixelInnerOffset / PixelsPerUnit, -height / 2 + pixelInnerOffset / PixelsPerUnit),
		                         new Vector3(width / 2 + pixelOuterOffset / PixelsPerUnit, -height / 2 - pixelOuterOffset / PixelsPerUnit),
		                         new Vector3(-width / 2 - pixelOuterOffset / PixelsPerUnit, -height / 2 - pixelOuterOffset / PixelsPerUnit),
		                         //left
		                         new Vector3(-width / 2 - pixelOuterOffset / PixelsPerUnit, height / 2 + pixelOuterOffset / PixelsPerUnit),
		                         new Vector3(-width / 2 + pixelInnerOffset / PixelsPerUnit, height / 2 - pixelInnerOffset / PixelsPerUnit),
		                         new Vector3(-width / 2 + pixelInnerOffset / PixelsPerUnit, -height / 2 + pixelInnerOffset / PixelsPerUnit),
		                         new Vector3(-width / 2 - pixelOuterOffset / PixelsPerUnit, -height / 2 - pixelOuterOffset / PixelsPerUnit),
		                       };

			int[] triangles = {
		                      0, 1, 3, 3, 1, 2,				//top
		                      4, 5, 6, 6, 7, 4,				//right
		                      8, 9, 11, 11, 9, 10,		//bottom
		                      12, 13, 14, 14, 15, 12	//left
		                    };

			Vector2[] uvs = {
		                    uv1, uv3, uv2, uv0,	//top
		                    uv0, uv1, uv3, uv2,	//right
		                    uv2, uv0, uv1, uv3,	//bottom
		                    uv3, uv2, uv0, uv1	//left
		                  };


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


		private static GameObject CreateTriangle(float width, float height, ItemMaterial itemMaterial)
		{
			Vector3[] vertices = {
														new Vector3(-width / 2, -height / 2, 0),
														new Vector3(-width / 2, height / 2, 0),
														new Vector3(width / 2, -height / 2, 0),
													};
			int[] triangles = { 0, 1, 2 };
			Vector2[] uvs = new Vector2[3];

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
			uvs[2] = TextureXY2UV((int)(TextureXYTopLeft.x + width * PixelsPerUnit - 1), (int)(TextureXYTopLeft.y + height * PixelsPerUnit - 1));

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

		private static GameObject CreateTriangleEffect(float width, float height, ItemEffect effect)
		{
			int pixelInnerOffset, pixelOuterOffset;

			Vector2[,] uv = new Vector2[3, 3];

			switch (effect)
			{
				case ItemEffect.Ice:
					pixelInnerOffset = 15;
					pixelOuterOffset = 10;
										
					uv[0, 0] = TextureXY2UV(70, 2022);	//top left
					uv[0, 1] = TextureXY2UV(79, 2022);	//top center
					uv[0, 2] = TextureXY2UV(87, 2022);	//top right
					uv[1, 0] = TextureXY2UV(70, 2034);	//middle left
					uv[1, 1] = TextureXY2UV(79, 2034);	//middle center
					uv[1, 2] = TextureXY2UV(87, 2034);	//middle right
					uv[2, 0] = TextureXY2UV(70, 2046);	//bottom left
					uv[2, 1] = TextureXY2UV(79, 2046);	//bottom center
					uv[2, 2] = TextureXY2UV(87, 2046);	//bottom right
					break;
				
				case ItemEffect.Solid:
					pixelInnerOffset = 9;
					pixelOuterOffset = 10;
										
					uv[0, 0] = TextureXY2UV(49, 2026);
					uv[0, 1] = TextureXY2UV(58, 2026);
					uv[0, 2] = TextureXY2UV(66, 2026);
					uv[1, 0] = TextureXY2UV(49, 2035);
					uv[1, 1] = TextureXY2UV(58, 2035);
					uv[1, 2] = TextureXY2UV(66, 2035);
					uv[2, 0] = TextureXY2UV(49, 2044);
					uv[2, 1] = TextureXY2UV(58, 2044);
					uv[2, 2] = TextureXY2UV(66, 2044);
					break;

				default:
					pixelInnerOffset = 9;
					pixelOuterOffset = 10;
										
					uv[0, 0] = TextureXY2UV(70, 2022);
					uv[0, 1] = TextureXY2UV(79, 2022);
					uv[0, 2] = TextureXY2UV(87, 2022);
					uv[1, 0] = TextureXY2UV(70, 2034);
					uv[1, 1] = TextureXY2UV(79, 2034);
					uv[1, 2] = TextureXY2UV(87, 2034);
					uv[2, 0] = TextureXY2UV(70, 2046);
					uv[2, 1] = TextureXY2UV(79, 2046);
					uv[2, 2] = TextureXY2UV(87, 2046);
					break;
			}

			double angle1 = Math.Atan(width / height);
			double angle2 = Math.Atan(height / width);
			double innerOffset = pixelInnerOffset / PixelsPerUnit;
			double outerOffset = pixelOuterOffset / PixelsPerUnit;

			Vector3[] vertices = {
														 //inside
														 new Vector3((float)(-width / 2 + innerOffset), (float)(-height / 2 + innerOffset)),
														 new Vector3((float)(-width / 2 + innerOffset), (float)(height / 2 - innerOffset / Math.Tan(angle1 / 2))),
														 new Vector3((float)(width / 2 - innerOffset / Math.Tan(angle2 / 2)), (float)(-height / 2 + innerOffset)),
														 //on edge
														 new Vector3(-width / 2, -height / 2),
														 new Vector3(-width / 2, height / 2),
														 new Vector3(width / 2, -height / 2),
														 //outside 0
														 new Vector3(-width / 2, (float)(-height / 2 - outerOffset)),
														 GetVector3(PointOnCircle(-width / 2, - height / 2, outerOffset, Math.PI + Math.PI / 4 )),
														 new Vector3((float)(-width / 2 - outerOffset), -height / 2),
														 //outside 1
														 new Vector3((float)(-width / 2 - outerOffset), height / 2),
														 GetVector3(PointOnCircle(-width / 2, height / 2, outerOffset, Math.PI - (Math.PI - angle1) / 2)),
														 GetVector3(PointOnCircle(-width / 2, height / 2, outerOffset, Math.PI - (Math.PI - angle1))),
														 //outside 3
														 GetVector3(PointOnCircle(width / 2, -height / 2, outerOffset, 3 * Math.PI / 2 + (Math.PI - angle2))),
														 GetVector3(PointOnCircle(width / 2, -height / 2, outerOffset, 3 * Math.PI / 2 + (Math.PI - angle2) / 2)),
														 new Vector3(width / 2, (float)(-height / 2 - outerOffset)),
													 };

			int[] triangles = {
													0, 3, 4, 0, 4, 1, 1, 4, 5, 1, 5, 2, 2, 5, 3, 2, 3, 0,
													3, 6, 7, 3, 7, 8, 3, 8, 9, 3, 9, 4,
													4, 9, 10, 4, 10, 11, 4, 11, 12, 4, 12, 5,
													5, 12, 13, 5, 13, 14, 5, 14, 6, 5, 6, 3
												};

			Vector2[] uvs = {
												uv[2, 0], uv[2, 1], uv[2, 2],
												uv[1, 0], uv[1, 1], uv[1, 2],
												uv[0, 0], uv[0, 1], uv[0, 0],
												uv[0, 1], uv[0, 1], uv[0, 1],
												uv[0, 0], uv[0, 1], uv[0, 2],
											};


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


		//Misc helper methods
		private static Vector2 TextureXY2UV(int textureX, int textureY)
		{
			return new Vector2((float)textureX / 2047, (float)(2047 - textureY) / 2047);
		}

		private static Vector2 TextureXY2UV(IntVector2 textureXY)
		{
			return TextureXY2UV(textureXY.x, textureXY.y);
		}

		private static Vector2 PointOnCircle(double circleCenterX, double circleCenterY, double radius, double angle)
		{
			return new Vector2((float)(circleCenterX + radius * Math.Cos(angle)), (float)(circleCenterY + radius * Math.Sin(angle)));
		}

		private static Vector3 GetVector3(Vector2 vector2)
		{
			return new Vector3(vector2.x, vector2.y);
		}




		//OLD
		public static GameObject CreateTriangleMesh()
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
			
			return obj;
		}

		public static GameObject CreateTriangleMesh2()
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
				TextureXY2UV(200, 200),
				TextureXY2UV(150, 100),
				TextureXY2UV(250, 100)
			};

			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uvs;

			GameObject obj = new GameObject();
			obj.AddComponent<MeshRenderer>();
			obj.AddComponent<MeshFilter>().mesh = mesh;

			obj.renderer.material = atlas2Material;

			obj.transform.localPosition = new Vector3(2, 0, 0);

			return obj;
		}

		private static GameObject CreateRectangleEffect_Old(float width, float height, ItemEffect effect)
		{
			int pixelInnerOffset, pixelOuterOffset;
			Vector2 uv0, uv1, uv2, uv3;

			switch (effect)
			{
				case ItemEffect.Ice:
					pixelInnerOffset = 15;
					pixelOuterOffset = 10;
					uv0 = TextureXY2UV(70, 2046);  //bottom left
					uv1 = TextureXY2UV(70, 2022);  //top left
					uv2 = TextureXY2UV(87, 2046);  //bottom right
					uv3 = TextureXY2UV(87, 2022);  //top right
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

			//from top left to bottom right, 4 rows, 4 vertices per row
			Vector3[] vertices = {
														 //1st row
														 new Vector3(-width / 2 - pixelOuterOffset / PixelsPerUnit, height / 2 + pixelOuterOffset / PixelsPerUnit),
														 new Vector3(-width / 2 + pixelInnerOffset / PixelsPerUnit, height / 2 + pixelOuterOffset / PixelsPerUnit),
														 new Vector3(width / 2 - pixelInnerOffset / PixelsPerUnit, height / 2 + pixelOuterOffset / PixelsPerUnit),
														 new Vector3(width / 2 + pixelOuterOffset / PixelsPerUnit, height / 2 + pixelOuterOffset / PixelsPerUnit),
														 //2nd row
														 new Vector3(-width / 2 - pixelOuterOffset / PixelsPerUnit, height / 2 - pixelInnerOffset / PixelsPerUnit),
														 new Vector3(-width / 2 + pixelInnerOffset / PixelsPerUnit, height / 2 - pixelInnerOffset / PixelsPerUnit),
														 new Vector3(width / 2 - pixelInnerOffset / PixelsPerUnit, height / 2 - pixelInnerOffset / PixelsPerUnit),
														 new Vector3(width / 2 + pixelOuterOffset / PixelsPerUnit, height / 2 - pixelInnerOffset / PixelsPerUnit),
														 //3rd row
														 new Vector3(-width / 2 - pixelOuterOffset / PixelsPerUnit, -height / 2 + pixelInnerOffset / PixelsPerUnit),
														 new Vector3(-width / 2 + pixelInnerOffset / PixelsPerUnit, -height / 2 + pixelInnerOffset / PixelsPerUnit),
														 new Vector3(width / 2 - pixelInnerOffset / PixelsPerUnit, -height / 2 + pixelInnerOffset / PixelsPerUnit),
														 new Vector3(width / 2 + pixelOuterOffset / PixelsPerUnit, -height / 2 + pixelInnerOffset / PixelsPerUnit),
														 //4th row
														 new Vector3(-width / 2 - pixelOuterOffset / PixelsPerUnit, -height / 2 - pixelOuterOffset / PixelsPerUnit),
														 new Vector3(-width / 2 + pixelInnerOffset / PixelsPerUnit, -height / 2 - pixelOuterOffset / PixelsPerUnit),
														 new Vector3(width / 2 - pixelInnerOffset / PixelsPerUnit, -height / 2 - pixelOuterOffset / PixelsPerUnit),
														 new Vector3(width / 2 + pixelOuterOffset / PixelsPerUnit, -height / 2 - pixelOuterOffset / PixelsPerUnit)
													 };

			//from top left to bottom right, 3 rows, 6 / 4 / 6 triangles per each row
			int[] triangles = {
													//1st row
													4, 0, 5, 0, 1, 5, 5, 1, 6, 1, 2, 6, 6, 2, 7, 2, 3, 7,
													//2nd row
													8, 4, 9, 4, 5, 9, 10, 6, 11, 6, 7, 11,
													//3rd row
													12, 8, 13, 8, 9, 13, 13, 9, 14, 9, 10, 14, 14, 10, 15, 10, 11, 15
												};

			//same order as the vertices, of course
			Vector2[] uvs = { uv1, uv3, uv1, uv3, uv0, uv2, uv0, uv2, uv1, uv3, uv1, uv3, uv0, uv2, uv0, uv2 };


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

		private static GameObject CreateTriangleEffect_Old(float width, float height, ItemEffect effect)
		{
			int pixelInnerOffset, pixelOuterOffset;
			Vector2 uv0, uv1, uv2, uv3;

			switch (effect)
			{
				case ItemEffect.Ice:
					pixelInnerOffset = 15;
					pixelOuterOffset = 10;
					uv0 = TextureXY2UV(70, 2046);  //bottom left
					uv1 = TextureXY2UV(70, 2022);  //top left
					uv2 = TextureXY2UV(87, 2046);  //bottom right
					uv3 = TextureXY2UV(87, 2022);  //top right
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

			double angle1 = Math.Atan(width / height);
			double angle2 = Math.Atan(height / width);
			double innerOffset = pixelInnerOffset / PixelsPerUnit;
			double outerOffset = pixelOuterOffset / PixelsPerUnit;

			Vector3[] vertices = {
		                         //inside
		                         new Vector3((float)(-width / 2 + innerOffset), (float)(-height / 2 + innerOffset)),
		                         new Vector3((float)(-width / 2 + innerOffset), (float)(height / 2 - innerOffset / Math.Tan(angle1 / 2))),
		                         new Vector3((float)(width / 2 - innerOffset / Math.Tan(angle2 / 2)), (float)(-height / 2 + innerOffset)),
		                         //outside 0
		                         new Vector3(-width / 2, (float)(-height / 2 - outerOffset)),
		                         GetVector3(PointOnCircle(-width / 2, - height / 2, outerOffset, Math.PI + Math.PI / 4 )),
		                         new Vector3((float)(-width / 2 - outerOffset), -height / 2),
		                         //outside 1
		                         new Vector3((float)(-width / 2 - outerOffset), height / 2),
		                         GetVector3(PointOnCircle(-width / 2, height / 2, outerOffset, Math.PI - (Math.PI - angle1) / 2)),
		                         GetVector3(PointOnCircle(-width / 2, height / 2, outerOffset, Math.PI - (Math.PI - angle1))),
		                         //outside 3
		                         GetVector3(PointOnCircle(width / 2, -height / 2, outerOffset, 3 * Math.PI / 2 + (Math.PI - angle2))),
		                         GetVector3(PointOnCircle(width / 2, -height / 2, outerOffset, 3 * Math.PI / 2 + (Math.PI - angle2) / 2)),
		                         new Vector3(width / 2, (float)(-height / 2 - outerOffset)),
		                         //double last inside vertex and last outside vertex to make texture mapping possible
		                         new Vector3((float)(width / 2 - innerOffset / Math.Tan(angle2 / 2)), (float)(-height / 2 + innerOffset)),
		                         new Vector3(width / 2, (float)(-height / 2 - outerOffset))
		                       };

			int[] triangles = {
		                      0, 12, 13, 0, 13, 3, 0, 3, 4, 0, 4, 5, 1, 0, 5, 1, 5, 6, 1, 6, 7, 1, 7, 8, 2, 1, 8, 2, 8, 9, 2, 9, 10, 2, 10, 11
		                    };

			Vector2[] uvs = {
		                    uv0, uv2, uv0, uv1, uv3, uv1, uv1, uv3, uv1, uv1, uv3, uv1, uv2, uv3
		                  };


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
	}
}