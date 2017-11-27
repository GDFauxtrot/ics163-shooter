using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShipType { LITE, MED, HEAVY };

public class PersistentData : MonoBehaviour {

    public int currentWorld;

    public int playerMoney;

    public ShipType playerClassType;

    // Player upgrade values
    public int playerHealth;
    public int playerDamage;
    public float playerChargeTime;

	void Awake () {
        // First things first, set persistent property to object
        DontDestroyOnLoad(gameObject);

        foreach (GameObject other in FindObjectsOfType(typeof(GameObject))) {
            if (other.name == "PersistentDataObject" && other != gameObject) {
                Destroy(gameObject);
            }
        }

        SetDefaultValues();
	}

	void Update () {
		
	}

    void SetDefaultValues() {
        currentWorld = 0;

        playerMoney = 5000;

        SetMedClassValues();
    }

    // Reference UpgradeManager in UpgradeMenu folder for values
    void SetLightClassValues() {
        playerClassType = ShipType.LITE;

        playerHealth = 2;
        playerDamage = 1;
        playerChargeTime = 1.5f;
    }

    void SetMedClassValues() {
        playerClassType = ShipType.MED;

        playerHealth = 3;
        playerDamage = 2;
        playerChargeTime = 2;
    }

    void SetHeavyClassValues() {
        playerClassType = ShipType.HEAVY;

        playerHealth = 4;
        playerDamage = 3;
        playerChargeTime = 3;
    }
}
