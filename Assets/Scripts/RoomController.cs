using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomClasses;

public class RoomController : MonoBehaviour {
    private Dictionary<string, GameObject> walls;

    public void DrawRoom(Room room, bool isMini = false) {
        walls = new Dictionary<string, GameObject>{
            {"north", transform.Find("nWall").gameObject},
            {"east", transform.Find("eWall").gameObject},
            {"south", transform.Find("sWall").gameObject},
            {"west", transform.Find("wWall").gameObject}
        };
        string[] dirs = new string[]{"north", "east", "south", "west"};
        string roomFolder = "";
        foreach (string d in dirs) {
            if (room.exit(d).wall) {
                roomFolder += d[0];
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
}
