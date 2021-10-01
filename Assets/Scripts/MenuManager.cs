using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour {
  protected gameManager gameManager;
  protected PlayerController playerController;
  protected Dictionary<string, GameObject> menus;
  protected PlayerControls controls;
  public Dictionary<int, GameObject> menuItems = new Dictionary<int, GameObject>();
  private MenuItemScript previousMenuItemSc;
  private MenuItemScript menuItemSc;
  int previousSelection;

  void Awake() {
    controls = new PlayerControls();
  }

  void Start() {
    
    // resumeButton = GetComponent<ResumeButton>();
    // resumeButton.onClick.AddListener(ResumeGame());
    // selfDestructButton = GetComponent<SelfDestructButton>();
    // selfDestructButton.onClick.AddListener(SelfDestruct());
    // playButton = GetComponent<PlayButton>();
    // playButton.onClick.AddListener(gameManager.StartGame());

    gameManager = GameObject.Find("GameManager").GetComponent<gameManager>();
    playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    menus = new Dictionary<string, GameObject>{
      {"map", GameObject.Find("Map")},
      {"stats", GameObject.Find("Stats")},
      {"mech", GameObject.Find("EditMech")},
      {"controls", GameObject.Find("Controls")},
    };
    foreach (var menu in menus.Values)
    {
      menu.SetActive(false);
    }
  }

  void Update() {
    // Vector2 realPos = Camera.main.ScreenToWorldPoint(controls.Gameplay.RotateM.ReadValue<Vector2>());
    // Vector2 normalisedMousePosition = new Vector2(realPos.x - Screen.width/2, realPos.y - Screen.height/2);
    // float currentAngle = Mathf.Atan2(normalisedMousePosition.y, normalisedMousePosition.x)*Mathf.Rad2Deg;

    // currentAngle = (currentAngle+360)%360;

    // int selection = (int) currentAngle/45;
    // if(selection!= previousSelection){
    //   previousMenuItemSc = menuItems[previousSelection].GetComponent<MenuItemScript>();
    //   previousMenuItemSc.Deselect();
    //   previousSelection = selection;

    //   menuItemSc = menuItems[selection].GetComponent<MenuItemScript>();
    // }
    // Debug.Log(currentAngle);

  }

  
  public void SelfDestruct() {
    playerController.Die();
  }

  public void CloseMenus() {
    foreach (string menu in menus.Keys) {
        menus[menu].SetActive(false);
    }
  }

  public void OpenMenu(string menu) {
    if (menus[menu].activeSelf) {
      CloseMenus();
    } else {
      CloseMenus();
      menus[menu].SetActive(true);
    }
  }
}
