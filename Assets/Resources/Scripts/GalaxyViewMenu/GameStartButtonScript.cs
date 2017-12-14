using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStartButtonScript : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
		
	}

    public void OnClick() {
        StartCoroutine(TransitionToTutorialStage());
    }

    private IEnumerator TransitionToTutorialStage() {
        GetComponent<Button>().interactable = false;
        GameObject.Find("ScreenFader").GetComponent<ScreenFaderManager>().FadeTo(Color.black, 1);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("TutorialStage");
    }
}
