using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CustomClasses;

public class EnemyController : MonoBehaviour {
    protected gameManager gameManager;
    private Vector3 targetLocation;
    public int tier;
    public float damage;
    public float attackSpeed;
    public float health;
    public float speed;
    private bool shooting = false;
    private float shootingcooldown;
    private float this[string prop]{
        get {
            switch(prop) {
                case "attackSpeed": return attackSpeed;
                case "damage": return damage;
                case "health": return health;
                case "speed": return speed;
                default: return -1f;
            }
        }
        set {
            switch(prop) {
                case "attackSpeed": attackSpeed = value; break;
                case "damage": damage = value; break;
                case "health": health = value; break;
                case "speed": speed = value; break;
                default: break;
            }
        }
    }

    public Part leftArm;
    public Part rightArm;
    private GameObject leftArmObject;
    private GameObject rightArmObject;

    void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<gameManager>();
        leftArm = new Part("arm", Mathf.Clamp(Random.Range(tier - 1, tier + 2), 1, 6));
        rightArm = new Part("arm", Mathf.Clamp(Random.Range(tier - 1, tier + 2), 1, 6));
        if (leftArm.slotNums == 0 && rightArm.slotNums == 0) {
            rightArm.slotNums = 1;
        }
        for (int i = 0; i < leftArm.slotNums; i++) {
            leftArm.slots.Add(new Part(gameManager.weaponArchetypes.Keys.ToArray().ElementAt(Random.Range(0, gameManager.weaponArchetypes.Keys.Count)), Mathf.Clamp(Random.Range(tier - 1, tier + 2), 1, 6)));
        }
        for (int i = 0; i < leftArm.slotNums; i++) {
            rightArm.slots.Add(new Part(gameManager.weaponArchetypes.Keys.ToArray().ElementAt(Random.Range(0, gameManager.weaponArchetypes.Keys.Count)), Mathf.Clamp(Random.Range(tier - 1, tier + 2), 1, 6)));
        }
        leftArmObject = leftArm.Draw(transform.Find("LeftArmSlot").transform);
        rightArmObject = rightArm.Draw(transform.Find("RightArmSlot").transform);
        Dictionary<string, float> propScales = new Dictionary<string, float> {
            {"damage", 1f},
            {"attackSpeed", .25f},
            {"health", 100f},
            {"speed", .75f}
        };
        foreach (string prop in propScales.Keys) {
            this[prop] = (int) Mathf.Round(Random.Range(1f, tier + 5) * propScales[prop]);
        }
        targetLocation = new Vector2(Random.Range(-7.5f, 7.5f), Random.Range(-3.5f, 3.5f));
        transform.position = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(-3.5f, 3.5f), 0);
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/enemies/tier"+tier);
        shootingcooldown = Random.Range(2f, 3f);
    }

    void Update() {
        shootingcooldown -= 1f * Time.deltaTime;
        if (shootingcooldown <= 0) {
            shootingcooldown = Random.Range(2f, 3f);
            shooting = !shooting;
        }
        if (GameObject.Find("Player") != null) {
            transform.up = GameObject.Find("Player").transform.position - transform.position;
            transform.position = Vector2.MoveTowards(transform.position, targetLocation, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetLocation) < .5) {
                targetLocation = new Vector2(Random.Range(-7.5f, 7.5f), Random.Range(-3.5f, 3.5f));
            }
            if (shooting) {
                leftArm.Fire(this, "Enemy");
                rightArm.Fire(this, "Enemy");
            }
        }
        if(health <= 0) {
            gameManager.enemies -= 1;
            GameObject.Destroy(gameObject);
        }
    }

    void OnDestroy() {
        leftArm.Drop(60, leftArmObject.transform);    
        rightArm.Drop(60, rightArmObject.transform);
    }
}
