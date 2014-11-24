using UnityEngine;
using System.Collections;
using Sandulas;
using ThisProject;
using System;

public class Main : MonoBehaviour
{
	#region Properties and variables

	//public
	public Camera uiCamera, gameCamera;
	public GameObject master, background;
	
	//game
	GameObject
		buttonLearnGallery, buttonPlayGallery, titlePlay, titleLearn,
		buttonMenu,	buttonPlay, buttonPause, buttonStop,
		toolbar, buttonRectangle, buttonCircle, buttonTriangle, buttonFixed, buttonMetal, buttonWood, buttonRubber, buttonIce,
		buttonMove, buttonRotate, buttonResize, buttonClone, itemControlsHolder;

	GameStatus gameStatus = GameStatus.Stop;

	GameObject selectedItem = null;
	ItemProperties selectedItemProps = null;

	PhysicsObject[] items = new PhysicsObject[0];
	GameObject itemsContainer;

	//operations
	float initialRotation, initialInputAngle;
	Vector2 initialSize, initialPosition, initialInputPosition, resizeCorner;
	GameObject resizeParent;
	bool isItemDragged = false;
	int makeKinematic = 0;

	
	//scene setup
	float dpi, pixelsPerUnit, aspectRatio, spritePixelsPerUnit;
	
	MyRect playgroundRect = new MyRect(10, -15, -10, 15);
	float wallWidth = 1f;
	
	MyRect
		homeRect, learnGalleryRect, playGalleryRect, gameRect,
		gameUIRect;
	
	//camera
	Vector2 cameraTargetPosition;
	float cameraTargetSize;
	float cameraDefaultSize = 5f;
	MyRect cameraTrapRect, targetCameraTrapRect;

	GameObject cameraFollowObject;
	Vector3 cameraDragOffset;
	bool isCameraDragged = false;
	float doubleTouchDistance = 0;
	float doubleTouchDistanceOffset;

	#endregion

	void Start()
	{
		SetupScene();
		SetupUI();

		Physics2D.gravity = Vector2.zero;
		Application.targetFrameRate = 60;
		targetCameraTrapRect = gameRect;

	}

