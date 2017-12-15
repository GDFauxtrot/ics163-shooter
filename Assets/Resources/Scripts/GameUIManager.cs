using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour {

    public int lives, hits, money;

    List<GameObject> healthObjects;

    int health;
    Sprite hpSprite, hpHalfSprite;

    void Start () {
        healthObjects = new List<GameObject>();

        hpSprite = Resources.Load<Sprite>("Sprites/pill");
        hpHalfSprite = Resources.Load<Sprite>("Sprites/halfpill");

        SetHealth(GameObject.Find("PersistentDataObject").GetComponent<PersistentData>().playerHealth);
        SetMoney(GameObject.Find("PersistentDataObject").GetComponent<PersistentData>().playerMoney);
    }
	
    public void AddHitsAndUpdate(int h) {
        hits += h;

        transform.Find("HitsCounterText").GetComponent<Text>().text = hits.ToString();
    }

    public void SetMoney(int muns) {
        money = muns;
        transform.Find("ScrapCounterText").GetComponent<Text>().text = money.ToString();
    }

    public void SetHealth(int hp) {
        foreach (GameObject healthObject in healthObjects) {
            Destroy(healthObject);
        }

        health = hp;
        bool half = (health % 2 == 1);

        for (int i = 0; i < health / 2 + (half ? 1 : 0); ++i) {
            GameObject healthObject = Instantiate(Resources.Load<GameObject>("Prefabs/UIHealthItem")) as GameObject;
            healthObject.transform.SetParent(transform);
            healthObject.transform.localScale = new Vector3(2.5f, 2.5f, 1.0f);
            healthObject.transform.SetSiblingIndex(healthObject.transform.GetSiblingIndex() - 1);
            healthObject.GetComponent<RectTransform>().sizeDelta = new Vector2(-32, -32);
            healthObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(64 * i, 0);
            //healthObject.transform.position = Vector3.zero;
            healthObjects.Add(healthObject);
        }
        if (half) {
            healthObjects[healthObjects.Count - 1].GetComponent<Image>().sprite = hpHalfSprite;
        }
    }
}
