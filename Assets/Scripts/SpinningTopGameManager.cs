using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SpinningTopGameManager : MonoBehaviourPunCallbacks {
  // Start is called before the first frame update
  void Start () {

  }

  // Update is called once per frame
  void Update () {

  }

  #region UI Callbacks
  public void JoinRandomRoom () {
    PhotonNetwork.JoinRandomRoom ();

  }
  #endregion

  #region PHOTON Callback Methods
  // TODO - What is short type?
  public override void OnJoinRandomFailed (short returnCode, string message) {
    // Equivalent to ruby super
    // base.OnJoinRandomFailed (returnCode, message);
    Debug.Log (message);
    CreateAndJoinRoom ();
  }

  public override void OnJoinedRoom () {
    base.OnJoinedRoom ();
    Debug.Log (PhotonNetwork.NickName + " joined to" + PhotonNetwork.CurrentRoom.Name);
  }

  public override void OnPlayerEnteredRoom (Player newPlayer) {
    base.OnPlayerEnteredRoom (newPlayer);
    Debug.Log (newPlayer.NickName + " joined to" + PhotonNetwork.CurrentRoom.Name + " - Player Count:" + PhotonNetwork.CurrentRoom.PlayerCount);
  }
  #endregion

  #region Private methods

  private void CreateAndJoinRoom () {
    string randomRoomName = "Room" + Random.Range (0, 10000);
    RoomOptions options = new RoomOptions ();

    options.MaxPlayers = 2;
    PhotonNetwork.CreateRoom (randomRoomName, options);
  }
  #endregion

}
