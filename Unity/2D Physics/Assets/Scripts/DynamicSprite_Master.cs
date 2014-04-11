using UnityEngine;
using System.Collections;

public class DynamicSprite_Master : MonoBehaviour
{
	void Awake()
	{

	}

	void Start()
	{
		GameObject testObject;

		for (int i = 0; i < 50; i++)
		{
			testObject = Master.CreateBox(ObjectMaterial.FixedMetal, new Vector2(800, 200));
			testObject.transform.position = new Vector3(-3, 4, 0);
		}

		for (int i = 0; i < 50; i++)
		{
			testObject = Master.CreateBox(ObjectMaterial.FixedMetal, new Vector2(800, 200));
			testObject.transform.position = new Vector3(-3, 0, 0);
		}

		testObject = Master.CreateBox(ObjectMaterial.Wood, new Vector2(500, 500));
		testObject.transform.position = new Vector3(-2, 2, 0);

		testObject = Master.CreateBox(ObjectMaterial.FixedMetal, new Vector2(820, 200));
		testObject.transform.position = new Vector3(2, 3, 0);

		testObject = Master.CreateBox(ObjectMaterial.Metal, new Vector2(800, 200));
		testObject.transform.position = new Vector3(2, 1.5f, 0);

		testObject = Master.CreateBox(ObjectMaterial.Wood, new Vector2(800, 200));
		testObject.transform.position = new Vector3(2, 0, 0);

		testObject = Master.CreateBox(ObjectMaterial.Rubber, new Vector2(800, 200));
		testObject.transform.position = new Vector3(2, -1.5f, 0);

		testObject = Master.CreateBox(ObjectMaterial.Ice, new Vector2(800, 200));
		testObject.transform.position = new Vector3(2, -3, 0);
	}

	void Update()
	{

	}
}
