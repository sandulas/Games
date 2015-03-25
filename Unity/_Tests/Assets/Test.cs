using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{
	public Camera camera;
	public GameObject cylinder;

	Vector3 worldPosition;

	// Use this for initialization
	void Start()
	{
		
	}
	
	// Update is called once per frame
	void Update()
	{
		camera.transform.position = new Vector3 (camera.transform.position.x, camera.transform.position.y + 0.1f, camera.transform.position.z);

		if (Input.GetMouseButton (0))
		{
			//Debug.Log (Input.mousePosition);

			worldPosition = camera.ScreenPointToRay(Input.mousePosition).GetPoint(4);

			Debug.Log (worldPosition);

			worldPosition.z = cylinder.transform.position.z;
			cylinder.transform.position = worldPosition;

		}
	}
}
