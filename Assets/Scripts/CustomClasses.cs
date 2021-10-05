using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomClasses {
    [Serializable]
    public class Room {
        public int index;
        private static gameManager gameManager = GameObject.Find("GameManager").GetComponent<gameManager>();
        private Dictionary<int, Room> level = gameManager.level;
        private static int maxX = gameManager.maxX;
        private static int maxY = gameManager.maxY;
        private static int minRooms = gameManager.minRooms;
        private static int maxRooms = gameManager.maxRooms;
        private static int density = gameManager.density;
        public List<string> tags = new List<string>();
        private static Dictionary<string, string> opposites = new Dictionary<string, string>{
            {"north", "south"},
            {"east",  "west"},
            {"south",  "north"},
            {"west",  "east"}
        };
        private Dictionary<string, string> _walls = new Dictionary<string, string>();

        public (Room room, Boolean wall, Boolean oob, int i) exit(string dir) {
            Dictionary<string, int> dirs = new Dictionary<string, int>{
                {"north", this.index - maxX},
                {"east", this.index + 1},
                {"south", this.index + maxX},
                {"west", this.index - 1}
            };
            if (this._walls.ContainsKey(dir)) {
                return (null, this._walls[dir] == "wall" || this._walls[dir] == "oob", this._walls[dir] == "oob", dirs[dir]);
            } else {
                if (level.ContainsKey(dirs[dir])) {
                    return (level[dirs[dir]], false, false, dirs[dir]);
                } else {
                    return (null, false, false, dirs[dir]);
                }
            }
        }

        public Room(int index, string req = null) {
            this.index = index;
            if (this.index - maxX < 0) {
                this._walls.Add("north", "oob");
                this.tags.Add("north-oob");
            } else if (req != "north" && level.ContainsKey(this.index - maxX) && level[this.index - maxX].exit("south").wall) {
                this._walls.Add("north", "wall");
                this.tags.Add("north");
            }
            if (Math.Floor((decimal) (this.index + 1) % maxX) <= 0) {
                this._walls.Add("east", "oob");
                this.tags.Add("east-oob");
            } else if (req != "east" && level.ContainsKey(this.index + 1) && level[this.index + 1].exit("west").wall) {
                this._walls.Add("east", "wall");
                this.tags.Add("east");
            }
            if (this.index >= maxX * (maxY - 1)) {
                this._walls.Add("south", "oob");
                this.tags.Add("south-oob");
            } else if (req != "south" && level.ContainsKey(this.index + maxX) && level[this.index + maxX].exit("north").wall) {
                this._walls.Add("south", "wall");
                this.tags.Add("south");
            }
            if (this.index % maxX == 0) {
                this._walls.Add("west", "oob");
                this.tags.Add("west-oob");
            } else if (req != "west" && level.ContainsKey(this.index - 1) && level[this.index - 1].exit("east").wall) {
                this._walls.Add("west", "wall");
                this.tags.Add("west");
            }
            string[] d = new string[]{"north", "east", "south", "west"};
            gameManager.shuffle(d);
            foreach (string dir in d) {
                if (req != dir && !this._walls.ContainsKey(dir) && this._walls.Count < 3 && Random.Range(0, 100) <= density && this.exit(dir).room == null) {
                    this._walls.Add(dir, "wall");
                    this.tags.Add(dir);
                }
            }
        }
    }

    [Serializable]
    public class Projectile {
        public List<string> sprites;
        public float lifetime;
        public float speed;

        public Projectile(List<string> s, float? s2, float? l) {
            sprites = s;
            speed = s2 != null ? (float) s2 : 10f;
            lifetime = l != null ? (float) l : 1f;
        }
    }
    
    public class Part {
        private static gameManager gameManager = GameObject.Find("GameManager").GetComponent<gameManager>();
        public string type;
        public int tier;
        public float damage;
        public float attackSpeed;
        public float health;
        public float speed;
        public List<Part> slots = new List<Part>();
        public List<GameObject> slotObjects = new List<GameObject>();
        public float slotNums;
        public bool canShoot = true;

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

        public Part(string type, int tier) {
            this.type = type;
            this.tier = tier;
            string[] props = new string[]{"damage", "attackSpeed", "health", "speed"};
            Dictionary<string, int> propScales = new Dictionary<string, int> {
                {"damage", 10},
                {"attackSpeed", 1},
                {"health", 100},
                {"speed", 1}
            };
            gameManager.shuffle(props);
            for (int i = 0; i < Mathf.Clamp(Random.Range(tier - 2, Mathf.Floor(.5f * tier + 2)), 0, 4); i++) {
                this[props[i]] = (int) Mathf.Round(Random.Range(0f, tier + 5) * propScales[props[i]]);
            }
            if (type == "arm") {
                this.slotNums = (int) Mathf.Round(Random.Range(Mathf.Ceil(.5f * tier - 1), Mathf.Floor(.5f * tier + 1)));
            } else {
                var info = GameObject.Find("GameManager").GetComponent<gameManager>().weaponArchetypes[type];
                foreach (string prop in props) {
                    if (info.props.ContainsKey(prop)) {
                        this[prop] += info.props[prop];
                    }
                }
            }
        }

        public GameObject Draw(Transform parent, Vector3? offset = null){
            GameObject part = new GameObject("part");
            part.AddComponent<SpriteRenderer>();
            part.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + (type == "arm" ? "arms/" : "weapons/" + type + "_") + "tier" + tier);
            part.GetComponent<SpriteRenderer>().sortingOrder = type == "arm" ? 3 : 4;
            part.transform.parent = parent;
            part.tag = parent.tag;
            part.transform.localPosition = offset != null ? (Vector3) offset : new Vector3(0, 0, 0);
            part.transform.localScale = new Vector3(1, 1, 1);
            foreach (GameObject weapon in slotObjects) {
                slotObjects.Remove(weapon);
                GameObject.Destroy(weapon);
            }
            for (int i = 0; i < slotNums; i++) {
                if (slots.Count > i) {
                    slotObjects.Add(slots[i].Draw(part.transform, new Vector3(0, i - (slotNums/2), 0)));
                }
            }
            return part;
        }

        public void Fire(MonoBehaviour mb, string tag, float bonusDamage = 0f, float bonusAttackSpeed = 0f, GameObject pos = null) {
            if (type == "arm") {
                for (int i = 0; i < slotNums; i++) {
                    if (slots.Count > i) {
                        slots[i].Fire(mb, tag, damage, attackSpeed, slotObjects[i]);
                    }
                }
            } else {
                if (canShoot) {
                    var projectileInfo = GameObject.Find("GameManager").GetComponent<gameManager>().weaponArchetypes[type].projectile;
                    GameObject projectile = new GameObject("projectile");
                    projectile.transform.position = pos.transform.position;
                    projectile.transform.rotation = pos.transform.rotation;
                    projectile.transform.Rotate(0f, 0f, Random.Range(-attackSpeed, attackSpeed) * 2f);
                    projectile.transform.localScale = new Vector3(-pos.transform.lossyScale.x, 1, 1);
                    if (type == "Laser Sword") {
                        projectile.transform.position += projectile.transform.up * Random.Range(1f, 1.5f);
                    }
                    projectile.AddComponent<SpriteRenderer>();
                    if (projectileInfo.sprites != null) {
                        projectile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/projectiles/" + projectileInfo.sprites[Random.Range(0, projectileInfo.sprites.Count)]);
                    } else {
                        projectile.GetComponent<SpriteRenderer>().sprite = pos.GetComponent<SpriteRenderer>().sprite;
                        pos.GetComponent<SpriteRenderer>().enabled = false;
                    }
                    GameObject.Destroy(projectile, projectileInfo.lifetime);
                    projectile.GetComponent<SpriteRenderer>().sortingOrder = 2;
                    projectile.AddComponent<Rigidbody2D>();
                    projectile.AddComponent<CircleCollider2D>();
                    projectile.GetComponent<CircleCollider2D>().isTrigger = true;
                    projectile.GetComponent<Rigidbody2D>().AddForce(projectile.transform.up * projectileInfo.speed);
                    projectile.AddComponent<ProjectileController>();
                    projectile.GetComponent<ProjectileController>().damage = damage + bonusDamage;
                    projectile.tag = tag + "Projectile";

                    canShoot = false;
                    mb.StartCoroutine(Reload(attackSpeed + bonusAttackSpeed, pos));
                }
            }
        }

        public IEnumerator Reload(float reloadTime = 0f, GameObject launchSelfObject = null) {
            yield return new WaitForSeconds(1/Mathf.Clamp(reloadTime, 1, 999));
            launchSelfObject.GetComponent<SpriteRenderer>().enabled = true;
            canShoot = true;
        }

        public void Drop(int chance, Transform pos) {
            if (type == "arm") {
                for (int i = 0; i < slotNums; i++) {
                    if (slots.Count > i) {
                        slots[i].Drop(chance, pos);
                        slots.RemoveAt(i);
                    }
                }
            }
            if (Random.Range(0, 100) < chance) {
                GameObject item = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/ItemDrop"), pos.position, pos.rotation) as GameObject;
                item.GetComponent<ItemDropController>().part = this;
                item.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + (type == "arm" ? "arms/" : "weapons/" + type + "_") + "tier" + tier);
                item.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(-10f, 10f) * 10f, Random.Range(-10f, 10f) * 10f, 0f));
            }
        }
    }
}