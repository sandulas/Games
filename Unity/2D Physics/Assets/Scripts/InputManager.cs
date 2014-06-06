using UnityEngine;
using System.Collections;
using ThisProject;

public class InputManager : MonoBehaviour
{
	GameObject draggedObject = null;
	Vector3 draggedObjectOffset;

	void Start()
	{
	}

	void Update()
	{
		Collider2D collider;
		Vector3 touchPosition;

		if (Input.GetMouseButton(0))
		{
			touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			touchPosition.z = 0;
		}
		else touchPosition = Vector3.zero;
		

		if (Input.GetMouseButtonDown(0))
		{
			collider = Physics2D.OverlapPoint(touchPosition);

			if (collider != null)
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


				if (collider.gameObject.name.StartsWith("Item"))
				{
					draggedObject = collider.gameObject;
					draggedObjectOffset = touchPosition - draggedObject.transform.position;
					Item.BringToFront(draggedObject);
				}
			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			draggedObject = null;
		}

		if (Input.GetMouseButton(0))
		{
			if (draggedObject != null)
				if (Time.timeScale == 0)
					Item.Move(draggedObject, touchPosition - draggedObjectOffset);
				else
					draggedObject.rigidbody2D.MovePosition(new Vector2((touchPosition - draggedObjectOffset).x, (touchPosition - draggedObjectOffset).y));
		}
	}
}
