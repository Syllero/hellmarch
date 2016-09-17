using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class suicideUnit : unit {

    int m_targetRange = 30;
    int m_fireRange = 10;
    int m_explosionRange = 20;
    int m_damage = 10;
    
	private AudioSource[] explode;

    // Use this for initialization
    new void Start () {
		explode = GetComponents<AudioSource> ();
        base.Start();
        m_animator.SetBool("runBomb", true);
        m_movementSpeed = 20;
        m_health = 15;
    }

    // Update is called once per frame
    new void Update () {
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

            if (target && smallestDist < m_fireRange)
            {
                enemyTeam.ForEach(x =>
                {
                    float dist = Vector3.Distance(x.transform.position, transform.position);

                    if ( dist < m_explosionRange)
                    {
                        x.GetComponent<unit>().ReceiveExplosion(m_damage, 1000, 200, transform.position);
                    }
                });

		explode[Random.Range(0,explode.Length)].Play ();

                m_main.units[m_team].ForEach(x =>
                {
                    float dist = Vector3.Distance(x.transform.position, transform.position);

                    if (dist < m_explosionRange)
                    {
                        x.GetComponent<unit>().ReceiveExplosion(m_damage, 1000, 200, transform.position);
                    }
                });

                ReceiveDamage(m_health);

                var transforms = gameObject.GetComponentsInChildren<Transform>();
                foreach (Transform child in transforms)
                {
                    if (child.CompareTag("bomb"))
                        Destroy(child.gameObject);
                }

                //var t = transform.FindChild("suicide_bomb");

                GameObject su = Instantiate(Resources.Load("SimpleFX/Prefabs/FX_Explosion_Rubble")) as GameObject;
                su.transform.position = transform.position;

                Destroy(su, 10);
            }
        }       
	}
}
