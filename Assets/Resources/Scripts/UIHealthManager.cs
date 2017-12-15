using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthManager : MonoBehaviour {

    List<GameObject> healthObjects;

    int startingHealth;

	void Start () {
        healthObjects = new List<GameObject>();

        startingHealth = GameObject.Find("PersistentDataObject").GetComponent<PersistentData>().playerHealth;

        for (int i = 0; i < startingHealth / 2; ++i) {
            GameObject healthObject = Instantiate(Resources.Load<GameObject>("Prefabs/UIHealthItem"));
            healthObjects.Add(healthObject);
        }
	}
	
	void Update () {
		
	}
}