	void SetupScene()
	{
		//initialize the main camera
		gameCamera.transform.position = new Vector3(0, 0, -12);
		cameraTargetPosition = new Vector2(0, 0);
		cameraTargetSize = cameraDefaultSize;

		//setup variables
		aspectRatio = (float)Screen.width / Screen.height;
		pixelsPerUnit = Screen.height / gameCamera.orthographicSize / 2;
		spritePixelsPerUnit = 1536f / 10; //Screen.height / uiCamera.orthographicSize / 2;

		//playground + walls area
		gameRect = new MyRect(
			playgroundRect.Top + wallWidth,
			playgroundRect.Left - wallWidth,
			playgroundRect.Bottom - wallWidth,
			playgroundRect.Right + wallWidth);
		gameRect.Draw();

		playGalleryRect = new MyRect(
			gameRect.Top + cameraDefaultSize * 2,
			-cameraDefaultSize * aspectRatio,
			gameRect.Top,
			cameraDefaultSize * aspectRatio);
		playGalleryRect.Draw();

		learnGalleryRect = new MyRect(
			playGalleryRect.Top + 6f,
			-cameraDefaultSize * aspectRatio,
			playGalleryRect.Top,
			cameraDefaultSize * aspectRatio);
		learnGalleryRect.Draw();

		homeRect = new MyRect(
			learnGalleryRect.Top + cameraDefaultSize * 2,
			-cameraDefaultSize * aspectRatio,
			learnGalleryRect.Top,
			cameraDefaultSize * aspectRatio);
		homeRect.Draw();

		//initialize the background
		Vector2 sceneSize = new Vector2(gameRect.Width, gameRect.Height + playGalleryRect.Height + learnGalleryRect.Height + homeRect.Height);
		background.transform.position = new Vector3(0, (sceneSize.y - gameRect.Height) / 2, 0);
		background.transform.localScale = new Vector3(sceneSize.x, sceneSize.y, 1);
		background.renderer.material.mainTextureScale = new Vector2(sceneSize.x / 10, sceneSize.y / 10);

		//setup the walls
		GameObject wall;

		wall = Item.Create(ItemShape.Rectangle, ItemMaterial.FixedMetal, playgroundRect.Width , wallWidth);
		Item.Move(wall, 0, playgroundRect.Height / 2 + wallWidth / 2);
		wall.name = "Wall - Top";

		Item.Duplicate(wall);
		Item.Move(wall, 0, -playgroundRect.Height / 2 - wallWidth / 2);
		wall.name = "Wall - Bottom";

		wall = Item.Create(ItemShape.Rectangle, ItemMaterial.FixedMetal, wallWidth, playgroundRect.Height + wallWidth * 2);
		Item.Move(wall, playgroundRect.Width / 2 + wallWidth / 2, 0);
		wall.name = "Wall - Right";

		Item.Duplicate(wall);
		Item.Move(wall, -playgroundRect.Width / 2 - wallWidth / 2, 0);
		wall.name = "Wall - Left";

		//create the resize parent helper object and items container
		resizeParent = new GameObject("Temporary parent for Resize");
		itemsContainer = new GameObject("Objects Container");
		itemsContainer.transform.position = playgroundRect.Center;
	}
	void SetupUI()
	{
		//setup UI camera size depending on the screen size
		if (Screen.dpi == 0) dpi = 270;
		else dpi = Screen.dpi;
		dpi = Mathf.Clamp(dpi, 100, 700);//dpi = 132;
		float scaleFactor = 1 + (Screen.height / dpi - 3.5f) * 0.15f;
		uiCamera.orthographicSize = Mathf.Clamp(0.4f + Screen.height / dpi / scaleFactor, 3.6f, 5f);

		//UI area
		gameUIRect = new MyRect(
			uiCamera.orthographicSize,
			-uiCamera.orthographicSize * aspectRatio,
			-uiCamera.orthographicSize,
			uiCamera.orthographicSize * aspectRatio);

		//define the UI objects
		buttonLearnGallery = GameObject.Find("ButtonLearnGallery");
		buttonPlayGallery = GameObject.Find("ButtonPlayGallery");
		titleLearn = GameObject.Find("TitleLearn");
		titlePlay = GameObject.Find("TitlePlay");
		
		buttonMenu = GameObject.Find("ButtonMenu");
		buttonPlay = GameObject.Find("ButtonPlay");
		buttonPause = GameObject.Find("ButtonPause");
		buttonStop = GameObject.Find("ButtonStop");

		toolbar = GameObject.Find("Toolbar");
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
		itemControlsHolder = GameObject.Find("Controls");

		//position the home elements
		float unit = learnGalleryRect.Width / 1000;
		MyTransform.SetPositionXY(buttonLearnGallery.transform, -1.2f, homeRect.Bottom + 1.2f);
		MyTransform.SetPositionXY(buttonPlayGallery.transform, 1.2f, homeRect.Bottom + 1.2f);

		//position the gallery elements
		MyTransform.SetPositionXY(titleLearn.transform, learnGalleryRect.Left + unit * 25, learnGalleryRect.Top - unit * 15);
		MyTransform.SetPositionXY(titlePlay.transform, playGalleryRect.Left + unit * 25, playGalleryRect.Top - unit * 15);
		MyTransform.SetScaleXY(titleLearn.transform, unit * 80, unit * 80);
		MyTransform.SetScaleXY(titlePlay.transform, unit * 80, unit * 80);
		GameObject gameObject = GameObject.Find("ItemBackground");
		MyTransform.SetPositionXY(
			gameObject.transform,
			learnGalleryRect.Left + unit * 20 + gameObject.GetComponent<SpriteRenderer>().sprite.rect.width / spritePixelsPerUnit * gameObject.transform.localScale.x / 2,
			learnGalleryRect.Top - unit * 70 - gameObject.GetComponent<SpriteRenderer>().sprite.rect.height / spritePixelsPerUnit * gameObject.transform.localScale.y / 2);

		//position the UI buttons
		MyTransform.SetPositionXY(buttonMenu.transform, gameUIRect.Left + 0.5f, gameUIRect.Top - 0.5f);
		MyTransform.SetPositionXY(buttonPlay.transform, gameUIRect.Left + gameUIRect.Width / 2 - 0.6f, gameUIRect.Top - 0.65f);
		MyTransform.SetPositionXY(buttonPause.transform, gameUIRect.Left + gameUIRect.Width / 2 - 0.6f, gameUIRect.Top - 0.65f);
		buttonPause.SetActive(false);
		MyTransform.SetPositionXY(buttonStop.transform, gameUIRect.Left + gameUIRect.Width / 2 + 0.6f, gameUIRect.Top - 0.65f);
		buttonStop.SetActive(false);

		//position the toolbar
		MyTransform.SetPositionXY(toolbar.transform, gameUIRect.Right, 0);
		gameObject = GameObject.Find("ToolbarBackground");
		MyTransform.SetPositionXY(gameObject.transform, gameUIRect.Right, uiCamera.orthographicSize + 0.01f);
		MyTransform.SetScaleY(gameObject.transform, (uiCamera.orthographicSize + 0.02f) * 2 * spritePixelsPerUnit / gameObject.GetComponent<SpriteRenderer>().sprite.rect.height);

		MyTransform.SetPositionXY(buttonRectangle.transform, gameUIRect.Right + 0.05f, gameUIRect.Top - 0.1f);
		MyTransform.SetPositionXY(buttonCircle.transform, gameUIRect.Right + 0.05f, gameUIRect.Top - 1.2f - 0.1f);
		MyTransform.SetPositionXY(buttonTriangle.transform, gameUIRect.Right + 0.05f, gameUIRect.Top - 2.2f - 0.1f);

		MyTransform.SetPositionXY(buttonFixed.transform, gameUIRect.Right + 0.03f, gameUIRect.Bottom + 2.8f + 0.8f);
		MyTransform.SetPositionXY(buttonMetal.transform, gameUIRect.Right + 0.03f, gameUIRect.Bottom + 2.1f + 0.8f);
		MyTransform.SetPositionXY(buttonWood.transform, gameUIRect.Right + 0.03f, gameUIRect.Bottom + 1.4f + 0.8f);
		MyTransform.SetPositionXY(buttonRubber.transform, gameUIRect.Right + 0.03f, gameUIRect.Bottom + 0.7f + 0.8f);
		MyTransform.SetPositionXY(buttonIce.transform, gameUIRect.Right + 0.03f, gameUIRect.Bottom + 0.8f);

		HideItemControls();

		SetupUIInputEvents();

	}
	private void SetupUIInputEvents()
	{
		MyInputEvents inputEvents;

		//Menu button
		buttonMenu.GetComponent<MyInputEvents>().OnTap += ButtonMenu_Tap;
		
		//Learn and play gallery buttons
		buttonLearnGallery.GetComponent<MyInputEvents>().OnTap += ButtonLearnGallery_Tap;
		buttonPlayGallery.GetComponent<MyInputEvents>().OnTap += ButtonPlayGallery_Tap;

		//Gallery items
		GameObject.Find("ItemBackground").GetComponent<MyInputEvents>().OnTap += GalleryItem_Tap;

		//Create buttons: rectangle, circle, triangle
		inputEvents = buttonRectangle.GetComponent<MyInputEvents>();
		inputEvents.OnTouch += ButtonCreate_Touch;
		inputEvents = buttonCircle.GetComponent<MyInputEvents>();
		inputEvents.OnTouch += ButtonCreate_Touch;
		inputEvents = buttonTriangle.GetComponent<MyInputEvents>();
		inputEvents.OnTouch += ButtonCreate_Touch;

		//Item buttons
		inputEvents = buttonMove.GetComponent<MyInputEvents>();
		inputEvents.OnTouch += ButtonMove_Touch;
		inputEvents = buttonRotate.GetComponent<MyInputEvents>();
		inputEvents.OnTouch += ButtonRotate_Touch;
		inputEvents.OnDrag += ButtonRotate_Drag;
		inputEvents.OnRelease += ButtonRotate_Release;
		inputEvents = buttonResize.GetComponent<MyInputEvents>();
		inputEvents.OnTouch += ButtonResize_Touch;
		inputEvents.OnDrag += ButtonResize_Drag;
		inputEvents.OnRelease += ButtonResize_Release;
		inputEvents = buttonClone.GetComponent<MyInputEvents>();
		inputEvents.OnTouch += ButtonClone_Touch;


		//background (for game camera movement)
		inputEvents = background.GetComponent<MyInputEvents>();
		inputEvents.OnTouch += Background_Touch;
		inputEvents.OnDrag += Background_Drag;
		inputEvents.OnRelease += Background_Release;

		//pinch and zoom on game camera
		inputEvents = master.GetComponent<MyInputEvents>();
		inputEvents.OnDoubleTouchStart += Master_DoubleTouchStart;
		inputEvents.OnDoubleTouchDrag += Master_DoubleTouchDrag;
		inputEvents.OnMouseScrollWheel += Master_MouseScrollWheel;

	}

