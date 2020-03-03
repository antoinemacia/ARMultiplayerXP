using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MySynchronisationScript : MonoBehaviour, IPunObservable {

  Rigidbody rb;
  PhotonView photonView;
  Vector3 networkPosition;
  Quaternion networkRotation;

  public bool synchronizeVelocity = true;
  public bool synchronizeAngularVelocity = true;
  public bool isTeleportEnabled = true;
  public float teleportIfDistanceGreaterThan = 1.0f;

  private float distance;
  private float angle;

  private GameObject battleArenaGameObject;
  // This is loaded before the script start (kinda like an initializer)
  private void Awake () {
    rb = GetComponent<Rigidbody> ();
    photonView = GetComponent<PhotonView> ();
    networkPosition = new Vector3 ();
    networkRotation = new Quaternion ();
    battleArenaGameObject = GameObject.Find ("BattleArena");
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
      // The more distance/angle * (1.0f / PhotonNetwork.SerializationRate) is high
      // The more the position will be refreshed
      rb.position = Vector3.MoveTowards (rb.position, networkPosition, distance * (1.0f / PhotonNetwork.SerializationRate));
      rb.rotation = Quaternion.RotateTowards (rb.rotation, networkRotation, angle * (1.0f / PhotonNetwork.SerializationRate));
    }
  }

  public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info) {
    // This stream is sending and receiving data to a PhotonView observable
    if (stream.IsWriting) {
      // If true, this means the photon view is running on my device and I'm the one to who
      // Controls this player. We should send our position & rotation data to the the "REMOTE ME"
      // Which is the version of my player on other peoples devices
      // NOTE - We extract our own battle arena position so that users can apply theirs.
      stream.SendNext (rb.position - battleArenaGameObject.transform.position);
      stream.SendNext (rb.rotation);

      if (synchronizeVelocity) {
        stream.SendNext (rb.velocity);
      }

      if (synchronizeAngularVelocity) {
        stream.SendNext (rb.angularVelocity);
      }
    } else {
      // If false, it means the stream is reading. Meaning we're listening to other players.
      // NOTE - We add our battle arena postion as offset to the other users position
      networkPosition = (Vector3) stream.ReceiveNext () + battleArenaGameObject.transform.position;
      networkRotation = (Quaternion) stream.ReceiveNext ();

      if (isTeleportEnabled) {
        // If the distance between local & network position is higher than
        // our teleport threshold, teleport!
        if (Vector3.Distance (rb.position, networkPosition) > teleportIfDistanceGreaterThan) {
          rb.position = networkPosition;
        }
      }

      // This segment aims to conpensate the lag caused by the network
      // See https://doc.photonengine.com/en-us/pun/v2/gameplay/lagcompensation
      if (synchronizeVelocity || synchronizeAngularVelocity) {
        // PhotonNetwork = Time accross all devices synced (server time)
        // SentServerTime = Time where information hads been sent
        float lag = Mathf.Abs ((float) (PhotonNetwork.Time - info.SentServerTime));

        if (synchronizeVelocity) {
          rb.velocity = (Vector3) stream.ReceiveNext ();
          // Note: The velocity is in meters per second
          // Timing it by the lag reduces the disrepency
          networkPosition += rb.velocity * lag;

          // This distance represent the difference between local position & network position
          distance = Vector3.Distance (rb.position, networkPosition);
        }

        if (synchronizeAngularVelocity) {
          rb.angularVelocity = (Vector3) stream.ReceiveNext ();

          networkRotation = Quaternion.Euler (rb.angularVelocity * lag) * networkRotation;

          angle = Quaternion.Angle (rb.rotation, networkRotation);
        }
      }
    }
  }
}
