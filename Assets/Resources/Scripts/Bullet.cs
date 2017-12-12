using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType { BASIC };
public enum BulletOrigin { PLAYER, HOSTILE}

public class Bullet : MonoBehaviour {

    public int damage;
    public float speed;
    public BulletType type;
    public BulletOrigin origin;

    void Start () {
        Destroy(gameObject, 5);
	}
	
	void FixedUpdate () {
        transform.position = new Vector3(transform.position.x, transform.position.y + speed, transform.position.z);
	}

    public int GetDamage()
    {
        return damage;
    }

    public void Hit()
    {
        Destroy(this.gameObject);
    }

    public void OnCollisionEnter2D (Collision2D collision)
    {
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
        if (bullet && (this.origin != bullet.origin))
        {
            Hit();
        }
    }
}
