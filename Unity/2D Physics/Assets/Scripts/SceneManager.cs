using UnityEngine;
using System.Collections;
using ThisProject;
using System;

namespace ThisProject
{
	public enum GameStatus
	{
		Play = 0,
		Pause = 1
	}

	public class SceneManager : MonoBehaviour
	{
		public GameStatus status = GameStatus.Play;

		public static GameObject background;
		public static Camera uiCamera, mainCamera;

		public static Vector2 cameraTargetPosition;
		public static float cameraTargetSize;
		public static float pixelsPerUnit, aspectRatio;


		Vector2 playgroundSize = new Vector2(40, 25);
		float titleHeight = 10; float learnGalleryHeight = 15; float playGalleryHeight = 25;
		float wallWidth = 0.5f;
		Vector2 sceneSize;

		MyRect cameraTrap;

		Vector3 dragOffset;

		GameObject[] obj;
		Vector2[] objVelocities;
		
		bool loaded = false;

		void Start()
		{
			Time.timeScale = 1;

			sceneSize = new Vector2(playgroundSize.x + 2 * wallWidth, playgroundSize.y + 2 * wallWidth + playGalleryHeight + learnGalleryHeight + titleHeight);

			//initialize the camera
			mainCamera = Camera.main;
			mainCamera.transform.position = new Vector3(0, 0, -12);
			cameraTargetPosition = new Vector2(0, 0);
			cameraTargetSize = mainCamera.orthographicSize;

			uiCamera = Camera.allCameras[1];

			//initialize the background
			background = GameObject.Find("Background");
			background.transform.position = new Vector3(0, sceneSize.y / 2 - wallWidth - playgroundSize.y / 2, 0);
			background.transform.localScale = new Vector3(sceneSize.x, sceneSize.y, 1);
			background.renderer.material.mainTextureScale = new Vector2(sceneSize.x / 10, sceneSize.y / 10);

			//setup the walls
			GameObject wall;

			wall = Item.Create(ItemShape.Rectangle, ItemMaterial.FixedMetal, playgroundSize.x, wallWidth);
			Item.Move(wall, 0, playgroundSize.y / 2 + wallWidth / 2);
			wall.name = "Wall - Top";

			Item.Duplicate(wall);
			Item.Move(wall, 0, -playgroundSize.y / 2 - wallWidth / 2);
			wall.name = "Wall - Bottom";

			wall = Item.Create(ItemShape.Rectangle, ItemMaterial.FixedMetal, wallWidth, playgroundSize.y + wallWidth * 2);
			Item.Move(wall, playgroundSize.x / 2 + wallWidth / 2, 0);
			wall.name = "Wall - Right";

			Item.Duplicate(wall);
			Item.Move(wall, -playgroundSize.x / 2 - wallWidth / 2, 0);
			wall.name = "Wall - Left";

			//setup variables
			pixelsPerUnit = Screen.height / mainCamera.orthographicSize / 2;
			aspectRatio = (float)Screen.width / Screen.height;

			//set camera trap to the playground area
			cameraTrap = new MyRect(
				playgroundSize.y / 2 + wallWidth,
				-playgroundSize.x / 2 - wallWidth,
				-playgroundSize.y / 2 - wallWidth,
				playgroundSize.x / 2 + wallWidth);

			//setup the event handlers
			InputManager.OnTouch += new InputManager.SingleTouchHandler(InputManager_OnTouch);
			InputManager.OnDrag += new InputManager.SingleTouchHandler(InputManager_OnDrag);
			InputManager.OnRelease += new InputManager.SingleTouchHandler(InputManager_OnRelease);
			InputManager.OnTap += new InputManager.SingleTouchHandler(InputManager_OnTap);

			//-------------- TEMPORARY -----------------
			obj = new GameObject[15];
			objVelocities = new Vector2[15];
		}

