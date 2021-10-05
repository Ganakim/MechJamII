using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {
    public float damage;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Enemy" && gameObject.tag == "PlayerProjectile") {
            other.GetComponent<EnemyController>().health -= damage;
        }
        if (other.gameObject.tag == "Player" && gameObject.tag == "EnemyProjectile") {
            other.GetComponent<PlayerController>().health -= damage;
        }
    }
}
