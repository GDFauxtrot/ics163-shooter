using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour {

    // Private Swarm Info
    // WIDTH: 4
    // Height: 4
    // Units: 7


    public bool CANMOVE;
    public bool MOVELEFT = true; // Set true for moving left at start of game
    public bool ISFILLING = true;
    public bool ISATTACKING = false;

    // swarm location information
    public float left_maxX = 0;
    public float left_minX = -4;
    public float right_maxX = 4;
    public float right_minX = 0;
    public Vector3 left_center = new Vector3(-2, 6, 0); // initially set to left swarm starting location
    public Vector3 right_center = new Vector3(2, 6, 0); // initically set to right swarm starting location
    public Vector3 temp_center;

    public GameObject left_swarm;
    public GameObject right_swarm;

    public int max_swarms = 8; // Should be same as size of list
    public int num_swarms_remaing; // Keep track until -1, where last swarm destroyed (no spawning after 0)
    public GameObject[] swarm_array; // Place swarm prefabs in order of appearance



	// Use this for initialization
	void Start () {
        num_swarms_remaing = max_swarms;
        LeftInstantiate();
        RightInstantiate();
	}
	
	// Update is called once per frame
	void Update () {
        Checks();
        // Function for creating attacks
	}

    void Checks()
    {

        // if is attacking or filling, set moving to false, else true
        if (ISFILLING || ISATTACKING)
        {
            CANMOVE = false;
        } else
        {
            CANMOVE = true;
        }

        // max_swarms - 2 to eliminate setting temp_center on first instantiation for wave
        if ((left_swarm == null) && (num_swarms_remaing > 0) && (num_swarms_remaing < max_swarms -2))
        {
            left_center = temp_center;
            LeftInstantiate();
        } else 
        if ((right_swarm == null) && (num_swarms_remaing > 0))
        {
            right_center = temp_center;
            RightInstantiate();
        } else
        if ((right_swarm == null) && (left_swarm == null) && num_swarms_remaing == 0)
        {
            // Call cleanup function here (all swarms destroyed) 
        }
    }

    public void SwapDirection()
    {
        MOVELEFT = !MOVELEFT;
    }

    public void LeftInstantiate()
    {
        ISFILLING = true;
        left_swarm = Instantiate(swarm_array[max_swarms - num_swarms_remaing], left_center,
                                    Quaternion.identity) as GameObject;
        num_swarms_remaing--;
    }

    public void RightInstantiate()
    {
        ISFILLING = true;
        left_swarm = Instantiate(swarm_array[max_swarms - num_swarms_remaing], right_center,
                                    Quaternion.identity) as GameObject;
        num_swarms_remaing--;
    }

    public void SetNewCenter(Vector3 center)
    {
        temp_center.x = center.x;
        temp_center.y = center.y;
        temp_center.z = center.z;
    }

    public void FillingComplete()
    {
        ISFILLING = false;
    }

    public void AttackComplete()
    {
        ISATTACKING = false;
    }
}
