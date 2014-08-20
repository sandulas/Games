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
		//DO NOT USE Input.MousePosition ANYWHERE. IT REPORTS WORNG POSITION FOR DOUBLE TOUCH. USE Touch.position INSTEAD.


		//touch and drag
		if (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && prevTouchCount == 0))
		{
			if (!RaiseTouchAndDrag(SceneManager.uiCamera))
				RaiseTouchAndDrag(SceneManager.mainCamera);
		}

		//drag
		if ((Input.GetMouseButton(0)) && (touchObject != null))
		{
			touchPosition = touchCamera.ScreenToWorldPoint(Input.mousePosition);
			touchPosition.z = 0;

			drag();
		}

		//tap and release
		if ((Input.GetMouseButtonUp(0) || (Input.touchCount != 1 && prevTouchCount == 1)) && (touchObject != null))
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
		if (Input.touchCount == 2 && prevTouchCount != 2)
		{
			doubleTouchDistance = Vector2.Distance(SceneManager.uiCamera.ScreenToWorldPoint(Input.touches[0].position), SceneManager.uiCamera.ScreenToWorldPoint(Input.touches[1].position));
		}
		//double touch drag
		if (Input.touchCount == 2 && prevTouchCount == 2)
		{
			doubleTouchDistanceOffset = doubleTouchDistance - Vector2.Distance(SceneManager.uiCamera.ScreenToWorldPoint(Input.touches[0].position), SceneManager.uiCamera.ScreenToWorldPoint(Input.touches[1].position));
			SceneManager.cameraTargetSize = SceneManager.mainCamera.orthographicSize + doubleTouchDistanceOffset * SceneManager.mainCamera.orthographicSize;

	
			doubleTouchDistance = Vector2.Distance(SceneManager.uiCamera.ScreenToWorldPoint(Input.touches[0].position), SceneManager.uiCamera.ScreenToWorldPoint(Input.touches[1].position));

		}
		//double touch end
		if (Input.touchCount != 2 && prevTouchCount == 2)
		{

		}

		//zoom
		if (Input.GetAxis("Mouse ScrollWheel") != 0)
		{
			SceneManager.cameraTargetSize = SceneManager.mainCamera.orthographicSize + -Input.GetAxis("Mouse ScrollWheel") * SceneManager.mainCamera.orthographicSize * 2;
		}

		prevTouchCount = Input.touchCount;
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

