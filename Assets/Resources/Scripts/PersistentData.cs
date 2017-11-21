using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour {

    public bool isFirstOfItsKind;

    public int currentWorld;

	void Awake () {
        // First things first, set persistent property to object
        DontDestroyOnLoad(gameObject);

        foreach (GameObject other in FindObjectsOfType(typeof(GameObject))) {
            if (other.name == "PersistentDataObject") {
                if (other != gameObject && !isFirstOfItsKind) {
                    Destroy(gameObject);
                }
            }
        }

        isFirstOfItsKind = true;

        SetDefaultValues();
	}

	void Update () {
		
	}

    void SetDefaultValues() {
        currentWorld = 0;
    }
}
