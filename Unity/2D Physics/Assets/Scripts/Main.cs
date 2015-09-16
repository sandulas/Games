using UnityEngine;
using System.Collections;
using Sandulas;
using ThisProject;
using System;
using System.Xml;
using System.IO;

namespace ThisProject
{
	public class Main : MonoBehaviour
	{
		#region PROPERTIES AND VARIABLES

		//public
		public Camera uiCamera, gameCamera;
		public GameObject master, background;
		
		//game
		GameObject
			buttonLearnGallery, buttonPlayGallery, buttonNewLevel, messageGalleryFull, titlePlay, titleLearn,
			buttonMenu,	buttonPlay, buttonGrabDisabled, buttonGrabEnabled, buttonStop,
			toolbar, materialSelector, buttonRectangle, buttonCircle, buttonTriangle, buttonFixed, buttonMetal, buttonWood, buttonRubber, buttonIce,
			buttonDelete, buttonMove, buttonRotate, buttonResize, buttonClone, itemControlsHolder;

		GameStatus gameStatus = GameStatus.Menu;
		string currentLevel = null;
		GameObject playLevelToDelete = null;
		bool returnToPlay;

		GameObject selectedItem = null;
		ItemProperties selectedItemProps = null;

		PhysicsObject[] items = new PhysicsObject[0];
		GameObject itemsContainer;

		ItemMaterial selectedMaterial = ItemMaterial.FixedMetal;

		//operations
		float initialRotation, initialInputAngle;
		Vector2 initialSize, initialPosition, initialInputPosition, resizeCorner;
		Vector3 moveOffset;
		Vector2 defaultCenterOfMass;
		bool customCenterOfMass;
		GameObject resizeParent;
		bool isItemDragged = false;
		int makeKinematic = 0;

		
		//scene setup
		float dpi, scaleFactor, pixelsPerUnit, aspectRatio, spritePixelsPerUnit, menuUnit;
		int learnLevelsCount = 6;
		int playLevelsCount;
		string[] playSavedFiles;
		
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


		#region INITIALIZATION

		void Start()
		{
			SetupScene();
			SetupUI();

			Physics2D.gravity = Vector2.zero;
			Application.targetFrameRate = 60;

	//		targetCameraTrapRect = gameRect;
			gameStatus = GameStatus.Menu;
			targetCameraTrapRect = new MyRect(homeRect.Top, learnGalleryRect.Left, playGalleryRect.Bottom, learnGalleryRect.Right);
			HideGameUI();
		}

