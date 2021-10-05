using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomClasses;

public class ItemDropController : MonoBehaviour {
    public Part part;

    void FixedUpdate() {
        transform.Rotate(0f, 0f, 2f);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -7.5f, 7.5f), Mathf.Clamp(transform.position.y, -3.5f, 3.5f), 0f);
    }

    void OnTriggerStay2D(Collider2D other) {
        if (other.tag == "item") {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0));
        }
    }
}
