using UnityEngine;
using System.Collections;
using ThisProject;

public class InputManager : MonoBehaviour
{
	Collider2D collider;
	GameObject draggedObject = null;
	Vector3 draggedObjectOffset, cameraOffset;
	Vector3 touchPosition = Vector3.zero;

	void Start()
	{
	}

	void Update()
	{

		if (Input.GetMouseButton(0))
		{
			touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			touchPosition.z = 0;

			if (draggedObject != null)
			{
				//drag the items
				if (draggedObject.name.StartsWith("Item"))
					if (Time.timeScale == 0)
						Item.Move(draggedObject, touchPosition - draggedObjectOffset);
					else
						draggedObject.rigidbody2D.MovePosition(new Vector2((touchPosition - draggedObjectOffset).x, (touchPosition - draggedObjectOffset).y));

				//drag the scene
				if (draggedObject == SceneManager.Background)
				{
					SceneManager.CameraTargetPosition = -Input.mousePosition / SceneManager.PixelsPerUnit * 1.5f - cameraOffset;
				}
			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			collider = Physics2D.OverlapPoint(touchPosition);

			if (collider != null)
			{
				draggedObject = collider.gameObject;
				draggedObjectOffset = touchPosition - draggedObject.transform.position;
				cameraOffset = -Input.mousePosition / SceneManager.PixelsPerUnit * 1.5f - Camera.main.transform.position;

				//bring the items to front
				if (draggedObject.name.StartsWith("Item")) Item.BringToFront(draggedObject);
			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			collider = Physics2D.OverlapPoint(touchPosition);

			//button clicks
			if (collider != null && collider.gameObject == draggedObject)
			{
				if (collider.gameObject.name == "Pause")
				{
					if (Time.timeScale == 0) Time.timeScale = 1;
					else Time.timeScale = 0;
				}
				if (collider.gameObject.name == "Rotate")
				{
					SceneManager.CameraTargetPosition = new Vector3(5, 0);
				}
				if (collider.gameObject.name == "Move")
				{
					SceneManager.CameraTargetPosition = new Vector3(0, 0);
				}
			}

			draggedObject = null;
		}

		//zoom
		if (Input.GetAxis("Mouse ScrollWheel") != 0)
		{
			SceneManager.CameraTargetSize = Camera.main.orthographicSize + -Input.GetAxis("Mouse ScrollWheel") * Camera.main.orthographicSize * 2;
		}
	}
}
