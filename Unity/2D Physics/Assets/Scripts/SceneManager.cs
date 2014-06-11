using UnityEngine;
using System.Collections;
using ThisProject;

namespace ThisProject
{
	public class SceneManager : MonoBehaviour
	{
		Vector2 SceneSize = new Vector2(40, 25);
		float WallWidth = 0.5f;

		public static GameObject Background;
		public static Camera UICamera;

		private Camera gameCamera;
		private Vector2 cameraPosition;
		public static Vector2 CameraTargetPosition;
		public static float CameraTargetSize;
		public static float PixelsPerUnit, AspectRatio;


		private static bool loaded = false;
		void Start()
		{
			Time.timeScale = 1;

			//initialize the camera
			gameCamera = Camera.main;
			gameCamera.transform.position = new Vector3(0, 0, -12);
			CameraTargetPosition = new Vector2(0, 0);
			CameraTargetSize = gameCamera.orthographicSize;

			UICamera = Camera.allCameras[1];

			//initialize the background
			Background = GameObject.Find("Background");
			Background.transform.position = new Vector3(0, 0, 0);
			Background.transform.localScale = new Vector3(SceneSize.x, SceneSize.y, 1);
			Background.renderer.material.mainTextureScale = new Vector2(SceneSize.x / 10, SceneSize.y / 10);

			//setup the walls
			GameObject obj;

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
			
			//setup variables
			PixelsPerUnit = Screen.height / gameCamera.orthographicSize / 2;
			AspectRatio = (float)Screen.width / Screen.height;
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
			//set the size
			if (CameraTargetSize < 3) CameraTargetSize = 3;
			else if (CameraTargetSize > SceneSize.y / 2 + WallWidth) CameraTargetSize = SceneSize.y / 2 + WallWidth;

			gameCamera.orthographicSize = Mathf.Lerp(gameCamera.orthographicSize, CameraTargetSize, 0.1f);
			PixelsPerUnit = Screen.height / gameCamera.orthographicSize / 2;

			//set the position
			MyTransform.SetPositionXY(gameCamera.transform, Vector2.Lerp(gameCamera.transform.position, CameraTargetPosition, 0.06f));
			if (gameCamera.transform.position.y > SceneSize.y / 2 + WallWidth - gameCamera.orthographicSize)
			{
				CameraTargetPosition.y = SceneSize.y / 2 + WallWidth - gameCamera.orthographicSize;
				MyTransform.SetPositionY(gameCamera.transform, CameraTargetPosition.y);
			}
			else if (gameCamera.transform.position.y < -SceneSize.y / 2 - WallWidth + gameCamera.orthographicSize)
			{
				CameraTargetPosition.y = -SceneSize.y / 2 - WallWidth + gameCamera.orthographicSize;
				MyTransform.SetPositionY(gameCamera.transform, CameraTargetPosition.y);
			}
			if (gameCamera.transform.position.x > SceneSize.x / 2 + WallWidth - gameCamera.orthographicSize * AspectRatio)
			{
				CameraTargetPosition.x = SceneSize.x / 2 + WallWidth - gameCamera.orthographicSize * AspectRatio;
				MyTransform.SetPositionX(gameCamera.transform, CameraTargetPosition.x);
			}
			else if (gameCamera.transform.position.x < -SceneSize.x / 2 - WallWidth + gameCamera.orthographicSize * AspectRatio)
			{
				CameraTargetPosition.x = -SceneSize.x / 2 - WallWidth + gameCamera.orthographicSize * AspectRatio;
				MyTransform.SetPositionX(gameCamera.transform, CameraTargetPosition.x);
			}
		}

	}
}