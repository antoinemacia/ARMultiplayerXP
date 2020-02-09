using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectionManager : MonoBehaviour {

  public int playerSelectionNumber;
  public GameObject[] spinnerTopModels;

  #region Unity Methods
  // Start is called before the first frame update
  void Start () {
    playerSelectionNumber = 1;
  }

  // Update is called once per frame
  void Update () {

  }
  #endregion

  #region UI Callback Methods

  public Transform playerSwitcherTransform;
  float degrees = 90;
  float secondsToTurn = 1.0f;

  public Button nextButton;
  public void NextPlayer () {
    // Disable buttons to avoid repetition
    nextButton.enabled = false;
    prevButton.enabled = false;
    // Change number selection
    selectNextPlayer ();
    // Vector3.up is shorthand for Vector3(0, 1, 0).
    StartCoroutine (Rotate (Vector3.up, playerSwitcherTransform, degrees, secondsToTurn));
  }

  public Button prevButton;
  public void PreviousPlayer () {
    // Disable buttons to avoid repetition
    nextButton.enabled = false;
    prevButton.enabled = false;
    // Change number selection
    selectPrevPlayer ();
    // Vector3.up is shorthand for Vector3(0, 1, 0).
    StartCoroutine (Rotate (Vector3.up, playerSwitcherTransform, -degrees, secondsToTurn));
  }

  public Button selectButton;
  public void OnSelectButtonClicked () {
    ExitGames.Client.Photon.Hashtable playerSeletionProp = new ExitGames.Client.Photon.Hashtable { { MultiplayerARSpinnerTopGame.PLAYER_SELECTION_NUMBER, playerSelectionNumber - 1 } };
    PhotonNetwork.LocalPlayer.SetCustomProperties (playerSeletionProp);
  }

  #endregion

  #region Private Methods

  IEnumerator Rotate (Vector3 axis, Transform transformToRotate, float angle, float duration = 1.0f) {
    Quaternion originalRotation = transformToRotate.rotation;
    // This is how you rotate a vector (x, y, z) in Unity
    Quaternion finalRotation = transformToRotate.rotation * Quaternion.Euler (axis * angle);

    float elapsedTime = 0.0f;

    // This loop allows a slow rotation of the object (as oppose to immediate)
    while (elapsedTime < duration) {
      // Slerp slowly rotate an object from a rotation to another rotation by a given amount
      transformToRotate.rotation = Quaternion.Slerp (originalRotation, finalRotation, elapsedTime / duration);
      elapsedTime += Time.deltaTime;
      yield return null;
    }

    transformToRotate.rotation = finalRotation;
    nextButton.enabled = true;
    prevButton.enabled = true;
  }

  private void selectNextPlayer () {
    if (playerSelectionNumber == spinnerTopModels.Length) {
      playerSelectionNumber = 1;
    } else {
      playerSelectionNumber += 1;
    }
  }

  private void selectPrevPlayer () {
    if (playerSelectionNumber == 1) {
      playerSelectionNumber = spinnerTopModels.Length;
    } else {
      playerSelectionNumber -= 1;
    }
  }

  #endregion
}
