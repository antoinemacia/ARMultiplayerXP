using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour {
  public float spinSpeed = 3600;
  public bool doSpin = false;

  private Rigidbody rigidbody;
  public GameObject playerGraphics;
  // Start is called before the first frame update
  void Start () {

  }

  // Update is called once per frame
  void Update () {

  }

  private void FixedUpdate () {
    if (doSpin) {
      spin ();
    }
  }

  private void spin () {
    Vector3 _spinning = new Vector3 (0, spinSpeed * Time.deltaTime, 0);
    playerGraphics.transform.Rotate (_spinning);
  }
}
