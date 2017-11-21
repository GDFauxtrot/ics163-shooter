using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DoneButtonScript : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
		
	}

    public void OnClick() {
        StartCoroutine(TransitionToGalaxyView());
    }

    private IEnumerator TransitionToGalaxyView() {
        GetComponent<Button>().interactable = false;
        GameObject.Find("ScreenFader").GetComponent<ScreenFaderManager>().FadeTo(Color.black, 1);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("GalaxyViewMenu");
    }
}
