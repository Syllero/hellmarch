using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class main : MonoBehaviour {

    public Dictionary<int, List<GameObject>> units = new Dictionary<int, List<GameObject>>();

    // Use this for initialization
    void Start () {

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("RedTeam"), LayerMask.NameToLayer("Ignore"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("BlueTeam"), LayerMask.NameToLayer("Ignore"));

        units.Add(0, new List<GameObject>());
        units.Add(1, new List<GameObject>());

        for (int i = 0; i < 10; i++)
        {
            GameObject su = Instantiate(Resources.Load("shooter_unit_new")) as GameObject;
            su.transform.Translate(-50 + i * 10, 0, -100);
            su.GetComponent<shooterUnit>().Initialize(new Vector3(0,0,1), 0, this);
            units[0].Add(su);
        }

        for (int i = 0; i < 10; i++)
        {
            GameObject su = Instantiate(Resources.Load("shooter_unit_new")) as GameObject;
            su.transform.Translate(-50 + i * 12, 0, 100);
            su.GetComponent<shooterUnit>().Initialize(new Vector3(0, 0, -1), 1, this);
            units[1].Add(su);
        }
    }

    List<GameObject> GetTeam(int team)
    {
        return new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
