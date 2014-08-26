using UnityEngine;
using System.Collections;
using ThisProject;

public class InputManager : MonoBehaviour
{
	public static Camera touchCamera;
	public static GameObject touchObject = null;
	public static Vector3 touchPosition = Vector3.zero;
	public static float doubleTouchDistance = 0;
	public static float doubleTouchDistanceOffset;

	Collider2D inputCollider;
	int prevTouchCount = 0;

	void Start()
	{
	}


	void Update()
	{
		//single touch start
		if (getSingleTouchStart())
		{
			if (!RaiseTouch(SceneManager.uiCamera))
				RaiseTouch(SceneManager.mainCamera);
		}

		//single touch drag
		if (getSingleTouchDrag() && touchObject != null)
		{
			touchPosition = touchCamera.ScreenToWorldPoint(Input.mousePosition);
			touchPosition.z = 0;

			drag();
		}

		//single touch tap and release
		if (getSingleTouchEnd() && touchObject != null)
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

		//double touch start
		if (getDoubleTouchStart())
		{
			doubleTouchDistance = Vector2.Distance(SceneManager.uiCamera.ScreenToWorldPoint(Input.touches[0].position), SceneManager.uiCamera.ScreenToWorldPoint(Input.touches[1].position));
		}

		//double touch drag
		if (getDoubleTouchDrag())
		{
			doubleTouchDistanceOffset = doubleTouchDistance - Vector2.Distance(SceneManager.uiCamera.ScreenToWorldPoint(Input.touches[0].position), SceneManager.uiCamera.ScreenToWorldPoint(Input.touches[1].position));
			SceneManager.cameraTargetSize = SceneManager.mainCamera.orthographicSize + doubleTouchDistanceOffset * SceneManager.mainCamera.orthographicSize;

	
			doubleTouchDistance = Vector2.Distance(SceneManager.uiCamera.ScreenToWorldPoint(Input.touches[0].position), SceneManager.uiCamera.ScreenToWorldPoint(Input.touches[1].position));
		}

		//double touch end
		if (getDoubleTouchEnd())
		{

		}

		//mouse wheel zoom
		if (Input.GetAxis("Mouse ScrollWheel") != 0)
		{
			SceneManager.cameraTargetSize = SceneManager.mainCamera.orthographicSize + -Input.GetAxis("Mouse ScrollWheel") * SceneManager.mainCamera.orthographicSize * 2;
		}

		prevTouchCount = Input.touchCount;
	}

	private bool getSingleTouchStart()
	{
		//left mouse button was pressed
		if (Input.GetMouseButtonDown(0) && Input.touchCount == 0) return true;

		//single touch started
		if (Input.touchCount == 1 && prevTouchCount == 0) return true;

		return false;
	}
	public bool getSingleTouchDrag()
	{
		//left mouse button is down
		if (Input.GetMouseButton(0) && Input.touchCount == 0) return true;

		//single touch maintained
		if (Input.touchCount == 1 && (prevTouchCount == 0 || prevTouchCount == 1)) return true;

		return false;
	}
	private bool getSingleTouchEnd()
	{
		//left mouse button was released
		if (Input.GetMouseButtonUp(0) && Input.touchCount == 0) return true;

		//single touch end or more touches added
		if ((Input.touchCount != 1 && prevTouchCount == 1)) return true;

		return false;
	}
	public bool getDoubleTouchStart()
	{
		//double touch started, either by adding or removing touches
		if (Input.touchCount == 2 && prevTouchCount != 2) return true;

		return false;
	}
	public bool getDoubleTouchDrag()
	{
		//double touch maintained
		if (Input.touchCount == 2) return true;

		return false;
	}
	public bool getDoubleTouchEnd()
	{
		//double touch ended, either by adding or removing touches
		if (Input.touchCount != 2 && prevTouchCount == 2) return true;

		return false;
	}

	private bool RaiseTouch(Camera camera)
	{
		touchPosition = camera.ScreenToWorldPoint(Input.mousePosition);
		touchPosition.z = 0;

		inputCollider = Physics2D.OverlapPoint(touchPosition, camera.cullingMask);

		if (inputCollider != null)
		{
			touchCamera = camera;
			touchObject = inputCollider.gameObject;

			touch();
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

