using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    // -- Public -- //

    public int health;
    public int money;

    [Tooltip("Measured in seconds")]
    public float mainGunFireRate;

    public AudioClip shooting, shootingFast, shootingHeld, shootingMedBurst, shootingHiBurst; // temp solution?

    [Range(0.01f, 1)]
    public float inputSmoothness;

    [Range(0, 0.25f)]
    public float pushbackRecover;

    [Header("Frequently-Accessed GameObjects")]
    public GameObject bulletPool;
    
    public bool tutorialDisableMove;
    public bool tutorialDisableShoot;
    public bool tutorialDisableChargeShot;
    public bool tutorialDisableEvade;
    bool tutorialEvadePressed;

    public Sprite shipSprite;

    // -- Private -- //

    // Sprite[] moveSprites;
    Coroutine invulnFlashCoroutine;
    Coroutine firingCoroutine, chargingCoroutine;
    AudioSource mainFiringSource, chargeFiringSource, secondFiringSource;

    GameObject chargeOutlineChild;

    Vector2 pushbackOffset;

    Vector2 mousePos, mousePosPrev;

    GameUIManager uiManager;

    float invulnTime;

    bool inDeathState, evading;

    public bool testBullet;

	void Start () {
        health = GameObject.Find("PersistentDataObject").GetComponent<PersistentData>().playerHealth;
        money = GameObject.Find("PersistentDataObject").GetComponent<PersistentData>().playerMoney;

        bulletPool = GameObject.Find("BulletPool");
        uiManager = GameObject.Find("Canvas").GetComponent<GameUIManager>();

        // LoadAll() handles tile sets
        // moveSprites = Resources.LoadAll<Sprite>("Sprites/tyrianship/ship");
        // GetComponent<SpriteRenderer>().sprite = moveSprites[2];
        GetComponent<SpriteRenderer>().sprite = shipSprite;

        mousePos = Vector2.zero;
        mousePosPrev = mousePos;

        // Audio source components added to player in the same order
        AudioSource[] sources = GetComponents<AudioSource>();
        mainFiringSource = sources[0];
        chargeFiringSource = sources[1];
        secondFiringSource = sources[2];

        chargeOutlineChild = transform.Find("ChargeOutline").gameObject;
        chargeOutlineChild.GetComponent<SpriteRenderer>().enabled = false;


        if (gameObject.scene.name == "TutorialStage") {
            StartCoroutine(TutorialScriptedCoroutine());
        }
	}
	
	void Update () {
        if (!evading) {
            if (!tutorialDisableMove)
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            else
                mousePos = Vector2.zero;
        }

        // Player movement boundaries (x values are the screen bounds, y values are arbitrary)
        mousePos = new Vector2(
            Mathf.Clamp(mousePos.x, -Camera.main.orthographicSize / 2, Camera.main.orthographicSize / 2),
            Mathf.Clamp(mousePos.y, -7.5f, 0f));
        
        // a hint of lerp to make movement feel less flat
        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, mousePos.x + pushbackOffset.x, inputSmoothness),
            Mathf.Lerp(transform.position.y, mousePos.y + pushbackOffset.y, inputSmoothness),
            transform.position.z);

        if (Input.GetMouseButtonDown(0) && !tutorialDisableShoot && !inDeathState) {
            if (firingCoroutine == null) {
                firingCoroutine = StartCoroutine(FiringCoroutine());
            }
        }
        if (Input.GetMouseButton(1) && !tutorialDisableChargeShot && !inDeathState) {
            if (chargingCoroutine == null) {
                chargingCoroutine = StartCoroutine(ChargeFiringCoroutine());
            }
        }

        if (testBullet) {
            testBullet = false;
            GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/EnemyBullet"));
            bullet.transform.position = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);
            bullet.GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(transform.position - bullet.transform.position, 7);
            // Add a bit of randomness
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(bullet.GetComponent<Rigidbody2D>().velocity.x + Random.Range(-0.5f, 0.5f), bullet.GetComponent<Rigidbody2D>().velocity.y + Random.Range(-0.5f, 0.5f));
        }

        if (invulnTime > 0) {
            invulnTime -= Time.deltaTime;
            if (invulnFlashCoroutine == null)
                invulnFlashCoroutine = StartCoroutine(InvulnFlashCoroutine());
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
        //float xDif = transform.position.x - lastX;
        //if (xDif < -0.2) {
        //    GetComponent<SpriteRenderer>().sprite = moveSprites[0];
        //} else if (xDif < -0.05) {
        //    GetComponent<SpriteRenderer>().sprite = moveSprites[1];
        //} else if (xDif < 0.05) {
        //    GetComponent<SpriteRenderer>().sprite = moveSprites[2];
        //} else if (xDif < 0.2) {
        //    GetComponent<SpriteRenderer>().sprite = moveSprites[3];
        //} else {
        //    GetComponent<SpriteRenderer>().sprite = moveSprites[4];
        //}
        //lastX = transform.position.x;

        // Handle charge outline child behavior (hackey, since the other half is in ChargeFiringCoroutine())
        if (Input.GetMouseButton(1)) {
            chargeOutlineChild.GetComponent<SpriteRenderer>().enabled = true;
            chargeOutlineChild.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        } else {
            chargeOutlineChild.GetComponent<SpriteRenderer>().enabled = false;
        }

        mousePosPrev = mousePos;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "EnemyBullet" && invulnTime <= 0 && !inDeathState) {
            uiManager.SetHealth(--health);
            if (health == 0) {
                StartCoroutine(DeathCoroutine());
            } else {
                invulnTime = 2f;
            }
            Destroy(collider.gameObject);
        }
    }

    public void AddMoney(int muns) {
        money += muns;
        uiManager.SetMoney(money);
    }

    public void LeftEvadeClicked() {
        if (!tutorialDisableEvade && !evading) {
            tutorialEvadePressed = true;
            StartCoroutine(EvadeCoroutine(-2f, 1f));
            invulnTime = 1f;
        }
        
    }

    public void RightEvadeClicked() {
        if (!tutorialDisableEvade && !evading) {
            tutorialEvadePressed = true;
            StartCoroutine(EvadeCoroutine(2f, 1f));
            invulnTime = 1f;
        }
    }

    private IEnumerator EvadeCoroutine(float xDirection, float time) {
        evading = true;
        mousePos = new Vector2(mousePos.x + xDirection, mousePos.y);
        yield return new WaitForSeconds(time);
        evading = false;
    }

    private IEnumerator DeathCoroutine() {
        if (invulnFlashCoroutine != null) {
            StopCoroutine(invulnFlashCoroutine);
            invulnFlashCoroutine = null;
        }
        inDeathState = true;
        invulnTime = 0f;
        GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 0f);
        GameObject explosion = Instantiate(Resources.Load<GameObject>("Prefabs/Explosion"));
        explosion.transform.position = transform.position;

        yield return new WaitForSeconds(2f);

        inDeathState = false;
        health = GameObject.Find("PersistentDataObject").GetComponent<PersistentData>().playerHealth;
        uiManager.SetHealth(health);
        GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 1f);
        invulnTime = 3f;
    }

    private IEnumerator InvulnFlashCoroutine() {
        bool visible = true;
        while (invulnTime > 0) {
            if (visible) {
                visible = false;
                GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 0f);
            } else {
                visible = true;
                GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 1f);
            }
            yield return new WaitForSeconds(0.16f);
        }
        GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 1f);
        invulnTime = 0;
        invulnFlashCoroutine = null;
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

    private IEnumerator TutorialScriptedCoroutine() {
        yield return new WaitForSeconds(3.0f);

        UITextBoxManager textBox = GameObject.Find("TextPanel").GetComponent<UITextBoxManager>();

        AudioClip flush = Resources.Load<AudioClip>("Sounds/flush");
        AudioSource audio = textBox.gameObject.transform.Find("Audio").GetComponent<AudioSource>();
        audio.volume = 0.5f;
        textBox.FadeIn();
        yield return new WaitForSeconds(1.5f);

        float wait = 0f;

        wait = textBox.TypeText("Running Diagnostics", 0.1f, false);
        yield return new WaitForSeconds(wait + 0.1f);
        wait = textBox.TypeText("\r\n...", 0.5f, true);
        yield return new WaitForSeconds(wait + 1f);
        textBox.TypeText("\r\nChecking Flux Capacitor", 0.0f, true);
        yield return new WaitForSeconds(0.5f);
        textBox.TypeText(" ✓", 0.0f, true);
        yield return new WaitForSeconds(0.1f);
        textBox.TypeText("Checking Flux Capacitor ✓\r\nCalibrating Light Speed", 0.0f, false);
        yield return new WaitForSeconds(0.35f);
        textBox.TypeText(" ✓", 0.0f, true);
        yield return new WaitForSeconds(0.1f);
        textBox.TypeText("Capacitor ✓\r\nCalibrating Light Speed ✓\r\nPreparing Lazors", 0.0f, false);
        yield return new WaitForSeconds(1f);
        textBox.TypeText(" ✓", 0.0f, true);
        yield return new WaitForSeconds(0.2f);
        textBox.TypeText("Calibrating Light Speed ✓\r\nPreparing Lazors ✓\r\nFlushing Toilets", 0.0f, false);
        yield return new WaitForSeconds(0.5f);
        audio.PlayOneShot(flush);
        yield return new WaitForSeconds(1.5f);
        textBox.TypeText(" ✓", 0.0f, true);
        yield return new WaitForSeconds(1f);
        textBox.TypeText("Speed ✓\r\nPreparing Lazors ✓\r\nFlushing Toilets ✓", 0.0f, false);
        yield return new WaitForSeconds(0.25f);
        textBox.TypeText("Preparing Lazors ✓\r\nFlushing Toilets ✓", 0.0f, false);
        yield return new WaitForSeconds(0.25f);
        textBox.TypeText("Flushing Toilets ✓", 0.0f, false);
        yield return new WaitForSeconds(0.25f);
        textBox.TypeText(" ", 0.0f, false);
        yield return new WaitForSeconds(0.5f);
        wait = textBox.TypeText("Diagnostics\r\nComplete.", 0.1f, false);
        yield return new WaitForSeconds(wait + 1f);
        wait = textBox.TypeText("\r\nShip is now\r\noperational.", 0.1f, true);
        yield return new WaitForSeconds(wait + 1f);
        textBox.TypeText("Complete.\r\nShip is now\r\noperational.", 0.0f, false);
        yield return new WaitForSeconds(0.25f);
        textBox.TypeText("Ship is now\r\noperational.", 0.0f, false);
        yield return new WaitForSeconds(0.25f);
        textBox.TypeText("operational.", 0.0f, false);
        yield return new WaitForSeconds(0.25f);
        textBox.TypeText(" ", 0.0f, false);
        yield return new WaitForSeconds(1f);

        // MOVING
        tutorialDisableMove = false;
        wait = textBox.TypeText("Tap the screen to move!", 0.05f, false);
        yield return new WaitForSeconds(wait);
        while (mousePos == mousePosPrev) {
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // SHOOTING
        tutorialDisableShoot = false;
        yield return new WaitForSeconds(0.25f);
        wait = textBox.TypeText("Your weapons will fire automatically.", 0.05f, false);
        yield return new WaitForSeconds(wait + 2f);
        wait = textBox.TypeText("Move around and try them out!", 0.05f, false);
        yield return new WaitForSeconds(wait);
        float moveAroundTime = 0f;
        while (moveAroundTime < 1f) {
            if (mousePos != mousePosPrev) {
                moveAroundTime += Time.deltaTime;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // ENEMY
        yield return new WaitForSeconds(0.5f);
        textBox.TypeText(" ", 0.0f, false);
        GameObject enemy = Instantiate(Resources.Load<GameObject>("Prefabs/Enemies/Enemy"));
        enemy.GetComponent<Enemy>().tutorialDisableFiring = true;
        yield return new WaitForSeconds(1.5f);
        wait = textBox.TypeText("Looks like an enemy is on their own. Take them down!", 0.05f, false);
        yield return new WaitForSeconds(wait + 0.5f);
        while (enemy != null) {
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // SCRAP
        yield return new WaitForSeconds(0.5f);
        wait = textBox.TypeText("Enemies will drop scrap for you to upgrade your ship.", 0.05f, false);
        yield return new WaitForSeconds(wait + 2f);

        //EVADE
        wait = textBox.TypeText("Your evasive maneuvers are now online.", 0.05f, false);
        yield return new WaitForSeconds(wait + 2f);
        wait = textBox.TypeText("Tap the arrows below to evade enemy fire.", 0.05f, false);
        yield return new WaitForSeconds(wait);
        tutorialDisableEvade = false;
        tutorialEvadePressed = false;
        while (!tutorialEvadePressed) {
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // SWARM
        textBox.TypeText(" ", 0.0f, false);
        yield return new WaitForSeconds(1f);
        wait = textBox.TypeText("MULTIPLE ENEMIES DETECTED NEARBY.", 0.1f, false);
        yield return new WaitForSeconds(wait + 1f);
        wait = textBox.TypeText("\r\nGET READY.", 0.2f, true);
        yield return new WaitForSeconds(wait + 3f);

        GameObject swarmParent = Instantiate(Resources.Load<GameObject>("Prefabs/Swarm"));

        textBox.TypeText(" ", 0.0f, false);
        yield return new WaitForSeconds(0.1f);
        textBox.FadeOut();

        while (!swarmParent.GetComponent<EnemySwarm>().areWavesOver) {
            yield return new WaitForSeconds(Time.deltaTime);
        }

        textBox.FadeIn();
        yield return new WaitForSeconds(2.5f);

        wait = textBox.TypeText("Very good! ", 0.05f, false);
        yield return new WaitForSeconds(wait + 0.5f);
        wait = textBox.TypeText("Now, get back to base before you run out of fuel.", 0.05f, true);
        yield return new WaitForSeconds(wait + 1f);
        textBox.FadeOut();
        yield return new WaitForSeconds(2f);

        GameObject.Find("ScreenFader").GetComponent<ScreenFaderManager>().FadeTo(Color.black, 2);
        yield return new WaitForSeconds(2.5f);
        GameObject endText = new GameObject("EndText");
        endText.AddComponent<Text>();
        endText.transform.parent = GameObject.Find("Canvas").transform;
        endText.GetComponent<Text>().font = Resources.Load<Font>("Fonts/PressStart2P");
        endText.GetComponent<Text>().fontSize = 72;
        endText.GetComponent<Text>().text = "Thank you for\r\nplaying our game!\r\n\r\nMore to come soon!";
        endText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        endText.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        endText.GetComponent<RectTransform>().offsetMin = new Vector2(-1000, -1000);
        endText.GetComponent<RectTransform>().offsetMax = new Vector2(1000, 1000);

        endText.GetComponent<Text>().color = new Color(1f, 1f, 1f, 0f);
        float textAlpha = 0f;
        while (textAlpha < 1f) {
            textAlpha += 0.02f;
            endText.GetComponent<Text>().color = new Color(1f, 1f, 1f, textAlpha);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return new WaitForSeconds(3f);
        while (textAlpha > 0f) {
            textAlpha -= 0.02f;
            endText.GetComponent<Text>().color = new Color(1f, 1f, 1f, textAlpha);
        }
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("GalaxyViewMenu");
    }
}
