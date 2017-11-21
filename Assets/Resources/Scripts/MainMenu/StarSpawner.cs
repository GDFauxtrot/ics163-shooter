using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarSpawner : MonoBehaviour {
    [Range(0, 50)]
    public int biggerStarMin, biggerStarMax;
    [Range(0f, 1f)]
    public float biggerStarMinDist;
    [Space()]

    [Range(0, 50)]
    public int bigStarMin, bigStarMax;
    [Range(0f, 1f)]
    public float bigStarMinDist;
    [Space()]

    [Range(0, 100)]
    public int smallStarMin, smallStarMax;
    [Range(0f, 1f)]
    public float smallStarMinDist;
    [Space()]

    [Range(0, 100)]
    public int smallerStarMin, smallerStarMax;
    [Range(0f, 1f)]
    public float smallerStarMinDist;
    [Space()]

    GameObject starsBigger, starsBig, starsSmall, starsSmaller;

    Sprite[] starSprites;

    float lerpStep;
    Vector2 parentMovePoint, previousMovePoint;

    // Use this for initialization
    void Start () {
        starsBigger = new GameObject("StarsBigger");
        starsBig = new GameObject("StarsBig");
        starsSmall = new GameObject("StarsSmall");
        starsSmaller = new GameObject("StarsSmaller");

        starsBigger.transform.SetParent(gameObject.transform);
        starsBig.transform.SetParent(gameObject.transform);
        starsSmall.transform.SetParent(gameObject.transform);
        starsSmaller.transform.SetParent(gameObject.transform);

        starSprites = Resources.LoadAll<Sprite>("Sprites/stars");

        SpawnStarsRandomly(biggerStarMin, biggerStarMax, starsBigger, biggerStarMinDist, starSprites[0]);
        SpawnStarsRandomly(bigStarMin, bigStarMax, starsBig, bigStarMinDist, starSprites[1]);
        SpawnStarsRandomly(smallStarMin, smallStarMax, starsSmall, smallStarMinDist, starSprites[2]);
        SpawnStarsRandomly(smallerStarMin, smallerStarMax, starsSmaller, smallerStarMinDist, starSprites[3]);
    }
	
	void Update () {
        if (Vector2.Distance(transform.position, parentMovePoint) < 0.05) {
            previousMovePoint = transform.position;
            parentMovePoint = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
            lerpStep = 0f;
        } else {
            lerpStep += 0.05f * Time.deltaTime;
        }
        Vector3 prevPos = transform.position;
        transform.position = new Vector3(Mathf.SmoothStep(previousMovePoint.x, parentMovePoint.x, lerpStep),
            Mathf.SmoothStep(previousMovePoint.y, parentMovePoint.y, lerpStep), 0f);

        Vector2 diff = new Vector2(transform.position.x - prevPos.x, transform.position.y - prevPos.y);

        starsBigger.transform.localPosition = new Vector3(
            starsBigger.transform.localPosition.x + diff.x*2,
            starsBigger.transform.localPosition.y + diff.y*2, 0);
        starsBig.transform.localPosition = new Vector3(
            starsBig.transform.localPosition.x + diff.x/2f,
            starsBig.transform.localPosition.y + diff.y/2f, 0);
        starsSmall.transform.localPosition = new Vector3(
            starsSmall.transform.localPosition.x + diff.x/16f,
            starsSmall.transform.localPosition.y + diff.y/16f, 0);
        starsSmaller.transform.localPosition = new Vector3(
            starsSmaller.transform.localPosition.x - diff.x/2f,
            starsSmaller.transform.localPosition.y - diff.y/2f, 0);

    }

    void SpawnStarsRandomly(int min, int max, GameObject parent, float minDist, Sprite sprite) {
        int starAmount = Random.Range(min, max);
        float ortho = Camera.main.orthographicSize + 2;
        float aspect = (Screen.width / (float)Screen.height);

        List<Vector2> starPoints = new List<Vector2>();
        for (int i = 0; i < starAmount; ++i) {
            // randomly pick spots in the world until one point suitably away from another is found. maximum kludge.
            Vector3 spawnSpot = new Vector3(Random.Range(-ortho * aspect, ortho * aspect), Random.Range(-ortho, ortho), 0);
            Vector2 closestSpot = ClosestVec2InList(spawnSpot, starPoints);
            while (Vector2.Distance(closestSpot, spawnSpot) < minDist) {
                spawnSpot = new Vector3(Random.Range(-ortho * aspect, ortho * aspect), Random.Range(-ortho, ortho), 0);
                closestSpot = ClosestVec2InList(spawnSpot, starPoints);
            }
            
            starPoints.Add(spawnSpot);
            string n = "";
            int order = 0;
            if (sprite == starSprites[0]) {
                n = "BiggerStar";
                order = 3;
            }
            if (sprite == starSprites[1]) {
                n = "BigStar";
                order = 2;
            }
            if (sprite == starSprites[2]) {
                n = "SmallStar";
                order = 1;
            }
            if (sprite == starSprites[3]) {
                n = "SmallerStar";
                order = 0;
            }
            GameObject star = new GameObject(n);
            SpriteRenderer sprRender = star.AddComponent<SpriteRenderer>();
            sprRender.sprite = sprite;
            sprRender.sortingOrder = order;
            if (Random.Range(0, 2) == 1)
                sprRender.flipX = true;
            star.transform.SetParent(parent.transform);
            star.transform.position = spawnSpot;
        }
    }

    Vector2 ClosestVec2InList(Vector2 p, List<Vector2> list) {
        if (list.Count == 0)
            return Vector2.positiveInfinity; // nothing will be remotely near

        Vector2 closest = list[0];
        float dist = float.MaxValue;
        foreach (Vector2 point in list) {
            float d = Vector2.Distance(p, point);
            if (d < dist)  {
                dist = d;
                closest = point;
            }
        }
        return closest;
    }
}
