using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPosition : MonoBehaviour {

    public Enemy enemy_prefab;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, 0.6f);
    }
}
