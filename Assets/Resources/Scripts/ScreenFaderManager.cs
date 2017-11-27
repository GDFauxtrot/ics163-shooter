using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenFaderManager : MonoBehaviour {

    private Coroutine currentFade;
    private GameObject fadeObject;

    void Awake() {
        DontDestroyOnLoad(gameObject);

        foreach (GameObject other in FindObjectsOfType(typeof(GameObject))) {
            if (other.name == "ScreenFader" && other != gameObject) {
                Destroy(gameObject);
            }
        }
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneChange;
    }
    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneChange;
    }

    private void OnSceneChange(Scene scene, LoadSceneMode mode) {
        if (GameObject.Find("FadeObject") == null)
            CreateFadeObject();

        if (scene.name == "GalaxyViewMenu" || scene.name == "UpgradeMenu") {
            FadeFrom(Color.black, 0.75f);
        }
    }

    private void CreateFadeObject() {
        fadeObject = new GameObject("FadeObject", typeof(Image));
        fadeObject.transform.SetParent(GameObject.Find("Canvas").transform, false);
        fadeObject.transform.SetSiblingIndex(fadeObject.transform.GetSiblingIndex());
        fadeObject.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        fadeObject.GetComponent<RectTransform>().anchorMax = Vector2.one;
        fadeObject.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        fadeObject.GetComponent<Image>().raycastTarget = false;
        fadeObject.GetComponent<Image>().color = Vector4.zero;
    }

    public void ClearColor() {
        fadeObject.GetComponent<Image>().color = Vector4.zero;
        if (currentFade != null) {
            StopCoroutine(currentFade);
        }
    }

    public void FadeIn() {
        if (currentFade != null) {
            StopCoroutine(currentFade);
        }
        Color c = new Color(fadeObject.GetComponent<Image>().color.r,
            fadeObject.GetComponent<Image>().color.g,
            fadeObject.GetComponent<Image>().color.b,
            0f);
        currentFade = StartCoroutine(Fade(fadeObject.GetComponent<Image>().color, c, 1, false));
    }

    public void FadeOut() {
        if (currentFade != null) {
            StopCoroutine(currentFade);
        }
        Color c = new Color(fadeObject.GetComponent<Image>().color.r,
            fadeObject.GetComponent<Image>().color.g,
            fadeObject.GetComponent<Image>().color.b,
            1f);
        currentFade = StartCoroutine(Fade(fadeObject.GetComponent<Image>().color, c, 1, false));
    }

    public void FadeFrom(Color color, float seconds, bool useFixedDeltaTime = false) {
        if (currentFade != null) {
            StopCoroutine(currentFade);
        }
        currentFade = StartCoroutine(Fade(color, Vector4.zero, seconds, useFixedDeltaTime));
    }

    public void FadeTo(Color color, float seconds, bool useFixedDeltaTime = false) {
        if (currentFade != null) {
            StopCoroutine(currentFade);
        }
        currentFade = StartCoroutine(Fade(Vector4.zero, color, seconds, useFixedDeltaTime));
    }

    public void FadeFromTo(Color from, Color to, float seconds, bool useFixedDeltaTime = false) {
        if (currentFade != null) {
            StopCoroutine(currentFade);
        }
        currentFade = StartCoroutine(Fade(from, to, seconds, useFixedDeltaTime));
    }

    private IEnumerator Fade(Color colorFrom, Color colorTo, float seconds, bool useFixedDeltaTime) {
        float startTime = Time.time;
        while (Time.time < startTime + seconds) {
            float ratio = (Time.time - startTime) / seconds;
            //color.a = alpha * amount;
            fadeObject.GetComponent<Image>().color = Color.Lerp(colorFrom, colorTo, ratio);
            //colorTex.SetPixel(0, 0, color);
            //colorTex.Apply();

            if (useFixedDeltaTime) {
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            } else {
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

        fadeObject.GetComponent<Image>().color = colorTo;
    }
}
