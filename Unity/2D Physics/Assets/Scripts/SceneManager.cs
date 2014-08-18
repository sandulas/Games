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
		GameObject background, buttonPause, buttonRectangle, buttonCircle, buttonTriangle, buttonFixed, buttonMetal, buttonWood, buttonRubber, buttonIce,
				   buttonMove, buttonRotate, buttonResize, buttonClone, holderControls;

		//settings
		Vector2 playgroundSize = new Vector2(40, 25);
		float titleHeight = 10;
		float learnGalleryHeight = 15;
		float playGalleryHeight = 25;
		float wallWidth = 0.5f;

		//scene variables
		GameStatus gameStatus = GameStatus.Play;
		Vector2 sceneSize;
		float dpi, pixelsPerUnit, aspectRatio, spritePixelsPerUnit;
		MyRect playViewRect, playgroundRect, uiRect, mainCameraRect;
		
		//operations variables
		Vector3 dragOffset;
		GameObject selectedItem = null;
		float initialRotation, initialInputAngle;
		Vector2 initialSize, initialPosition, initialInputPosition, resizeCorner;
		GameObject tmpGameObject;

		GameObject[] obj; int objIndex = 0;
		Vector2[] objVelocities;
		
		bool loaded = false;


		void Start()
		{
			Time.timeScale = 1;

			//define the UI objects
			mainCamera = Camera.main;
			uiCamera = Camera.allCameras[1];
			background = GameObject.Find("Background");
			buttonPause = GameObject.Find("ButtonPause");
			buttonRectangle = GameObject.Find("ButtonRectangle");
			buttonCircle = GameObject.Find("ButtonCircle");
			buttonTriangle = GameObject.Find("ButtonTriangle");
			buttonFixed = GameObject.Find("ButtonFixed");
			buttonMetal = GameObject.Find("ButtonMetal");
			buttonWood = GameObject.Find("ButtonWood");
			buttonRubber = GameObject.Find("ButtonRubber");
			buttonIce = GameObject.Find("ButtonIce");
			buttonMove = GameObject.Find("ButtonMove");
			buttonRotate = GameObject.Find("ButtonRotate");
			buttonResize = GameObject.Find("ButtonResize");
			buttonClone = GameObject.Find("ButtonClone");
			holderControls = GameObject.Find("Controls");


			//initialize the cameras
			mainCamera.transform.position = new Vector3(0, 0, -12);
			cameraTargetPosition = new Vector2(0, 0);
			cameraTargetSize = mainCamera.orthographicSize;

			//float dpi = Mathf.Clamp(Screen.dpi, 1, 1000);
			dpi = 132;
			float scaleFactor = 1 + (Screen.height / dpi - 3.5f) * 0.15f;
			uiCamera.orthographicSize = Mathf.Clamp(0.4f + Screen.height / dpi / scaleFactor, 3.6f, 5f);

			//setup variables
			aspectRatio = (float)Screen.width / Screen.height;
			pixelsPerUnit = Screen.height / mainCamera.orthographicSize / 2;
			spritePixelsPerUnit = 1536f / 10; //Screen.height / uiCamera.orthographicSize / 2;
			
			//ui area
			uiRect = new MyRect(
				uiCamera.orthographicSize,
				-uiCamera.orthographicSize * aspectRatio,
				-uiCamera.orthographicSize,
				uiCamera.orthographicSize * aspectRatio);

			//playground area
			playgroundRect = new MyRect(
				playgroundSize.y / 2,
				-playgroundSize.x / 2,
				-playgroundSize.y / 2,
				playgroundSize.x / 2);

			//playground + walls area
			playViewRect = new MyRect(
				playgroundRect.Top + wallWidth,
				playgroundRect.Left - wallWidth,
				playgroundRect.Bottom - wallWidth,
				playgroundRect.Right + wallWidth);
			
			//initialize the background
			sceneSize = new Vector2(playgroundSize.x + 2 * wallWidth, playgroundSize.y + 2 * wallWidth + playGalleryHeight + learnGalleryHeight + titleHeight);
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

			//position the pause button
			MyTransform.SetPositionXY(buttonPause.transform,	uiRect.Left + 0.5f, uiRect.Top - 0.5f);

			//position the toolbar
			MyTransform.SetPositionXY(GameObject.Find("Toolbar").transform, uiRect.Right, 0);
			GameObject gameObject = GameObject.Find("ToolbarBackground");
			MyTransform.SetPositionXY(gameObject.transform, uiRect.Right, uiCamera.orthographicSize + 0.01f);
			MyTransform.SetScaleY(gameObject.transform, (uiCamera.orthographicSize + 0.02f) * 2 * spritePixelsPerUnit / gameObject.GetComponent<SpriteRenderer>().sprite.rect.height);

			MyTransform.SetPositionXY(buttonRectangle.transform, uiRect.Right + 0.05f, uiRect.Top - 0.1f);
			MyTransform.SetPositionXY(buttonCircle.transform, uiRect.Right + 0.05f, uiRect.Top - 1.2f - 0.1f);
			MyTransform.SetPositionXY(buttonTriangle.transform, uiRect.Right + 0.05f, uiRect.Top - 2.2f - 0.1f);

			MyTransform.SetPositionXY(buttonFixed.transform, uiRect.Right + 0.03f, uiRect.Bottom + 2.8f + 0.8f);
			MyTransform.SetPositionXY(buttonMetal.transform, uiRect.Right + 0.03f, uiRect.Bottom + 2.1f + 0.8f);
			MyTransform.SetPositionXY(buttonWood.transform, uiRect.Right + 0.03f, uiRect.Bottom + 1.4f + 0.8f);
			MyTransform.SetPositionXY(buttonRubber.transform, uiRect.Right + 0.03f, uiRect.Bottom + 0.7f + 0.8f);
			MyTransform.SetPositionXY(buttonIce.transform, uiRect.Right + 0.03f, uiRect.Bottom + 0.8f);

			HideControls();

			//setup the event handlers
			InputManager.OnTouch += new InputManager.SingleTouchHandler(InputManager_OnTouch);
			InputManager.OnDrag += new InputManager.SingleTouchHandler(InputManager_OnDrag);
			InputManager.OnRelease += new InputManager.SingleTouchHandler(InputManager_OnRelease);
			InputManager.OnTap += new InputManager.SingleTouchHandler(InputManager_OnTap);

			//-------------- TEMPORARY -----------------
			obj = new GameObject[0];
			objVelocities = new Vector2[0];
			tmpGameObject = new GameObject();
		}

		void Update()
		{
			UpdateCamera();

			if (Time.realtimeSinceStartup > 1 && !loaded)
			{
				for (int i = 0; i < 5; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						Array.Resize<GameObject>(ref obj, objIndex + 1);
						Array.Resize<Vector2>(ref objVelocities, objIndex + 1);

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
			else if (cameraTargetSize > playViewRect.Height / 2) cameraTargetSize = playViewRect.Height / 2;
			if (cameraTargetSize * aspectRatio > playViewRect.Width / 2) cameraTargetSize = playViewRect.Width / 2 / aspectRatio;

			//set the size(animated) and update variables
			mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, cameraTargetSize, 10f * Time.deltaTime);
			
			pixelsPerUnit = Screen.height / mainCamera.orthographicSize / 2;
			mainCameraRect = new MyRect(
				playViewRect.Top - mainCamera.orthographicSize,
				playViewRect.Left + +mainCamera.orthographicSize * aspectRatio,
				playViewRect.Bottom + mainCamera.orthographicSize,
				playViewRect.Right - mainCamera.orthographicSize * aspectRatio);

			//set the position(animated)
			MyTransform.SetPositionXY(mainCamera.transform, Vector2.Lerp(mainCamera.transform.position, cameraTargetPosition, 10f * Time.deltaTime));

			//restrict the position
			Vector2 trappedPosition = mainCameraRect.GetInsidePosition(mainCamera.transform.position);

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


		//Events
		void InputManager_OnTouch()
		{
			//start dragging the camera
			if (InputManager.touchObject == background && InputManager.touchCamera == mainCamera)
			{
				cameraTargetPosition = mainCamera.transform.position;
				dragOffset = -(Vector2)Input.mousePosition / pixelsPerUnit - cameraTargetPosition;
			}

			//bring to front and start move
			if (InputManager.touchObject.name.StartsWith("Item"))
			{
				dragOffset = InputManager.touchPosition - InputManager.touchObject.transform.position;
				if (gameStatus == GameStatus.Pause) Item.BringToFront(InputManager.touchObject);
			}

			//start move
			if (InputManager.touchObject == buttonMove)
			{
				Item.BringToFront(InputManager.touchObject);
				DragObject(selectedItem);
			}
			
			//start rotate
			if (InputManager.touchObject == buttonRotate)
			{
				initialInputAngle = Vector2.Angle(mainCamera.ScreenToWorldPoint(Input.mousePosition) - selectedItem.transform.position, Vector2.right);
				if (mainCamera.ScreenToWorldPoint(Input.mousePosition).y < selectedItem.transform.position.y) initialInputAngle = 360 - initialInputAngle;

				initialRotation = selectedItem.transform.eulerAngles.z;
			}
			
			//start resize
			if (InputManager.touchObject == buttonResize)
			{
				tmpGameObject.transform.position = selectedItem.transform.position;
				tmpGameObject.transform.rotation = selectedItem.transform.rotation;
				selectedItem.transform.parent = tmpGameObject.transform;

				initialSize = new Vector2(selectedItem.GetComponent<ItemProperties>().width, selectedItem.GetComponent<ItemProperties>().height);
				initialPosition = selectedItem.transform.localPosition;
				initialInputPosition = tmpGameObject.transform.InverseTransformPoint(mainCamera.ScreenToWorldPoint(Input.mousePosition));
			}
			
			//create and start dragging a new item
			if (InputManager.touchObject == buttonRectangle)
				CreateItem(ItemShape.Rectangle, ItemMaterial.Wood);
			else if (InputManager.touchObject == buttonCircle)
				CreateItem(ItemShape.Circle, ItemMaterial.Rubber);
			else if (InputManager.touchObject == buttonTriangle)
				CreateItem(ItemShape.Triangle, ItemMaterial.Ice);

			HideControls();
		}

		void InputManager_OnDrag()
		{
			//camera
			if (InputManager.touchObject == background && InputManager.touchCamera == mainCamera)
			{
				cameraTargetPosition = -Input.mousePosition / pixelsPerUnit - dragOffset;
			}

			//item
			if (InputManager.touchObject.name.StartsWith("Item"))
			{
				//if (InputManager.touchObject.GetComponent<ItemProperties>().Material == ItemMaterial.FixedMetal)
				//{
				//	InputManager.touchObject.rigidbody2D.isKinematic = false;
				//}

				Vector2 trappedPosition = playgroundRect.GetInsidePosition(InputManager.touchPosition - dragOffset);

				if (gameStatus == GameStatus.Pause)
					Item.Move(InputManager.touchObject, trappedPosition);
				else
					InputManager.touchObject.rigidbody2D.MovePosition(trappedPosition);
			}

			//rotate button
			if (InputManager.touchObject == buttonRotate)
			{
				float currentAngle = Vector2.Angle(mainCamera.ScreenToWorldPoint(Input.mousePosition) - selectedItem.transform.position, Vector2.right);
				if (mainCamera.ScreenToWorldPoint(Input.mousePosition).y < selectedItem.transform.position.y) currentAngle = 360 - currentAngle;
	
				selectedItem.transform.eulerAngles = new Vector3(0, 0, (float)Math.Round(initialRotation + currentAngle - initialInputAngle, MidpointRounding.AwayFromZero));
			}

			//resize button
			if (InputManager.touchObject == buttonResize)
			{
				Vector2 currentInputPosition = tmpGameObject.transform.InverseTransformPoint(mainCamera.ScreenToWorldPoint(Input.mousePosition));
				Vector2 resizeOffset = Vector2.Scale(currentInputPosition - initialInputPosition, resizeCorner);

				Item.Resize(selectedItem, initialSize.x + resizeOffset.x, initialSize.y + resizeOffset.y);

				Vector2 moveOffset = Vector2.Scale(resizeOffset / 2, resizeCorner);
				MyTransform.SetLocalPositionXY(selectedItem.transform, initialPosition + moveOffset);
			}
		}

		void InputManager_OnRelease()
		{
			//Items
			if (InputManager.touchObject.name.StartsWith("Item"))
			{
				if (gameStatus != GameStatus.Pause) return;

				selectedItem = InputManager.touchObject;
				PositionControls();
			}
			
			if (InputManager.touchObject == buttonRotate || InputManager.touchObject == buttonResize)
			{
				PositionControls();
			}

			if (InputManager.touchObject == buttonResize)
			{
				selectedItem.transform.parent = null;
			}
			
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
			//Pause Button
			if (InputManager.touchObject == buttonPause) Pause();
		}


		//Functions
		void Pause()
		{

			if (gameStatus == GameStatus.Pause)
			{
				for (int i = 0; i < obj.Length; i++)
				{
					if (obj[i].GetComponent<ItemProperties>().material != ItemMaterial.FixedMetal)
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

		void CreateItem(ItemShape itemShape, ItemMaterial itemMaterial)
		{
			Array.Resize<GameObject>(ref obj, objIndex + 1);
			Array.Resize<Vector2>(ref objVelocities, objIndex + 1);

			float size = 1f / uiCamera.orthographicSize * mainCamera.orthographicSize;

			obj[objIndex] = Item.Create(itemShape, itemMaterial, size, size);
			objVelocities[objIndex] = Vector2.zero;

			if (gameStatus == GameStatus.Pause)
				obj[objIndex].rigidbody2D.isKinematic = true;

			MyTransform.SetPositionXY(obj[objIndex].transform, mainCamera.ScreenToWorldPoint(Input.mousePosition));
			DragObject(obj[objIndex]);

			objIndex++;
		}

		void PositionControls()
		{
			holderControls.SetActive(true);

			ItemProperties itemProperties = selectedItem.GetComponent<ItemProperties>();

			float offset = (Screen.height / uiCamera.orthographicSize / 2 * 0.26f) / pixelsPerUnit;
			float width, height;
			if (itemProperties.shape == ItemShape.Circle)
			{
				width = (itemProperties.width) / (float)Math.Sqrt(2);
				height = width;
			}
			else
			{
				width = itemProperties.width;
				height = itemProperties.height;
			}

			Vector2[] corners = new Vector2[4];
			corners[0] = selectedItem.transform.TransformPoint(-width / 2 - offset, -height / 2 - offset, selectedItem.transform.localPosition.z);
			corners[1] = selectedItem.transform.TransformPoint(width / 2 + offset, -height / 2 - offset, selectedItem.transform.localPosition.z);
			corners[2] = selectedItem.transform.TransformPoint(width / 2 + offset, height / 2 + offset, selectedItem.transform.localPosition.z);
			corners[3] = selectedItem.transform.TransformPoint(-width / 2 - offset, height / 2 + offset, selectedItem.transform.localPosition.z);

			Array.Sort(corners, delegate(Vector2 v1, Vector2 v2) { return v1.y.CompareTo(v2.y); });

			if (Math.Abs(corners[0].y - corners[3].y) >= Math.Abs(corners[1].x - corners[2].x))
			{
				if (corners[0].x >= corners[1].x)
				{
					MyTransform.SetPositionXY(buttonMove.transform, uiCamera.ScreenToWorldPoint(mainCamera.WorldToScreenPoint(corners[1])));
					MyTransform.SetPositionXY(buttonResize.transform, uiCamera.ScreenToWorldPoint(mainCamera.WorldToScreenPoint(corners[0])));
				}
				else
				{
					MyTransform.SetPositionXY(buttonMove.transform, uiCamera.ScreenToWorldPoint(mainCamera.WorldToScreenPoint(corners[0])));
					MyTransform.SetPositionXY(buttonResize.transform, uiCamera.ScreenToWorldPoint(mainCamera.WorldToScreenPoint(corners[1])));
				}

				if (corners[2].x >= corners[3].x)
				{
					MyTransform.SetPositionXY(buttonRotate.transform, uiCamera.ScreenToWorldPoint(mainCamera.WorldToScreenPoint(corners[3])));
					MyTransform.SetPositionXY(buttonClone.transform, uiCamera.ScreenToWorldPoint(mainCamera.WorldToScreenPoint(corners[2])));
				}
				else
				{
					MyTransform.SetPositionXY(buttonRotate.transform, uiCamera.ScreenToWorldPoint(mainCamera.WorldToScreenPoint(corners[2])));
					MyTransform.SetPositionXY(buttonClone.transform, uiCamera.ScreenToWorldPoint(mainCamera.WorldToScreenPoint(corners[3])));
				}
			}
			else
			{
				Array.Sort(corners, delegate(Vector2 v1, Vector2 v2) { return v1.x.CompareTo(v2.x); });

				if (corners[0].y >= corners[1].y)
				{
					MyTransform.SetPositionXY(buttonMove.transform, uiCamera.ScreenToWorldPoint(mainCamera.WorldToScreenPoint(corners[1])));
					MyTransform.SetPositionXY(buttonRotate.transform, uiCamera.ScreenToWorldPoint(mainCamera.WorldToScreenPoint(corners[0])));
				}
				else
				{
					MyTransform.SetPositionXY(buttonMove.transform, uiCamera.ScreenToWorldPoint(mainCamera.WorldToScreenPoint(corners[0])));
					MyTransform.SetPositionXY(buttonRotate.transform, uiCamera.ScreenToWorldPoint(mainCamera.WorldToScreenPoint(corners[1])));
				}

				if (corners[2].y >= corners[3].y)
				{
					MyTransform.SetPositionXY(buttonResize.transform, uiCamera.ScreenToWorldPoint(mainCamera.WorldToScreenPoint(corners[3])));
					MyTransform.SetPositionXY(buttonClone.transform, uiCamera.ScreenToWorldPoint(mainCamera.WorldToScreenPoint(corners[2])));
				}
				else
				{
					MyTransform.SetPositionXY(buttonResize.transform, uiCamera.ScreenToWorldPoint(mainCamera.WorldToScreenPoint(corners[2])));
					MyTransform.SetPositionXY(buttonClone.transform, uiCamera.ScreenToWorldPoint(mainCamera.WorldToScreenPoint(corners[3])));
				}
			}

			resizeCorner = selectedItem.transform.InverseTransformPoint(mainCamera.ScreenToWorldPoint(uiCamera.WorldToScreenPoint(buttonResize.transform.position)));
			resizeCorner.x = Math.Sign(resizeCorner.x);
			resizeCorner.y = Math.Sign(resizeCorner.y);
		}

		void HideControls()
		{
			holderControls.SetActive(false);
		}

		void DragObject(GameObject obj)
		{
			InputManager.touchCamera = mainCamera;
			InputManager.touchPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
			InputManager.touchObject = obj;
			dragOffset = InputManager.touchPosition - InputManager.touchObject.transform.position;
		}
	}
}