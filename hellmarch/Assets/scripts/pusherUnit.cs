﻿using UnityEngine;
using System.Collections;

public class pusherUnit : unit {

    int pushRange = 10;
    bool pushing = false;

	// Use this for initialization
	new void Start () {
        base.Start();
        m_animator.SetBool("runNormal", true);
        m_movementSpeed = 20;
    }
	
	// Update is called once per frame
	new void Update () {
        base.Update();

        if (m_move)
        {
            float dist = Vector3.Distance(m_main.nuke.transform.position, transform.position);

            if (dist <= pushRange)
            {
                Debug.Log(dist);
                transform.parent = m_main.nuke.transform;

                if (!pushing)
                {
                    m_main.nuke.GetComponent<nuke>().AddPusher(m_team);
                    pushing = true;
                }
            }
            else
            {
                float step = m_movementSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, m_main.nuke.transform.position, step);
                transform.LookAt(m_main.nuke.transform.position);
                pushing = false;
            }
        }
	
	}

    public override bool ReceiveDamage(int damage)
    {
        if(base.ReceiveDamage(damage))
        {
            m_main.nuke.GetComponent<nuke>().RemovePusher(m_team);
            pushing = false;
            return true;
        }

        return false;
    }
}