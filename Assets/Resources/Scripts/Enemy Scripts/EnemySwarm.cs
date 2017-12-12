using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwarm : MonoBehaviour {

    /* This class will control:
     *      Spawning Enemy Waves
     *      When Enemies Move
     *      Notifiying when the wave is destroyed / finished
     */

    public float formation_width, formation_height;
    public GameObject enemy_prefab; 

	// Use this for initialization
	void Start () {
        SpawnAll();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(this.transform.position,
                            new Vector3(formation_width, formation_height, 0));
    }


        /* Spawning
         * --------
         * Each enemy needs to be given an end point
         * Calculate start point based on movement path
         * Spawn
         */

    void SpawnAll()
    {
        foreach (Transform child in this.transform)
        {
            GameObject enemy = Instantiate(enemy_prefab, child.transform.position,
                                            Quaternion.identity) as GameObject;
            enemy.transform.parent = child;
        }
    }

    }
