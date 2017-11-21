using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeButtonScript : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
		
	}

    public void OnClick() {
        StartCoroutine(TransitionToUpgradeScreen());
    }

    private IEnumerator TransitionToUpgradeScreen() {
        GetComponent<Button>().interactable = false;
        GameObject.Find("ScreenFader").GetComponent<ScreenFaderManager>().FadeTo(Color.black, 1);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("UpgradeMenu");
    }
}
