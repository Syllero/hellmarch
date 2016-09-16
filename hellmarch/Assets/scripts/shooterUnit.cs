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
        base.Update();
        if (Input.anyKeyDown)
        {
            //m_animator.SetBool("walk", true);
            m_animator.enabled = false;
        }  
    }
}
