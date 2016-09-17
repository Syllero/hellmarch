using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public main main_object;

	// Use this for initialization
	void Start ()
	{
		main_object = GameObject.FindGameObjectWithTag("main").GetComponent<main>();
	}
	
	// Update is called once per frame
	void Update () {
		if(main.winningTeam == 0)
		{
			Vector3 target = new Vector3(-50.0f, 20.0f, 200);
			transform.LookAt(target);
			transform.position = new Vector3(-50, 40, 200) + new Vector3(Mathf.Cos(Time.time * 0.1f), 0, Mathf.Sin(Time.time * 0.1f)) * 75;
			gameObject.GetComponent<Camera>().fov = 60;
		}
		else if (main.winningTeam == 1)
		{
			Vector3 target = new Vector3(-50.0f, 20.0f, -200);
			transform.LookAt(target);
			transform.position = new Vector3(-50, 40, -200) + new Vector3(Mathf.Cos(Time.time * 0.1f), 0, Mathf.Sin(Time.time * 0.1f)) * 75;
			gameObject.GetComponent<Camera>().fov = 60;
		}
		else if (main_object.nuke != null)
		{
			transform.LookAt(main_object.nuke.transform.position);
			transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(main_object.nuke.transform.position.z * 2, -250, 250));

			float dist = Mathf.Abs(transform.position.z);

			gameObject.GetComponent<Camera>().fov = Mathf.Clamp(250 - dist, 20, 60);
		}
		else
		{
			gameObject.GetComponent<Camera>().fov = 60;
			transform.LookAt(new Vector3(100.0f, 0.0f, 0.0f));
		}
	}

	
}
