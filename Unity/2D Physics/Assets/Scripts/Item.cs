using UnityEngine;
using System.Collections;
using System;

namespace ThisProject
{
	public enum ItemShape { Rectangle, Circle, Triangle }
	public enum ItemMaterial { FixedMetal, Metal, Wood, Rubber, Ice }

	public class Item
	{
		enum ItemEffect { Solid, Ice }

		static Texture2D[] itemTextures = new Texture2D[5];
		static Material[] itemMaterials = new Material[5];

		static Texture2D atlas1;
		static Material atlas1Material;
		static float PixelsPerUnit = 1536f / 10;
		static int circleSegments = 50;
		
		static int frontLayerIndex;
		const float layerSpacing = 0.001f;

		static Item()
		{
			int materialsCount = Enum.GetValues(typeof(ItemMaterial)).Length;

			for (int i = 0; i < materialsCount; i++)
			{
				itemTextures[i] = (Texture2D)Resources.Load(Enum.GetName(typeof(ItemMaterial), i));
				
				itemMaterials[i] = new Material(Shader.Find("Mobile/Unlit (Supports Lightmap)"));
				itemMaterials[i].mainTexture = itemTextures[i];

				switch (i)
				{
					case (int)ItemMaterial.FixedMetal:
						itemMaterials[i].mainTextureScale = new Vector2(1f, 1f);
						break;
					case (int)ItemMaterial.Ice:
						itemMaterials[i].mainTextureScale = new Vector2(0.5f, 0.5f);
						break;
					case (int)ItemMaterial.Metal:
						itemMaterials[i].mainTextureScale = new Vector2(1f, 1f);
						break;
					case (int)ItemMaterial.Rubber:
						itemMaterials[i].mainTextureScale = new Vector2(0.5f, 0.5f);
						break;
					case (int)ItemMaterial.Wood:
						itemMaterials[i].mainTextureScale = new Vector2(0.5f, 0.5f);
						break;
				}
			}

			atlas1 = (Texture2D)Resources.Load("Atlas1");

			atlas1Material = new Material(Shader.Find("Custom/UnlitTransparent"));
			atlas1Material.mainTexture = atlas1;

			frontLayerIndex = 0;

			Application.targetFrameRate = -1;
		}

		public static GameObject Create(ItemShape shape, ItemMaterial itemMaterial, float width, float height)
		{
			Mesh objMesh, objEffectMesh;
			ItemEffect effect;

			if (itemMaterial == ItemMaterial.Ice) effect = ItemEffect.Ice;
			else effect = ItemEffect.Solid;

			switch (shape)
			{
				case ItemShape.Circle:
					objMesh = createCircleMesh(width / 2, itemMaterial);
					objEffectMesh = createCircleEffectMesh(width / 2, effect);
					break;
				case ItemShape.Rectangle:
					objMesh = createRectangleMesh(width, height, itemMaterial);
					objEffectMesh = createRectangleEffectMesh(width, height, effect);
					break;
				case ItemShape.Triangle:
					objMesh = createTriangleMesh(width, height, itemMaterial);
					objEffectMesh = createTriangleEffectMesh(width, height, effect);
					break;
				default:
					objMesh = createTriangleMesh(width, height, itemMaterial);
					objEffectMesh = createTriangleEffectMesh(width, height, effect);
					break;
			}

			//create the object
			GameObject obj = new GameObject();
			obj.AddComponent<MeshRenderer>();
			obj.AddComponent<MeshFilter>().mesh = objMesh;
			obj.renderer.material = itemMaterials[(int)itemMaterial];

			//create the object effect
			GameObject objEffect = new GameObject();
			objEffect.AddComponent<MeshRenderer>();
			objEffect.AddComponent<MeshFilter>().mesh = objEffectMesh;
			objEffect.renderer.material = atlas1Material;

			//setup the object and the object effect in the scene
			obj.name = "Item: " + shape + " " + frontLayerIndex;
			objEffect.name = shape + " Effect " + frontLayerIndex;
			
			objEffect.transform.parent = obj.transform;
			obj.layer = 0;

			//bring the effect in front of the object, bring the object to front
			Vector3 pos = objEffect.transform.localPosition;
			objEffect.transform.localPosition = new Vector3(pos.x, pos.y, -layerSpacing / 2);
			BringToFront(obj);

			//add the object properties
			ItemProperties itemProperties = obj.AddComponent<ItemProperties>();
			itemProperties.Shape = shape;
			itemProperties.Material = itemMaterial;
			itemProperties.Width = width;
			itemProperties.Height = height;

			setPhysics(obj);

			return obj;
		}

