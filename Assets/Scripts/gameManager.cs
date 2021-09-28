using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using CustomClasses;

public class gameManager : MonoBehaviour {
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
    public Room currentRoom;
    public static Dictionary<int, Room> level = new Dictionary<int, Room>();
    private static Dictionary<string, string> opposites = new Dictionary<string, string>{
        {"north", "south"},
        {"east",  "west"},
        {"south",  "north"},
        {"west",  "east"}
    };
    private Room redraw;

    void Start() {
        minRooms = Mathf.Clamp(minRooms, 5, maxX * maxY);
        maxRooms = Mathf.Clamp(maxRooms, minRooms, maxX * maxY);
        Debug.Log("Generating Level with " + minRooms + "-" + maxRooms + " rooms, in a " + maxX + "x" + maxY + " grid:");
        GenerateLevel();
        Debug.Log("Done!");
        foreach (KeyValuePair<int, Room> a in level) {
            Debug.Log(a.Key + ": " + JsonUtility.ToJson(a.Value).ToString());
        }
    }

    void Update() {
        if (currentRoom != redraw) {
            redraw = currentRoom;
            GameObject.Find("CurrentRoom").GetComponent<RoomController>().DrawRoom(currentRoom);
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
                    Debug.Log("Creating room: " + dirInfo.i + " " + d + " of here");
                    GenerateRoom(dirInfo.i, d);
                }
            }
        }
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
