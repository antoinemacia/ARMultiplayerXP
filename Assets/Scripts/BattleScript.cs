using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
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

      // Compare the speed of both players, the fastest makes the gamages
      if (opponent.velocity.magnitude > deviceRigidBody.velocity.magnitude) {
        float mySpeed = deviceRigidBody.velocity.magnitude;
        float opponentSpeed = opponent.velocity.magnitude;

        Debug.Log ("My Speed:" + mySpeed + "----- Opponent Speed:" + opponentSpeed);

        if (mySpeed > opponentSpeed) {
          // Apply damage to the slower player
          if (collision.collider.gameObject.GetComponent<PhotonView> ().IsMine) {
            float damagePerHit = 400f;
            // Here we use RPC calls so thatn they're broadcasted to all users
            collision.collider.gameObject.GetComponent<PhotonView> ().RPC ("DoDamage", RpcTarget.AllBuffered, damagePerHit);
          }
        } else {
          Debug.Log ("You got damaged!!");
        }
      }
    }
  }

  [PunRPC]
  public void DoDamage (float damageAmount) {
    spinnerScript.spinSpeed -= damageAmount;
    currentSpinSpeed = spinnerScript.spinSpeed;

    spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
    spinSpeedBar_Text.text = currentSpinSpeed + "/" + startSpinSpeed;
  }
}
