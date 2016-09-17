using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class unit : MonoBehaviour {

    protected int m_team;
    protected int m_health;
    protected int m_movementSpeed;

    protected Animator m_animator;
    protected Collider m_collider;

    protected Vector3 m_direction;
    protected Vector3 m_startDirection;

    protected float m_maxRange;

    protected main m_main;

    protected bool m_move;

    public int GetTeam() { return m_team; }


	// Use this for initialization
	protected void Start ()
    {
        m_health = 10;
        m_animator = GetComponent<Animator>();
        m_collider = GetComponent<Collider>();
        m_movementSpeed = 20;
        m_move = true;
    }

    public void Initialize(Vector3 startDirection, int team, main main)
    {
        SetTeam(team);
        m_direction = m_startDirection = startDirection;
        m_main = main;
    }

    public void SetTeam(int team)
    {
        m_team = team;
    }

    public bool ReceiveExplosion(int damage, int force, int range, Vector3 position)
    {
        if(ReceiveDamage(damage))
        {
            var rigidBodies = GetComponentsInChildren<Rigidbody>();

            for(int i = 0; i < rigidBodies.Length; i++)
            {
                //rigidBodies[i].AddExplosionForce(force, position, range);
            } 

            //GetComponent<Rigidbody>().AddExplosionForce(force, position, range);
            //GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 1) * 10000, ForceMode.Impulse);
            return true;
        }

        return false;
    }

    public bool ReceiveDamage(int damage)
    {
        m_health -= damage;

        GameObject su = Instantiate(Resources.Load("SimpleFX/Prefabs/FX_BloodSplatter")) as GameObject;
        su.transform.position = transform.position + new Vector3(0, 2, 0);
        su.transform.RotateAround(Vector3.up, Random.Range(0, 359));

        Destroy(su, 10); 

        if (m_health <= 0)
        {
            m_main.units[m_team].Remove(this.gameObject);

            m_animator.enabled = false;
            m_collider.enabled = false;
            m_move = false;

            Invoke("KillMe", 20);
            return true;
        }
        return false;
    }

    protected void KillMe()
    {
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    protected void Update ()
    {
      
    }
}
