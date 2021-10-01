using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour{
    protected Rigidbody rb;
    public int damage;
    public int attackSpeed;
    public int health;
    public int speed;
    public GameObject Projectile;
    private float cooldown = 0f;

    void Start(){
        rb = GetComponent<Rigidbody>();
    }
    
    void Update(){
        rb.MovePosition(new Vector3(Input.GetAxis("LeftStickHorizontal") * Time.deltaTime, 0, Input.GetAxis("LeftStickVertical") * Time.deltaTime));
        transform.rotation = Quaternion.Euler(new Vector3(Input.GetAxis("RightStickHorizontal"), 0, Input.GetAxis("RightStickVertical")));
        cooldown -= Time.deltaTime;
        if (Input.GetButton("Fire1") && cooldown <= 0f) {
            cooldown = attackSpeed;
            Instantiate(Projectile, rb.position, rb.rotation);
        }
    }
    public void Die() {
        if(health == 0){
            SceneManager.LoadScene(2);
        }
    }
}