	void Update()
	{
		if (cameraFollowObject != null) cameraTargetPosition = cameraFollowObject.transform.position;

		UpdateCamera();

		if (selectedItem != null && itemControlsHolder.activeSelf) ShowItemControls();
	}
	void UpdateCamera()
	{
		//restrict the size
		if (cameraTargetSize < 3) cameraTargetSize = 3;
		else if (cameraTargetSize > gameRect.Height / 2) cameraTargetSize = gameRect.Height / 2;
		if (cameraTargetSize * aspectRatio > gameRect.Width / 2) cameraTargetSize = gameRect.Width / 2 / aspectRatio;

		//set the size(animated) and update variables
		gameCamera.orthographicSize = Mathf.Lerp(gameCamera.orthographicSize, cameraTargetSize, 10f * Time.deltaTime);

		pixelsPerUnit = Screen.height / gameCamera.orthographicSize / 2;
		cameraTrapRect = new MyRect(
			targetCameraTrapRect.Top - gameCamera.orthographicSize,
			targetCameraTrapRect.Left + gameCamera.orthographicSize * aspectRatio,
			targetCameraTrapRect.Bottom + gameCamera.orthographicSize,
			targetCameraTrapRect.Right - gameCamera.orthographicSize * aspectRatio);

		//set the position(animated)
		MyTransform.SetPositionXY(gameCamera.transform, Vector2.Lerp(gameCamera.transform.position, cameraTargetPosition, 10f * Time.deltaTime));

		//restrict the position
		Vector2 trappedPosition = cameraTrapRect.GetInsidePosition(gameCamera.transform.position);

		if (trappedPosition.x != gameCamera.transform.position.x)
		{
			cameraTargetPosition.x = trappedPosition.x;
			MyTransform.SetPositionX(gameCamera.transform, cameraTargetPosition.x);

			if (isCameraDragged)
				cameraDragOffset.x = -Input.mousePosition.x / pixelsPerUnit - cameraTargetPosition.x;
		}

		if (trappedPosition.y != gameCamera.transform.position.y)
		{
			cameraTargetPosition.y = trappedPosition.y;
			MyTransform.SetPositionY(gameCamera.transform, cameraTargetPosition.y);

			if (isCameraDragged)
				cameraDragOffset.y = -Input.mousePosition.y / pixelsPerUnit - cameraTargetPosition.y;
		}
	}

