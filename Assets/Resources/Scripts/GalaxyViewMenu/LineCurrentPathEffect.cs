using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCurrentPathEffect : MonoBehaviour {

    private PersistentData persistentData;
    private Dictionary<int, List<GameObject>> lines;
    private int lineOpacityIndex;

	void Start () {
        persistentData = GameObject.Find("PersistentDataObject").GetComponent<PersistentData>();
        
        lines = new Dictionary<int, List<GameObject>>();
        lines.Add(0, new List<GameObject>());
        lines.Add(1, new List<GameObject>());
        lines.Add(2, new List<GameObject>());
        lines.Add(3, new List<GameObject>());
        lines.Add(4, new List<GameObject>());
        lines.Add(5, new List<GameObject>());
        lines.Add(6, new List<GameObject>());

        lines[0].Add((transform.Find("Line01").gameObject));
        lines[0].Add((transform.Find("Line02").gameObject));
        lines[1].Add((transform.Find("Line13").gameObject));
        lines[2].Add((transform.Find("Line23").gameObject));
        lines[3].Add((transform.Find("Line34").gameObject));
        lines[3].Add((transform.Find("Line35").gameObject));
        lines[4].Add((transform.Find("Line46").gameObject));
        lines[5].Add((transform.Find("Line56").gameObject));

        lineOpacityIndex = 0;
    }
	
	void Update () {
		
	}

    void FixedUpdate() {
        foreach (GameObject line in lines[persistentData.currentWorld]) {
            // Opacity bounces between 1f (no transparency) and 0.25f (75% transparency)... that seems right
            line.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.Sin(Mathf.Deg2Rad * lineOpacityIndex)*0.75f + 0.25f);
        }
        lineOpacityIndex += 3;
        if (lineOpacityIndex == 180)
            lineOpacityIndex = 0;
    }
}
