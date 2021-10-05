using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private PlayerControls controls;
    private InputAction move;
    private InputAction rotate;
    private InputAction rotateM;
    private bool usingMouse = false;
    private bool shooting = false;
    public Part standingOn;

    public Part leftArm;
    public Part rightArm;
    private GameObject leftArmObject;
    private GameObject rightArmObject;

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
        controls.Gameplay.Shoot.started += a => {shooting=true;};
        controls.Gameplay.Shoot.canceled += a => {shooting=false;};
        controls.Gameplay.Special.started += a => {new Part(gameManager.weaponArchetypes.Keys.ToArray().ElementAt(Random.Range(0, gameManager.weaponArchetypes.Keys.Count)), Random.Range(1, 7)).Drop(100, transform);};
        controls.Gameplay.QuickSwap.started += a => {
            if (standingOn != null) {
                menuManager.OpenMenu("mech");
            }
        };
        controls.Gameplay.QuickSwap.canceled += a => {
            if (GameObject.Find("EditMech") != null) {
                menuManager.CloseMenus();
            }
        };
    }

    void OnEnable() {
        controls.Enable();
    }

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.Find("GameManager").GetComponent<gameManager>();
        menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();
        leftArm = new Part("arm", 6);
        leftArm.slotNums = 3;
        leftArm.slots.Add(new Part("Laser Sword", 6));
        leftArm.slots.Add(new Part("Laser Sword", 6));
        leftArm.slots.Add(new Part("Laser Sword", 6));
        leftArmObject = leftArm.Draw(transform.Find("LeftArmSlot").transform);
        rightArm = new Part("arm", 6);
        rightArm.slotNums = 3;
        rightArm.slots.Add(new Part("Laser Sword", 6));
        rightArm.slots.Add(new Part("Laser Sword", 6));
        rightArm.slots.Add(new Part("Laser Sword", 6));
        rightArmObject = rightArm.Draw(transform.Find("RightArmSlot").transform);
        Debug.Log(leftArm);
    }

    void FixedUpdate() {
        rb.MovePosition(transform.position + (new Vector3(move.ReadValue<Vector2>().x, move.ReadValue<Vector2>().y, 0) * speed)/50);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -8.5f, 8.5f), Mathf.Clamp(transform.position.y, -4.5f, 4.5f), 0f);
    }
    
    void Update(){
        this.attackSpeed = leftArm.attackSpeed + rightArm.attackSpeed;
        Vector2 lookDir = usingMouse ? (Vector2) (transform.position - Camera.main.ScreenToWorldPoint(rotateM.ReadValue<Vector2>())) : rotate.ReadValue<Vector2>();
        transform.rotation = Quaternion.AngleAxis((0 - Mathf.Atan2(lookDir.x, lookDir.y) * Mathf.Rad2Deg + (usingMouse ? 180 : 0)), Vector3.forward);
        if (shooting) {
            leftArm.Fire(this, "Player");
            rightArm.Fire(this, "Player");
        }
        if(health <= 0) {
            GameObject.Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        switch (other.gameObject.name) {
            case "nWall":
                gameManager.currentRoom = gameManager.currentRoom.exit("north").room;
                transform.position = new Vector3(transform.position.x, -3.25f, 0);
                break;
            case "eWall":
                gameManager.currentRoom = gameManager.currentRoom.exit("east").room;
                transform.position = new Vector3(-7.25f, transform.position.y, 0);
                break;
            case "sWall":
                gameManager.currentRoom = gameManager.currentRoom.exit("south").room;
                transform.position = new Vector3(transform.position.x, 3.25f, 0);
                break;
            case "wWall":
                gameManager.currentRoom = gameManager.currentRoom.exit("west").room;
                transform.position = new Vector3(7.25f, transform.position.y, 0);
                break;
            default:
                if (other.tag == "item") {
                    standingOn = other.gameObject.GetComponent<ItemDropController>().part;
                }
                break;
        }
        
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "item") {
            standingOn = null;
        }
    }

    public void Die() {
        if(health == 0){
            SceneManager.LoadScene(2);
        }
    }
}
