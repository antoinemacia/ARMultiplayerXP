using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARPlacementDetectionController : MonoBehaviour {

  public GameObject placeButton;
  public GameObject adjustButton;
  public GameObject searchForGameButton;
  public TextMeshProUGUI informUiPanelText;
  ARPlacementManager m_ARPlacementManager;
  ARPlaneManager m_ARPlaneManager;

  // Start is called before the first frame update
  private void Awake () {
    m_ARPlacementManager = GetComponent<ARPlacementManager> ();
    m_ARPlaneManager = GetComponent<ARPlaneManager> ();
  }

  // Start is called before the first frame update
  void Start () {
    placeButton.SetActive (true);
    adjustButton.SetActive (false);
    searchForGameButton.SetActive (false);
    informUiPanelText.text = "Move phone detect planes and place the arena";
  }

  public void DisableARPlacementAndPlaneDetection () {
    m_ARPlaneManager.enabled = false;
    m_ARPlacementManager.enabled = false;

    SetAllPlanesActiveOrDeactive (false);
    placeButton.SetActive (false);
    adjustButton.SetActive (true);
    searchForGameButton.SetActive (true);

    informUiPanelText.text = "ON YA! The arena is set... Now search for games to battle";

  }

  public void EnableARPlacementAndPlaneDetection () {
    m_ARPlaneManager.enabled = true;
    m_ARPlacementManager.enabled = true;

    SetAllPlanesActiveOrDeactive (true);
    adjustButton.SetActive (false);
    placeButton.SetActive (true);
    searchForGameButton.SetActive (false);
    
    informUiPanelText.text = "Move phone detect planes and place the arena";
  }

  private void SetAllPlanesActiveOrDeactive (bool value) {
    foreach (var plane in m_ARPlaneManager.trackables) {
      plane.gameObject.SetActive (value);
    }
  }
}
