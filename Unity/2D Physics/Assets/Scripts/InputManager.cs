using UnityEngine;
using System.Collections;
using ThisProject;

public class InputManager : MonoBehaviour
{
	public static Camera touchCamera;
	public static GameObject touchObject = null;
	public static Vector3 touchPosition = Vector3.zero;

	Collider2D inputCollider;
	Vector3 touchOffset;
	public static Vector3 CameraOffset;

	void Start()
	{
	}


	void Update()
	{
		//touch and drag
		if (Input.GetMouseButtonDown(0))
		{
			if (!RaiseTouchAndDrag(SceneManager.uiCamera))
				RaiseTouchAndDrag(SceneManager.mainCamera);
		}

		//drag
		if (Input.GetMouseButton(0) && (touchObject != null))
		{
			touchPosition = touchCamera.ScreenToWorldPoint(Input.mousePosition);
			touchPosition.z = 0;

			drag();
		}

		//tap and release
		if (Input.GetMouseButtonUp(0) && (touchObject != null))
		{
			touchPosition = touchCamera.ScreenToWorldPoint(Input.mousePosition);
			touchPosition.z = 0;

			inputCollider = Physics2D.OverlapPoint(touchPosition, touchCamera.cullingMask);
			if (inputCollider != null)
			{
				if (touchObject == inputCollider.gameObject)
				{
					tap();
				}
			}

			release();
			touchObject = null;
		}

		//zoom
		if (Input.GetAxis("Mouse ScrollWheel") != 0)
		{
			SceneManager.cameraTargetSize = SceneManager.mainCamera.orthographicSize + -Input.GetAxis("Mouse ScrollWheel") * SceneManager.mainCamera.orthographicSize * 2;
		}
	}

	private bool RaiseTouchAndDrag(Camera camera)
	{
		touchPosition = camera.ScreenToWorldPoint(Input.mousePosition);
		touchPosition.z = 0;

		inputCollider = Physics2D.OverlapPoint(touchPosition, camera.cullingMask);

		if (inputCollider != null)
		{
			touchCamera = camera;
			touchObject = inputCollider.gameObject;
			touchOffset = touchPosition - touchObject.transform.position;
			CameraOffset = -Input.mousePosition / SceneManager.pixelsPerUnit - touchCamera.transform.position;

			touch();
			drag();
			return true;
		}
		return false;
	}


	void Update2()
	{
		//touch
		if (Input.GetMouseButtonDown(0))
		{
			touchPosition = SceneManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);
			touchPosition.z = 0;

			inputCollider = Physics2D.OverlapPoint(touchPosition);


			if (inputCollider != null)
			{
				touchObject = inputCollider.gameObject;
				touchOffset = touchPosition - touchObject.transform.position;
				CameraOffset = -Input.mousePosition / SceneManager.pixelsPerUnit * 1.5f - SceneManager.mainCamera.transform.position;
				OnTouch();

				//bring the items to front
				if (touchObject.name.StartsWith("Item")) Item.BringToFront(touchObject);
			}
		}

		//drag
		if (Input.GetMouseButton(0))
		{
			touchPosition = SceneManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);
			touchPosition.z = 0;

			if (touchObject != null)
			{
				//drag the items
				if (touchObject.name.StartsWith("Item"))
					if (Time.timeScale == 0)
						Item.Move(touchObject, touchPosition - touchOffset);
					else
						touchObject.rigidbody2D.MovePosition(new Vector2((touchPosition - touchOffset).x, (touchPosition - touchOffset).y));

				//drag the scene
				if (touchObject == SceneManager.background)
				{
					SceneManager.cameraTargetPosition = -Input.mousePosition / SceneManager.pixelsPerUnit * 1.5f - CameraOffset;
				}
			}
		}

		//release
		if (Input.GetMouseButtonUp(0))
		{
			inputCollider = Physics2D.OverlapPoint(touchPosition);

			//button clicks
			if (inputCollider != null && inputCollider.gameObject == touchObject)
			{
				if (inputCollider.gameObject.name == "Pause")
				{
					if (Time.timeScale == 0) Time.timeScale = 1;
					else Time.timeScale = 0;
				}
				if (inputCollider.gameObject.name == "Rotate")
				{
					SceneManager.cameraTargetPosition = new Vector3(5, 0);
				}
				if (inputCollider.gameObject.name == "Move")
				{
					SceneManager.cameraTargetPosition = new Vector3(0, 0);
				}
			}

			touchObject = null;
		}

		//zoom
		if (Input.GetAxis("Mouse ScrollWheel") != 0)
		{
			SceneManager.cameraTargetSize = SceneManager.mainCamera.orthographicSize + -Input.GetAxis("Mouse ScrollWheel") * SceneManager.mainCamera.orthographicSize * 2;
		}
	}

	public delegate void SingleTouchHandler();

	public static event SingleTouchHandler OnTouch;
	private static void touch() { if (OnTouch != null) OnTouch(); }

	public static event SingleTouchHandler OnDrag;
	private static void drag() { if (OnDrag != null) OnDrag(); }

	public static event SingleTouchHandler OnTap;
	private static void tap() { if (OnTap != null) OnTap(); }

	public static event SingleTouchHandler OnRelease;
	private static void release() { if (OnRelease != null) OnRelease(); }
}

