﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class nuke : MonoBehaviour {

    private Dictionary<int, int> m_pushers;

	// Use this for initialization
	new void Start () {
        m_pushers = new Dictionary<int, int>();
        m_pushers.Add(0, 0);
        m_pushers.Add(1, 0);
    }

    public void AddPusher(int team)
    {
        m_pushers[team]++;
    }

    public void RemovePusher(int team)
    {
        m_pushers[team]--;
    }

    // Update is called once per frame
    new void Update () {

        if (m_pushers[0] == m_pushers[1])
        {
            //Do nothing
        }
        else
        {
            Vector3 direction = m_pushers[0] > m_pushers[1] ? new Vector3(0, 0, 1) : new Vector3(0, 0, -1);
            transform.position += direction * Mathf.Abs(m_pushers[0] - m_pushers[1]) * Time.deltaTime;
        } 
    }
}