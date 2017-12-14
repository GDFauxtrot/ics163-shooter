using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public enum EnemyType{NORMAL, DASH, ARTILLERY};

    public int health = 10;
    public EnemyType enemytype; // Select enum from above in designer

    // Animations
    Animator animator;

    // Add sprite transfomr array?


	// Use this for initialization
	void Start () {
        animator = this.GetComponent<Animator>();
        animator.Play("Arrival");
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
            //bullet.Hit();
            if (health <= 0)
            {
                Destroy(this.gameObject);
            }
            else
            {
                WhenHit();
            }
        }
        
    }

    void WhenHit()
    {
        animator.SetTrigger("Hit");
    }

    void Reset()
    {
        animator.SetTrigger("Reset");
    }

}
