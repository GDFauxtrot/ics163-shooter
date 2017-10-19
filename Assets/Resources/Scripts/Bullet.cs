using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType { BASIC };

public class Bullet : MonoBehaviour {

    public int damage;
    public float speed;
    public BulletType type;

    void Start () {
        Destroy(gameObject, 5);
	}
	
	void FixedUpdate () {
        transform.position = new Vector3(transform.position.x, transform.position.y + speed, transform.position.z);
	}
}
