using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleScript : MonoBehaviour {

  Rigidbody deviceRigidBody;

  // This is loaded before the script start (kinda like an initializer)
  private void Awake () {
    deviceRigidBody = GetComponent<Rigidbody> ();
  }

  void OnCollisionEnter (Collision collision) {
    if (collision.rigidbody) {
      Rigidbody opponent = collision.rigidbody;

      if (opponent.velocity.magnitude > deviceRigidBody.velocity.magnitude) {
        Debug.Log ("Collision!!!!");
      }
    }

  }
}
