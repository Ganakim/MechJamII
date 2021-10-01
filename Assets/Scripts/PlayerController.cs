using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using CustomClasses;

public class PlayerController : MonoBehaviour{
    protected gameManager gameManager;
    protected MenuManager menuManager;
    protected Rigidbody2D rb;
    public float damage;
    public float attackSpeed;
    public float health;
    public float speed;
    public GameObject Projectile;
    private PlayerControls controls;
    private InputAction move;
    private InputAction rotate;
    private InputAction rotateM;
    private bool usingMouse = false;
    private float cooldown;

    public Part leftArm;
    public Part rightArm;

    void Awake() {
        controls = new PlayerControls();
        move = controls.Gameplay.Move;
        rotate = controls.Gameplay.Rotate;
        rotateM = controls.Gameplay.RotateM;
        rotate.performed += a => {usingMouse = false;};
        rotateM.performed += a => {usingMouse = true;};
        Dictionary<Vector2, string> dirs = new Dictionary<Vector2, string> {
            {Vector2.up, "mech"},
            {Vector2.right, "stats"},
            {Vector2.down, "controls"},
            {Vector2.left, "map"}
        };
        controls.Gameplay.Menu.started += a => {menuManager.OpenMenu(dirs[a.ReadValue<Vector2>()]);};
    }

    void OnEnable() {
        controls.Enable();
    }

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.Find("GameManager").GetComponent<gameManager>();
        menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();
        leftArm = new Part("arm", 1);
        rightArm = new Part("arm", 1);
    }

    void FixedUpdate() {
        rb.MovePosition(transform.position + (new Vector3(move.ReadValue<Vector2>().x, move.ReadValue<Vector2>().y, 0) * speed)/50);
    }
    
    void Update(){
        this.attackSpeed = leftArm.attackSpeed + rightArm.attackSpeed;
        Vector2 lookDir = usingMouse ? (Vector2) (transform.position - Camera.main.ScreenToWorldPoint(rotateM.ReadValue<Vector2>())) : rotate.ReadValue<Vector2>();
        transform.rotation = Quaternion.AngleAxis((0 - Mathf.Atan2(lookDir.x, lookDir.y) * Mathf.Rad2Deg + (usingMouse ? 180 : 0)), Vector3.forward);
        cooldown -= Time.deltaTime;
        if (controls.Gameplay.Shoot.triggered && cooldown <= 0f) {
            cooldown = 1 / attackSpeed;
            leftArm.Fire();
            rightArm.Fire();
            // GameObject projectile = Instantiate(Projectile, Projectile.transform.position, Quaternion.identity) as GameObject;
            // projectile.GetComponent<Rigidbody2D>().simulated = true;
            // projectile.GetComponent<Rigidbody2D>().AddForce(transform.up * 1000);
            // Object.Destroy(projectile, 1.0f);
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
    public void Die() {
        if(health == 0){
            SceneManager.LoadScene(2);
        }
    }
}
