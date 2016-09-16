using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class main : MonoBehaviour {

    List<GameObject> units = new List<GameObject>();

	// Use this for initialization
	void Start () {
        Debug.Log("asofpasfpoasfusa");

        for(int i = 0; i < 500; i++)
        {
            
            GameObject su = Instantiate(Resources.Load("shooter_unit")) as GameObject;
            su.transform.Translate(-50 + i * 5, 0, 0);
            su.GetComponent<shooterUnit>().SetTeam(0);
            units.Add(su);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