		public static void Resize(GameObject item, float width, float height)
		{
			update(item, item.GetComponent<ItemProperties>().Material, width, height);
		}
		public static void Move(GameObject item, Vector3 position)
		{
			item.transform.position = new Vector3(position.x, position.y, item.transform.position.z);
		}
		public static void Move(GameObject item, Vector2 position)
		{
			item.transform.position = new Vector3(position.x, position.y, item.transform.position.z);
		}
		public static void Move(GameObject item, float x, float y)
		{
			Move(item, new Vector2(x, y));
		}
		public static void ChangeMaterial(GameObject item, ItemMaterial itemMaterial)
		{
			update(item, itemMaterial, item.GetComponent<ItemProperties>().Width, item.GetComponent<ItemProperties>().Height);
		}
		public static void Duplicate(GameObject item)
		{
			GameObject.Instantiate(item);
			item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 0);
			BringToFront(item);
		}
		public static void BringToFront(GameObject item)
		{
			Vector3 pos = item.transform.position;
			float frontPosition = frontLayerIndex * layerSpacing;

			if (pos.z > frontPosition || pos.z >= 0)
			{
				frontLayerIndex--;
				item.transform.position = new Vector3(pos.x, pos.y, frontLayerIndex * layerSpacing);
			}
		}

		private static void update(GameObject item, ItemMaterial itemMaterial, float width, float height)
		{
			ItemProperties itemProperties = item.GetComponent<ItemProperties>();

			Mesh objMesh, objEffectMesh;
			ItemEffect effect;

			if (itemMaterial == ItemMaterial.Ice) effect = ItemEffect.Ice;
			else effect = ItemEffect.Solid;

			switch (itemProperties.Shape)
			{
				case ItemShape.Circle:
					objMesh = createCircleMesh(width / 2, itemMaterial);
					objEffectMesh = createCircleEffectMesh(width / 2, effect);
					break;
				case ItemShape.Rectangle:
					objMesh = createRectangleMesh(width, height, itemMaterial);
					objEffectMesh = createRectangleEffectMesh(width, height, effect);
					break;
				case ItemShape.Triangle:
					objMesh = createTriangleMesh(width, height, itemMaterial);
					objEffectMesh = createTriangleEffectMesh(width, height, effect);
					break;
				default:
					objMesh = createTriangleMesh(width, height, itemMaterial);
					objEffectMesh = createTriangleEffectMesh(width, height, effect);
					break;
			}

			//update the object
			item.GetComponent<MeshFilter>().mesh = objMesh;
			item.renderer.material = itemMaterials[(int)itemMaterial];

			//update the object effect
			GameObject itemEffect = item.transform.GetChild(0).gameObject;
			itemEffect.GetComponent<MeshFilter>().mesh = objEffectMesh;

			//update the object properties
			itemProperties.Material = itemMaterial;
			itemProperties.Width = width;
			itemProperties.Height = height;

			//update physics
			updatePhysics(item);
		}
		
