using UnityEngine;
using System.Collections;

public class unit : MonoBehaviour {

    protected int m_team;
    protected int m_health;
    protected int m_movementSpeed;

    protected Animator m_animator;

	// Use this for initialization
	protected void Start ()
    {
        m_health = 10;
        m_animator = GetComponent<Animator>();
        m_movementSpeed = 3;
    }

    public void SetTeam(int team)
    {
        m_team = team;
    }

    // Update is called once per frame
    protected void Update ()
    {
        float step = m_movementSpeed * Time.deltaTime;
        //transform.position = Vector3.MoveTowards(transform.position, new Vector3(100, 0, 100), step);
        //transform.LookAt(new Vector3(100, 0, 100));
	}
}
