using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleScript : MonoBehaviour {

  Rigidbody deviceRigidBody;
  public Spinner spinnerScript;
  private float startSpinSpeed;
  private float currentSpinSpeed;

  public TextMeshProUGUI spinSpeedBar_Text;
  public Image spinSpeedBar_Image;

  // This is loaded before the script start (kinda like an initializer)
  private void Awake () {
    startSpinSpeed = spinnerScript.spinSpeed;
    currentSpinSpeed = spinnerScript.spinSpeed;

    spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
    deviceRigidBody = GetComponent<Rigidbody> ();
  }

  // TODO - Change text form spinner using TMP
  //

  private void OnCollisionEnter (Collision collision) {
    if (collision.gameObject.CompareTag ("Player")) {
      Rigidbody opponent = collision.collider.gameObject.GetComponent<Rigidbody> ();

      if (opponent.velocity.magnitude > deviceRigidBody.velocity.magnitude) {
        float mySpeed = deviceRigidBody.velocity.magnitude;
        float opponentSpeed = opponent.velocity.magnitude;

        Debug.Log ("My Speed:" + mySpeed + "----- Opponent Speed:" + opponentSpeed);

        if (mySpeed > opponentSpeed) {
          Debug.Log ("You dameged other player");
        } else {
          Debug.Log ("You got damaged!!");
        }
      }
    }

  }
}
