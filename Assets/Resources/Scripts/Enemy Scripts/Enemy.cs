using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public enum EnemyType{NORMAL, DASH, ARTILLERY};

    public int health;
    public EnemyType enemytype; // Select enum from above in designer

    public bool tutorialDisableFiring;

    // Animations
    Animator animator;

    // Add sprite transfomr array?

	// Use this for initialization
	void Start () {
        animator = this.GetComponent<Animator>();
        animator.Play("Arrival");

        StartCoroutine(EnemyFireCoroutine());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator EnemyFireCoroutine() {
        yield return new WaitForSeconds(Random.Range(1f, 5f));
        while(true) {
            if (!tutorialDisableFiring) {
                GameObject bullet = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/EnemyBullet")) as GameObject;
                bullet.transform.localPosition = new Vector3(transform.position.x, transform.position.y - 0.25f, transform.position.z);
                bullet.GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(GameObject.Find("Player").transform.position - bullet.transform.position, 7);
                // Add a bit of randomness
                bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(bullet.GetComponent<Rigidbody2D>().velocity.x + Random.Range(-0.5f, 0.5f), bullet.GetComponent<Rigidbody2D>().velocity.y + Random.Range(-0.5f, 0.5f));
            }
            yield return new WaitForSeconds(Random.Range(4f, 8f));
        }
    }

    // When hit
    void OnTriggerEnter2D (Collider2D collider)
    {
        Bullet bullet = collider.gameObject.GetComponent<Bullet>();

        if (bullet)
        {
            health -= bullet.GetDamage();
            //bullet.Hit();
            if (health <= 0)
            {
                Destroy(gameObject);

                GameObject.Find("Canvas").GetComponent<GameUIManager>().AddHitsAndUpdate(1);
                GameObject.Find("Player").GetComponent<PlayerController>().AddMoney(100);
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

    public void Attack(int pattern, float offset)
    {
        Debug.Log(this + " is attacking");
        animator.SetInteger("Attack Pattern", pattern);
        Invoke("AttackInvoke", 1 + offset);
    }

    void AttackInvoke()
    {
        animator.SetTrigger("Attack Trigger");
        animator.SetTrigger("Attack Reset");
        Invoke("Disappear", 2);
    }

    // Destroy enemy without dropping scrap / powerups
    // For use at end of an attack run / wave
    public void Disappear()
    {
        Debug.Log("Destroying " + this);
        Destroy(this.gameObject);
    }

}
