using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
  protected gameManager gameManager;
  protected PlayerController playerController;
  protected Dictionary<string, GameObject> menus;

  void Start() {
    gameManager = GameObject.Find("GameManager").GetComponent<gameManager>();
    playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    menus = new Dictionary<string, GameObject> {
      {"map", GameObject.Find("Map")},
      {"stats", GameObject.Find("Stats")},
      {"mech", GameObject.Find("Mech")},
      {"controls", GameObject.Find("Controls")},
    };
  }

  void Update() {

  }

  public void StartGame() {
    SceneManager.LoadScene(2);
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