	void FixedUpdate()
	{
		if (isItemDragged)
		{
			selectedItem.rigidbody2D.angularVelocity = 0;
			selectedItem.rigidbody2D.velocity = Vector2.zero;
		}

		if (makeKinematic == 1)
		{
			makeKinematic = 2;
		}
		else if (makeKinematic == 2)
		{
			selectedItem.rigidbody2D.isKinematic = true;
			makeKinematic = 0;
		}

	}

	//Toolbar
	void ButtonCreate_Touch(GameObject sender, Camera camera)
	{
		if (sender == buttonRectangle)
			CreateNewItem(ItemShape.Rectangle, ItemMaterial.Wood);
		else if (sender == buttonCircle)
			CreateNewItem(ItemShape.Circle, ItemMaterial.Rubber);
		else if (sender == buttonTriangle)
			CreateNewItem(ItemShape.Triangle, ItemMaterial.Ice);

		HideItemControls();
	}

	//Operations
	void Item_Touch(GameObject sender, Camera camera)
	{
		DragStart(sender);
	}
	void Item_Release(GameObject sender, Camera camera)
	{
		//sender.rigidbody2D.isKinematic = true;
		isItemDragged = false;
		makeKinematic = 1;

		ShowItemControls();
	}

	private void ButtonMove_Touch(GameObject sender, Camera camera)
	{
		DragItem(selectedItem);
	}

