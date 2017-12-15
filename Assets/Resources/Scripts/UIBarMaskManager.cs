using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBarMaskManager : MonoBehaviour {

    public float percentage;

    GameObject barChild;
    Vector2 childPosition;

    Vector2 startPos;
    float emptyX;

	void Start () {
        barChild = transform.GetChild(0).gameObject;
        childPosition = barChild.transform.position;

        startPos = GetComponent<RectTransform>().anchoredPosition;
        emptyX = -GetComponent<RectTransform>().rect.width;
	}
	
	void Update () {
        percentage = Mathf.Clamp01(percentage);

        GetComponent<RectTransform>().anchoredPosition = new Vector2(emptyX * (1-percentage), GetComponent<RectTransform>().anchoredPosition.y);
        barChild.transform.position = childPosition;
    }
}
