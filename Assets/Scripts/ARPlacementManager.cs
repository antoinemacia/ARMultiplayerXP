using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacementManager : MonoBehaviour {

  ARRaycastManager m_ARRaycastManager;
  // TODO - Check what is the keyword static;
  static List<ARRaycastHit> raycast_Hits = new List<ARRaycastHit> ();

  public Camera arCamera;

  public GameObject battleArenaGameObject;

  private void Awake () {
    m_ARRaycastManager = GetComponent<ARRaycastManager> ();
  }

  // Start is called before the first frame update
  void Start () { }

  // Update is called once per frame
  void Update () {

    // Raycast from the center of the screen
    Vector3 centerOfScreen = new Vector3 (Screen.width / 2, Screen.height / 2);
    Ray ray = arCamera.ScreenPointToRay (centerOfScreen);

    // Means a ray is sent and has detected a plane
    if (m_ARRaycastManager.Raycast (ray, raycast_Hits, TrackableType.PlaneWithinPolygon)) {
      // Intersection!
      // First hit is always the closest
      Pose hitPose = raycast_Hits[0].pose;

      Vector3 positionToReplace = hitPose.position;

      battleArenaGameObject.transform.position = positionToReplace;
    }
  }
}
