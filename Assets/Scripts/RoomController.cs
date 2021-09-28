using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomClasses;

public class RoomController : MonoBehaviour {
    private Dictionary<string, GameObject> walls = new Dictionary<string, GameObject>();

    void Start() {
        walls.Add("north", transform.Find("nWall").gameObject);
        walls.Add("east", transform.Find("eWall").gameObject);
        walls.Add("south", transform.Find("sWall").gameObject);
        walls.Add("west", transform.Find("wWall").gameObject);
    }
    
    void Update() {
        
    }

    public void DrawRoom(Room room, bool isMini = false) {
        string[] dirs = new string[]{"north", "east", "south", "west"};
        string roomFolder = "";
        foreach (string d in dirs) {
            if (room.exit(d).wall) {
                roomFolder += d[0];
            }
            walls[d].GetComponent<SpriteRenderer>().enabled = room.exit(d).wall;
            walls[d].GetComponent<BoxCollider2D>().isTrigger = !room.exit(d).wall;
        }
        GameObject floor = transform.Find("Floor").gameObject;
        if (isMini) {
            floor.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/rectBase.png") as Sprite;
        } else {
            if (roomFolder != ""){
                Debug.Log(Resources.LoadAll<Sprite>("Sprites/rooms/" + roomFolder).Length);
                Sprite[] floors = Resources.LoadAll<Sprite>("Sprites/rooms/" + roomFolder);
                floor.GetComponent<SpriteRenderer>().sprite = floors[Random.Range(0, floors.Length-1)];
            }
        }
    }
}
