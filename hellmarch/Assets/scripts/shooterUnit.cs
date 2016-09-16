using UnityEngine;
using System.Collections;

public class shooterUnit : unit {

	// Use this for initialization
	new void Start ()
    {
        base.Start();
	}
	
	// Update is called once per frame
	new void  Update ()
    {
        if (Input.anyKeyDown)
        {
            Debug.Log("ayy lmao");
            Debug.Log(health);
            m_animator.SetBool("walk", true);
            //m_animator.Play("walk");
        }
    
    }
}
