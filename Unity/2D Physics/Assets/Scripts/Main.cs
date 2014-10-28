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
		buttonMenu, buttonPlay, buttonPause, buttonStop,
		buttonRectangle, buttonCircle, buttonTriangle, buttonFixed, buttonMetal, buttonWood, buttonRubber, buttonIce,
		buttonMove, buttonRotate, buttonResize, buttonClone, itemControlsHolder;

	GameStatus gameStatus = GameStatus.Stop;

	GameObject selectedItem = null;
	ItemProperties selectedItemProps = null;

	PhysicsObject[] items = new PhysicsObject[0];
	GameObject itemsContainer;

	float initialRotation, initialInputAngle;
	Vector2 initialSize, initialPosition, initialInputPosition, resizeCorner;
	GameObject resizeParent;
	bool isItemDragged = false;

	
	//setup, camera
	Vector2 playgroundSize = new Vector2(40, 25);
	float titleHeight = 10;
	float learnGalleryHeight = 15;
	float playGalleryHeight = 25;
	float wallWidth = 0.5f;

	Vector2 sceneSize;
	float dpi, pixelsPerUnit, aspectRatio, spritePixelsPerUnit;
	MyRect playViewRect, playgroundRect, uiRect, mainCameraRect;
	
	Vector2 cameraTargetPosition;
	float cameraTargetSize;
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
	}

	void SetupScene()
	{
		Time.timeScale = 1;

		//initialize the main camera
		gameCamera.transform.position = new Vector3(0, 0, -12);
		cameraTargetPosition = new Vector2(0, 0);
		cameraTargetSize = gameCamera.orthographicSize;

		//setup variables
		aspectRatio = (float)Screen.width / Screen.height;
		pixelsPerUnit = Screen.height / gameCamera.orthographicSize / 2;
		spritePixelsPerUnit = 1536f / 10; //Screen.height / uiCamera.orthographicSize / 2;

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
		dpi = Mathf.Clamp(dpi, 100, 700);
		//dpi = 132;
		float scaleFactor = 1 + (Screen.height / dpi - 3.5f) * 0.15f;
		uiCamera.orthographicSize = Mathf.Clamp(0.4f + Screen.height / dpi / scaleFactor, 3.6f, 5f);


		//UI area
		uiRect = new MyRect(
			uiCamera.orthographicSize,
			-uiCamera.orthographicSize * aspectRatio,
			-uiCamera.orthographicSize,
			uiCamera.orthographicSize * aspectRatio);

		//define the UI objects
		buttonMenu = GameObject.Find("ButtonMenu");
		buttonPlay = GameObject.Find("ButtonPlay");
		buttonPause = GameObject.Find("ButtonPause");
		buttonStop = GameObject.Find("ButtonStop");
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

		//position the UI buttons
		MyTransform.SetPositionXY(buttonMenu.transform, uiRect.Left + 0.5f, uiRect.Top - 0.5f);
		MyTransform.SetPositionXY(buttonPlay.transform, uiRect.Left + uiRect.Width / 2 - 0.6f, uiRect.Top - 0.65f);
		MyTransform.SetPositionXY(buttonPause.transform, uiRect.Left + uiRect.Width / 2 - 0.6f, uiRect.Top - 0.65f);
		buttonPause.SetActive(false);
		MyTransform.SetPositionXY(buttonStop.transform, uiRect.Left + uiRect.Width / 2 + 0.6f, uiRect.Top - 0.65f);
		buttonStop.SetActive(false);

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

		HideItemControls();

		SetupUIInputEvents();

	}
	private void SetupUIInputEvents()
	{
		MyInputEvents inputEvents;

		//Menu Button
		inputEvents = buttonMenu.GetComponent<MyInputEvents>();
		inputEvents.OnTap += ButtonMenu_Tap;

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
		if (isItemDragged) selectedItem.rigidbody2D.angularVelocity = 0;
	}
	void UpdateCamera()
	{
		//restrict the size
		if (cameraTargetSize < 3) cameraTargetSize = 3;
		else if (cameraTargetSize > playViewRect.Height / 2) cameraTargetSize = playViewRect.Height / 2;
		if (cameraTargetSize * aspectRatio > playViewRect.Width / 2) cameraTargetSize = playViewRect.Width / 2 / aspectRatio;

		//set the size(animated) and update variables
		gameCamera.orthographicSize = Mathf.Lerp(gameCamera.orthographicSize, cameraTargetSize, 10f * Time.deltaTime);

		pixelsPerUnit = Screen.height / gameCamera.orthographicSize / 2;
		mainCameraRect = new MyRect(
			playViewRect.Top - gameCamera.orthographicSize,
			playViewRect.Left + +gameCamera.orthographicSize * aspectRatio,
			playViewRect.Bottom + gameCamera.orthographicSize,
			playViewRect.Right - gameCamera.orthographicSize * aspectRatio);

		//set the position(animated)
		MyTransform.SetPositionXY(gameCamera.transform, Vector2.Lerp(gameCamera.transform.position, cameraTargetPosition, 10f * Time.deltaTime));

		//restrict the position
		Vector2 trappedPosition = mainCameraRect.GetInsidePosition(gameCamera.transform.position);

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


	//EVENTS
	private void ButtonMenu_Tap(GameObject sender, Camera camera)
	{
		Debug.Log("Tap: " + sender.name + " -> " + camera.name);
	}

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

	void Item_Touch(GameObject sender, Camera camera)
	{
		DragStart(sender);
	}
	void Item_Release(GameObject sender, Camera camera)
	{
		sender.rigidbody2D.isKinematic = true;
		isItemDragged = false;

		ShowItemControls();
	}

	private void ButtonMove_Touch(GameObject sender, Camera camera)
	{
		DragItem(selectedItem);
	}

	private void ButtonRotate_Touch(GameObject sender, Camera camera)
	{
		initialInputAngle = Vector2.Angle(gameCamera.ScreenToWorldPoint(Input.mousePosition) - selectedItem.transform.position, Vector2.right);
		if (gameCamera.ScreenToWorldPoint(Input.mousePosition).y < selectedItem.transform.position.y) initialInputAngle = 360 - initialInputAngle;

		initialRotation = selectedItem.transform.eulerAngles.z;

		selectedItem.rigidbody2D.isKinematic = false;
		Physics2D.gravity = Vector2.zero;

		HideItemControls();
	}
	private void ButtonRotate_Drag(GameObject sender, Camera camera)
	{
		float currentAngle = Vector2.Angle(gameCamera.ScreenToWorldPoint(Input.mousePosition) - selectedItem.transform.position, Vector2.right);
		if (gameCamera.ScreenToWorldPoint(Input.mousePosition).y < selectedItem.transform.position.y) currentAngle = 360 - currentAngle;

		selectedItem.transform.eulerAngles = new Vector3(0, 0, initialRotation + currentAngle - initialInputAngle);
	}
	private void ButtonRotate_Release(GameObject sender, Camera camera)
	{
		selectedItem.rigidbody2D.isKinematic = true;
		Physics2D.gravity = -9.81f * Vector2.up;

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
		Physics2D.gravity = Vector2.zero;

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
		MyTransform.SetLocalPositionXY(selectedItem.transform, initialPosition + moveOffset);
	}
	private void ButtonResize_Release(GameObject sender, Camera camera)
	{
		selectedItem.transform.parent = itemsContainer.transform;
	
		selectedItem.rigidbody2D.isKinematic = true;
		Physics2D.gravity = -9.81f * Vector2.up;

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

		resizeCorner = selectedItem.transform.InverseTransformPoint(gameCamera.ScreenToWorldPoint(uiCamera.WorldToScreenPoint(buttonResize.transform.position)));
		resizeCorner.x = Math.Sign(resizeCorner.x);
		resizeCorner.y = Math.Sign(resizeCorner.y);
	}

	#region Camera movement and zoom

	//main camera movement
	void Background_Touch(GameObject sender, Camera camera)
	{
		cameraTargetPosition = gameCamera.transform.position;
		cameraDragOffset = -(Vector2)Input.mousePosition / pixelsPerUnit - cameraTargetPosition;
		isCameraDragged = true;
	}
	void Background_Drag(GameObject sender, Camera camera)
	{
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
		doubleTouchDistanceOffset = doubleTouchDistance - Vector2.Distance(uiCamera.ScreenToWorldPoint(touch0.position), uiCamera.ScreenToWorldPoint(touch1.position));
		cameraTargetSize = gameCamera.orthographicSize + doubleTouchDistanceOffset * gameCamera.orthographicSize;

		doubleTouchDistance = Vector2.Distance(uiCamera.ScreenToWorldPoint(touch0.position), uiCamera.ScreenToWorldPoint(touch1.position));
	}
	private void Master_MouseScrollWheel(float amount)
	{
		cameraTargetSize = gameCamera.orthographicSize + -amount * gameCamera.orthographicSize * 2;
	}

	#endregion
}

public enum GameStatus
{
	Play = 0,
	Pause = 1,
	Stop = 2
}

public class PhysicsObject
{
	public GameObject gameObject;
	public Vector2 velocity;
	public float angularVelocity;
}
