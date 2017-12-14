using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // -- Public -- //

    [Tooltip("Measured in seconds")]
    public float mainGunFireRate;

    public AudioClip shooting, shootingFast, shootingHeld, shootingMedBurst, shootingHiBurst; // temp solution?

    [Range(0.01f, 1)]
    public float inputSmoothness;

    [Range(0, 0.25f)]
    public float pushbackRecover;

    [Header("Frequently-Accessed GameObjects")]
    public GameObject bulletPool;

    // -- Private -- //

    Sprite[] moveSprites;
    Coroutine firingCoroutine, chargingCoroutine;
    AudioSource mainFiringSource, chargeFiringSource, secondFiringSource;

    GameObject chargeOutlineChild;

    Vector2 pushbackOffset;

    float lastX; // Used for changing player sprite when moving horizontally

    public bool tutorialDisableMove;
    public bool tutorialDisableShoot;
    public bool tutorialDisableChargeShot;

	void Start () {
        bulletPool = GameObject.Find("BulletPool");

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
	
	void Update () {
        Vector2 mousePos = Vector2.zero;

        if (!tutorialDisableMove)
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // a hint of lerp to make movement feel less flat
        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, mousePos.x + pushbackOffset.x, inputSmoothness),
            Mathf.Lerp(transform.position.y, mousePos.y + pushbackOffset.y, inputSmoothness),
            transform.position.z);

        if (Input.GetMouseButtonDown(0) && !tutorialDisableShoot) {
            if (firingCoroutine == null) {
                firingCoroutine = StartCoroutine(FiringCoroutine());
            }
        }
        if (Input.GetMouseButton(1) && !tutorialDisableChargeShot) {
            if (chargingCoroutine == null) {
                chargingCoroutine = StartCoroutine(ChargeFiringCoroutine());
            }
        }
	}

    void FixedUpdate() {
        // Used when firing secondary burst -- set pushbackOffset to something and let 'er rip
        pushbackOffset = new Vector2(
            Mathf.Lerp(pushbackOffset.x, 0, pushbackRecover),
            Mathf.Lerp(pushbackOffset.y, 0, pushbackRecover));
    }

    void LateUpdate() {
        // Change sprite based on movement
        float xDif = transform.position.x - lastX;
        if (xDif < -0.2) {
            GetComponent<SpriteRenderer>().sprite = moveSprites[0];
        } else if (xDif < -0.05) {
            GetComponent<SpriteRenderer>().sprite = moveSprites[1];
        } else if (xDif < 0.05) {
            GetComponent<SpriteRenderer>().sprite = moveSprites[2];
        } else if (xDif < 0.2) {
            GetComponent<SpriteRenderer>().sprite = moveSprites[3];
        } else {
            GetComponent<SpriteRenderer>().sprite = moveSprites[4];
        }
        lastX = transform.position.x;

        // Handle charge outline child behavior (hackey, since the other half is in ChargeFiringCoroutine())
        if (Input.GetMouseButton(1)) {
            chargeOutlineChild.GetComponent<SpriteRenderer>().enabled = true;
            chargeOutlineChild.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        } else {
            chargeOutlineChild.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private IEnumerator ChargeFiringCoroutine() {
        int medCharge = 30, hiCharge = 180;
        float medPitch = 2, hiPitch = 2.5f; // Misleading - charge outline uses this too
        float pitchStep = 1/60f;


        // Everything here relies on this
        int charge = 0;

        chargeFiringSource.Play();

        while (Input.GetMouseButton(1)) {
            charge++;
            if (charge >= medCharge && charge < hiCharge) {
                if (chargeFiringSource.pitch < medPitch) {
                    chargeFiringSource.pitch += pitchStep;
                    chargeOutlineChild.transform.localScale = new Vector3(1 + chargeFiringSource.pitch / 8, 1 + chargeFiringSource.pitch / 8, chargeOutlineChild.transform.localScale.z);
                } else {
                    chargeFiringSource.pitch = medPitch;
                    chargeOutlineChild.transform.localScale = new Vector3(1 + medPitch / 8, 1 + medPitch / 8, chargeOutlineChild.transform.localScale.z);
                }
            } else if (charge >= hiCharge) {
                if (chargeFiringSource.pitch < hiPitch) {
                    chargeFiringSource.pitch += pitchStep;
                    chargeOutlineChild.transform.localScale = new Vector3(1 + chargeFiringSource.pitch / 8, 1 + chargeFiringSource.pitch / 8, chargeOutlineChild.transform.localScale.z);
                } else {
                    chargeFiringSource.pitch = hiPitch;
                    chargeOutlineChild.transform.localScale = new Vector3(1 + hiPitch / 8, 1 + hiPitch / 8, chargeOutlineChild.transform.localScale.z);
                }
            }
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        // Yes, we're using pitch to figure out what shot to fire - so sue me
        if (chargeFiringSource.pitch > (medPitch/2f) && chargeFiringSource.pitch < medPitch) {
            for (int i = 0; i < 3; ++i) {
                GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerBullet"));
                bullet.transform.parent = bulletPool.transform;
                bullet.transform.position = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);
                secondFiringSource.clip = shootingFast;
                secondFiringSource.Play();
                yield return new WaitForSeconds(Time.fixedDeltaTime*2);
            }
        } else if (chargeFiringSource.pitch >= medPitch && chargeFiringSource.pitch < hiPitch) {
            GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerMedBurst"));
            bullet.transform.parent = bulletPool.transform;
            bullet.transform.position = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);
            secondFiringSource.clip = shootingMedBurst;
            secondFiringSource.Play();

            pushbackOffset = new Vector2(pushbackOffset.x, -0.25f);
        } else if (chargeFiringSource.pitch >= hiPitch) {
            GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerHiBurst"));
            bullet.transform.parent = bulletPool.transform;
            bullet.transform.position = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);
            secondFiringSource.clip = shootingHiBurst;
            secondFiringSource.Play();

            pushbackOffset = new Vector2(pushbackOffset.x, -0.5f);
        }

        chargeOutlineChild.transform.localScale = Vector3.one;

        chargeFiringSource.Stop();
        chargeFiringSource.pitch = 1;

        chargingCoroutine = null;
    }
    private IEnumerator FiringCoroutine() {
        bool held = false; // to play a modified audio clip after the first shot, so it loops better
        do {
            GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerBullet"));
            bullet.transform.parent = bulletPool.transform;
            bullet.transform.position = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);

            if (mainGunFireRate < 0.1) {
                mainFiringSource.clip = shootingFast;
            } else {
                if (held) {
                    mainFiringSource.clip = shootingHeld;
                } else {
                    mainFiringSource.clip = shooting;
                }
                
            }

            mainFiringSource.Play();
            yield return new WaitForSeconds(mainGunFireRate);
            held = true;
        } while (Input.GetMouseButton(0));

        firingCoroutine = null;
    }
}
