using UnityEngine;
using System.Collections;
using ThisProject;

public class InputManager : MonoBehaviour
{
	Collider2D inputCollider;
	GameObject touchedObject = null;
	Vector3 touchOffset, cameraOffset;
	Vector3 inputPosition = Vector3.zero;
    Camera touchCamera;

	void Start()
	{
	}


    void Update()
    {
        //touch and drag
        if (Input.GetMouseButtonDown(0))
        {
            if (!RaiseTouchAndDrag(SceneManager.UICamera))
                RaiseTouchAndDrag(SceneManager.MainCamera);
        }

        //drag
        if (Input.GetMouseButton(0) && (touchedObject != null))
        {
            drag(touchedObject, touchCamera, touchOffset);
        }

        //tap and release
        if (Input.GetMouseButtonUp(0) && (touchedObject != null))
        {
            inputPosition = touchCamera.ScreenToWorldPoint(Input.mousePosition);
            inputPosition.z = 0;

            inputCollider = Physics2D.OverlapPoint(inputPosition, touchCamera.cullingMask);
            if (inputCollider != null)
            {
                if (touchedObject == inputCollider.gameObject)
                {
                    tap(touchedObject, touchCamera, touchOffset);
                }
            }

            release(touchedObject, touchCamera, touchOffset);
            touchedObject = null;
        }


        //zoom
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            SceneManager.CameraTargetSize = SceneManager.MainCamera.orthographicSize + -Input.GetAxis("Mouse ScrollWheel") * SceneManager.MainCamera.orthographicSize * 2;
        }

    }

    private bool RaiseTouchAndDrag(Camera camera)
    {
        inputPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        inputPosition.z = 0;

        inputCollider = Physics2D.OverlapPoint(inputPosition, camera.cullingMask);

        if (inputCollider != null)
        {
            touchedObject = inputCollider.gameObject;
            touchOffset = inputPosition - touchedObject.transform.position;
            touchCamera = camera;

            touch(touchedObject, touchCamera, touchOffset);
            drag(touchedObject, touchCamera, touchOffset);
            return true;
        }
        return false;
    }


	void Update2()
	{
		//touch
		if (Input.GetMouseButtonDown(0))
		{
			inputPosition = SceneManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
			inputPosition.z = 0;

			inputCollider = Physics2D.OverlapPoint(inputPosition);


			if (inputCollider != null)
			{
				touchedObject = inputCollider.gameObject;
				touchOffset = inputPosition - touchedObject.transform.position;
                cameraOffset = -Input.mousePosition / SceneManager.PixelsPerUnit * 1.5f - SceneManager.MainCamera.transform.position;
                OnTouch(inputCollider.gameObject, SceneManager.MainCamera, touchOffset);

				//bring the items to front
				if (touchedObject.name.StartsWith("Item")) Item.BringToFront(touchedObject);
			}
		}

		//drag
		if (Input.GetMouseButton(0))
		{
            inputPosition = SceneManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
			inputPosition.z = 0;

			if (touchedObject != null)
			{
				//drag the items
				if (touchedObject.name.StartsWith("Item"))
					if (Time.timeScale == 0)
						Item.Move(touchedObject, inputPosition - touchOffset);
					else
						touchedObject.rigidbody2D.MovePosition(new Vector2((inputPosition - touchOffset).x, (inputPosition - touchOffset).y));

				//drag the scene
				if (touchedObject == SceneManager.Background)
				{
					SceneManager.CameraTargetPosition = -Input.mousePosition / SceneManager.PixelsPerUnit * 1.5f - cameraOffset;
				}
			}
		}

		//release
		if (Input.GetMouseButtonUp(0))
		{
			inputCollider = Physics2D.OverlapPoint(inputPosition);

			//button clicks
			if (inputCollider != null && inputCollider.gameObject == touchedObject)
			{
				if (inputCollider.gameObject.name == "Pause")
				{
					if (Time.timeScale == 0) Time.timeScale = 1;
					else Time.timeScale = 0;
				}
				if (inputCollider.gameObject.name == "Rotate")
				{
					SceneManager.CameraTargetPosition = new Vector3(5, 0);
				}
				if (inputCollider.gameObject.name == "Move")
				{
					SceneManager.CameraTargetPosition = new Vector3(0, 0);
				}
			}

			touchedObject = null;
		}

		//zoom
		if (Input.GetAxis("Mouse ScrollWheel") != 0)
		{
            SceneManager.CameraTargetSize = SceneManager.MainCamera.orthographicSize + -Input.GetAxis("Mouse ScrollWheel") * SceneManager.MainCamera.orthographicSize * 2;
		}
	}

	public delegate void SingleTouchHandler(GameObject target, Camera camera, Vector3 offset);
	
    public static event SingleTouchHandler OnTouch;
	private static void touch(GameObject target, Camera camera, Vector3 offset) { if (OnTouch != null) OnTouch(target, camera, offset); }

    public static event SingleTouchHandler OnDrag;
    private static void drag(GameObject target, Camera camera, Vector3 offset) { if (OnDrag != null) OnDrag(target, camera, offset); }

    public static event SingleTouchHandler OnTap;
    private static void tap(GameObject target, Camera camera, Vector3 offset) { if (OnTap != null) OnTap(target, camera, offset); }

    public static event SingleTouchHandler OnRelease;
    private static void release(GameObject target, Camera camera, Vector3 offset) { if (OnRelease != null) OnRelease(target, camera, offset); }
}

