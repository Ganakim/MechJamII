using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

  

  
  public gameManager gameManager;
  public PlayerController playerController;

  public GameObject mainMenuScene;
  public GameObject gameOverScene;
  public GameObject creditsScene;

  public GameObject mechScreen;
  public GameObject controlsScreen;
  public GameObject statsScreen;
  public GameObject mapScreen;

  public Vector2 normalisedMousePosition;
  public float currentAngle;
  public int selection;
  public int previousSelection;

  public GameObject[] menuItems;

  private MenuItemScript menuItemSc;
  private MenuItemScript previousMenuItemSc;

  public Button resumeButton;
  public Button selfDestructButton;
  public Button playButton;

  

  public int scrapMetal;

  void Start() {
    
    // gameManager = GameObject.Find("GameManager").GetComponent<gameManager>();
    // playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    // resumeButton = GetComponent<ResumeButton>();
    // resumeButton.onClick.AddListener(ResumeGame());
    // selfDestructButton = GetComponent<SelfDestructButton>();
    // selfDestructButton.onClick.AddListener(SelfDestruct());
    // playButton = GetComponent<PlayButton>();
    // playButton.onClick.AddListener(gameManager.StartGame());

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

  
}