		private static void setPhysics(GameObject item)
		{
			item.AddComponent<Rigidbody2D>();
			item.rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

			ItemProperties properties = item.GetComponent<ItemProperties>();

			switch (properties.Shape)
			{
				case ItemShape.Circle:
					item.AddComponent<CircleCollider2D>();
					break;
				case ItemShape.Rectangle:
					item.AddComponent<BoxCollider2D>();
					break;
				case ItemShape.Triangle:
					item.AddComponent<PolygonCollider2D>();
					break;
				default:
					break;
			}
			
			updatePhysics(item);
		}
		private static void updatePhysics(GameObject item)
		{
			ItemProperties properties = item.GetComponent<ItemProperties>();
			double itemArea = 1;

			switch (properties.Shape)
			{
				case ItemShape.Circle:
					CircleCollider2D circleCollider = item.GetComponent<CircleCollider2D>();
					circleCollider.radius = properties.Width / 2;

					itemArea = Math.PI * (properties.Width / 2) * (properties.Width / 2);
					break;

				case ItemShape.Rectangle:
					BoxCollider2D boxCollider = item.GetComponent<BoxCollider2D>();
					boxCollider.size = new Vector2(properties.Width, properties.Height);

					itemArea = properties.Width * properties.Height;
					break;

				case ItemShape.Triangle:
					PolygonCollider2D polygonCollider = item.GetComponent<PolygonCollider2D>();
					Vector2[] points = new Vector2[3];
					points[0] = new Vector2(-properties.Width / 2, -properties.Height / 2);
					points[1] = new Vector2(-properties.Width / 2, properties.Height / 2);
					points[2] = new Vector2(properties.Width / 2, -properties.Height / 2);
					polygonCollider.points = points;

					itemArea = properties.Width * properties.Height / 2;
					break;

				default:
					break;
			}


			item.rigidbody2D.isKinematic = false;
			switch (properties.Material)
			{
				case ItemMaterial.FixedMetal:
					item.rigidbody2D.isKinematic = true;
					break;
				case ItemMaterial.Ice:
					item.rigidbody2D.mass = 0.91f * (float)itemArea;
					item.rigidbody2D.drag = 0.1f;
					item.rigidbody2D.angularDrag = 0.2f;
					break;
				case ItemMaterial.Metal:
					item.rigidbody2D.mass = 7.9f * (float)itemArea;
					item.rigidbody2D.drag = 0.1f;
					item.rigidbody2D.angularDrag = 0.2f;
					break;
				case ItemMaterial.Rubber:
					item.rigidbody2D.mass = 1.0f * (float)itemArea;
					item.rigidbody2D.drag = 0.1f;
					item.rigidbody2D.angularDrag = 0.2f;

					PhysicsMaterial2D physicsMaterial = new PhysicsMaterial2D("Rubber");
					physicsMaterial.friction = 0.9f;
					physicsMaterial.bounciness = 0.8f;
					item.GetComponent<Collider2D>().sharedMaterial = physicsMaterial;
					break;
				case ItemMaterial.Wood:
					item.rigidbody2D.mass = 0.65f * (float)itemArea;;
					item.rigidbody2D.drag = 0.1f;
					item.rigidbody2D.angularDrag = 0.2f;
					break;
				default:
					break;
			}
		}

