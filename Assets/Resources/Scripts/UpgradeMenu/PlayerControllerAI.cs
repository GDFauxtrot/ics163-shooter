using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerAI : MonoBehaviour {

    public AudioClip shooting, shootingFast, shootingHeld, shootingMedBurst, shootingHiBurst; // temp solution?

    float mainGunFireRate = 0.1f;
    float inputSmoothness = 0.015f;
    float pushbackRecover = 0.05f;

    public Vector2 offScreenPosition, onScreenPosition;
    public float horizFloatAmount;

    [Header("Frequently-Accessed GameObjects")]
    public GameObject bulletPool;

    Sprite[] moveSprites;
    Coroutine firingCoroutine, chargingCoroutine;
    AudioSource mainFiringSource, chargeFiringSource, secondFiringSource;

    GameObject chargeOutlineChild;

    Vector2 desiredPosition;

    float lastX; // Used for changing player sprite when moving horizontally

    int ySinMovementDegree;
    float sinOffset;

    void Start() {
        bulletPool = GameObject.Find("BulletPool");

        desiredPosition = offScreenPosition;

        StartCoroutine(SillyAIMovement());
        // LoadAll() handles tile sets
        moveSprites = Resources.LoadAll<Sprite>("Sprites/tyrianship/ship");
        GetComponent<SpriteRenderer>().sprite = moveSprites[2];
        lastX = transform.position.x;

        // Audio source components added to player in the same order
        AudioSource[] sources = GetComponents<AudioSource>();
        mainFiringSource = sources[0];
        chargeFiringSource = sources[1];
        secondFiringSource = sources[2];

        chargeOutlineChild = transform.Find("ChargeOutline").gameObject;
        chargeOutlineChild.GetComponent<SpriteRenderer>().enabled = false;
    }

    void Update() {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // a hint of lerp to make movement feel less flat
        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, desiredPosition.x, inputSmoothness),
            Mathf.Lerp(transform.position.y, desiredPosition.y + sinOffset, inputSmoothness),
            transform.position.z);
    }

    void FixedUpdate() {
        sinOffset = Mathf.Sin(Mathf.Deg2Rad * ySinMovementDegree)/16f;
        ySinMovementDegree += 3;
        if (ySinMovementDegree == 360)
            ySinMovementDegree = 0;
    }
    void LateUpdate() {
        // Change sprite based on movement
        float xDif = transform.position.x - lastX;
        if (xDif < -0.01) {
            GetComponent<SpriteRenderer>().sprite = moveSprites[0];
        } else if (xDif < -0.0015) {
            GetComponent<SpriteRenderer>().sprite = moveSprites[1];
        } else if (xDif < 0.0015) {
            GetComponent<SpriteRenderer>().sprite = moveSprites[2];
        } else if (xDif < 0.01) {
            GetComponent<SpriteRenderer>().sprite = moveSprites[3];
        } else {
            GetComponent<SpriteRenderer>().sprite = moveSprites[4];
        }
        lastX = transform.position.x;
    }

    // warning: very silly
    private IEnumerator SillyAIMovement() {
        yield return new WaitForSeconds(1f);
        desiredPosition = onScreenPosition;
        yield return new WaitForSeconds(Random.Range(2f, 4f));

        bool beingSilly = false;
        while (true) {
            if (!beingSilly) {
                int choice = Random.Range(0, 20);
                if (choice < 15) {
                    // 75% chance - do nothing
                    yield return new WaitForSeconds(Random.Range(1f, 2f));
                } else {
                    beingSilly = true;
                    inputSmoothness = 0.01f;
                }
            }
            // not an 'else' - want to execute right after it becomes true
            if (beingSilly) {
                int sillyChoice = Random.Range(0, 10);

                if (sillyChoice < 3) {
                    // 20% chance return to position
                    beingSilly = false;
                    inputSmoothness = 0.015f;
                    desiredPosition = onScreenPosition;
                } else {
                    if (desiredPosition.x == onScreenPosition.x) {
                        desiredPosition = new Vector2(onScreenPosition.x + Random.Range(-horizFloatAmount, horizFloatAmount), onScreenPosition.y);
                    } else if (desiredPosition.x < onScreenPosition.x) {
                        desiredPosition = new Vector2(onScreenPosition.x + Random.Range(0f, horizFloatAmount), onScreenPosition.y);
                    } else {
                        desiredPosition = new Vector2(onScreenPosition.x + Random.Range(-horizFloatAmount, 0f), onScreenPosition.y);
                    }
                    if (Random.Range(0f, 1f) < 0.75f) {
                        yield return new WaitForSeconds(Random.Range(1.5f, 2f));
                    } else {
                        yield return new WaitForSeconds(Random.Range(2f, 2.5f));
                    }
                }
            }
        }
    }
}
