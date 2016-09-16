using UnityEngine;
using System.Collections;

public class shooterUnit : unit {

	// Use this for initialization
	new void Start ()
    {
        base.Start();
        m_animator.SetBool("walk", true);
    }
	
	// Update is called once per frame
	new void  Update ()
    {
        base.Update();
        if (Input.anyKeyDown)
        {
            
            //m_animator.enabled = false;
        }  
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != LayerMask.NameToLayer("Ground"))
            m_animator.enabled = false;
    }
}
