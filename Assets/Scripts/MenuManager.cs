using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
  protected gameManager gameManager;
  protected PlayerController playerController;
  protected Dictionary<string, GameObject> menus;

  void Start() {
    
    // gameManager = GameObject.Find("GameManager").GetComponent<gameManager>();
    // playerController = GameObject.Find("Player").GetComponent<PlayerController>();
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
      {"mech", GameObject.Find("Mech")},
      {"controls", GameObject.Find("Controls")},
    };
  }

  void Update() {
    normalisedMousePosition = new Vector2(Input.mousePosition.x - Screen.width/2, Input.mousePosition.y - Screen.height/2);
    currentAngle = Mathf.Atan2(normalisedMousePosition.y, normalisedMousePosition.x)*Mathf.Rad2Deg;

    currentAngle = (currentAngle+360)%360;

    selection = (int) currentAngle/45;
    if(selection!= previousSelection){
      previousMenuItemSc = menuItems[previousSelection].GetComponent<MenuItemScript>();
      previousMenuItemSc.Deselect();
      previousSelection = selection;

      menuItemSc = menuItems[selection].GetComponent<MenuItemScript>();
    }
    Debug.Log(currentAngle);

  }

  
  public void SelfDestruct() {
    playerController.Die();
  }

  public void CloseMenu() {
    foreach (string menu in menus.Keys) {
        menus[menu].SetActive(false);
    }
  }

  public void OpenMenu(string menu) {
    menus[menu].SetActive(true);
  }
}
