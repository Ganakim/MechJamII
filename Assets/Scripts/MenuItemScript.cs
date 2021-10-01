using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomClasses;

public class MenuItemScript : MonoBehaviour
{
    public Color hoverColor;
    public Color baseColor;
    public Image background;
    private Part part;
    // Start is called before the first frame update
    void Start()
    {
        background.color = baseColor;
    }

    // Update is called once per frame
    
    public void Select() {
        background.color = hoverColor;
    }

    public void Deselect() {
        background.color = baseColor;
    }

}
