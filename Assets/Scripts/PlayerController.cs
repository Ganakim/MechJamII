using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour{
    protected Rigidbody2D rb;
    protected gameManager gameManager;
    public float FireRate = 1;
    public float MoveSpeed = 5f;
    public GameObject Projectile;
    private PlayerControls controls;
    private InputAction move;
    private InputAction rotate;
    private InputAction rotateM;
    private bool usingMouse = false;
    private float cooldown;

    void Awake() {
        controls = new PlayerControls();
        move = controls.Gameplay.Move;
        rotate = controls.Gameplay.Rotate;
        rotateM = controls.Gameplay.RotateMouse;
        rotate.performed += a => {usingMouse = false;};
        rotateM.performed += a => {usingMouse = true;};
    }

    void OnEnable() {
        controls.Enable();
    }

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.Find("GameManager").GetComponent<gameManager>();
    }

    void FixedUpdate() {
        rb.MovePosition(transform.position + new Vector3(move.ReadValue<Vector2>().x, move.ReadValue<Vector2>().y, 0) * MoveSpeed * Time.deltaTime);
    }
    
    void Update(){
        Vector2 lookDir = usingMouse ? (Vector2) (transform.position - Camera.main.ScreenToWorldPoint(rotateM.ReadValue<Vector2>())) : rotate.ReadValue<Vector2>();
        transform.rotation = Quaternion.AngleAxis((0 - Mathf.Atan2(lookDir.x, lookDir.y) * Mathf.Rad2Deg + (usingMouse ? 180 : 0)), Vector3.forward);
        cooldown -= Time.deltaTime;
        if (controls.Gameplay.Shoot.triggered && cooldown <= 0f) {
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