		void SetupScene()
		{
			//initialize the main camera
			gameCamera.transform.position = new Vector3(0, 1000, -12);
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

			menuUnit = cameraDefaultSize * aspectRatio * 2 / 1000;

			playSavedFiles = Directory.GetFiles(Application.persistentDataPath);
			playSavedFiles = Array.FindAll(playSavedFiles, x => x.EndsWith(".xml"));
			playLevelsCount = playSavedFiles.Length;

			UpdateSceneZones();

			//setup the walls
			GameObject wall;

			wall = Item.Create(ItemShape.Rectangle, ItemMaterial.FixedMetal, playgroundRect.Width , wallWidth);
			Item.Move(wall, 0, playgroundRect.Height / 2 + wallWidth / 2);
			Item.UpdatePhysicsMaterial(wall);
			wall.name = "Wall - Top";

			Item.Duplicate(wall);
			Item.Move(wall, 0, -playgroundRect.Height / 2 - wallWidth / 2);
			Item.UpdatePhysicsMaterial(wall);
			wall.name = "Wall - Bottom";

			wall = Item.Create(ItemShape.Rectangle, ItemMaterial.FixedMetal, wallWidth * 4, playgroundRect.Height + wallWidth * 2);
			Item.Move(wall, playgroundRect.Width / 2 + wallWidth * 2, 0);
			Item.UpdatePhysicsMaterial(wall);
			wall.name = "Wall - Right";

			wall = Item.Create(ItemShape.Rectangle, ItemMaterial.FixedMetal, wallWidth, playgroundRect.Height + wallWidth * 2);
			Item.Move(wall, -playgroundRect.Width / 2 - wallWidth / 2, 0);
			Item.UpdatePhysicsMaterial(wall);
			wall.name = "Wall - Left";

			//create the resize parent helper object and items container
			resizeParent = new GameObject("Temporary parent for Resize");
			itemsContainer = new GameObject("Objects Container");
			itemsContainer.transform.position = playgroundRect.Center;
		}
		void UpdateSceneZones()
		{
			float playGalleryTopOffset;
			if (playGalleryRect == null)
				playGalleryTopOffset = float.MaxValue;
			else
				playGalleryTopOffset = playGalleryRect.Top;

			playGalleryRect = new MyRect(
				gameRect.Top + menuUnit * 320 + (playLevelsCount) / 4 * menuUnit * 225,
				-cameraDefaultSize * aspectRatio,
				gameRect.Top,
				cameraDefaultSize * aspectRatio);
			playGalleryRect.Draw();

			if (playGalleryTopOffset != float.MaxValue)
				playGalleryTopOffset = playGalleryRect.Top - playGalleryTopOffset;
			else
				playGalleryTopOffset = 0;


			//update UI screens vertical positions to match the variable height of the play gallery
			MyTransform.MoveY(GameObject.Find("Play Gallery Screen").transform, playGalleryTopOffset);
			MyTransform.MoveY(GameObject.Find("Learn Gallery Screen").transform, playGalleryTopOffset);
			MyTransform.MoveY(GameObject.Find("Home Screen").transform, playGalleryTopOffset);
			MyTransform.MoveY(gameCamera.transform, playGalleryTopOffset);
			cameraTargetPosition = gameCamera.transform.position;


			learnGalleryRect = new MyRect(
				playGalleryRect.Top + menuUnit * 315 + (learnLevelsCount - 1) / 4 * menuUnit * 225,
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

			targetCameraTrapRect = new MyRect(homeRect.Top, learnGalleryRect.Left, playGalleryRect.Bottom, learnGalleryRect.Right);

			//initialize the background
			Vector2 sceneSize = new Vector2(gameRect.Width, gameRect.Height + playGalleryRect.Height + learnGalleryRect.Height + homeRect.Height);
			background.transform.position = new Vector3(0, (sceneSize.y - gameRect.Height) / 2, 0);
			background.transform.localScale = new Vector3(sceneSize.x * 2, sceneSize.y, 1);
			background.renderer.material.mainTextureScale = new Vector2(sceneSize.x / 5, sceneSize.y / 10);
		}

		void SetupUI()
		{
			//setup UI camera size depending on the screen size
			if (Screen.dpi < 1) dpi = 270;
			else dpi = Screen.dpi;
			dpi = Mathf.Clamp(dpi, 100, 700);//dpi = 132;
			scaleFactor = 1 + (Screen.height / dpi - 3.5f) * 0.15f;
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
			buttonNewLevel = GameObject.Find("ButtonNewLevel");
			messageGalleryFull = GameObject.Find("MessageGalleryFull");
			titleLearn = GameObject.Find("TitleLearn");
			titlePlay = GameObject.Find("TitlePlay");
			
			buttonMenu = GameObject.Find("ButtonMenu");
			buttonPlay = GameObject.Find("ButtonPlay");
			buttonStop = GameObject.Find("ButtonStop");
			buttonGrabDisabled = GameObject.Find("ButtonGrabDisabled");
			buttonGrabEnabled = GameObject.Find("ButtonGrabEnabled");

			toolbar = GameObject.Find("Toolbar");
			materialSelector = GameObject.Find("MaterialSelector");
			buttonRectangle = GameObject.Find("ButtonRectangle");
			buttonCircle = GameObject.Find("ButtonCircle");
			buttonTriangle = GameObject.Find("ButtonTriangle");
			buttonFixed = GameObject.Find("ButtonFixed");
			buttonMetal = GameObject.Find("ButtonMetal");
			buttonWood = GameObject.Find("ButtonWood");
			buttonRubber = GameObject.Find("ButtonRubber");
			buttonIce = GameObject.Find("ButtonIce");
			
			buttonDelete = GameObject.Find("ButtonDelete");
			buttonMove = GameObject.Find("ButtonMove");
			buttonRotate = GameObject.Find("ButtonRotate");
			buttonResize = GameObject.Find("ButtonResize");
			buttonClone = GameObject.Find("ButtonClone");
			itemControlsHolder = GameObject.Find("Controls");

			//LAYOUT

			//home elements
			LoadTitle();
			MyTransform.SetPositionXY(buttonLearnGallery.transform, -1.4f, homeRect.Bottom + 0.6f + buttonLearnGallery.GetComponent<SpriteRenderer>().sprite.rect.height / spritePixelsPerUnit * buttonLearnGallery.transform.localScale.y * 1.1f);
			MyTransform.SetPositionXY(buttonPlayGallery.transform, 1.4f, homeRect.Bottom + 0.6f + buttonLearnGallery.GetComponent<SpriteRenderer>().sprite.rect.height / spritePixelsPerUnit * buttonLearnGallery.transform.localScale.y * 1.1f);

			//gallery titles
			MyTransform.SetPositionXY(titleLearn.transform, learnGalleryRect.Left + menuUnit * 25, learnGalleryRect.Top - menuUnit * 15);
			MyTransform.SetPositionXY(titlePlay.transform, playGalleryRect.Left + menuUnit * 25, playGalleryRect.Top - menuUnit * 15);
			MyTransform.SetScaleXY(titleLearn.transform, menuUnit * 80, menuUnit * 80);
			MyTransform.SetScaleXY(titlePlay.transform, menuUnit * 80, menuUnit * 80);

			LoadGalleries();

			//UI buttons
			MyTransform.SetPositionXY(buttonDelete.transform, gameUIRect.Left + 0.5f, gameUIRect.Bottom + 0.5f);
			MyTransform.SetPositionXY(buttonMenu.transform, gameUIRect.Left + 0.5f, gameUIRect.Top - 0.5f);
			MyTransform.SetPositionXY(buttonPlay.transform, gameUIRect.Left + gameUIRect.Width / 2 - 0.6f, gameUIRect.Top - 0.65f);
			MyTransform.SetPositionXY(buttonGrabDisabled.transform, gameUIRect.Left + gameUIRect.Width / 2 + 0.6f, gameUIRect.Top - 0.65f);
			MyTransform.SetPositionXY(buttonGrabEnabled.transform, gameUIRect.Left + gameUIRect.Width / 2 + 0.6f, gameUIRect.Top - 0.65f);
			buttonGrabDisabled.SetActive(false);
			buttonGrabEnabled.SetActive(false);
			MyTransform.SetPositionXY(buttonStop.transform, gameUIRect.Left + gameUIRect.Width / 2 - 0.6f, gameUIRect.Top - 0.65f);
			buttonStop.SetActive(false);

			//toolbar
			MyTransform.SetPositionXY(toolbar.transform, gameUIRect.Right, 0);
			GameObject obj = GameObject.Find("ToolbarBackground");
			MyTransform.SetPositionXY(obj.transform, gameUIRect.Right, uiCamera.orthographicSize + 0.01f);
			MyTransform.SetScaleY(obj.transform, (uiCamera.orthographicSize + 0.02f) * 2 * spritePixelsPerUnit / obj.GetComponent<SpriteRenderer>().sprite.rect.height);

			MyTransform.SetPositionXY(buttonRectangle.transform, gameUIRect.Right + 0.05f, gameUIRect.Top - 0.1f);
			MyTransform.SetPositionXY(buttonCircle.transform, gameUIRect.Right + 0.05f, gameUIRect.Top - 1.2f - 0.1f);
			MyTransform.SetPositionXY(buttonTriangle.transform, gameUIRect.Right + 0.05f, gameUIRect.Top - 2.2f - 0.1f);

			MyTransform.SetPositionXY(buttonFixed.transform, gameUIRect.Right + 0.03f, gameUIRect.Bottom + 2.8f + 0.8f);
			MyTransform.SetPositionXY(buttonMetal.transform, gameUIRect.Right + 0.03f, gameUIRect.Bottom + 2.1f + 0.8f);
			MyTransform.SetPositionXY(buttonWood.transform, gameUIRect.Right + 0.03f, gameUIRect.Bottom + 1.4f + 0.8f);
			MyTransform.SetPositionXY(buttonRubber.transform, gameUIRect.Right + 0.03f, gameUIRect.Bottom + 0.7f + 0.8f);
			MyTransform.SetPositionXY(buttonIce.transform, gameUIRect.Right + 0.03f, gameUIRect.Bottom + 0.8f);
			SetSelectedMaterial(ItemMaterial.FixedMetal);

			HideItemControls();

			SetupUIInputEvents();
		}
		void LoadTitle()
		{
			XmlDocument xmlDoc = new XmlDocument();
			TextAsset textAsset = (TextAsset)Resources.Load("title");
			xmlDoc.LoadXml(textAsset.text);

			XmlNode root = xmlDoc.DocumentElement;
			ItemShape shape; ItemMaterial material; float width; float height;
			for (int i = 0; i < root.ChildNodes.Count; i++)
			{
				shape = (ItemShape)Enum.Parse(typeof(ItemShape), root.ChildNodes[i].Attributes["s"].Value);
				material = (ItemMaterial)Enum.Parse(typeof(ItemMaterial), root.ChildNodes[i].Attributes["m"].Value);
				width = float.Parse(root.ChildNodes[i].Attributes["w"].Value);
				height = float.Parse(root.ChildNodes[i].Attributes["h"].Value);

				GameObject item = Item.Create(shape, material, width, height);
				Destroy(item.rigidbody2D);

				MyTransform.SetLocalPositionXY(item.transform, float.Parse(root.ChildNodes[i].Attributes["x"].Value) - 0.5f, homeRect.Top + float.Parse(root.ChildNodes[i].Attributes["y"].Value) - 8.2f);
				item.transform.eulerAngles = new Vector3(0, 0, float.Parse(root.ChildNodes[i].Attributes["r"].Value));
			}
		}
		void LoadGalleries()
		{
			//learn gallery first level
			GameObject levelHolder = GameObject.Find("LearnLevel");
			MyTransform.SetScaleXY(levelHolder.transform, menuUnit * 85, menuUnit * 85);

			Vector2 startPos = new Vector2(
				learnGalleryRect.Left + menuUnit * 17 + levelHolder.GetComponent<SpriteRenderer>().sprite.rect.width / spritePixelsPerUnit * levelHolder.transform.localScale.x / 2,
				learnGalleryRect.Top - menuUnit * 70 - levelHolder.GetComponent<SpriteRenderer>().sprite.rect.height / spritePixelsPerUnit * levelHolder.transform.localScale.y / 2);

			MyTransform.SetPositionXY(levelHolder.transform,startPos.x, startPos.y);

			StartCoroutine(LoadGalleryLevel("learn.01", levelHolder));

			//learn gallery levels
			GameObject tmp;
			for (int i = 1; i < learnLevelsCount; i++)
			{
				tmp = (GameObject)GameObject.Instantiate(levelHolder);
				tmp.transform.parent = levelHolder.transform.parent;
				MyTransform.SetPositionXY(tmp.transform, startPos.x + i % 4 * menuUnit * 244, startPos.y - i / 4 * menuUnit * 225);

				StartCoroutine(LoadGalleryLevel("learn." + (i + 1).ToString("00"), tmp));
			}

			//play gallery "new" level
			startPos = new Vector2(
				playGalleryRect.Left + menuUnit * 17 + levelHolder.GetComponent<SpriteRenderer>().sprite.rect.width / spritePixelsPerUnit * levelHolder.transform.localScale.x / 2,
				playGalleryRect.Top - menuUnit * 75 - levelHolder.GetComponent<SpriteRenderer>().sprite.rect.height / spritePixelsPerUnit * levelHolder.transform.localScale.y / 2);

			tmp = (GameObject)GameObject.Instantiate(levelHolder);
			tmp.name = "PlayNewLevel";
			MyTransform.SetPositionXY(tmp.transform, startPos.x, startPos.y);
			MyTransform.SetPositionXY(buttonNewLevel.transform, tmp.transform.Find("Thumb").position);
			MyTransform.SetScaleXY(buttonNewLevel.transform, menuUnit * 85, menuUnit * 85);
			MyTransform.SetPositionXY(messageGalleryFull.transform, buttonNewLevel.transform.position.x, buttonNewLevel.transform.position.y - tmp.GetComponent<SpriteRenderer>().sprite.rect.height / spritePixelsPerUnit * tmp.transform.localScale.y * 0.45f);
			MyTransform.SetScaleXY(messageGalleryFull.transform, menuUnit * 85, menuUnit * 85);
			messageGalleryFull.SetActive(false);

			//play gallery levels
			levelHolder = GameObject.Find("PlayLevel");
			MyTransform.SetScaleXY(levelHolder.transform, menuUnit * 85, menuUnit * 85);
			tmp.transform.parent = levelHolder.transform.parent;

			int pos;
			for (int i = 1; i <= playLevelsCount; i++)
			{
				pos = playLevelsCount - i + 1;
				tmp = (GameObject)GameObject.Instantiate(levelHolder);
				tmp.transform.parent = levelHolder.transform.parent;
				tmp.name = "PlayLevel." + pos.ToString("00");
				MyTransform.SetPositionXY(tmp.transform, startPos.x + pos % 4 * menuUnit * 244, startPos.y - pos / 4 * menuUnit * 225);

				StartCoroutine(LoadGalleryLevel(Path.GetFileNameWithoutExtension(playSavedFiles[i - 1]), tmp));
			}
		}
	    IEnumerator LoadGalleryLevel(string levelName, GameObject levelHolder)
		{
	        Texture2D texture = null;

			//if learn level - load from resources
	        if (levelName.StartsWith("learn."))
	        {
	            ResourceRequest request = Resources.LoadAsync<Texture2D>("Learn/" + levelName);
	            while (!request.isDone)
	            {
	                yield return 0;
	            }
	            texture = request.asset as Texture2D;
	        }
			//if play level - load from disk
	        else if (levelName.StartsWith("play."))
	        {
				try
				{
		            texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
		            byte[] textureData = File.ReadAllBytes(Application.persistentDataPath + "/" + levelName + ".png");

		            texture.wrapMode = TextureWrapMode.Clamp;
		            texture.filterMode = FilterMode.Bilinear;
		            texture.LoadImage(textureData);
				}
				catch {}
	        }

			//set thumbnail texture and other properties
			GameObject obj = levelHolder.transform.Find("Thumb").gameObject;
			
			Material material = new Material(Shader.Find("Mobile/Unlit (Supports Lightmap)"));
			material.mainTexture = texture;
			material.mainTextureScale = new Vector2 (1, obj.transform.localScale.y / obj.transform.localScale.x);
			material.mainTextureOffset = new Vector2 (0, (1 - material.mainTextureScale.y) / 2);
			obj.renderer.material = material;

			obj.name = levelName;
			obj.GetComponent<MyInputEvents>().OnTap += LevelThumb_Tap;

			//if play level - set delete button event
			if (levelName.StartsWith("play."))
			{
				obj = levelHolder.transform.Find("ButtonDeleteLevel").gameObject;
				obj.name = "ButtonDelete." + levelName;
				obj.GetComponent<MyInputEvents>().OnTap += ButtonDeleteLevel_Tap;

				levelHolder.transform.Find("ConfirmContainer/ButtonCancel").gameObject.GetComponent<MyInputEvents>().OnTap += ButtonDeleteLevelCancel_Tap;
				levelHolder.transform.Find("ConfirmContainer/ButtonConfirm").gameObject.GetComponent<MyInputEvents>().OnTap += ButtonDeleteLevelConfirm_Tap;
			}
		}
		void SetSelectedMaterial(ItemMaterial material)
		{
			selectedMaterial = material;
			MyTransform.SetPositionXY(materialSelector.transform, gameUIRect.Right - 1.065f, gameUIRect.Bottom + 3.31f - 0.7f * (int)selectedMaterial);
		}

		void SetupUIInputEvents()
		{
			MyInputEvents inputEvents;
			
			//Home and Galleries
			buttonLearnGallery.GetComponent<MyInputEvents>().OnTap += ButtonLearnGallery_Tap;
			buttonPlayGallery.GetComponent<MyInputEvents>().OnTap += ButtonPlayGallery_Tap;
			buttonNewLevel.GetComponent<MyInputEvents>().OnTap += ButtonNewLevel_Tap;

			//Game
			buttonMenu.GetComponent<MyInputEvents>().OnTap += ButtonMenu_Tap;
			buttonPlay.GetComponent<MyInputEvents>().OnTap += ButtonPlay_Tap;
			buttonStop.GetComponent<MyInputEvents>().OnTap += ButtonStop_Tap;
			buttonGrabDisabled.GetComponent<MyInputEvents>().OnTap += ButtonGrabDisabled_Tap;
			buttonGrabEnabled.GetComponent<MyInputEvents>().OnTap += ButtonGrabEnabled_Tap;


			//Toolbar buttons
			buttonRectangle.GetComponent<MyInputEvents>().OnTouch += ButtonCreate_Touch;
			buttonCircle.GetComponent<MyInputEvents>().OnTouch += ButtonCreate_Touch;
			buttonTriangle.GetComponent<MyInputEvents>().OnTouch += ButtonCreate_Touch;
			buttonFixed.GetComponent<MyInputEvents>().OnTap += ButtonMaterial_Tap;
			buttonMetal.GetComponent<MyInputEvents>().OnTap += ButtonMaterial_Tap;
			buttonWood.GetComponent<MyInputEvents>().OnTap += ButtonMaterial_Tap;
			buttonRubber.GetComponent<MyInputEvents>().OnTap += ButtonMaterial_Tap;
			buttonIce.GetComponent<MyInputEvents>().OnTap += ButtonMaterial_Tap;

			//Item buttons
			inputEvents = buttonDelete.GetComponent<MyInputEvents>();
			inputEvents.OnTap += ButtonDelete_Tap;
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
			inputEvents = background.GetComponent<MyInputEvents>();
			inputEvents.OnTap += Background_Tap;

			//Game camera movement and zoom
			inputEvents = master.GetComponent<MyInputEvents>();

			inputEvents.OnScreenTouch += Screen_Touch;
			inputEvents.OnScreenDrag += Screen_Drag;
			inputEvents.OnScreenRelease += Screen_Release;

			inputEvents.OnDoubleTouchStart += Master_ScreenDoubleTouchStart;
			inputEvents.OnDoubleTouchDrag += Master_ScreenDoubleTouchDrag;
			inputEvents.OnMouseScrollWheel += Master_MouseScrollWheel;
		}

		#endregion


		#region NAVIGATION

		//home
		void ButtonLearnGallery_Tap(GameObject sender, Camera camera)
		{
			if (gameStatus == GameStatus.Transition) return;

			StartCoroutine(TransitionTo(
				GameStatus.Menu,
				new MyRect(homeRect.Top, learnGalleryRect.Left, playGalleryRect.Bottom, learnGalleryRect.Right),
				new Vector2(0, learnGalleryRect.Top - cameraDefaultSize),
				cameraDefaultSize));
		}
		void ButtonPlayGallery_Tap(GameObject sender, Camera camera)
		{
			if (gameStatus == GameStatus.Transition) return;

			float yPos = playGalleryRect.Top - cameraDefaultSize;
			if (playGalleryRect.Height <= (cameraDefaultSize * 2))
				yPos = playGalleryRect.Bottom + cameraDefaultSize;

			StartCoroutine(TransitionTo(
				GameStatus.Menu,
				new MyRect(homeRect.Top, learnGalleryRect.Left, playGalleryRect.Bottom, learnGalleryRect.Right),
				new Vector2(0, yPos),
				cameraDefaultSize));
		}

		//galleries
		void ButtonNewLevel_Tap(GameObject sender, Camera camera)
		{
			if (gameStatus == GameStatus.Transition) return;

			if (playLevelsCount >= 35)
			{
				messageGalleryFull.SetActive(true);
				return;
			}

			currentLevel = "play." + System.DateTime.Now.ToString("yyyyMMddHHmmssff");

			//create new empty xml
			XmlDocument xmlDoc = new XmlDocument();
			XmlNode rootNode = xmlDoc.CreateElement("r");

			XmlAddAttribute(xmlDoc, rootNode, "s", cameraDefaultSize.ToString()); //size
			XmlAddAttribute(xmlDoc, rootNode, "x", gameRect.Center.x.ToString()); // x position
			XmlAddAttribute(xmlDoc, rootNode, "y", gameRect.Center.y.ToString()); // y position
			xmlDoc.AppendChild(rootNode);
			xmlDoc.Save(Application.persistentDataPath + "/" + currentLevel + ".xml");

			//copy new empty thumbnail
			Texture2D texture = Resources.Load<Texture2D>("NewLevelBg");
			File.WriteAllBytes(Application.persistentDataPath + "/" + currentLevel + ".png", texture.EncodeToPNG());


			//move the play gallery levels to the right to make room for the new level
			Vector2 startPos = GameObject.Find("PlayNewLevel").transform.position;

			GameObject obj;
			for (int i = playLevelsCount; i >= 1; i--)
			{
				obj = GameObject.Find("PlayLevel." + i.ToString("00"));
				obj.name = "PlayLevel." + (i + 1).ToString("00");
				MyTransform.SetPositionXY(obj.transform, startPos.x + (i + 1) % 4 * menuUnit * 244, startPos.y - (i + 1) / 4 * menuUnit * 225);
			}

			//add the new level to the gallery in the first position
			obj = (GameObject)GameObject.Instantiate(GameObject.Find("PlayLevel"));
			obj.transform.parent = GameObject.Find("PlayLevel").transform.parent;

			obj.name = "PlayLevel.01";
			MyTransform.SetPositionXY(obj.transform, startPos.x + (1) % 4 * menuUnit * 244, startPos.y - (1) / 4 * menuUnit * 225);
			StartCoroutine(LoadGalleryLevel(currentLevel, obj));

			playLevelsCount++;
			UpdateSceneZones();

			//load the new level
			LoadLevel(currentLevel);

			StartCoroutine(TransitionTo(
				GameStatus.Stop,
				gameRect,
				gameRect.Center,
				cameraDefaultSize));

			ShowGameUI();
		}
		void ButtonDeleteLevel_Tap(GameObject sender, Camera camera)
		{
			DeleteLevelCancel();

			sender.SetActive(false);
			GameObject levelHolder = sender.transform.parent.gameObject;
			levelHolder.transform.Find("ConfirmContainer").gameObject.SetActive(true);

			playLevelToDelete = sender.transform.parent.gameObject;
		}
		void ButtonDeleteLevelCancel_Tap(GameObject sender, Camera camera)
		{
			DeleteLevelCancel();
		}
		void ButtonDeleteLevelConfirm_Tap(GameObject sender, Camera camera)
		{
			//delete the level files
			try
			{
				string levelName = playLevelToDelete.transform.GetChild(0).name;
				File.Delete(Application.persistentDataPath + "/" + levelName + ".png");
				File.Delete(Application.persistentDataPath + "/" + levelName + ".xml");
			}
			catch {	return;	}

			//remove the level from the gallery
			playLevelToDelete.SetActive(false);
			GameObject.Destroy(playLevelToDelete);

			//move the following levels to the left to fill the gap
			Vector2 startPos = GameObject.Find("PlayNewLevel").transform.position;
			int index = int.Parse(playLevelToDelete.name.Substring(playLevelToDelete.name.Length - 2));
			GameObject obj;
			for (int i = index + 1; i <= playLevelsCount; i++)
			{
				obj = GameObject.Find("PlayLevel." + i.ToString("00"));
				obj.name = "PlayLevel." + (i - 1).ToString("00");
				MyTransform.SetPositionXY(obj.transform, startPos.x + (i - 1) % 4 * menuUnit * 244, startPos.y - (i - 1) / 4 * menuUnit * 225);
			}

			playLevelsCount--;
			playLevelToDelete = null;

			messageGalleryFull.SetActive(false);
			UpdateSceneZones();
		}
		void LevelThumb_Tap(GameObject sender, Camera camera)
		{
			if (gameStatus == GameStatus.Transition) return;
			if (playLevelToDelete == sender.transform.parent.gameObject)
				return;

			LoadLevel(sender.name);

			StartCoroutine(TransitionTo(
				GameStatus.Stop,
				gameRect,
				cameraTargetPosition,
				cameraTargetSize));

			ShowGameUI();
		}

		//menu
		void ButtonMenu_Tap(GameObject sender, Camera camera)
		{
			if (gameStatus == GameStatus.Transition) return;

			if (gameStatus == GameStatus.Stop) SaveLevel();

			HideGameUI();

			if (returnToPlay)
			{
				float yPos = playGalleryRect.Top - cameraDefaultSize;
				if (playGalleryRect.Height <= (cameraDefaultSize * 2))
					yPos = playGalleryRect.Bottom + cameraDefaultSize;

				StartCoroutine(TransitionTo(
					GameStatus.Menu,
					new MyRect(homeRect.Top, learnGalleryRect.Left, playGalleryRect.Bottom, learnGalleryRect.Right),
					new Vector2(0, yPos),
					cameraDefaultSize));				
			}
			else
				StartCoroutine(TransitionTo(
					GameStatus.Menu,
					new MyRect(homeRect.Top, learnGalleryRect.Left, playGalleryRect.Bottom, learnGalleryRect.Right),
					new Vector2(0, learnGalleryRect.Top - cameraDefaultSize),
					cameraDefaultSize));				
		}
			
		//main camera movement
		void Screen_Touch()
		{
			if (gameStatus == GameStatus.Transition) return;

			cameraTargetPosition = gameCamera.transform.position;
			cameraDragOffset = -(Vector2)Input.mousePosition / pixelsPerUnit - cameraTargetPosition;
			isCameraDragged = true;
		}
		void Screen_Drag()
		{
			if (gameStatus == GameStatus.Transition) return;

			if (isCameraDragged) cameraTargetPosition = -Input.mousePosition / pixelsPerUnit - cameraDragOffset;
		}
		void Screen_Release()
		{
			isCameraDragged = false;
		}

		//main camera pinch and zoom
		void Master_ScreenDoubleTouchStart(Touch touch0, Touch touch1)
		{
			doubleTouchDistance = Vector2.Distance(uiCamera.ScreenToWorldPoint(touch0.position), uiCamera.ScreenToWorldPoint(touch1.position));
		}
		void Master_ScreenDoubleTouchDrag(Touch touch0, Touch touch1)
		{
			if (gameStatus != GameStatus.Play && gameStatus != GameStatus.Stop) return;

			doubleTouchDistanceOffset = doubleTouchDistance - Vector2.Distance(uiCamera.ScreenToWorldPoint(touch0.position), uiCamera.ScreenToWorldPoint(touch1.position));
			cameraTargetSize = gameCamera.orthographicSize + doubleTouchDistanceOffset * gameCamera.orthographicSize;

			doubleTouchDistance = Vector2.Distance(uiCamera.ScreenToWorldPoint(touch0.position), uiCamera.ScreenToWorldPoint(touch1.position));
		}
		void Master_MouseScrollWheel(float amount)
		{
			if (gameStatus != GameStatus.Play && gameStatus != GameStatus.Stop) return;

			cameraTargetSize = gameCamera.orthographicSize + -amount * gameCamera.orthographicSize * 2;
		}

		//helpers
		void HideGameUI()
		{
			toolbar.SetActive(false);
			buttonMenu.SetActive(false);
			buttonPlay.SetActive(false);
			buttonStop.SetActive(false);
			buttonGrabDisabled.SetActive(false);
			buttonGrabEnabled.SetActive(false);
		}
		void ShowGameUI()
		{
			toolbar.SetActive(true);
			buttonMenu.SetActive(true);
			buttonPlay.SetActive(true);
			buttonStop.SetActive(false);
		}
		void DeleteLevelCancel()
		{
			if (playLevelToDelete == null)
				return;
			playLevelToDelete.transform.GetChild(1).gameObject.SetActive(true);
			playLevelToDelete.transform.GetChild(2).gameObject.SetActive(false);
			playLevelToDelete = null;
		}

		IEnumerator TransitionTo(GameStatus newGameStatus, MyRect newCameraTrapRect, Vector2 newCameraPosition, float newCameraSize)
		{
			targetCameraTrapRect = new MyRect(homeRect.Top, gameRect.Left, gameRect.Bottom, gameRect.Right + 10); //add 10 to the right to make sure we compansate for the variable right position of the camera trap
			cameraTargetSize = newCameraSize;
			cameraTargetPosition = newCameraPosition;

			gameStatus = GameStatus.Transition;

			yield return new WaitForSeconds(0.6f);

			targetCameraTrapRect = newCameraTrapRect;
			gameStatus = newGameStatus;
		}

		#endregion


		#region SAVE AND LOAD LEVEL

		void SaveLevel()
		{
			if (currentLevel == null || currentLevel.StartsWith ("learn.")) return;

			//SAVE XML DATA
			XmlDocument xmlDoc = new XmlDocument();
			XmlNode rootNode = xmlDoc.CreateElement("r");

			//root node - camera size and position
			XmlAddAttribute(xmlDoc, rootNode, "s", gameCamera.orthographicSize.ToString()); //size
			XmlAddAttribute(xmlDoc, rootNode, "x", gameCamera.transform.position.x.ToString()); // x position
			XmlAddAttribute(xmlDoc, rootNode, "y", gameCamera.transform.position.y.ToString()); // y position
			xmlDoc.AppendChild(rootNode);

			//item nodes - item properties
			XmlNode itemNode;
			ItemProperties itemProps;
			for (int i = 0; i < items.Length; i++)
			{
				itemNode = xmlDoc.CreateElement("i");
				itemProps = items[i].gameObject.GetComponent<ItemProperties>();

				XmlAddAttribute(xmlDoc, itemNode, "s", itemProps.shape.ToString()); //shape
				XmlAddAttribute(xmlDoc, itemNode, "m", itemProps.material.ToString()); //material
				XmlAddAttribute(xmlDoc, itemNode, "w", itemProps.width.ToString()); //width
				XmlAddAttribute(xmlDoc, itemNode, "h", itemProps.height.ToString()); //height
				XmlAddAttribute(xmlDoc, itemNode, "x", items[i].gameObject.transform.localPosition.x.ToString()); //x position
				XmlAddAttribute(xmlDoc, itemNode, "y", items[i].gameObject.transform.localPosition.y.ToString()); //y position
				XmlAddAttribute(xmlDoc, itemNode, "r", items[i].gameObject.transform.rotation.eulerAngles.z.ToString());// z-axis rotation

				rootNode.AppendChild(itemNode);
			}

			xmlDoc.Save(Application.persistentDataPath + "/" + currentLevel + ".xml");

			//SAVE PREVIEW IMAGE
			RenderTexture renderTexture = new RenderTexture(256, 256, 24, RenderTextureFormat.ARGB32);
			//renderTexture.antiAliasing = 2;
			gameCamera.targetTexture = renderTexture;
			gameCamera.Render();
			RenderTexture.active = renderTexture;
			Texture2D texture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
			texture.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
			texture.Apply();

			File.WriteAllBytes(Application.persistentDataPath + "/" + currentLevel + ".png", texture.EncodeToPNG());

			GameObject.Find(currentLevel).renderer.material.mainTexture = texture;

			//Clean up
			gameCamera.targetTexture = null;
			RenderTexture.active = null; //added to avoid errors 
			DestroyImmediate(renderTexture);
		}
		void LoadLevel(string levelName)
		{
			DeleteAllItems();

			XmlDocument xmlDoc = new XmlDocument();

			if (levelName.StartsWith ("learn."))
			{
				TextAsset textAsset = (TextAsset)Resources.Load("Learn/" + levelName.Replace("learn", "level"));
				xmlDoc.LoadXml(textAsset.text);
				returnToPlay = false;
			}
			else if (levelName.StartsWith("play."))
			{
				xmlDoc.Load(Application.persistentDataPath + "/" + levelName + ".xml");
				returnToPlay = true;
			}

			//set camera position and size
			XmlNode root = xmlDoc.DocumentElement;
			cameraTargetPosition = new Vector2(float.Parse(root.Attributes["x"].Value), float.Parse(root.Attributes["y"].Value));
			cameraTargetSize = float.Parse(root.Attributes["s"].Value);

			//create items
			ItemShape shape; ItemMaterial material; float width; float height;
			for (int i = 0; i < root.ChildNodes.Count; i++)
			{
				shape = (ItemShape)Enum.Parse(typeof(ItemShape), root.ChildNodes[i].Attributes["s"].Value);
				material = (ItemMaterial)Enum.Parse(typeof(ItemMaterial), root.ChildNodes[i].Attributes["m"].Value);
				width = float.Parse(root.ChildNodes[i].Attributes["w"].Value);
				height = float.Parse(root.ChildNodes[i].Attributes["h"].Value);

				CreateItem(shape, material, width, height);

				MyTransform.SetLocalPositionXY(items[i].gameObject.transform, float.Parse(root.ChildNodes[i].Attributes["x"].Value), float.Parse(root.ChildNodes[i].Attributes["y"].Value));
				items[i].gameObject.transform.eulerAngles = new Vector3(0, 0, float.Parse(root.ChildNodes[i].Attributes["r"].Value));
			}
			currentLevel = levelName;
		}
		void XmlAddAttribute(XmlDocument xmlDoc, XmlNode parentXmlNode, string attributeName, string attributeValue)
		{
			XmlAttribute attribute = xmlDoc.CreateAttribute(attributeName);
			attribute.Value = attributeValue;
			parentXmlNode.Attributes.Append(attribute);
		}

		#endregion


		#region EDITING

		//play, stop, grab
		void ButtonPlay_Tap(GameObject sender, Camera camera)
		{
			if (gameStatus == GameStatus.Transition) return;

			SaveLevel();

			gameStatus = GameStatus.Play;

			toolbar.SetActive(false);
			buttonPlay.SetActive(false);
			buttonStop.SetActive(true);
			buttonGrabDisabled.SetActive(true);

			HideItemControls();
			selectedItem = null;

			Physics2D.gravity = -Vector2.up * 9.8f;
			for (int i = 0; i < items.Length; i++)
			{
				Item.UpdatePhysicsMaterial(items[i].gameObject);
			}
		}
		void ButtonStop_Tap(GameObject sender, Camera camera)
		{
			if (gameStatus == GameStatus.Transition) return;

			LoadLevel(currentLevel);

			gameStatus = GameStatus.Stop;

			toolbar.SetActive(true);
			buttonPlay.SetActive(true);
			buttonStop.SetActive(false);
			buttonGrabDisabled.SetActive(false);
			buttonGrabEnabled.SetActive(false);

			Physics2D.gravity = Vector2.zero;
			for (int i = 0; i < items.Length; i++)
			{
				items[i].gameObject.rigidbody2D.isKinematic = true;
			}
		}
		void ButtonGrabDisabled_Tap(GameObject sender, Camera camera)
		{
			buttonGrabEnabled.SetActive(true);
			buttonGrabDisabled.SetActive(false);
		}
		void ButtonGrabEnabled_Tap(GameObject sender, Camera camera)
		{
			buttonGrabEnabled.SetActive(false);
			buttonGrabDisabled.SetActive(true);
		}
			
		//toolbar
		void ButtonCreate_Touch(GameObject sender, Camera camera)
		{
			if (sender == buttonRectangle)
				CreateNewItem(ItemShape.Rectangle, selectedMaterial);
			else if (sender == buttonCircle)
				CreateNewItem(ItemShape.Circle, selectedMaterial);
			else if (sender == buttonTriangle)
				CreateNewItem(ItemShape.Triangle, selectedMaterial);

			HideItemControls();
		}
		void ButtonMaterial_Tap(GameObject sender, Camera camera)
		{
			if (sender == buttonFixed)
			{
				SetSelectedMaterial(ItemMaterial.FixedMetal);
			}
			else if (sender == buttonMetal)
			{
				SetSelectedMaterial(ItemMaterial.Metal);
			}
			else if (sender == buttonWood)
			{
				SetSelectedMaterial(ItemMaterial.Wood);
			}
			else if (sender == buttonRubber)
			{
				SetSelectedMaterial(ItemMaterial.Rubber);
			}
			else if (sender == buttonIce)
			{
				SetSelectedMaterial(ItemMaterial.Ice);
			}
			if (selectedItem != null)
			{
				Item.ChangeMaterial(selectedItem, selectedMaterial);
			}
		}

		//item
		void ButtonDelete_Tap(GameObject sender, Camera camera)
		{
			items = Array.FindAll(items, delegate(PhysicsObject x)
				{
					return x.gameObject != selectedItem;
				});
			Destroy(selectedItem);
			selectedItem = null;
			HideItemControls();
		}
		void Item_Touch(GameObject sender, Camera camera)
		{
			//when editing
			if (gameStatus == GameStatus.Stop)
			{
				DragStart(sender, false);

				if (customCenterOfMass)
					moveOffset = gameCamera.WorldToScreenPoint(sender.rigidbody2D.worldCenterOfMass) - camera.WorldToScreenPoint(sender.transform.position);
				else
					moveOffset = Input.mousePosition - camera.WorldToScreenPoint(sender.transform.position);
			}
			//when playing
			else if (gameStatus == GameStatus.Play)
			{
				if (!buttonGrabEnabled.activeSelf)
				{
					sender.GetComponent<MyInputEvents>().allowScreenEvents = true;
					return;	
				}
				sender.GetComponent<MyInputEvents>().allowScreenEvents = false;

				selectedItemProps = sender.GetComponent<ItemProperties>();
				if (selectedItemProps.material != ItemMaterial.FixedMetal)
				{
					moveOffset = Input.mousePosition - camera.WorldToScreenPoint(sender.transform.position);
					sender.rigidbody2D.fixedAngle = true;
					sender.rigidbody2D.velocity = Vector2.zero;
					//sender.rigidbody2D.gravityScale = 0;
				}
			}
		}
		void Item_Drag(GameObject sender, Camera camera)
		{
			//when editing
			if (gameStatus == GameStatus.Stop)
			{
				if (customCenterOfMass)
					moveOffset = gameCamera.WorldToScreenPoint(sender.rigidbody2D.worldCenterOfMass) - camera.WorldToScreenPoint(sender.transform.position);
				sender.rigidbody2D.MovePosition(playgroundRect.GetInsidePosition(camera.ScreenToWorldPoint(Input.mousePosition - moveOffset)));
			}
			//when playing
			else if (gameStatus == GameStatus.Play)
			{
				if (buttonGrabEnabled.activeSelf && selectedItemProps.material != ItemMaterial.FixedMetal)
				{
					sender.rigidbody2D.MovePosition(playgroundRect.GetInsidePosition(camera.ScreenToWorldPoint(Input.mousePosition - moveOffset)));
					//cameraTargetPosition = sender.transform.position;
				}
			}
		}
		void Item_Release(GameObject sender, Camera camera)
		{
			//when editing
			if (gameStatus == GameStatus.Stop)
			{
				//sender.rigidbody2D.isKinematic = true;
				isItemDragged = false;
				makeKinematic = 1;
				if (customCenterOfMass)
					sender.rigidbody2D.centerOfMass = defaultCenterOfMass;

				ShowItemControls();
			}
			//when playing
			else if (gameStatus == GameStatus.Play)
			{
				sender.rigidbody2D.fixedAngle = false;
				//sender.rigidbody2D.gravityScale = 1;
			}
		}

		void ButtonMove_Touch(GameObject sender, Camera camera)
		{
			DragItem(selectedItem, true);
		}

		void ButtonRotate_Touch(GameObject sender, Camera camera)
		{
			selectedItem.rigidbody2D.isKinematic = false;
			isItemDragged = true;

			initialInputAngle = Vector2.Angle((Vector2)gameCamera.ScreenToWorldPoint(Input.mousePosition) - selectedItem.rigidbody2D.worldCenterOfMass, Vector2.right);
			if (gameCamera.ScreenToWorldPoint(Input.mousePosition).y < selectedItem.rigidbody2D.worldCenterOfMass.y) initialInputAngle = 360 - initialInputAngle;

			initialRotation = selectedItem.transform.eulerAngles.z;

			HideItemControls();
		}
		void ButtonRotate_Drag(GameObject sender, Camera camera)
		{
			float currentAngle = Vector2.Angle((Vector2)gameCamera.ScreenToWorldPoint(Input.mousePosition) - selectedItem.rigidbody2D.worldCenterOfMass, Vector2.right);
			if (gameCamera.ScreenToWorldPoint(Input.mousePosition).y < selectedItem.rigidbody2D.worldCenterOfMass.y) currentAngle = 360 - currentAngle;

			selectedItem.rigidbody2D.MoveRotation(initialRotation + currentAngle - initialInputAngle);
		}
		void ButtonRotate_Release(GameObject sender, Camera camera)
		{
			//selectedItem.rigidbody2D.isKinematic = true;
			makeKinematic = 1;
			isItemDragged = false;

			ShowItemControls();
		}

		void ButtonResize_Touch(GameObject sender, Camera camera)
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
		void ButtonResize_Drag(GameObject sender, Camera camera)
		{
			Vector2 currentInputPosition = resizeParent.transform.InverseTransformPoint(gameCamera.ScreenToWorldPoint(Input.mousePosition));
			Vector2 resizeOffset = Vector2.Scale(currentInputPosition - initialInputPosition, resizeCorner);

			if (selectedItemProps.shape == ItemShape.Circle)
			if (resizeOffset.x > resizeOffset.y) resizeOffset.x = resizeOffset.y;
			else resizeOffset.y = resizeOffset.x;

			if (initialSize.x + resizeOffset.x < 0.2f) resizeOffset.x = -initialSize.x + 0.2f;
			if (initialSize.x + resizeOffset.x > playgroundRect.Width - 2) resizeOffset.x = playgroundRect.Width - 2 - initialSize.x;
			if (initialSize.y + resizeOffset.y < 0.2f) resizeOffset.y = -initialSize.y + 0.2f;
			if (initialSize.y + resizeOffset.y > playgroundRect.Height - 2) resizeOffset.y = playgroundRect.Height - 2 - initialSize.y;

			Item.Resize(selectedItem, initialSize.x + resizeOffset.x, initialSize.y + resizeOffset.y);

			Vector2 moveOffset = Vector2.Scale(resizeOffset / 2, resizeCorner);
			Vector2 curLocPos = selectedItem.transform.localPosition;
			MyTransform.SetLocalPositionXY(selectedItem.transform, initialPosition + moveOffset);
			Vector2 newPos = selectedItem.transform.position;
			MyTransform.SetLocalPositionXY(selectedItem.transform, curLocPos);

			selectedItem.rigidbody2D.MovePosition(newPos);
		}
		void ButtonResize_Release(GameObject sender, Camera camera)
		{
			selectedItem.transform.parent = itemsContainer.transform;

			//selectedItem.rigidbody2D.isKinematic = true;
			makeKinematic = 1;
			isItemDragged = false;
			selectedItem.rigidbody2D.fixedAngle = false;

			ShowItemControls();
		}

		void ButtonClone_Touch(GameObject sender, Camera camera)
		{
			CloneItem();
		}

		void Background_Tap(GameObject sender, Camera camera)
		{
			if (gameStatus == GameStatus.Transition) return;

			if (gameStatus == GameStatus.Stop)
			{
				selectedItem = null;
				HideItemControls();
			}
		}

		//helpers
		void CreateNewItem(ItemShape itemShape, ItemMaterial itemMaterial)
		{
			float size = 1f / uiCamera.orthographicSize * gameCamera.orthographicSize;
			CreateItem(itemShape, itemMaterial, size, size);
			MyTransform.SetPositionXY(items[items.Length - 1].gameObject.transform, playgroundRect.GetInsidePosition(gameCamera.ScreenToWorldPoint(Input.mousePosition)));
			DragItem(items[items.Length - 1].gameObject, true);
		}
		void CloneItem()
		{
			CreateItem(selectedItemProps.shape, selectedItemProps.material, selectedItemProps.width, selectedItemProps.height);
			items[items.Length - 1].gameObject.transform.rotation = selectedItem.transform.rotation;
			MyTransform.SetPositionXY(items[items.Length - 1].gameObject.transform, selectedItem.transform.position);
			//items[items.Length - 1].gameObject.transform.position = selectedItem.transform.position;
			DragItem(items[items.Length - 1].gameObject, true);
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
			physicsObject.gameObject.rigidbody2D.isKinematic = true;

			//setup input events
			MyInputEvents inputEvents = physicsObject.gameObject.AddComponent<MyInputEvents>();

			inputEvents.OnTouch += Item_Touch;
			inputEvents.OnDrag += Item_Drag;
			inputEvents.OnRelease += Item_Release;

			//add the item to the list
			Array.Resize<PhysicsObject>(ref items, items.Length + 1);
			items[items.Length - 1] = physicsObject;
		}
		void DeleteAllItems()
		{
			for (int i = 0; i < items.Length; i++)
			{
				Destroy(items[i].gameObject);
			}
			Array.Resize<PhysicsObject>(ref items, 0);
		}

		void DragItem(GameObject item, bool fixedAngle)
		{
			DragStart(item, fixedAngle);

			if (customCenterOfMass)
				moveOffset = gameCamera.WorldToScreenPoint(item.rigidbody2D.worldCenterOfMass) - gameCamera.WorldToScreenPoint(item.transform.position);
			else
				moveOffset = Input.mousePosition - gameCamera.WorldToScreenPoint(item.transform.position);
			MyInput.Drag(item, gameCamera);
		}
		void DragStart(GameObject item, bool fixedAngle)
		{
			item.rigidbody2D.isKinematic = false;
			item.rigidbody2D.fixedAngle = fixedAngle;

			if (!fixedAngle)
			{
				defaultCenterOfMass = item.rigidbody2D.centerOfMass;
				customCenterOfMass = true;
				item.rigidbody2D.centerOfMass = item.transform.InverseTransformPoint(gameCamera.ScreenToWorldPoint(Input.mousePosition));
			}
			else
				customCenterOfMass = false;

			selectedItem = item;
			selectedItemProps = selectedItem.GetComponent<ItemProperties>();
			isItemDragged = true;
			SetSelectedMaterial(selectedItemProps.material);

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

		#endregion


	    #region UPDATE

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
	        
			if (gameStatus == GameStatus.Stop || gameStatus == GameStatus.Play)
			{
				cameraTrapRect = new MyRect(
					targetCameraTrapRect.Top - gameCamera.orthographicSize,
					targetCameraTrapRect.Left + gameCamera.orthographicSize * aspectRatio,
					targetCameraTrapRect.Bottom + gameCamera.orthographicSize,
					targetCameraTrapRect.Right - gameCamera.orthographicSize * aspectRatio - wallWidth + gameCamera.orthographicSize * 1.12f / uiCamera.orthographicSize); //keep the edge of the right wall aligned with the edge of the toolbar
			}
			else
			{
				cameraTrapRect = new MyRect(
					targetCameraTrapRect.Top - gameCamera.orthographicSize,
					targetCameraTrapRect.Left + gameCamera.orthographicSize * aspectRatio,
					targetCameraTrapRect.Bottom + gameCamera.orthographicSize,
					targetCameraTrapRect.Right - gameCamera.orthographicSize * aspectRatio);
			}

	        //set the position(animated)
	        MyTransform.SetPositionXY(gameCamera.transform, Vector2.Lerp(gameCamera.transform.position, cameraTargetPosition, 10f * Time.deltaTime));

	        //restrict the position
	        Vector2 trappedPosition = cameraTrapRect.GetInsidePosition(gameCamera.transform.position);

			if (Math.Abs(trappedPosition.x - gameCamera.transform.position.x) > 0.0001f)
	        {
	            cameraTargetPosition.x = trappedPosition.x;
	            MyTransform.SetPositionX(gameCamera.transform, cameraTargetPosition.x);

				if (isCameraDragged)
	                cameraDragOffset.x = -Input.mousePosition.x / pixelsPerUnit - cameraTargetPosition.x;
	        }

			if (Math.Abs(trappedPosition.y - gameCamera.transform.position.y) > 0.0001f)
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
}