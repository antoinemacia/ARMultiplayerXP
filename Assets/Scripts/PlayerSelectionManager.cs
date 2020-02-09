using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectionManager : MonoBehaviour {

  #region Unity Methods
  // Start is called before the first frame update
  void Start () {

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
    nextButton.enabled = false;
    prevButton.enabled = false;
    // Vector3.up is shorthand for Vector3(0, 1, 0).
    StartCoroutine (Rotate (Vector3.up, playerSwitcherTransform, degrees, secondsToTurn));
  }

  public Button prevButton;
  public void PreviousPlayer () {
    nextButton.enabled = false;
    prevButton.enabled = false;
    // Vector3.up is shorthand for Vector3(0, 1, 0).
    StartCoroutine (Rotate (Vector3.up, playerSwitcherTransform, -degrees, secondsToTurn));
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

  #endregion
}