		void Update()
		{
			UpdateCamera();

			if (Time.realtimeSinceStartup > 1 && !loaded)
			{
				int objIndex = 0;
				for (int i = 0; i < 5; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						obj[objIndex] = Item.Create((ItemShape)j, (ItemMaterial)i, 1.5f, 1.5f);
						obj[objIndex].transform.position = new Vector3(i * 1.5f - 3, j * 1.5f, obj[objIndex].transform.position.z);

						Item.Resize(obj[objIndex], 1.5f, 0.5f);
						objIndex++;
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
			//set and restrict the size
			if (cameraTargetSize < 3) cameraTargetSize = 3;
			else if (cameraTargetSize > cameraTrap.Height / 2) cameraTargetSize = cameraTrap.Height / 2;
			if (cameraTargetSize * aspectRatio > cameraTrap.Width / 2) cameraTargetSize = cameraTrap.Width / 2 / aspectRatio;

			mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, cameraTargetSize, 0.15f);
			pixelsPerUnit = Screen.height / mainCamera.orthographicSize / 2;

			//set and restrict the position
			MyTransform.SetPositionXY(mainCamera.transform, Vector2.Lerp(mainCamera.transform.position, cameraTargetPosition, 0.15f));

			if (mainCamera.transform.position.y > cameraTrap.Top - mainCamera.orthographicSize)
			{
				cameraTargetPosition.y = (float)Math.Round(cameraTrap.Top - mainCamera.orthographicSize, 4);
				MyTransform.SetPositionY(mainCamera.transform, cameraTargetPosition.y);
				dragOffset.y = -Input.mousePosition.y / SceneManager.pixelsPerUnit - cameraTargetPosition.y;
			}
			else if (mainCamera.transform.position.y < cameraTrap.Bottom + mainCamera.orthographicSize)
			{
				cameraTargetPosition.y = (float)Math.Round(cameraTrap.Bottom + mainCamera.orthographicSize, 4);
				MyTransform.SetPositionY(mainCamera.transform, cameraTargetPosition.y);
				dragOffset.y = -Input.mousePosition.y / SceneManager.pixelsPerUnit - cameraTargetPosition.y;
			}

			if (mainCamera.transform.position.x > cameraTrap.Right - mainCamera.orthographicSize * aspectRatio)
			{
				cameraTargetPosition.x = (float)Math.Round(cameraTrap.Right - mainCamera.orthographicSize * aspectRatio, 4);
				MyTransform.SetPositionX(mainCamera.transform, cameraTargetPosition.x);
				dragOffset.x = -Input.mousePosition.x / SceneManager.pixelsPerUnit - cameraTargetPosition.x;
			}
			else if (mainCamera.transform.position.x < cameraTrap.Left + mainCamera.orthographicSize * aspectRatio)
			{
				cameraTargetPosition.x = (float)Math.Round(cameraTrap.Left + mainCamera.orthographicSize * aspectRatio, 4);
				MyTransform.SetPositionX(mainCamera.transform, cameraTargetPosition.x);
				dragOffset.x = -Input.mousePosition.x / SceneManager.pixelsPerUnit - cameraTargetPosition.x;
			}
		}

		void InputManager_OnTouch()
		{
			//Debug.Log("Touch: " + InputManager.touchObject.name + ", " + InputManager.touchCamera.name + ", " + Input.mousePosition);

			if (InputManager.touchObject == background && InputManager.touchCamera == mainCamera)
			{
				cameraTargetPosition = mainCamera.transform.position;
				dragOffset = -(Vector2)Input.mousePosition / SceneManager.pixelsPerUnit - cameraTargetPosition;
			}
		}

		void InputManager_OnDrag()
		{
			//Debug.Log("Drag: " + InputManager.touchObject.name + ", " + InputManager.touchCamera.name + ", " + Input.mousePosition);

			if (InputManager.touchObject == background && InputManager.touchCamera == mainCamera)
			{
				cameraTargetPosition = -Input.mousePosition / pixelsPerUnit - dragOffset;
				Debug.Log(dragOffset + " - " + cameraTargetPosition + " - " + cameraTargetSize);
			}
		}

		void InputManager_OnRelease()
		{
			//Debug.Log("Release: " + InputManager.touchObject.name + ", " + InputManager.touchCamera.name + ", " + Input.mousePosition);
		}

		void InputManager_OnTap()
		{
			//Debug.Log("Tap: " + InputManager.touchObject.name + ", " + InputManager.touchCamera.name + ", " + Input.mousePosition);

			//Pause Button
			if (InputManager.touchObject.name == "PauseButton") PauseButton_Tap();

			//-------
			if (InputManager.touchObject.name.StartsWith("Item"))
			{

			}
		}

		void PauseButton_Tap()
		{

			if (status == GameStatus.Pause)
			{
				for (int i = 0; i < obj.Length; i++)
				{
					if (obj[i].GetComponent<ItemProperties>().Material != ItemMaterial.FixedMetal)
					{
						obj[i].rigidbody2D.isKinematic = false;
						obj[i].rigidbody2D.velocity = objVelocities[i];
					}
				}
				status = GameStatus.Play;
			}
			else
			{
				for (int i = 0; i < obj.Length; i++)
				{
					objVelocities[i] = obj[i].rigidbody2D.velocity;
					obj[i].rigidbody2D.isKinematic = true;
				}
				status = GameStatus.Pause;
			}
		}

	}
}