	private void ButtonRotate_Touch(GameObject sender, Camera camera)
	{
		selectedItem.rigidbody2D.isKinematic = false;
		isItemDragged = true;

		initialInputAngle = Vector2.Angle((Vector2)gameCamera.ScreenToWorldPoint(Input.mousePosition) - selectedItem.rigidbody2D.worldCenterOfMass, Vector2.right);
		if (gameCamera.ScreenToWorldPoint(Input.mousePosition).y < selectedItem.rigidbody2D.worldCenterOfMass.y) initialInputAngle = 360 - initialInputAngle;
	
		initialRotation = selectedItem.transform.eulerAngles.z;

		HideItemControls();
	}
	private void ButtonRotate_Drag(GameObject sender, Camera camera)
	{
		float currentAngle = Vector2.Angle((Vector2)gameCamera.ScreenToWorldPoint(Input.mousePosition) - selectedItem.rigidbody2D.worldCenterOfMass, Vector2.right);
		if (gameCamera.ScreenToWorldPoint(Input.mousePosition).y < selectedItem.rigidbody2D.worldCenterOfMass.y) currentAngle = 360 - currentAngle;

		selectedItem.rigidbody2D.MoveRotation(initialRotation + currentAngle - initialInputAngle);
	}
	private void ButtonRotate_Release(GameObject sender, Camera camera)
	{
		//selectedItem.rigidbody2D.isKinematic = true;
		makeKinematic = 1;
		isItemDragged = false;

		ShowItemControls();
	}

	private void ButtonResize_Touch(GameObject sender, Camera camera)
	{
		resizeParent.transform.position = selectedItem.transform.position;
		resizeParent.transform.rotation = selectedItem.transform.rotation;
		selectedItem.transform.parent = resizeParent.transform;

		initialSize = new Vector2(selectedItemProps.width, selectedItemProps.height);
		initialPosition = selectedItem.transform.localPosition;
		initialInputPosition = resizeParent.transform.InverseTransformPoint(gameCamera.ScreenToWorldPoint(Input.mousePosition));

		selectedItem.rigidbody2D.isKinematic = false;
		selectedItem.rigidbody2D.fixedAngle = true;
		isItemDragged = true;


		HideItemControls();
	}
	private void ButtonResize_Drag(GameObject sender, Camera camera)
	{
		Vector2 currentInputPosition = resizeParent.transform.InverseTransformPoint(gameCamera.ScreenToWorldPoint(Input.mousePosition));
		Vector2 resizeOffset = Vector2.Scale(currentInputPosition - initialInputPosition, resizeCorner);

		if (selectedItemProps.shape == ItemShape.Circle)
			if (resizeOffset.x > resizeOffset.y) resizeOffset.x = resizeOffset.y;
			else resizeOffset.y = resizeOffset.x;

		Item.Resize(selectedItem, initialSize.x + resizeOffset.x, initialSize.y + resizeOffset.y);

		Vector2 moveOffset = Vector2.Scale(resizeOffset / 2, resizeCorner);
		Vector2 curLocPos = selectedItem.transform.localPosition;
		MyTransform.SetLocalPositionXY(selectedItem.transform, initialPosition + moveOffset);
		Vector2 newPos = selectedItem.transform.position;
		MyTransform.SetLocalPositionXY(selectedItem.transform, curLocPos);

		selectedItem.rigidbody2D.MovePosition(newPos);
	}
	private void ButtonResize_Release(GameObject sender, Camera camera)
	{
		selectedItem.transform.parent = itemsContainer.transform;
	
		//selectedItem.rigidbody2D.isKinematic = true;
		makeKinematic = 1;
		selectedItem.rigidbody2D.fixedAngle = false;

		ShowItemControls();
	}

	private void ButtonClone_Touch(GameObject sender, Camera camera)
	{
		CloneItem();
	}

