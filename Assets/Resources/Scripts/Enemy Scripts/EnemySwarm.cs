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
    public float minX;
    public float maxX;
    public int num_waves_remaining = 5;

    // Sets direction of movement (left if true, right if false)
    // TODO: Sync these across all spawned enemies
    public bool move_left = true;
    public float speed = 5f;
    public float spawn_delay = 0.05f;
    public float step = 0f; // How much level lowers each time


    // Bools for checking if attack can be called
    public bool is_filling = true;
    public bool is_attacking = false;

    public bool areWavesOver;

    // Use this for initialization
    void Start () {
        is_filling = true;
        SpawnUntilFull();
	}
	
	// Update is called once per frame
	void Update () {
        if (!is_attacking && !is_filling)
        {
            MoveFormationSideToSide();
        }
        if (AllMembersDead())
        {
            if (num_waves_remaining > 0)
            {
                this.transform.position = new Vector3(this.transform.position.x,
                                                      this.transform.position.y - step,
                                                      this.transform.position.z);
                SpawnUntilFull();
                num_waves_remaining--;
            } else
            {
                areWavesOver = true;
            }
        }


        if ((Random.Range(1f, 1000000f) > 999000) && CanAttack()) // Arbitrary for the time being
        {
            Debug.Log("In Range");
            AttackSwarm(1);
        }
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
        is_filling = false;
    }

    void MoveFormationSideToSide()
    {
        // Move left and right, until it reaches the edge, then change direciton
        if (move_left)
        {
            // Direction is moving left
            this.transform.position += Vector3.left * speed * Time.deltaTime;
        }
        else
        {
            // Direction is moving right
            this.transform.position += Vector3.right * speed * Time.deltaTime;
        }
        CheckPositionAndDirection();
    }

    void CheckPositionAndDirection()
    {
        // Restricts the enemy to the gamespace
        float newX = Mathf.Clamp(transform.position.x, minX, maxX);
        this.transform.position = new Vector3(newX, this.transform.position.y,
                                                  this.transform.position.z);

        if (newX <= minX)
        {
            move_left = false;
        }
        else
        if (newX >= maxX)
        {
            move_left = true;
        } else 
        if (Random.value > 0.99f)
        {
            move_left = !move_left;
        }
    }

    bool AllMembersDead()
    {
        foreach (Transform child_position_game_object in this.transform)
        {
            if (child_position_game_object.childCount > 0)
            {
                return false;
            }
        }
        is_attacking = false;
        return true;
    }

    Transform NextFreePosition()
    {
        foreach (Transform child_position_game_object in this.transform)
        {
            if (child_position_game_object.childCount == 0)
            {
                return child_position_game_object.transform;
            }
        }
        return null;
    }

    void SpawnUntilFull()
    {
        is_filling = true;
        Transform free_position = NextFreePosition();
        if (free_position)
        {
            GameObject enemy = Instantiate(enemy_prefab, free_position.position,
                                       Quaternion.identity) as GameObject;
            enemy.transform.parent = free_position;
        }

        if (NextFreePosition())
        {
            Invoke("SpawnUntilFull", spawn_delay);
        }
        Invoke("_resetis_fillingOnDelay", 2);
    }

    void AttackSwarm(int pattern)
    {
        float offset_incrementer = 0;
        foreach (Transform enemy_position in this.transform)
        {
            if (enemy_position.childCount > 0)
            {
                GameObject enemy = enemy_position.GetChild(0).gameObject;
                Enemy enemy_script = enemy.GetComponent<Enemy>();
                enemy_script.Attack(pattern, offset_incrementer);
                offset_incrementer += 0.3f;
            }
        }
    }
    
    bool CanAttack()
    {
        if (!is_filling && !is_attacking)
        {
            is_attacking = true;
            return true;
        }
        return false;
    }


    void _resetis_fillingOnDelay()
    {
        is_filling = false;
    }
}
