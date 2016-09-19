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

    protected int m_killTimer;
	
	public int GetTeam() { return m_team; }

	GameObject death_player;

	// Use this for initialization
	protected void Start ()
    {
		death_player =  Instantiate(Resources.Load("Death")) as GameObject;

        m_health = 10;
        m_animator = GetComponent<Animator>();
        m_collider = GetComponent<Collider>();
        m_movementSpeed = 20;
        m_killTimer = 3;
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

    public virtual bool ReceiveExplosion(int damage, int force, int range, Vector3 position)
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

	public virtual bool ReceiveDamage(int damage, bool deadly = false)
	{
		if (deadly)
		{
			m_health = 0;
		}
		else
		{
			m_health -= damage;
		}

		if (m_move) {
			GameObject su = Instantiate(Resources.Load("SimpleFX/Prefabs/FX_BloodSplatter")) as GameObject;
			su.transform.position = transform.position + new Vector3(0, 2, 0);
			su.transform.RotateAround(Vector3.up, Random.Range(0, 359));
			Destroy(su, 10);
		}


        if (m_health <= 0)
        { 
            if (m_animator)
                m_animator.enabled = false;

            if (m_collider)
                m_collider.enabled = false;

            m_move = false;

	death_player.transform.SetParent (this.transform);

            if(Random.Range(0, 100) < 20)
            {
                death_player.GetComponent<DeathSoundPlayer>().Play();
            }

            m_main.units[m_team].Remove(this.gameObject);

            Invoke("KillMe", m_killTimer);

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
		if (transform.position.z > main.zOffset && m_direction.z > 0)
        {
            ReceiveDamage(1, true);
        }
        else if(transform.position.z < -main.zOffset - 10 && m_direction.z < 0)
        {
            ReceiveDamage(1, true);
        }
    }
}