	//METHODS
	void CreateNewItem(ItemShape itemShape, ItemMaterial itemMaterial)
	{
		float size = 1f / uiCamera.orthographicSize * gameCamera.orthographicSize;
		CreateItem(itemShape, itemMaterial, size, size);
		MyTransform.SetPositionXY(items[items.Length - 1].gameObject.transform, gameCamera.ScreenToWorldPoint(Input.mousePosition));
		DragItem(items[items.Length - 1].gameObject);
	}
	void CloneItem()
	{
		CreateItem(selectedItemProps.shape, selectedItemProps.material, selectedItemProps.width, selectedItemProps.height);
		items[items.Length - 1].gameObject.transform.rotation = selectedItem.transform.rotation;
		items[items.Length - 1].gameObject.transform.position = selectedItem.transform.position;
		DragItem(items[items.Length - 1].gameObject);
	}
	void CreateItem(ItemShape itemShape, ItemMaterial itemMaterial, float width, float height)
	{
		PhysicsObject physicsObject = new PhysicsObject();

		//create the item
		physicsObject.gameObject = Item.Create(itemShape, itemMaterial, width, height);
		physicsObject.gameObject.transform.parent = itemsContainer.transform;
		//physicsObject.gameObject.rigidbody2D.isKinematic = true;
		physicsObject.velocity = Vector2.zero;
		physicsObject.angularVelocity = 0;

		//setup input events
		DragAndDrop dragAndDrop = physicsObject.gameObject.AddComponent<DragAndDrop>();
		dragAndDrop.MoveToPositionMethod = delegate(GameObject gameObject, Vector3 position) { gameObject.rigidbody2D.MovePosition(playgroundRect.GetInsidePosition(position)); };
		
		MyInputEvents inputEvents = physicsObject.gameObject.GetComponent<MyInputEvents>();
		inputEvents.OnRelease += Item_Release;
		inputEvents.OnTouch += Item_Touch;

		//add the item to the list
		Array.Resize<PhysicsObject>(ref items, items.Length + 1);
		items[items.Length - 1] = physicsObject;
	}

	void DragItem(GameObject item)
	{
		item.GetComponent<DragAndDrop>().Drag(gameCamera);
		DragStart(item);
	}
	void DragStart(GameObject item)
	{
		item.rigidbody2D.isKinematic = false;
		//item.rigidbody2D.fixedAngle = true;
		selectedItem = item;
		selectedItemProps = selectedItem.GetComponent<ItemProperties>();
		isItemDragged = true;

		HideItemControls();
	}