		//	circle
		private static Mesh createCircleMesh(float radius, ItemMaterial itemMaterial)
		{
			Vector3[] vertices = new Vector3[circleSegments + 1];
			int[] triangles = new int[circleSegments * 3];
			Vector2[] uvs = new Vector2[circleSegments + 1];

			vertices[0] = new Vector3(0, 0, 0);

			Texture2D texture = itemTextures[(int)itemMaterial];

			IntVector2 TextureXYCenter = new IntVector2(
				(int)Math.Round((texture.width / 2) / itemMaterials[(int)itemMaterial].mainTextureScale.x),
				(int)Math.Round((texture.height / 2) / itemMaterials[(int)itemMaterial].mainTextureScale.y));

			uvs[0] = TextureXY2UV2(texture, TextureXYCenter);

			double angle = 2 * Math.PI / circleSegments;

			for (int i = 0; i < circleSegments; i++)
			{
				vertices[i + 1] = new Vector3((float)(radius * Math.Cos(i * angle)), (float)(radius * Math.Sin(i * angle)), 0);

				uvs[i + 1] = TextureXY2UV2(texture, TextureXYCenter.x + (int)(radius * PixelsPerUnit * Math.Cos(i * angle)), TextureXYCenter.y + (int)(radius * PixelsPerUnit * Math.Sin(i * angle)));

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

			return mesh;
		}
		private static Mesh createCircleEffectMesh(float radius, ItemEffect effect)
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
					uv0 = TextureXY2UV(atlas1, 70, 2046);  //bottom left
					uv1 = TextureXY2UV(atlas1, 70, 2022);  //top left
					uv2 = TextureXY2UV(atlas1, 87, 2046);  //bottom right
					uv3 = TextureXY2UV(atlas1, 87, 2022);  //top right
					break;
				case ItemEffect.Solid:
					pixelInnerOffset = 9;
					pixelOuterOffset = 10;
					uv0 = TextureXY2UV(atlas1, 49, 2044);
					uv1 = TextureXY2UV(atlas1, 49, 2026);
					uv2 = TextureXY2UV(atlas1, 66, 2044);
					uv3 = TextureXY2UV(atlas1, 66, 2026);
					break;
				default:
					pixelInnerOffset = 9;
					pixelOuterOffset = 10;
					uv0 = TextureXY2UV(atlas1, 49, 2044);
					uv1 = TextureXY2UV(atlas1, 49, 2026);
					uv2 = TextureXY2UV(atlas1, 66, 2044);
					uv3 = TextureXY2UV(atlas1, 66, 2026);
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

			return mesh;
		}

