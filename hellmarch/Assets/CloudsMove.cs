using UnityEngine;
using System.Collections;

public class CloudsMove : MonoBehaviour {

	private float xpos = 0f;
	private float zpos = 0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		xpos += Time.deltaTime * 6.0f;
		if (xpos > 1000.0f) xpos -= 2000.0f;

		zpos += Time.deltaTime * 10.0f;
		if (zpos > 1000.0f) zpos -= 2000.0f;

		transform.position = new Vector3(xpos, transform.position.y, zpos);
	}
}
