using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class shooterUnit : unit {

    // Use this for initialization

    int m_targetRange = 30;
    int m_shootRange = 15;

    int m_shotInterval = 1;
    float m_timeSinceLastShot;

    int m_damage = 2;

	new void Start ()
    {
        base.Start();
        m_animator.SetBool("run", true);

        m_timeSinceLastShot = m_shotInterval;
    }
	
	// Update is called once per frame
	new void  Update ()
    {
        base.Update();

        if (m_move)
        {
            float step = m_movementSpeed * Time.deltaTime;
            transform.position += m_direction * step;//Vector3.MoveTowards(transform.position, new Vector3(0, 0, 0), step);
            transform.LookAt(transform.position + m_direction);

            List<GameObject> enemyTeam = m_main.units[(m_team + 1) % 2];
            unit target = null;
            float smallestDist = float.MaxValue;
            m_direction = m_startDirection;
            enemyTeam.ForEach(x =>
            {
                float dist = Vector3.Distance(x.transform.position, transform.position);
                if (dist < m_targetRange && dist < smallestDist)
                {
                    smallestDist = dist;
                    m_direction = Vector3.Normalize(x.transform.position - transform.position);
                    
                    target = x.GetComponent<unit>();
                }
            });

            if (target && smallestDist < m_shootRange)
            {
                m_animator.SetBool("shoot", true);
                m_animator.SetBool("run", false);

                m_movementSpeed = 0;

                if (m_timeSinceLastShot >= m_shotInterval)
                {
                    target.ReceiveDamage(m_damage);
                    m_timeSinceLastShot = 0;
                }

                m_timeSinceLastShot += Time.deltaTime;
            }
            else
            {
                m_animator.SetBool("run", true);
                m_animator.SetBool("shoot", false);
                m_movementSpeed = 10;
            }                     
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var otherGameObject = other.gameObject;
        if(otherGameObject == null)
        {
            return;
        }

        var otherUnit = otherGameObject.GetComponent<unit>();
        if(otherUnit == null)
        {
            return;
        }

        var team = otherUnit.GetTeam();
        if (otherGameObject.layer != LayerMask.NameToLayer("Ground") && team != m_team)
        {
            m_animator.enabled = false;
            m_collider.enabled = false;
            m_move = false;

            m_main.units[m_team].Remove(this.gameObject);
        }
    }
}
