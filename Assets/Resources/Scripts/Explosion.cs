using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Explosion : MonoBehaviour {

    Sprite[] sprites;

	void Start () {
        sprites = Resources.LoadAll<Sprite>("Sprites/coolexplosion");

        StartCoroutine(PlayExplosion());
	}
	
	private IEnumerator PlayExplosion() {
        for (int i = 0; i < sprites.Length; ++i) {
            string name = sprites[i].name;
            name = name.Substring(name.IndexOf('-') + 1, 4);

            GetComponent<SpriteRenderer>().sprite = sprites[i];
            yield return new WaitForSeconds(float.Parse(name));
        }

        Destroy(gameObject);
    }
}
