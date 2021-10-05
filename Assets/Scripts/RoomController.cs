using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomClasses;

public class RoomController : MonoBehaviour {
    private Dictionary<string, GameObject> walls;
    private List<string> unlocks;
    public bool locked;
    private static string[] dirs = new string[]{"north", "east", "south", "west"};

    public void DrawRoom(Room room, bool isMini = false) {
        walls = new Dictionary<string, GameObject>{
            {"north", transform.Find("nWall").gameObject},
            {"east", transform.Find("eWall").gameObject},
            {"south", transform.Find("sWall").gameObject},
            {"west", transform.Find("wWall").gameObject}
        };
        unlocks = new List<string>();
        string roomFolder = "";
        foreach (string d in dirs) {
            if (room.exit(d).wall) {
                roomFolder += d[0];
            } else {
                unlocks.Add(d);
            }
            walls[d].GetComponent<BoxCollider2D>().isTrigger = !room.exit(d).wall;
            if (isMini) {
                walls[d].GetComponent<BoxCollider2D>().enabled = false;
            }
        }
        GameObject floor = transform.Find("Floor").gameObject;
        if (roomFolder == "") {
            floor.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/rooms/Base");
        } else {
            Sprite[] floors = Resources.LoadAll<Sprite>("Sprites/rooms/" + roomFolder);
            floor.GetComponent<SpriteRenderer>().sprite = floors[Random.Range(0, floors.Length-1)];
        }
        if (isMini) {
            transform.localScale = new Vector3(.1f, .1f, .1f);
            floor.GetComponent<SpriteRenderer>().sortingOrder = 5;
        }
    }

    public void Lock() {
        foreach (string d in unlocks) {
            walls[d].GetComponent<BoxCollider2D>().enabled = false;
        }
        locked = true;
    }

    public void Unlock() {
        foreach (string d in unlocks) {
            walls[d].GetComponent<BoxCollider2D>().enabled = true;
        }
        locked = false;
    }
}
