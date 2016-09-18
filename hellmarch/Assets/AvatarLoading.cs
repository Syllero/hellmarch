using UnityEngine;
using System.Collections;

public class AvatarLoading : MonoBehaviour {

	public string url = "http://images.earthcam.com/ec_metros/ourcams/fridays.jpg";
	private Camera m_Camera = null;

	IEnumerator Start() {
		GameObject cameraobj = GameObject.FindGameObjectWithTag ("MainCamera");
		if (cameraobj) {
			m_Camera = cameraobj.GetComponent<Camera> ();
		}

		// Start a download of the given URL
		WWW www = new WWW(url);

		// Wait for download to complete
		yield return www;

		// assign texture
		Renderer renderer = GetComponent<Renderer>();
		renderer.material.mainTexture = www.texture;
	}

	void Update()
	{
		if (m_Camera) {
			transform.LookAt (transform.position + m_Camera.transform.rotation * Vector3.forward,
				m_Camera.transform.rotation * Vector3.up);
		}
	}
}