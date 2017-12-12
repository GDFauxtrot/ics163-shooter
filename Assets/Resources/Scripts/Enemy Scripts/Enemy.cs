using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public enum EnemyType{NORMAL, DASH, ARTILLERY};

    public int health = 10;
    public GameObject projectile;
    // TODO: Add comment for type list to appear in the designer
    public EnemyType enemytype; // Select enum from above in designer

    // Add sprite transfomr array?


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // When hit
    void OnTriggerEnter2D (Collider2D collider)
    {
        Debug.Log("Hit");
        Bullet bullet = collider.gameObject.GetComponent<Bullet>();

        if (bullet)
        {
            health -= bullet.GetDamage();
        }
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
