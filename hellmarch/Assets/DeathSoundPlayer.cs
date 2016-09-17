using UnityEngine;
using System.Collections;

public class DeathSoundPlayer : MonoBehaviour {

	private AudioSource[] sources;
	// Use this for initialization
	void Start () {
		sources = GetComponents<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Play() {
		sources [Random.Range (0, sources.Length)].Play ();
	}
}
