using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SpawnManager : MonoBehaviourPunCallbacks {

  public GameObject[] playerPrefabs;
  public Transform[] spawnPositions;
  public GameObject battleArenaGameObject;

  public enum RaiseEventCodes {
    PlayerSpawnEventCode = 0
  }

  void Start () {
    // Whenever an event rise, this event will be automatically called with the event data
    PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
  }

  // Update is called once per frame
  void Update () {

  }

  private void OnDestroy () {
    PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
  }

  #region Photon Callback Methods

  // Callback method to be called with event data
  void OnEvent (EventData photonEvent) {
    if (photonEvent.Code == (byte) RaiseEventCodes.PlayerSpawnEventCode) {
      // Store data from event into an array
      // This data corresponds to the data object sent in the bellow "BrodcastLocation" method
      object[] data = (object[]) photonEvent.CustomData;
      Vector3 receivedPosition = (Vector3) data[0];
      Quaternion receivedRotation = (Quaternion) data[1];
      int photonViewID = (int) data[2];
      int receivedPlayerSelectionData = (int) data[3];

      // Instantiate our player in remote people space!
      GameObject player = Instantiate (
        playerPrefabs[(int) receivedPlayerSelectionData],
        // NOTE - Adding the battle arena position now is crucial to place our player
        // in their space
        receivedPosition + battleArenaGameObject.transform.position,
        receivedRotation
      );

      PhotonView _photonView = player.GetComponent<PhotonView> ();
      _photonView.ViewID = photonViewID;
    }
  }

  public override void OnJoinedRoom () {
    if (PhotonNetwork.IsConnectedAndReady) {
      SpawnPlayer ();
    }
  }

  #endregion

  #region Private methods

  private void SpawnPlayer () {
    object playerSelectionNumber;
    // TODO: Check what is keyword out in C#
    // TODO: Check what is type object in C#
    // TODO: Check Quartenion.Identity
    if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue (MultiplayerARSpinnerTopGame.PLAYER_SELECTION_NUMBER, out playerSelectionNumber)) {

      int randomSpawnPoint = Random.Range (0, spawnPositions.Length - 1);

      Vector3 instantiatePosition = spawnPositions[randomSpawnPoint].position;

      // Instantiate player locally using random placement and prefab based on spinner ID
      GameObject playerGameObject = Instantiate (
        playerPrefabs[(int) playerSelectionNumber],
        instantiatePosition,
        Quaternion.identity
      );

      // Get the PhotonView of object (attached to all spinner prefabs)
      PhotonView _photonView = playerGameObject.GetComponent<PhotonView> ();

      if (PhotonNetwork.AllocateViewID (_photonView)) {
        BrodcastLocation (playerGameObject, _photonView, playerSelectionNumber);
      } else {
        Debug.Log ("failed to allocate a viewID");
        Destroy (playerGameObject);
      }
    }
  }

  // Send RPC call to remote players with spawn location
  private void BrodcastLocation (GameObject playerGameObject, PhotonView _photonView, object playerSelectionNumber) {
    object[] data = new object[] {
      // We don't need to broadcast our battle arena position to others users,
      // We therefore minus it from the playgerGameObjectLocation
      playerGameObject.transform.position - battleArenaGameObject.transform.position,
      playerGameObject.transform.rotation,
      _photonView.ViewID,
      playerSelectionNumber
    };

    RaiseEventOptions raisedEventOptions = new RaiseEventOptions {
      Receivers = ReceiverGroup.Others,
      CachingOption = EventCaching.AddToRoomCache
    };

    SendOptions sendOptions = new SendOptions {
      Reliability = true
    };

    // Send RPC call to the Photon Newtork with event type and position data
    PhotonNetwork.RaiseEvent ((byte) RaiseEventCodes.PlayerSpawnEventCode, data, raisedEventOptions, sendOptions);
  }

  #endregion

}
