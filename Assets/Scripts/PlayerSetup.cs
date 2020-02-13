using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// ----------------------------------------------------------------
// This script ensures the setup of the game is for local gaming only
// ----------------------------------------------------------------
public class PlayerSetup : MonoBehaviourPun {
  // Start is called before the first frame update
  void Start () {
    if (photonView.IsMine) {
      // This means this player is my player
      // NOTE - the bellow object can be accessed this way since it lives on the same GameObject
      transform.GetComponent<MovementController> ().enabled = true;
      transform.GetComponent<MovementController> ().joystick.gameObject.SetActive (true);
    } else {
      // Player is other players (remote)
      transform.GetComponent<MovementController> ().enabled = false;
      transform.GetComponent<MovementController> ().joystick.gameObject.SetActive (false);
    }
  }

  // Update is called once per frame
  void Update () {

  }
}
