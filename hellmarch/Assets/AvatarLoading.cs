using UnityEngine;
using System.Collections;

public class AvatarLoading : MonoBehaviour {

	public string url = "http://images.earthcam.com/ec_metros/ourcams/fridays.jpg";

	IEnumerator Start() {
		// Start a download of the given URL
		WWW www = new WWW(url);

		// Wait for download to complete
		yield return www;

		// assign texture
		Renderer renderer = GetComponent<Renderer>();
		renderer.material.mainTexture = www.texture;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
