using UnityEngine;
using System.Collections;
using ThisProject;

public class InputManager : MonoBehaviour
{
	public static Camera touchCamera;
	public static GameObject touchObject = null;
	public static Vector3 touchPosition = Vector3.zero;

	Collider2D inputCollider;

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

			touch();
			drag();
			return true;
		}
		return false;
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