	void HideItemControls()
	{
		itemControlsHolder.SetActive(false);
	}
	void ShowItemControls()
	{
		itemControlsHolder.SetActive(true);

		float offset = (Screen.height / uiCamera.orthographicSize / 2 * 0.26f) / pixelsPerUnit;
		float width, height;

		if (selectedItemProps.shape == ItemShape.Circle)
		{
			width = (selectedItemProps.width) / (float)Math.Sqrt(2);
			height = width;
		}
		else
		{
			width = selectedItemProps.width;
			height = selectedItemProps.height;
		}

		Vector2[] corners = new Vector2[4];
		corners[0] = selectedItem.transform.TransformPoint(-width / 2 - offset, -height / 2 - offset, selectedItem.transform.localPosition.z);
		corners[1] = selectedItem.transform.TransformPoint(width / 2 + offset, -height / 2 - offset, selectedItem.transform.localPosition.z);
		corners[2] = selectedItem.transform.TransformPoint(width / 2 + offset, height / 2 + offset, selectedItem.transform.localPosition.z);
		corners[3] = selectedItem.transform.TransformPoint(-width / 2 - offset, height / 2 + offset, selectedItem.transform.localPosition.z);

		Vector2 standardPosition = Vector2.zero, tweakedPosition = Vector2.zero;
		if (selectedItemProps.shape == ItemShape.Triangle)
		{
			standardPosition = uiCamera.ScreenToWorldPoint(gameCamera.WorldToScreenPoint(corners[2]));

			float angle = (float)Math.Atan(width / height);
			tweakedPosition = selectedItem.transform.TransformPoint((float)Math.Cos(angle) * offset * 1.5f, (float)Math.Sin(angle) * offset * 1.5f, selectedItem.transform.localPosition.z);
	
			tweakedPosition = uiCamera.ScreenToWorldPoint(gameCamera.WorldToScreenPoint(tweakedPosition));
		}

		Array.Sort(corners, delegate(Vector2 v1, Vector2 v2) { return v1.y.CompareTo(v2.y); });

		if (Math.Abs(corners[0].y - corners[3].y) >= Math.Abs(corners[1].x - corners[2].x))
		{
			if (corners[0].x >= corners[1].x)
			{
				MyTransform.SetPositionXY(buttonMove.transform, uiCamera.ScreenToWorldPoint(gameCamera.WorldToScreenPoint(corners[1])));
				MyTransform.SetPositionXY(buttonResize.transform, uiCamera.ScreenToWorldPoint(gameCamera.WorldToScreenPoint(corners[0])));
			}
			else
			{
				MyTransform.SetPositionXY(buttonMove.transform, uiCamera.ScreenToWorldPoint(gameCamera.WorldToScreenPoint(corners[0])));
				MyTransform.SetPositionXY(buttonResize.transform, uiCamera.ScreenToWorldPoint(gameCamera.WorldToScreenPoint(corners[1])));
			}

			if (corners[2].x >= corners[3].x)
			{
				MyTransform.SetPositionXY(buttonRotate.transform, uiCamera.ScreenToWorldPoint(gameCamera.WorldToScreenPoint(corners[3])));
				MyTransform.SetPositionXY(buttonClone.transform, uiCamera.ScreenToWorldPoint(gameCamera.WorldToScreenPoint(corners[2])));
			}
			else
			{
				MyTransform.SetPositionXY(buttonRotate.transform, uiCamera.ScreenToWorldPoint(gameCamera.WorldToScreenPoint(corners[2])));
				MyTransform.SetPositionXY(buttonClone.transform, uiCamera.ScreenToWorldPoint(gameCamera.WorldToScreenPoint(corners[3])));
			}
		}
		else
		{
			Array.Sort(corners, delegate(Vector2 v1, Vector2 v2) { return v1.x.CompareTo(v2.x); });

			if (corners[0].y >= corners[1].y)
			{
				MyTransform.SetPositionXY(buttonMove.transform, uiCamera.ScreenToWorldPoint(gameCamera.WorldToScreenPoint(corners[1])));
				MyTransform.SetPositionXY(buttonRotate.transform, uiCamera.ScreenToWorldPoint(gameCamera.WorldToScreenPoint(corners[0])));
			}
			else
			{
				MyTransform.SetPositionXY(buttonMove.transform, uiCamera.ScreenToWorldPoint(gameCamera.WorldToScreenPoint(corners[0])));
				MyTransform.SetPositionXY(buttonRotate.transform, uiCamera.ScreenToWorldPoint(gameCamera.WorldToScreenPoint(corners[1])));
			}

			if (corners[2].y >= corners[3].y)
			{
				MyTransform.SetPositionXY(buttonResize.transform, uiCamera.ScreenToWorldPoint(gameCamera.WorldToScreenPoint(corners[3])));
				MyTransform.SetPositionXY(buttonClone.transform, uiCamera.ScreenToWorldPoint(gameCamera.WorldToScreenPoint(corners[2])));
			}
			else
			{
				MyTransform.SetPositionXY(buttonResize.transform, uiCamera.ScreenToWorldPoint(gameCamera.WorldToScreenPoint(corners[2])));
				MyTransform.SetPositionXY(buttonClone.transform, uiCamera.ScreenToWorldPoint(gameCamera.WorldToScreenPoint(corners[3])));
			}
		}

		if (selectedItemProps.shape == ItemShape.Triangle)
		{
			if ((Vector2)buttonResize.transform.position == standardPosition)
				MyTransform.SetPositionXY(buttonResize.transform, tweakedPosition);
			else if ((Vector2)buttonRotate.transform.position == standardPosition)
				MyTransform.SetPositionXY(buttonRotate.transform, tweakedPosition);
			else if ((Vector2)buttonMove.transform.position == standardPosition)
				MyTransform.SetPositionXY(buttonMove.transform, tweakedPosition);
			else if ((Vector2)buttonClone.transform.position == standardPosition)
				MyTransform.SetPositionXY(buttonClone.transform, tweakedPosition);
		}

		resizeCorner = selectedItem.transform.InverseTransformPoint(gameCamera.ScreenToWorldPoint(uiCamera.WorldToScreenPoint(buttonResize.transform.position)));
		resizeCorner.x = Math.Sign(resizeCorner.x);
		resizeCorner.y = Math.Sign(resizeCorner.y);
	}

	#region Navigation, camera movement and zoom


