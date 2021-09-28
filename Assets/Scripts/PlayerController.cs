using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{
    protected Rigidbody2D rb;
    protected gameManager gameManager;
    public float FireRate = 1;
    public float MoveSpeed = 5f;
    public GameObject Projectile;
    private float cooldown = 0f;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.Find("GameManager").GetComponent<gameManager>();
    }

    void FixedUpdate() {
        rb.MovePosition(transform.position + new Vector3(Input.GetAxis("LeftStickHorizontal"), Input.GetAxis("LeftStickVertical"), 0) * MoveSpeed * Time.deltaTime);
    }
    
    void Update(){
        if (Input.mousePresent) {
            Vector3 mouseDir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            transform.rotation = Quaternion.AngleAxis(0-(Mathf.Atan2(mouseDir.x, mouseDir.y) * Mathf.Rad2Deg), Vector3.forward);
        } else {
            transform.rotation = Quaternion.Euler(new Vector3(Input.GetAxis("RightStickHorizontal"), Input.GetAxis("RightStickVertical"), 0));
        }
        cooldown -= Time.deltaTime;
        if (Input.GetButton("Fire1") && cooldown <= 0f) {
            cooldown = 1 / FireRate;
            GameObject projectile = Instantiate(Projectile, Projectile.transform.position, Quaternion.identity) as GameObject;
            projectile.GetComponent<Rigidbody2D>().simulated = true;
            projectile.GetComponent<Rigidbody2D>().AddForce(transform.up * 1000);
            Object.Destroy(projectile, 1.0f);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        switch (other.gameObject.name) {
            case "nWall":
                gameManager.currentRoom = gameManager.currentRoom.exit("north").room;
                transform.position = new Vector3(transform.position.x, -3.4f, 0);
                break;
            case "eWall":
                gameManager.currentRoom = gameManager.currentRoom.exit("east").room;
                transform.position = new Vector3(-7.4f, transform.position.y, 0);
                break;
            case "sWall":
                gameManager.currentRoom = gameManager.currentRoom.exit("south").room;
                transform.position = new Vector3(transform.position.x, 3.4f, 0);
                break;
            case "wWall":
                gameManager.currentRoom = gameManager.currentRoom.exit("west").room;
                transform.position = new Vector3(7.4f, transform.position.y, 0);
                break;
            default:
                break;
        }
        
    }
}
