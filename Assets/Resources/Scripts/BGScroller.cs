using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScroller : MonoBehaviour {

    public Sprite chosenSprite;

    List<GameObject> children;
    float heightToUnits;

	void Start () {
        children = new List<GameObject>();
        heightToUnits = chosenSprite.rect.height / chosenSprite.pixelsPerUnit;
        float heightNeededForLoop = (Camera.main.orthographicSize * 2) + heightToUnits * 2;
        for (float i = heightNeededForLoop; i > 0; i -= heightToUnits*2) {
            children.Add(Instantiate(Resources.Load<GameObject>("Prefabs/BGLoop")));
        }
        float h = heightToUnits - Camera.main.orthographicSize;
        foreach (GameObject child in children) {
            child.transform.position = new Vector3(transform.position.x, h, transform.position.z);
            h += heightToUnits*2;
            child.transform.parent = transform;
        }
	}
	
	void FixedUpdate () {
	    if (transform.position.y < heightToUnits*-2) {
            transform.position = new Vector3(transform.position.x, transform.position.y + heightToUnits * 2, transform.position.z);
        } else {
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.07f, transform.position.z);
        }
	}
}
