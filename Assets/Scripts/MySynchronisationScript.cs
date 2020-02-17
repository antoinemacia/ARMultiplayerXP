using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MySynchronisationScript : MonoBehaviour, IPunObservable {

  Rigidbody rb;
  PhotonView photonView;
  Vector3 networkPosition;
  Quaternion networkRotation;

  // This is loaded before the script start (kinda like an initializer)
  private void Awake () {
    rb = GetComponent<Rigidbody> ();
    photonView = GetComponent<PhotonView> ();
    networkPosition = new Vector3 ();
    networkRotation = new Quaternion ();
  }

  // Start is called before the first frame update
  void Start () {

  }

  // Update is called once per frame
  void Update () {

  }

  // Here using FixedUodate since we're dealing with a RigidBody
  private void FixedUpdate () {

    if (!photonView.IsMine) {
      rb.position = Vector3.MoveTowards (rb.position, networkPosition, Time.fixedDeltaTime);
      rb.rotation = Quaternion.RotateTowards (rb.rotation, networkRotation, Time.fixedDeltaTime * 100);
    }
  }

  public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info) {
    // This stream is sending and receiving data to a PhotonView observable
    if (stream.IsWriting) {
      // If true, this means the photon view is running on my device and I'm the one to who
      // Controls this player. We should send our position & rotation data to the the "REMOTE ME"
      // Which is the version of my player on other peoples devices
      stream.SendNext (rb.position);
      stream.SendNext (rb.rotation);
    } else {
      // If false, it means the stream is reading. Meaning we're listening to other players.
      networkPosition = (Vector3) stream.ReceiveNext ();
      networkRotation = (Quaternion) stream.ReceiveNext ();
    }
  }
}
