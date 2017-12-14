using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour {

    public int score, lives, hits;

	void Start () {
		
	}
	
    public void AddScoreAndUpdate(int s) {
        score += s;
        transform.Find("ScoreCounterText").GetComponent<Text>().text = score.ToString();
    }

    public void AddHitsAndUpdate(int h) {
        hits += h;

        transform.Find("HitsCounterText").GetComponent<Text>().text = hits.ToString() + " Hits";
    }
}
