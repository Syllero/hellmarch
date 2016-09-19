using UnityEngine;
using System.Collections;

public class pusherUnit : unit {

    int pushRange = 10;
    bool pushing = false;
	private AudioSource wilhelm;
	// Use this for initialization
	new void Start () {
		wilhelm = GetComponents<AudioSource> () [1];
        base.Start();
        m_animator.SetBool("runNormal", true);
        m_movementSpeed = 20;
        m_killTimer = 1;
    }
	
	// Update is called once per frame
	new void Update () {
        base.Update();

        if (m_move)
        {
            float dist = Vector3.Distance(m_main.nuke.transform.position, transform.position);

            if (dist <= pushRange)
            {
                transform.parent = m_main.nuke.transform;

                if (!pushing)
                {
                    m_main.nuke.GetComponent<nuke>().AddPusher(m_team);
                    pushing = true;
                    m_animator.SetBool("push", true);
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

    public override bool ReceiveExplosion(int damage, int force, int range, Vector3 position)
    {
        if(ReceiveDamage(damage))
        {
            return base.ReceiveExplosion(damage, force, range, position);
        }

        return false;
    }

    public override bool ReceiveDamage(int damage, bool deadly = false)
    {
        if(base.ReceiveDamage(damage))
        {
            if (pushing)
            {
                m_main.nuke.GetComponent<nuke>().RemovePusher(m_team);
                pushing = false;
            }

            return true;
        }

        return false;
    }
}
