using UnityEngine;
using System.Collections;

public class unit : MonoBehaviour {

    protected int health;

    protected Animator m_animator;

	// Use this for initialization
	protected void Start ()
    {
        health = 10;
        m_animator = GetComponent<Animator>();
	}

    // Update is called once per frame
    protected void Update ()
    {

       
	}
}
