using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public main main_object;
	public Vector3 startPosition;

	// Use this for initialization
	void Start ()
	{
		main_object = GameObject.FindGameObjectWithTag("main").GetComponent<main>();
		startPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		gameObject.GetComponent<Camera>().fieldOfView = 60;
		transform.LookAt(new Vector3(100.0f, 0.0f, 0.0f));
		transform.position = startPosition;

		if (main.winningTeam == 0)
		{
			float t = 0.15f * Mathf.PI * Mathf.Sin(Time.time * 0.1f) + Mathf.PI - 0.25f;
			Vector3 target = new Vector3(-50.0f, 20.0f, 200);
			transform.position = new Vector3(-50, 40, 200) + new Vector3(Mathf.Cos(t), 0, Mathf.Sin(t)) * 75;
			transform.LookAt(target);
			gameObject.GetComponent<Camera>().fieldOfView = 60;
		}
		else if (main.winningTeam == 1)
		{
			float t = 0.15f * Mathf.PI * Mathf.Sin(Time.time * 0.1f) + Mathf.PI + 0.25f;
			Vector3 target = new Vector3(-50.0f, 20.0f, -200);
			transform.position = new Vector3(-50, 40, -200) + new Vector3(Mathf.Cos(t), 0, Mathf.Sin(t)) * 75;
			transform.LookAt(target);
			gameObject.GetComponent<Camera>().fieldOfView = 60;
		}
		else if (main_object.nuke != null)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(main_object.nuke.transform.position.z * 2, -250, 250));
			transform.LookAt(main_object.nuke.transform.position);

			float dist = Mathf.Abs(transform.position.z);

			gameObject.GetComponent<Camera>().fieldOfView = Mathf.Clamp(250 - dist, 20, 60);
		}
	}

	
}