		//	rectangle
		private static Mesh createRectangleMesh(float width, float height, ItemMaterial itemMaterial)
		{
			Vector3[] vertices = {
														new Vector3(-width / 2, -height / 2, 0),
														new Vector3(-width / 2, height / 2, 0),
														new Vector3(width / 2, height / 2, 0),
														new Vector3(width / 2, -height / 2, 0),
													};
			int[] triangles = { 0, 1, 2, 2, 3, 0 };
			Vector2[] uvs = new Vector2[4];

			Texture2D texture = itemTextures[(int)itemMaterial];

			IntVector2 TextureXYCenter = new IntVector2(
				(int)Math.Round((texture.width / 2) / itemMaterials[(int)itemMaterial].mainTextureScale.x),
				(int)Math.Round((texture.height / 2) / itemMaterials[(int)itemMaterial].mainTextureScale.y));

			uvs[0] = TextureXY2UV2(texture, TextureXYCenter.x + (int)(-width / 2 * PixelsPerUnit), TextureXYCenter.y + (int)(-height / 2 * PixelsPerUnit));
			uvs[1] = TextureXY2UV2(texture, TextureXYCenter.x + (int)(-width / 2 * PixelsPerUnit), TextureXYCenter.y + (int)(height / 2 * PixelsPerUnit));
			uvs[2] = TextureXY2UV2(texture, TextureXYCenter.x + (int)(width / 2 * PixelsPerUnit), TextureXYCenter.y + (int)(height / 2 * PixelsPerUnit));
			uvs[3] = TextureXY2UV2(texture, TextureXYCenter.x + (int)(width / 2 * PixelsPerUnit), TextureXYCenter.y + (int)(-height / 2 * PixelsPerUnit));

			Mesh mesh = new Mesh();

			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uvs;

			return mesh;
		}
		private static Mesh createRectangleEffectMesh(float width, float height, ItemEffect effect)
		{
			int pixelInnerOffset, pixelOuterOffset;
			Vector2 uv0, uv1, uv2, uv3;

			switch (effect)
			{
				case ItemEffect.Ice:
					pixelInnerOffset = 15;
					pixelOuterOffset = 10;
					uv0 = TextureXY2UV(atlas1, 70, 2046);  //bottom left
					uv1 = TextureXY2UV(atlas1, 70, 2022);  //top left
					uv2 = TextureXY2UV(atlas1, 87, 2046);  //bottom right
					uv3 = TextureXY2UV(atlas1, 87, 2022);  //top right
					break;
				case ItemEffect.Solid:
					pixelInnerOffset = 9;
					pixelOuterOffset = 10;
					uv0 = TextureXY2UV(atlas1, 49, 2044);
					uv1 = TextureXY2UV(atlas1, 49, 2026);
					uv2 = TextureXY2UV(atlas1, 66, 2044);
					uv3 = TextureXY2UV(atlas1, 66, 2026);
					break;
				default:
					pixelInnerOffset = 9;
					pixelOuterOffset = 10;
					uv0 = TextureXY2UV(atlas1, 49, 2044);
					uv1 = TextureXY2UV(atlas1, 49, 2026);
					uv2 = TextureXY2UV(atlas1, 66, 2044);
					uv3 = TextureXY2UV(atlas1, 66, 2026);
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

			return mesh;
		}

		//	triangle
		private static Mesh createTriangleMesh(float width, float height, ItemMaterial itemMaterial)
		{
			Vector3[] vertices = {
														new Vector3(-width / 2, -height / 2, 0),
														new Vector3(-width / 2, height / 2, 0),
														new Vector3(width / 2, -height / 2, 0),
													};
			int[] triangles = { 0, 1, 2 };
			Vector2[] uvs = new Vector2[3];

			Texture2D texture = itemTextures[(int)itemMaterial];

			IntVector2 TextureXYCenter = new IntVector2(
				(int)Math.Round((texture.width / 2) / itemMaterials[(int)itemMaterial].mainTextureScale.x),
				(int)Math.Round((texture.height / 2) / itemMaterials[(int)itemMaterial].mainTextureScale.y));

			uvs[0] = TextureXY2UV2(texture, TextureXYCenter.x + (int)(-width / 2 * PixelsPerUnit), TextureXYCenter.y + (int)(-height / 2 * PixelsPerUnit));
			uvs[1] = TextureXY2UV2(texture, TextureXYCenter.x + (int)(-width / 2 * PixelsPerUnit), TextureXYCenter.y + (int)(height / 2 * PixelsPerUnit));
			uvs[2] = TextureXY2UV2(texture, TextureXYCenter.x + (int)(width / 2 * PixelsPerUnit), TextureXYCenter.y - (int)(height / 2 * PixelsPerUnit));

			Mesh mesh = new Mesh();

			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uvs;

			return mesh;
		}
		private static Mesh createTriangleEffectMesh(float width, float height, ItemEffect effect)
		{
			int pixelInnerOffset, pixelOuterOffset;

			Vector2[,] uv = new Vector2[3, 3];

			switch (effect)
			{
				case ItemEffect.Ice:
					pixelInnerOffset = 15;
					pixelOuterOffset = 10;

					uv[0, 0] = TextureXY2UV(atlas1, 70, 2022);	//top left
					uv[0, 1] = TextureXY2UV(atlas1, 79, 2022);	//top center
					uv[0, 2] = TextureXY2UV(atlas1, 87, 2022);	//top right
					uv[1, 0] = TextureXY2UV(atlas1, 70, 2034);	//middle left
					uv[1, 1] = TextureXY2UV(atlas1, 79, 2034);	//middle center
					uv[1, 2] = TextureXY2UV(atlas1, 87, 2034);	//middle right
					uv[2, 0] = TextureXY2UV(atlas1, 70, 2046);	//bottom left
					uv[2, 1] = TextureXY2UV(atlas1, 79, 2046);	//bottom center
					uv[2, 2] = TextureXY2UV(atlas1, 87, 2046);	//bottom right
					break;

				case ItemEffect.Solid:
					pixelInnerOffset = 9;
					pixelOuterOffset = 10;

					uv[0, 0] = TextureXY2UV(atlas1, 49, 2026);
					uv[0, 1] = TextureXY2UV(atlas1, 58, 2026);
					uv[0, 2] = TextureXY2UV(atlas1, 66, 2026);
					uv[1, 0] = TextureXY2UV(atlas1, 49, 2035);
					uv[1, 1] = TextureXY2UV(atlas1, 58, 2035);
					uv[1, 2] = TextureXY2UV(atlas1, 66, 2035);
					uv[2, 0] = TextureXY2UV(atlas1, 49, 2044);
					uv[2, 1] = TextureXY2UV(atlas1, 58, 2044);
					uv[2, 2] = TextureXY2UV(atlas1, 66, 2044);
					break;

				default:
					pixelInnerOffset = 9;
					pixelOuterOffset = 10;

					uv[0, 0] = TextureXY2UV(atlas1, 70, 2022);
					uv[0, 1] = TextureXY2UV(atlas1, 79, 2022);
					uv[0, 2] = TextureXY2UV(atlas1, 87, 2022);
					uv[1, 0] = TextureXY2UV(atlas1, 70, 2034);
					uv[1, 1] = TextureXY2UV(atlas1, 79, 2034);
					uv[1, 2] = TextureXY2UV(atlas1, 87, 2034);
					uv[2, 0] = TextureXY2UV(atlas1, 70, 2046);
					uv[2, 1] = TextureXY2UV(atlas1, 79, 2046);
					uv[2, 2] = TextureXY2UV(atlas1, 87, 2046);
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
														 MyVector3.Get(PointOnCircle(-width / 2, - height / 2, outerOffset, Math.PI + Math.PI / 4 )),
														 new Vector3((float)(-width / 2 - outerOffset), -height / 2),
														 //outside 1
														 new Vector3((float)(-width / 2 - outerOffset), height / 2),
														 MyVector3.Get(PointOnCircle(-width / 2, height / 2, outerOffset, Math.PI - (Math.PI - angle1) / 2)),
														 MyVector3.Get(PointOnCircle(-width / 2, height / 2, outerOffset, Math.PI - (Math.PI - angle1))),
														 //outside 3
														 MyVector3.Get(PointOnCircle(width / 2, -height / 2, outerOffset, 3 * Math.PI / 2 + (Math.PI - angle2))),
														 MyVector3.Get(PointOnCircle(width / 2, -height / 2, outerOffset, 3 * Math.PI / 2 + (Math.PI - angle2) / 2)),
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

			return mesh;
		}


		//	misc helper methods
		private static Vector2 TextureXY2UV(Texture2D texture, int textureX, int textureY)
		{
			return new Vector2((float)textureX / texture.width, (float)(texture.height - textureY) / texture.height);
		}
		private static Vector2 TextureXY2UV(Texture2D texture, IntVector2 textureXY)
		{
			return TextureXY2UV(texture, textureXY.x, textureXY.y);
		}

		private static Vector2 TextureXY2UV2(Texture2D texture, int textureX, int textureY)
		{
			return new Vector2((float)textureX / texture.width, (float)textureY / texture.height);
		}
		private static Vector2 TextureXY2UV2(Texture2D texture, IntVector2 textureXY)
		{
			return TextureXY2UV2(texture, textureXY.x, textureXY.y);
		}

		private static Vector2 PointOnCircle(double circleCenterX, double circleCenterY, double radius, double angle)
		{
			return new Vector2((float)(circleCenterX + radius * Math.Cos(angle)), (float)(circleCenterY + radius * Math.Sin(angle)));
		}
	}

	public class ItemProperties : MonoBehaviour
	{
		public ItemShape Shape;
		public ItemMaterial Material;
		public float Width, Height;
	}

	public class IntVector2
	{
		public int x, y;

		public IntVector2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	public static class MyVector3
	{
		public static Vector3 Get(Vector2 vector2)
		{
			return new Vector3(vector2.x, vector2.y);
		}
	}

	public static class MyTransform
	{
		public static void SetPositionXY(Transform transform, Vector2 positionXY)
		{
			transform.position = new Vector3(positionXY.x, positionXY.y, transform.position.z);
		}

		public static void SetPositionX(Transform transform, float positionX)
		{
			transform.position = new Vector3(positionX, transform.position.y, transform.position.z);
		}

		public static void SetPositionY(Transform transform, float positionY)
		{
			transform.position = new Vector3(transform.position.x, positionY, transform.position.z);
		}
	}
}