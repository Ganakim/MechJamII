using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using CustomClasses;
using UnityEngine.UI;

public class gameManager : MonoBehaviour {
    public static int depth;
    public int maxX;
    public int maxY;
    [Range(5, 100)]
    public int minRooms;
    [Range(5, 100)]
    public int maxRooms;
    [Range(0, 100)]
    public int density;
    [Range(0, 100)]
    public int scrapMetal;
    protected MenuManager menuManager;
    public GameObject piratePrefab;
    // public TextMeshProUGUI scrapMetalText;

    public Room currentRoom;
    public static Dictionary<int, Room> level = new Dictionary<int, Room>();
    private static Dictionary<string, string> opposites = new Dictionary<string, string>{
        {"north", "south"},
        {"east",  "west"},
        {"south",  "north"},
        {"west",  "east"}
    };
    private Room redraw;
    public int enemies = 0;

    public Dictionary<string, (Projectile projectile, Dictionary<string, float> props)> weaponArchetypes = new Dictionary<string, (Projectile, Dictionary<string, float>)>{
        {"Machine Gun", (new Projectile(new List<string>{"Bullet"}, 1000f, 1f), new Dictionary<string, float>{
            {"attackSpeed", 10f}
        })},
        {"Cannon", (new Projectile(new List<string>{"Cannon Ball"}, 800f, 1f), new Dictionary<string, float>{
            {"health", 20f}
        })},
        {"Torpedo", (new Projectile(null, 500f, 2f), new Dictionary<string, float>{
            {"damage", 8f}
        })},
        {"Laser Sword", (new Projectile(new List<string>{"Slash 1", "Slash 2"}, 0f, .2f), new Dictionary<string, float>{
            {"speed", 2f}
        })}
    };

    void Start() {
        Physics2D.gravity = new Vector3(0, 0, 0);
        menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();
        minRooms = Mathf.Clamp(minRooms, 5, maxX * maxY);
        maxRooms = Mathf.Clamp(maxRooms, minRooms, maxX * maxY);
        Debug.Log("Generating Level with " + minRooms + "-" + maxRooms + " rooms, in a " + maxX + "x" + maxY + " grid:");
        bool hasBoss;
        do {
            level = new Dictionary<int, Room>();
            GenerateLevel();
            hasBoss = false;
            foreach (Room room in level.Values) {
                if (Array.FindAll(new string[4]{"north", "east", "south", "west"}, dir=>room.exit(dir).wall).Length == 3 && room != currentRoom) {
                    if (hasBoss) {
                        room.tags.Add("shop");
                        break;
                    } else {
                        room.tags.Add("boss");
                        hasBoss = true;
                    }
                }
            }
        } while (level.Count < minRooms || level.Count > maxRooms || !hasBoss);
        Debug.Log("Done!");
    }

    void Update() {
        if (currentRoom != redraw) {
            redraw = currentRoom;
            RoomController room = GameObject.Find("CurrentRoom").GetComponent<RoomController>();
            room.DrawRoom(currentRoom);
            if (!currentRoom.tags.Contains("visited")) {
                currentRoom.tags.Add("visited");
                SpawnEnemies();
                room.Lock();
            }
            if (enemies == 0 && room.locked) {
                room.Unlock();
            }
            foreach (GameObject item in GameObject.FindGameObjectsWithTag("item")) {
                GameObject.Destroy(item);
            }
            menuManager.CloseMenus();
        }
    }

    void GenerateLevel() {
        int start = Random.Range(0, maxX * maxY - 1);
        GenerateRoom(start);
        currentRoom = level[start];
        currentRoom.tags.Add("visited");
        void GenerateRoom(int index, string dir = null) {
            Room room = new Room(index, dir != null ? opposites[dir] : null);
            level.Add(index, room);
            string[] dirs = new string[]{"north", "east", "south", "west"};
            shuffle(dirs);
            foreach (string d in dirs) {
                var dirInfo = room.exit(d);
                if (dirInfo.room == null && !dirInfo.wall) {
                    GenerateRoom(dirInfo.i, d);
                }
            }
        }
    }

    public void SpawnEnemies() {
        for (int i = 0; i < Random.Range(2, 12); i++) {
            enemies += 1;
            GameObject enemy = Instantiate(piratePrefab) as GameObject;
            enemy.GetComponent<EnemyController>().tier = Mathf.Clamp(Random.Range(depth - 1, depth + 2), 1, 6);
            enemy.SetActive(true);
        }
    }

    public void OpenMainMenuScene() {
        SceneManager.LoadScene(0);
    }

    public void ExitGame() {
        // Environment.Exit();
    }

    public void StartGame() {
        SceneManager.LoadScene(1);
    }

    public void GameOver() {
        SceneManager.LoadScene(2);
    }

    public void RestartGame() {
        SceneManager.LoadScene(1);
    }

    public void UpdateScrapMetalScore(int scrapMetalToAdd) {
        scrapMetal += scrapMetalToAdd;
        // scrapMetalText.text = "Scrap Metal: " + scrapMetal;
    }

    public static void shuffle(string[] texts) {
        for (int t = 0; t < texts.Length; t++ ) {
            string tmp = texts[t];
            int r = Random.Range(t, texts.Length);
            texts[t] = texts[r];
            texts[r] = tmp;
        }
    }
}
