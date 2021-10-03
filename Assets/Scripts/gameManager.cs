using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using CustomClasses;
using UnityEngine.UI;

public class gameManager : MonoBehaviour {
    public int depth;
    public int maxX;
    public int maxY;
    [Range(5, 100)]
    public int minRooms;
    [Range(5, 100)]
    public int maxRooms;
    [Range(0, 100)]
    public int density;
    [Range(0, 100)]
    public int shopChance;
    public int scrapMetal;
    protected MenuManager menuManager;
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

    public Dictionary<string, (Projectile projectile, Dictionary<string, float> props)> weaponArchetypes = new Dictionary<string, (Projectile, Dictionary<string, float>)>{
        {"Machine Gun", (new Projectile("Bullet", 20f, 1f), new Dictionary<string, float>{
            {"attackSpeed", 10f}
        })},
        {"Cannon", (new Projectile("CannonBall", 10f, 1f), new Dictionary<string, float>{
            {"health", 20f}
        })},
        {"Torpedo", (null, new Dictionary<string, float>{
            {"damage", 8f}
        })},
        {"Laser Sword", (new Projectile("Sword", 0f, .2f), new Dictionary<string, float>{
            {"speed", 2f}
        })}
    };

    private static (string name, string type, int tier, float? health, float? damage, float? attackSpeed, float? speed)[] parts = new (string, string, int, float?, float?, float?, float?)[]{
        ("Left-Overs", "leftArm", 1, 0f, 0f, 0f, 0f),
        ("Not-Quite-Righty-Tighty", "rightArm", 1, 0f, 0f, 0f, 0f)
    };

    void Start() {
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
        foreach (KeyValuePair<int, Room> a in level) {
            Debug.Log(a.Key + ": " + JsonUtility.ToJson(a.Value).ToString());
        }
    }

    void Update() {
        if (currentRoom != redraw) {
            redraw = currentRoom;
            GameObject.Find("CurrentRoom").GetComponent<RoomController>().DrawRoom(currentRoom);
            currentRoom.tags.Add("visited");
        }
    }

    void GenerateLevel() {
        int start = Random.Range(0, maxX * maxY - 1);
        Debug.Log("Starting at Index of " + start);
        GenerateRoom(start);
        currentRoom = level[start];
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

    public Enemy[] SpawnEnemies() {
        return new Enemy[0];
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
