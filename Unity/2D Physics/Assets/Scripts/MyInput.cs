using UnityEngine;
using System.Collections;

public class MyInput : MonoBehaviour
{
	public Camera[] cameras;

	static GameObject touchedObject = null;
	static Camera touchedObjectCamera;
	
	static MyInputEvents touchedObjectInputEvents;
	static Collider2D inputCollider;
	static Vector3 touchPosition = Vector3.zero;

	static int prevTouchCount = 0;

	void Update()
	{
		#region Single Touch
		
		//single touch start
		if (singleTouchStart)
		{
			for (int i = 0; i < cameras.Length; i++)
			{
				touchPosition = cameras[i].ScreenToWorldPoint(Input.mousePosition);
				touchPosition.z = 0;

				inputCollider = Physics2D.OverlapPoint(touchPosition, cameras[i].cullingMask);

				if (inputCollider != null)
				{
					touchedObjectInputEvents = inputCollider.gameObject.GetComponent<MyInputEvents>();

					if (touchedObjectInputEvents != null)
					{
						touchedObject = inputCollider.gameObject;
						touchedObjectCamera = cameras[i];
						touchedObjectInputEvents.Touch(touchedObject, touchedObjectCamera);
						break;
					}
				}
			}
		}

		//single touch drag
		if (singleTouchDrag && touchedObject != null)
		{
			touchedObjectInputEvents.Drag(touchedObject, touchedObjectCamera);
		}

		//single touch tap and release
		if (singleTouchEnd && touchedObject != null)
		{
			touchPosition = touchedObjectCamera.ScreenToWorldPoint(Input.mousePosition);
			touchPosition.z = 0;

			inputCollider = Physics2D.OverlapPoint(touchPosition, touchedObjectCamera.cullingMask);
			if (inputCollider != null)
			{
				if (touchedObject == inputCollider.gameObject)
				{
					touchedObjectInputEvents.Tap(touchedObject, touchedObjectCamera);
				}
			}

			touchedObjectInputEvents.Release(touchedObject, touchedObjectCamera);
			touchedObject = null;
		}

		#endregion

		#region Double Touch

		if (doubleTouchStart)
		{
			gameObject.GetComponent<MyInputEvents>().DoubleTouchStart(Input.touches[0], Input.touches[1]);
		}
		if (doubleTouchDrag)
		{
			gameObject.GetComponent<MyInputEvents>().DoubleTouchDrag(Input.touches[0], Input.touches[1]);
		}
		if (doubleTouchEnd)
		{
			gameObject.GetComponent<MyInputEvents>().DoubleTouchEnd();
		}

		#endregion

		//mouse scroll wheel
		if (Input.GetAxis("Mouse ScrollWheel") != 0)
		{
			gameObject.GetComponent<MyInputEvents>().MouseScrollWheel(Input.GetAxis("Mouse ScrollWheel"));
		}

		prevTouchCount = Input.touchCount;
	}

	public static void Drag(GameObject gameObject, Camera camera)
	{
		touchedObject = gameObject;
		touchedObjectCamera = camera;
		touchedObjectInputEvents = touchedObject.GetComponent<MyInputEvents>();
	}

	bool singleTouchStart
	{
		get
		{
			//left mouse button was pressed
			if (Input.GetMouseButtonDown(0) && Input.touchCount == 0) return true;

			//single touch started
			if (Input.touchCount == 1 && prevTouchCount == 0) return true;

			return false;
		}
	}
	bool singleTouchDrag
	{
		get
		{
			//left mouse button is down
			if (Input.GetMouseButton(0) && Input.touchCount == 0) return true;

			//single touch maintained
			if (Input.touchCount == 1 && (prevTouchCount == 0 || prevTouchCount == 1)) return true;

			return false;
		}
	}
	bool singleTouchEnd
	{
		get
		{
			//left mouse button was released
			if (Input.GetMouseButtonUp(0) && Input.touchCount == 0) return true;

			//single touch end or more touches added
			if ((Input.touchCount != 1 && prevTouchCount == 1)) return true;

			return false;
		}
	}

	public bool doubleTouchStart
	{
		get
		{
			//double touch started, either by adding or removing touches
			if (Input.touchCount == 2 && prevTouchCount != 2) return true;

			return false;
		}
	}
	public bool doubleTouchDrag
	{
		get
		{
			//double touch maintained
			if (Input.touchCount == 2) return true;

			return false;
		}
	}
	public bool doubleTouchEnd
	{
		get
		{
			//double touch ended, either by adding or removing touches
			if (Input.touchCount != 2 && prevTouchCount == 2) return true;

			return false;
		}
	}
}

