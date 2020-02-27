using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SpawnManager : MonoBehaviourPunCallbacks {

  public GameObject[] playerPrefabs;
  public Transform[] spawnPositions;

  public enum RaiseEventCodes {
    PlayerSpawnEventCode = 0
  }

  // Start is called before the first frame update
  void Start () {

  }

  // Update is called once per frame
  void Update () {

  }

  #region Photon Callback Methods
  public override void OnJoinedRoom () {

    if (PhotonNetwork.IsConnectedAndReady) {
      //   object playerSelectionNumber;
      //   // TODO: Check what is keyword out in C#
      //   // TODO: Check what is type object in C#
      //   // TODO: Check Quartenion.Identity
      //   if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue (MultiplayerARSpinnerTopGame.PLAYER_SELECTION_NUMBER, out playerSelectionNumber)) {
      //     int randomSpawnPoint = Random.Range (0, spawnPositions.Length - 1);
      //     Vector3 instantiatePosition = spawnPositions[randomSpawnPoint].position;
      //     // This is how you cast a type
      //     PhotonNetwork.Instantiate (playerPrefabs[(int) playerSelectionNumber - 1].name, instantiatePosition, Quaternion.identity);
      //   }

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

  private void BrodcastLocation (GameObject playerGameObject, PhotonView _photonView, object playerSelectionNumber) {
    object[] data = new object[] {
      playerGameObject.transform.position,
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
