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
		//public
		public static Camera uiCamera, mainCamera;
		public static Vector2 cameraTargetPosition;
		public static float cameraTargetSize;

		//UI
		GameObject background, buttonPause, buttonRectangle, buttonCircle, buttonTriangle, buttonFixed, buttonMetal, buttonWood, buttonRubber, buttonIce;

		//settings
		Vector2 playgroundSize = new Vector2(40, 25);
		float titleHeight = 10;
		float learnGalleryHeight = 15;
		float playGalleryHeight = 25;
		float wallWidth = 0.5f;

		//variables
		GameStatus gameStatus = GameStatus.Play;
		Vector2 sceneSize;
		float pixelsPerUnit, aspectRatio, uiPixelsPerUnit, uiTop, uiBottom, uiLeft, uiRight;
		MyRect cameraViewTrap, cameraTrap, playgroundTrap;
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

			//set camera trap to the playground area + the walls
			cameraViewTrap = new MyRect(
				playgroundSize.y / 2 + wallWidth,
				-playgroundSize.x / 2 - wallWidth,
				-playgroundSize.y / 2 - wallWidth,
				playgroundSize.x / 2 + wallWidth);

			//set the playground trap to the playground
			playgroundTrap = new MyRect(
				playgroundSize.y / 2,
				-playgroundSize.x / 2,
				-playgroundSize.y / 2,
				playgroundSize.x / 2);

			uiCamera = Camera.allCameras[1];
			//float dpi = Mathf.Clamp(Screen.dpi, 1, 1000);
			float dpi = 132;
			float scaleFactor = 1 + (Screen.height / dpi - 3.5f) * 0.15f;
			uiCamera.orthographicSize = Mathf.Clamp(0.4f + Screen.height / dpi / scaleFactor, 3.6f, 5f);
			
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
			aspectRatio = (float)Screen.width / Screen.height;
			pixelsPerUnit = Screen.height / mainCamera.orthographicSize / 2;
			uiPixelsPerUnit = 1536f / 10; //Screen.height / uiCamera.orthographicSize / 2;
			uiTop = uiCamera.orthographicSize;
			uiBottom = -uiCamera.orthographicSize;
			uiLeft = -uiCamera.orthographicSize * aspectRatio;
			uiRight = uiCamera.orthographicSize * aspectRatio;

			//position the pause button
			MyTransform.SetPositionXY(GameObject.Find("PauseButton").transform,	uiLeft + 0.5f, uiTop - 0.5f);

			//position the toolbar
			MyTransform.SetPositionXY(GameObject.Find("Toolbar").transform, uiRight, 0);
			
			GameObject gameObject = GameObject.Find("ToolbarBackground");
			MyTransform.SetPositionXY(gameObject.transform, uiRight, uiCamera.orthographicSize + 0.01f);
			MyTransform.SetScaleY(gameObject.transform, (uiCamera.orthographicSize + 0.02f) * 2 * uiPixelsPerUnit / gameObject.GetComponent<SpriteRenderer>().sprite.rect.height);

			MyTransform.SetPositionXY(GameObject.Find("Rectangle").transform, uiRight + 0.05f, uiTop - 0.1f);
			MyTransform.SetPositionXY(GameObject.Find("Circle").transform, uiRight + 0.05f, uiTop - 1.2f - 0.1f);
			MyTransform.SetPositionXY(GameObject.Find("Triangle").transform, uiRight + 0.05f, uiTop - 2.2f - 0.1f);

			MyTransform.SetPositionXY(GameObject.Find("Material_Ice").transform, uiRight + 0.03f, uiBottom + 0.8f);
			MyTransform.SetPositionXY(GameObject.Find("Material_Rubber").transform, uiRight + 0.03f, uiBottom + 0.7f + 0.8f);
			MyTransform.SetPositionXY(GameObject.Find("Material_Wood").transform, uiRight + 0.03f, uiBottom + 1.4f + 0.8f);
			MyTransform.SetPositionXY(GameObject.Find("Material_Metal").transform, uiRight + 0.03f, uiBottom + 2.1f + 0.8f);
			MyTransform.SetPositionXY(GameObject.Find("Material_Fixed").transform, uiRight + 0.03f, uiBottom + 2.8f + 0.8f);


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
			//restrict the size
			if (cameraTargetSize < 3) cameraTargetSize = 3;
			else if (cameraTargetSize > cameraViewTrap.Height / 2) cameraTargetSize = cameraViewTrap.Height / 2;
			if (cameraTargetSize * aspectRatio > cameraViewTrap.Width / 2) cameraTargetSize = cameraViewTrap.Width / 2 / aspectRatio;

			//set the size(animated) and update variables
			mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, cameraTargetSize, 10f * Time.deltaTime);
			
			pixelsPerUnit = Screen.height / mainCamera.orthographicSize / 2;
			cameraTrap = new MyRect(
				cameraViewTrap.Top - mainCamera.orthographicSize,
				cameraViewTrap.Left + +mainCamera.orthographicSize * aspectRatio,
				cameraViewTrap.Bottom + mainCamera.orthographicSize,
				cameraViewTrap.Right - mainCamera.orthographicSize * aspectRatio);

			//set the position(animated)
			MyTransform.SetPositionXY(mainCamera.transform, Vector2.Lerp(mainCamera.transform.position, cameraTargetPosition, 10f * Time.deltaTime));

			//restrict the position
			Vector2 trappedPosition = cameraTrap.GetInsidePosition(mainCamera.transform.position);

			if (trappedPosition.x != mainCamera.transform.position.x)
			{
				cameraTargetPosition.x = trappedPosition.x;
				MyTransform.SetPositionX(mainCamera.transform, cameraTargetPosition.x);

				if (InputManager.touchObject == background && InputManager.touchCamera == mainCamera)
					dragOffset.x = -Input.mousePosition.x / pixelsPerUnit - cameraTargetPosition.x;
			}

			if (trappedPosition.y != mainCamera.transform.position.y)
			{
				cameraTargetPosition.y = trappedPosition.y;
				MyTransform.SetPositionY(mainCamera.transform, cameraTargetPosition.y);

				if (InputManager.touchObject == background && InputManager.touchCamera == mainCamera)
					dragOffset.y = -Input.mousePosition.y / pixelsPerUnit - cameraTargetPosition.y;
			}
		}

		void InputManager_OnTouch()
		{
			//Debug.Log("Touch: " + InputManager.touchObject.name + ", " + InputManager.touchCamera.name + ", " + Input.mousePosition);

			//camera
			if (InputManager.touchObject == background && InputManager.touchCamera == mainCamera)
			{
				cameraTargetPosition = mainCamera.transform.position;
				dragOffset = -(Vector2)Input.mousePosition / pixelsPerUnit - cameraTargetPosition;
			}

			//item
			if (InputManager.touchObject.name.StartsWith("Item"))
			{
				dragOffset = InputManager.touchPosition - InputManager.touchObject.transform.position;
			}
		}

		void InputManager_OnDrag()
		{
			//camera
			if (InputManager.touchObject == background && InputManager.touchCamera == mainCamera)
			{
				cameraTargetPosition = -Input.mousePosition / pixelsPerUnit - dragOffset;
				//Debug.Log(dragOffset + " - " + cameraTargetPosition + " - " + cameraTargetSize);
			}

			//item
			if (InputManager.touchObject.name.StartsWith("Item"))
			{
				//if (InputManager.touchObject.GetComponent<ItemProperties>().Material == ItemMaterial.FixedMetal)
				//{
				//	InputManager.touchObject.rigidbody2D.isKinematic = false;
				//}

				Vector2 trappedPosition = playgroundTrap.GetInsidePosition(InputManager.touchPosition - dragOffset);

				if (gameStatus == GameStatus.Pause)
					Item.Move(InputManager.touchObject, trappedPosition);
				else
					InputManager.touchObject.rigidbody2D.MovePosition(trappedPosition);
			}

		}

		void InputManager_OnRelease()
		{
			//item
			//if (InputManager.touchObject.name.StartsWith("Item"))
			//{
			//	if (InputManager.touchObject.GetComponent<ItemProperties>().Material == ItemMaterial.FixedMetal)
			//	{
			//		InputManager.touchObject.rigidbody2D.isKinematic = true;
			//		InputManager.touchObject.rigidbody2D.angularVelocity = 0;
			//	}
			//}
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

			if (gameStatus == GameStatus.Pause)
			{
				for (int i = 0; i < obj.Length; i++)
				{
					if (obj[i].GetComponent<ItemProperties>().Material != ItemMaterial.FixedMetal)
					{
						obj[i].rigidbody2D.isKinematic = false;
						obj[i].rigidbody2D.velocity = objVelocities[i];
					}
				}
				gameStatus = GameStatus.Play;
			}
			else
			{
				for (int i = 0; i < obj.Length; i++)
				{
					objVelocities[i] = obj[i].rigidbody2D.velocity;
					obj[i].rigidbody2D.isKinematic = true;
				}
				gameStatus = GameStatus.Pause;
			}
		}

	}
}