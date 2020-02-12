using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SpawnManager : MonoBehaviourPunCallbacks {

  public GameObject[] playerPrefabs;
  public Transform[] spawnPositions;
  // Start is called before the first frame update
  void Start () {

  }

  // Update is called once per frame
  void Update () {

  }

  #region Photon Callback Methods
  public override void OnJoinedRoom () {

    if (PhotonNetwork.IsConnectedAndReady) {
      object playerSelectionNumber;
      // TODO: Check what is keyword out in C#
      // TODO: Check what is type object in C#
      // TODO: Check Quartenion.Identity
      if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue (MultiplayerARSpinnerTopGame.PLAYER_SELECTION_NUMBER, out playerSelectionNumber)) {
        int randomSpawnPoint = Random.Range (0, spawnPositions.Length - 1);
        Vector3 instantiatePosition = spawnPositions[randomSpawnPoint].position;
        // This is how you cast a type
        PhotonNetwork.Instantiate (playerPrefabs[(int) playerSelectionNumber - 1].name, instantiatePosition, Quaternion.identity);
      }

    }
  }
  #endregion
}
