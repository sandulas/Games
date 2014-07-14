using UnityEngine;
using System.Collections;
using ThisProject;

namespace ThisProject
{
	public enum GameStatus
	{
		Play = 0,
		Pause = 1
	}

	public class SceneManager : MonoBehaviour
	{
		public GameStatus Status = GameStatus.Play;

		Vector2 playgroundSize = new Vector2(40, 25);
		float wallWidth = 0.5f;
		Vector2 sceneSize;
		float titleHeight = 10; float learnGalleryHeight = 15; float playGalleryHeight = 25;

		public static GameObject Background;
		public static Camera UICamera, MainCamera;
		
        private Vector2 cameraPosition;
		public static Vector2 CameraTargetPosition;
		public static float CameraTargetSize;
		public static float PixelsPerUnit, AspectRatio;

        GameObject[] obj;
        Vector2[] objVelocities;
        bool loaded = false;

		void Start()
		{
			Time.timeScale = 1;

			sceneSize = new Vector2(playgroundSize.x + 2 * wallWidth, playgroundSize.y + 2 * wallWidth + playGalleryHeight + learnGalleryHeight + titleHeight);

			//initialize the camera
			MainCamera = Camera.main;
			MainCamera.transform.position = new Vector3(0, 0, -12);
			CameraTargetPosition = new Vector2(0, 0);
			CameraTargetSize = MainCamera.orthographicSize;

			UICamera = Camera.allCameras[1];

			//initialize the background
			Background = GameObject.Find("Background");
			Background.transform.position = new Vector3(0, sceneSize.y / 2 - wallWidth - playgroundSize.y / 2, 0);
			Background.transform.localScale = new Vector3(sceneSize.x, sceneSize.y, 1);
			Background.renderer.material.mainTextureScale = new Vector2(sceneSize.x / 10, sceneSize.y / 10);

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
			PixelsPerUnit = Screen.height / MainCamera.orthographicSize / 2;
			AspectRatio = (float)Screen.width / Screen.height;

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
			//set the size
			if (CameraTargetSize < 3) CameraTargetSize = 3;
			else if (CameraTargetSize > playgroundSize.y / 2 + wallWidth) CameraTargetSize = playgroundSize.y / 2 + wallWidth;

			MainCamera.orthographicSize = Mathf.Lerp(MainCamera.orthographicSize, CameraTargetSize, 0.15f);
			PixelsPerUnit = Screen.height / MainCamera.orthographicSize / 2;

			//set the position
			MyTransform.SetPositionXY(MainCamera.transform, Vector2.Lerp(MainCamera.transform.position, CameraTargetPosition, 0.15f));
			if (MainCamera.transform.position.y > playgroundSize.y / 2 + wallWidth - MainCamera.orthographicSize)
			{
				CameraTargetPosition.y = playgroundSize.y / 2 + wallWidth - MainCamera.orthographicSize;
				MyTransform.SetPositionY(MainCamera.transform, CameraTargetPosition.y);
			}
			else if (MainCamera.transform.position.y < -playgroundSize.y / 2 - wallWidth + MainCamera.orthographicSize)
			{
				CameraTargetPosition.y = -playgroundSize.y / 2 - wallWidth + MainCamera.orthographicSize;
				MyTransform.SetPositionY(MainCamera.transform, CameraTargetPosition.y);
			}
			if (MainCamera.transform.position.x > playgroundSize.x / 2 + wallWidth - MainCamera.orthographicSize * AspectRatio)
			{
				CameraTargetPosition.x = playgroundSize.x / 2 + wallWidth - MainCamera.orthographicSize * AspectRatio;
				MyTransform.SetPositionX(MainCamera.transform, CameraTargetPosition.x);
			}
			else if (MainCamera.transform.position.x < -playgroundSize.x / 2 - wallWidth + MainCamera.orthographicSize * AspectRatio)
			{
				CameraTargetPosition.x = -playgroundSize.x / 2 - wallWidth + MainCamera.orthographicSize * AspectRatio;
				MyTransform.SetPositionX(MainCamera.transform, CameraTargetPosition.x);
			}
		}

		void InputManager_OnTouch(GameObject target, Camera camera, Vector3 offset)
		{
			//Debug.Log("Touch: " + target.name + "\r\n" + camera.name);
		}

        void InputManager_OnDrag(GameObject target, Camera camera, Vector3 offset)
        {
            //Debug.Log("Drag: " + target.name + "\r\n" + camera.name);
        }

        void InputManager_OnRelease(GameObject target, Camera camera, Vector3 offset)
        {
            //Debug.Log("Release: " + target.name + "\r\n" + camera.name);
        }

        void InputManager_OnTap(GameObject target, Camera camera, Vector3 offset)
        {

            //Pause Button
			if (target.name == "PauseButton") PauseButton_Tap(target, camera, offset);

			//-------
            if (target.name.StartsWith("Item"))
            {

            }
        }

		void PauseButton_Tap(GameObject target, Camera camera, Vector3 offset)
		{

			if (Status == GameStatus.Pause)
			{
				for (int i = 0; i < obj.Length; i++)
				{
					if (obj[i].GetComponent<ItemProperties>().Material != ItemMaterial.FixedMetal)
					{
						obj[i].rigidbody2D.isKinematic = false;
						obj[i].rigidbody2D.velocity = objVelocities[i];
					}
				}
				Status = GameStatus.Play;
			}
			else
			{
				for (int i = 0; i < obj.Length; i++)
				{
					objVelocities[i] = obj[i].rigidbody2D.velocity;
					obj[i].rigidbody2D.isKinematic = true;
				}
				Status = GameStatus.Pause;
			}

		}
    }
}