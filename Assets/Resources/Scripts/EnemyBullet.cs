using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBullet : MonoBehaviour {

    SpriteRenderer spriteRenderer;
    Sprite[] sprites;

	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        sprites = Resources.LoadAll<Sprite>("Sprites/EnemyBulletFlash");

        StartCoroutine(BulletFlash());

        Destroy(gameObject, 5f);
    }
	
	private IEnumerator BulletFlash() {
        while(true) {
            if (spriteRenderer.sprite == sprites[0])
                spriteRenderer.sprite = sprites[1];
            else
                spriteRenderer.sprite = sprites[0];
            yield return new WaitForSeconds(1 / 20f);
        }
    }
}
