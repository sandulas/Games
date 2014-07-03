using UnityEngine;
using System.Collections;
using ThisProject;

public class InputManager : MonoBehaviour
{
	Collider2D inputCollider;
	GameObject draggedObject = null;
	Vector3 draggedObjectOffset, cameraOffset;
	Vector3 inputPosition = Vector3.zero;

	void Start()
	{
	}

	//Trebuie sa tratez separat inputul pentru UI fata de cel pentru joc; pentru ui trebuie folosita camera de UI pentru a face ScreenToWorldPoint.

    void Update2()
    {

    }

	void Update()
	{
		//touch
		if (Input.GetMouseButtonDown(0))
		{
			inputPosition = SceneManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
			inputPosition.z = 0;

			inputCollider = Physics2D.OverlapPoint(inputPosition);

			OnTouch(inputCollider.gameObject);

			if (inputCollider != null)
			{
				draggedObject = inputCollider.gameObject;
				draggedObjectOffset = inputPosition - draggedObject.transform.position;
                cameraOffset = -Input.mousePosition / SceneManager.PixelsPerUnit * 1.5f - SceneManager.MainCamera.transform.position;

				//bring the items to front
				if (draggedObject.name.StartsWith("Item")) Item.BringToFront(draggedObject);
			}
		}

		//drag
		if (Input.GetMouseButton(0))
		{
            inputPosition = SceneManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
			inputPosition.z = 0;

			if (draggedObject != null)
			{
				//drag the items
				if (draggedObject.name.StartsWith("Item"))
					if (Time.timeScale == 0)
						Item.Move(draggedObject, inputPosition - draggedObjectOffset);
					else
						draggedObject.rigidbody2D.MovePosition(new Vector2((inputPosition - draggedObjectOffset).x, (inputPosition - draggedObjectOffset).y));

				//drag the scene
				if (draggedObject == SceneManager.Background)
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
			if (inputCollider != null && inputCollider.gameObject == draggedObject)
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

			draggedObject = null;
		}

		//zoom
		if (Input.GetAxis("Mouse ScrollWheel") != 0)
		{
            SceneManager.CameraTargetSize = SceneManager.MainCamera.orthographicSize + -Input.GetAxis("Mouse ScrollWheel") * SceneManager.MainCamera.orthographicSize * 2;
		}
	}

	public delegate void TouchHandler(GameObject target);
	public static event TouchHandler OnTouch;
	public static void Touch(GameObject target) { if (OnTouch != null) OnTouch(target); }
}

