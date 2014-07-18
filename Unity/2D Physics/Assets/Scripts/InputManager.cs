using UnityEngine;
using System.Collections;
using ThisProject;

public class InputManager : MonoBehaviour
{
	Collider2D inputCollider;
	GameObject touchedObject = null;
	Vector3 touchOffset;
	public static Vector3 CameraOffset;
	public static Vector3 worldPosition = Vector3.zero;
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
			worldPosition = touchCamera.ScreenToWorldPoint(Input.mousePosition);
			worldPosition.z = 0;

			drag(touchedObject, touchCamera, touchOffset);
		}

        //tap and release
        if (Input.GetMouseButtonUp(0) && (touchedObject != null))
        {
            worldPosition = touchCamera.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0;

            inputCollider = Physics2D.OverlapPoint(worldPosition, touchCamera.cullingMask);
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
        worldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0;

        inputCollider = Physics2D.OverlapPoint(worldPosition, camera.cullingMask);

        if (inputCollider != null)
        {
			touchCamera = camera;
			touchedObject = inputCollider.gameObject;
            touchOffset = worldPosition - touchedObject.transform.position;
			CameraOffset = -Input.mousePosition / SceneManager.PixelsPerUnit - touchCamera.transform.position;

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
			worldPosition = SceneManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
			worldPosition.z = 0;

			inputCollider = Physics2D.OverlapPoint(worldPosition);


			if (inputCollider != null)
			{
				touchedObject = inputCollider.gameObject;
				touchOffset = worldPosition - touchedObject.transform.position;
                CameraOffset = -Input.mousePosition / SceneManager.PixelsPerUnit * 1.5f - SceneManager.MainCamera.transform.position;
                OnTouch(inputCollider.gameObject, SceneManager.MainCamera, touchOffset);

				//bring the items to front
				if (touchedObject.name.StartsWith("Item")) Item.BringToFront(touchedObject);
			}
		}

		//drag
		if (Input.GetMouseButton(0))
		{
            worldPosition = SceneManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
			worldPosition.z = 0;

			if (touchedObject != null)
			{
				//drag the items
				if (touchedObject.name.StartsWith("Item"))
					if (Time.timeScale == 0)
						Item.Move(touchedObject, worldPosition - touchOffset);
					else
						touchedObject.rigidbody2D.MovePosition(new Vector2((worldPosition - touchOffset).x, (worldPosition - touchOffset).y));

				//drag the scene
				if (touchedObject == SceneManager.Background)
				{
					SceneManager.CameraTargetPosition = -Input.mousePosition / SceneManager.PixelsPerUnit * 1.5f - CameraOffset;
				}
			}
		}

		//release
		if (Input.GetMouseButtonUp(0))
		{
			inputCollider = Physics2D.OverlapPoint(worldPosition);

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

