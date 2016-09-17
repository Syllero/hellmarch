using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class shooterUnit : unit {

	// Use this for initialization
	new void Start ()
    {
        base.Start();
        m_animator.SetBool("walk", true);

        m_maxRange = 25;
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

            float smallestDist = float.MaxValue;
            m_direction = m_startDirection;
            enemyTeam.ForEach(x =>
            {
                float dist = Vector3.Distance(x.transform.position, transform.position);
                if (dist < m_maxRange && dist < smallestDist)
                {
                    smallestDist = dist;
                    m_direction = Vector3.Normalize(x.transform.position - transform.position);
                    m_movementSpeed = 0;
                }
            });      
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
