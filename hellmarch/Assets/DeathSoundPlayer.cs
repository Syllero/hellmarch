using UnityEngine;
using System.Collections;

public class DeathSoundPlayer : MonoBehaviour {

	private AudioSource[] sources;
    bool isPlaying = false;
	// Use this for initialization
	void Start () {
		sources = GetComponents<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Play() {
        if (!isPlaying)
        {
            isPlaying = true;
            sources[Random.Range(0, sources.Length)].Play();
        }
    }
}
