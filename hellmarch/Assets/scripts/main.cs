using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class main : MonoBehaviour {

    List<GameObject> units = new List<GameObject>();

	// Use this for initialization
	void Start () {
        Debug.Log("asofpasfpoasfusa");

        for(int i = 0; i < 100; i++)
        {
            GameObject shooterUnit = Instantiate(Resources.Load("shooter_unit")) as GameObject;
            shooterUnit.transform.Translate(-50 + i, 0, 0);
            units.Add(shooterUnit);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