	//Navigation
	private void ButtonMenu_Tap(GameObject sender, Camera camera)
	{
		if (gameStatus == GameStatus.Transition) return;

		HideGameUI();

		StartCoroutine(TransitionTo(
			GameStatus.Menu,
			homeRect,
			new Vector2(0, homeRect.Top - cameraDefaultSize),
			cameraDefaultSize));
	}
	private void ButtonLearnGallery_Tap(GameObject sender, Camera camera)
	{
		if (gameStatus == GameStatus.Transition) return;

		StartCoroutine(TransitionTo(
			GameStatus.Menu,
			new MyRect(learnGalleryRect.Top, learnGalleryRect.Left, playGalleryRect.Bottom, learnGalleryRect.Right),
			new Vector2(0, learnGalleryRect.Top - cameraDefaultSize),
			cameraDefaultSize));
	}
	private void ButtonPlayGallery_Tap(GameObject sender, Camera camera)
	{
		if (gameStatus == GameStatus.Transition) return;

		StartCoroutine(TransitionTo(
			GameStatus.Menu,
			new MyRect(learnGalleryRect.Top, learnGalleryRect.Left, playGalleryRect.Bottom, learnGalleryRect.Right),
			new Vector2(0, playGalleryRect.Top - cameraDefaultSize),
			cameraDefaultSize));
	}
	void GalleryItem_Tap(GameObject sender, Camera camera)
	{
		if (gameStatus == GameStatus.Transition) return;

		StartCoroutine(TransitionTo(
			GameStatus.Stop,
			gameRect,
			gameRect.Center,
			cameraDefaultSize));

		ShowGameUI();
	}

	
	void HideGameUI()
	{
		toolbar.SetActive(false);
		buttonMenu.SetActive(false);
		buttonPlay.SetActive(false);
		buttonStop.SetActive(false);
	}
	void ShowGameUI()
	{
		toolbar.SetActive(true);
		buttonMenu.SetActive(true);
		buttonPlay.SetActive(true);
		buttonStop.SetActive(false);
	}


	//main camera movement
	void Background_Touch(GameObject sender, Camera camera)
	{
		if (gameStatus == GameStatus.Transition) return;

		cameraTargetPosition = gameCamera.transform.position;
		cameraDragOffset = -(Vector2)Input.mousePosition / pixelsPerUnit - cameraTargetPosition;
		isCameraDragged = true;
	}
	void Background_Drag(GameObject sender, Camera camera)
	{
		if (gameStatus == GameStatus.Transition) return;
		if (!isCameraDragged) return;

		cameraTargetPosition = -Input.mousePosition / pixelsPerUnit - cameraDragOffset;
	}
	void Background_Release(GameObject sender, Camera camera)
	{
		isCameraDragged = false;
	}

	//main camera pinch and zoom
	private void Master_DoubleTouchStart(Touch touch0, Touch touch1)
	{
		doubleTouchDistance = Vector2.Distance(uiCamera.ScreenToWorldPoint(touch0.position), uiCamera.ScreenToWorldPoint(touch1.position));
	}
	private void Master_DoubleTouchDrag(Touch touch0, Touch touch1)
	{
		if (gameStatus != GameStatus.Play && gameStatus != GameStatus.Stop) return;

		doubleTouchDistanceOffset = doubleTouchDistance - Vector2.Distance(uiCamera.ScreenToWorldPoint(touch0.position), uiCamera.ScreenToWorldPoint(touch1.position));
		cameraTargetSize = gameCamera.orthographicSize + doubleTouchDistanceOffset * gameCamera.orthographicSize;

		doubleTouchDistance = Vector2.Distance(uiCamera.ScreenToWorldPoint(touch0.position), uiCamera.ScreenToWorldPoint(touch1.position));
	}
	private void Master_MouseScrollWheel(float amount)
	{
		if (gameStatus != GameStatus.Play && gameStatus != GameStatus.Stop) return;

		cameraTargetSize = gameCamera.orthographicSize + -amount * gameCamera.orthographicSize * 2;
	}

	IEnumerator TransitionTo(GameStatus newGameStatus, MyRect newCameraTrapRect, Vector2 newCameraPosition, float newCameraSize)
	{
		targetCameraTrapRect = new MyRect(homeRect.Top, gameRect.Left, gameRect.Bottom, gameRect.Right);
		cameraTargetSize = newCameraSize;
		cameraTargetPosition = newCameraPosition;

		gameStatus = GameStatus.Transition;
		yield return new WaitForSeconds(0.8f);
		targetCameraTrapRect = newCameraTrapRect;
		gameStatus = newGameStatus;
	}

	#endregion
}

public enum GameStatus
{
	Menu = 0,
	Transition = 1,
	Play = 2,
	Stop = 3
}

public class PhysicsObject
{
	public GameObject gameObject;
	public Vector2 velocity;
	public float angularVelocity;
}
