using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextBoxManager : MonoBehaviour {

    public GameObject player;
    public float alphaFadeTo;
    public float fadeTime;
    public bool active;

    Vector4 bounds; // Two entries contained - top left (X,Y), bottom right (Z, W)
    float currentFadeTime, desiredAlpha, previousAlpha, step;

    Vector4 currentBoundsPosition, previousBoundsPosition;

    bool fadeRunning;

	void Start () {
        bounds = new Vector4();
        
        // Just shortening names here
        RectTransform rt = GetComponent<RectTransform>();
        Rect rect = GetComponent<RectTransform>().rect;

        desiredAlpha = 0.0f;
        previousAlpha = 0.0f;
        currentFadeTime = fadeTime;
        step = fadeTime;

        currentBoundsPosition = new Vector4(
            player.transform.position.x - player.GetComponent<PolygonCollider2D>().bounds.size.x,
            player.transform.position.y + player.GetComponent<PolygonCollider2D>().bounds.size.y,
            player.transform.position.x + player.GetComponent<PolygonCollider2D>().bounds.size.x,
            player.transform.position.y - player.GetComponent<PolygonCollider2D>().bounds.size.y);

        previousBoundsPosition = currentBoundsPosition;

        Vector2 b0 = Camera.main.ScreenToWorldPoint(new Vector2(rt.position.x + rect.x / 2f, rt.position.y - rect.y / 2f));
        Vector2 b1 = Camera.main.ScreenToWorldPoint(new Vector2(rt.position.x - rect.x / 2f, rt.position.y + rect.y / 2f));
        bounds = new Vector4(b0.x, b0.y, b1.x, b1.y);

        active = false;
    }
	
    void Update() {
        if (active) {
            if (step < currentFadeTime) {
                step += Time.deltaTime;
            } else {
                step = currentFadeTime;

                if (desiredAlpha == 0.0f) {
                    active = false;
                }
                if (fadeRunning) {
                    fadeRunning = false;
                    currentFadeTime = fadeTime;
                }
            }

            currentBoundsPosition = new Vector4(
                player.transform.position.x - player.GetComponent<PolygonCollider2D>().bounds.size.x,
                player.transform.position.y + player.GetComponent<PolygonCollider2D>().bounds.size.y,
                player.transform.position.x + player.GetComponent<PolygonCollider2D>().bounds.size.x,
                player.transform.position.y - player.GetComponent<PolygonCollider2D>().bounds.size.y);

            if ((DoVec4BoundsOverlap(currentBoundsPosition, bounds) != DoVec4BoundsOverlap(previousBoundsPosition, bounds)) && !fadeRunning && active) {
                step = 0;
                previousAlpha = GetComponent<Image>().color.a;

                if (DoVec4BoundsOverlap(currentBoundsPosition, bounds)) {
                    desiredAlpha = alphaFadeTo;
                } else {
                    desiredAlpha = 1.0f;
                }
            }

            GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b,
                Mathf.Lerp(previousAlpha, desiredAlpha, step / currentFadeTime));
            transform.Find("Text").GetComponent<Text>().color = GetComponent<Image>().color;
        } else {
            fadeRunning = false;
            GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b,
                0.0f);
            transform.Find("Text").GetComponent<Text>().color = GetComponent<Image>().color;
        }
    }

    void LateUpdate() {
        previousBoundsPosition = new Vector4(
            player.transform.position.x - player.GetComponent<PolygonCollider2D>().bounds.size.x,
            player.transform.position.y + player.GetComponent<PolygonCollider2D>().bounds.size.y,
            player.transform.position.x + player.GetComponent<PolygonCollider2D>().bounds.size.x,
            player.transform.position.y - player.GetComponent<PolygonCollider2D>().bounds.size.y);
    }

    private bool DoVec4BoundsOverlap(Vector4 player, Vector4 bounds) {
        return (player.w < bounds.y && player.y > bounds.w && player.x < bounds.z && player.z > bounds.x);
    }

    public void FadeIn() {
        desiredAlpha = DoVec4BoundsOverlap(currentBoundsPosition, bounds) ? alphaFadeTo : 1.0f;
        previousAlpha = 0.0f;
        currentFadeTime = 1.0f;
        step = 0;
        active = true;
        fadeRunning = true;
    }

    public void FadeOut() {
        previousAlpha = DoVec4BoundsOverlap(currentBoundsPosition, bounds) ? alphaFadeTo : 1.0f;
        desiredAlpha = 0.0f;
        currentFadeTime = 1.0f;
        step = 0;
        active = true;
        fadeRunning = true;
    }

    public float TypeText(string text, float charSpeedSecs, bool append) {
        StartCoroutine(TextCoroutine(text, charSpeedSecs, append));
        return (text.Length+1) * charSpeedSecs;
    }

    private IEnumerator TextCoroutine(string text, float speed, bool append) {
        string textToDisplay = "";

        if (append) {
            textToDisplay = transform.Find("Text").GetComponent<Text>().text;
        }
        
        int i = 0;
        while (i < text.Length) {
            while (text[i] == '\r' || text[i] == '\n') {
                textToDisplay += text[i++]; // No delay for line break characters
            }
            textToDisplay += text[i++];
            transform.Find("Text").GetComponent<Text>().text = textToDisplay;
            if (speed != 0f)
                yield return new WaitForSeconds(speed);
        }
        yield return new WaitForSeconds(0f);
    }
}
