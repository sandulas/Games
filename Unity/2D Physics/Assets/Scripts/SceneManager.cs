using UnityEngine;
using System.Collections;
using ThisProject;

namespace ThisProject
{
	public class SceneManager : MonoBehaviour
	{
		Vector2 SceneSize = new Vector2(80, 50);
		float WallWidth = 0.5f;

		public GameObject Background;

		private Camera camera;
		private Vector2 cameraPosition;
		public static Vector2 CameraTargetPosition;


		private static bool loaded = false;
		void Start()
		{
			GameObject obj;
			Time.timeScale = 1;

			Background.transform.position = new Vector3(0, 0, 0);
			Background.transform.localScale = new Vector3(SceneSize.x, SceneSize.y, 1);
			Background.renderer.material.mainTextureScale = new Vector2(SceneSize.x / 10, SceneSize.y / 10);

			obj = Item.Create(ItemShape.Rectangle, ItemMaterial.FixedMetal, SceneSize.x, WallWidth);
			Item.Move(obj, 0, SceneSize.y / 2 + WallWidth / 2);
			obj.name = "Wall - Top";

			Item.Duplicate(obj);
			Item.Move(obj, 0, -SceneSize.y / 2 - WallWidth / 2);
			obj.name = "Wall - Bottom";

			obj = Item.Create(ItemShape.Rectangle, ItemMaterial.FixedMetal, WallWidth, SceneSize.y + WallWidth * 2);
			Item.Move(obj, SceneSize.x / 2 + WallWidth / 2, 0);
			obj.name = "Wall - Right";

			Item.Duplicate(obj);
			Item.Move(obj, -SceneSize.x / 2 - WallWidth / 2, 0);
			obj.name = "Wall - Left";

			camera = Camera.main;
			camera.transform.position = new Vector3(0, 0, -12);
			CameraTargetPosition = new Vector2(0, 0);

		}

		void Update()
		{
			UpdateCamera();


			GameObject obj;
			if (Time.realtimeSinceStartup > 1 && !loaded)
			{
				for (int i = 0; i < 5; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						obj = Item.Create((ItemShape)j, (ItemMaterial)i, 1.5f, 1.5f);
						obj.transform.position = new Vector3(i * 1.5f - 3, j * 1.5f, obj.transform.position.z);

						Item.Resize(obj, 1.5f, 0.5f);
					}
				}

				loaded = true;
			}

			//if (Time.realtimeSinceStartup > 2)
			//{
			//  obj = GameObject.Find("Item: Circle 10");
			//  Item.Resize(obj, obj.GetComponent<ItemProperties>().Width + 0.01f, 2);
			//  Item.ChangeMaterial(obj, ItemMaterial.Ice);
			//}
		}

		void UpdateCamera()
		{
			cameraPosition = camera.transform.position;

			Vector2 direction = (CameraTargetPosition - cameraPosition).normalized;

			float totalDistange = (CameraTargetPosition - cameraPosition).magnitude;
			float displacement = totalDistange * 6 * Time.deltaTime;

			cameraPosition = cameraPosition + direction * displacement;
			camera.transform.position = camera.transform.position.SetXY(cameraPosition.x, cameraPosition.y);
		}
	}
}