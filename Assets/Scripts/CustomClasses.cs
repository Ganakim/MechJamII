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
                if (req != dir && !this._walls.ContainsKey(dir) && this._walls.Count < 3 && Random.Range(0, 100) <= density && this.exit(opposites[dir]).room == null) {
                    this._walls.Add(dir, "wall");
                    this.tags.Add(dir);
                }
            }
        }
    }

    [Serializable]
    public class Projectile {
        public string sprite;
        public float lifetime;
        public float speed;

        public Projectile(string s, float? s2, float? l) {
            sprite = "Sprites/Projectiles/" + s;
            speed = s2 != null ? (float) s2 : 10f;
            lifetime = l != null ? (float) l : 1f;
        }
    }
    
    public class Part {
        private static gameManager gameManager = GameObject.Find("GameManager").GetComponent<gameManager>();
        public string type;
        public Sprite sprite;
        public int tier;
        public float _damage;
        public float damage {
            get {
                float d = this._damage;
                foreach (Part p in this.slots) {
                    d += p.damage;
                }
                return d;
            }
            set {
                this._damage = value;
            }
        }
        public float _attackSpeed;
        public float attackSpeed {
            get {
                float a = this._attackSpeed;
                foreach (Part p in this.slots) {
                    a += p.attackSpeed;
                }
                return a;
            }
            set {
                this._attackSpeed = value;
            }
        }
        public float _health;
        public float health {
            get {
                float h = this._health;
                foreach (Part p in this.slots) {
                    h += p.health;
                }
                return h;
            }
            set {
                this._health = value;
            }
        }
        public float _speed;
        public float speed {
            get {
                float s = this._speed;
                foreach (Part p in this.slots) {
                    s += p.speed;
                }
                return s;
            }
            set {
                this._speed = value;
            }
        }
        public Part[] slots;

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
                {"health", 12},
                {"speed", 1}
            };
            gameManager.shuffle(props);
            for (int i = 0; i < Random.Range(Mathf.Clamp(tier - 2, 0, 3), Mathf.Floor(.5f * tier + 2)); i++) {
                this[props[i]] = Random.Range(0, tier + 5) * propScales[props[i]];
            }
            if (type == "arm") {
                this.slots = new Part[(int) Random.Range(Mathf.Ceil(.5f * tier - 1), Mathf.Floor(.5f * tier + 1))];
            } else {
                var info = GameObject.Find("GameManager").GetComponent<gameManager>().weaponArchetypes[type];
                foreach (string prop in props) {
                    if (info.props.ContainsKey(prop)) {
                        this[prop] += info.props[prop];
                    }
                }
            }
        }

        public void Fire() {

        }
    }

    [Serializable]
    public class Enemy {
        public string sprite;
        public Sprite projectile;
        public int health;
        public int damage;
        public int attackSpeed;
        public int speed;
        public string name;

        public Enemy(Sprite s) {
            this.sprite = "Sprites/enemies/" + s;
        }
    }
}