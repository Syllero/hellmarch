using UnityEngine;
using System.Collections;

public class pusherUnit : unit {

	// Use this for initialization
	new void Start () {
        base.Start();
        m_animator.SetBool("run", true);
        m_movementSpeed = 20;
    }
	
	// Update is called once per frame
	new void Update () {
        base.Update();
	
	}
}
