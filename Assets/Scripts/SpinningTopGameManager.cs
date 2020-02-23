using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class SpinningTopGameManager : MonoBehaviourPunCallbacks {

  [Header ("UI")]
  public GameObject ui_InformPanelGameObject;
  public TextMeshProUGUI ui_InformText;
  public GameObject searchForGamesButtonGameObject;
  // Start is called before the first frame update
  void Start () {
    ui_InformPanelGameObject.SetActive (true);
    ui_InformText.text = "Searching for games to battle...";
  }

  // Update is called once per frame
  void Update () {

  }

  #region UI Callbacks
  public void JoinRandomRoom () {
    ui_InformText.text = "Searching for available rooms...";
    PhotonNetwork.JoinRandomRoom ();
    searchForGamesButtonGameObject.SetActive (false);
  }

  public void OnQuitMatchButtonClicked () {
    if (PhotonNetwork.InRoom) {
      PhotonNetwork.LeaveRoom ();
    } else {
      SceneSwitcher.Instance.LoadScene ("Scene_Lobby");
    }
  }

  #endregion

  #region PHOTON Callback Methods
  // TODO - What is short type?
  public override void OnJoinRandomFailed (short returnCode, string message) {
    // Equivalent to ruby super
    // base.OnJoinRandomFailed (returnCode, message);
    ui_InformText.text = message;
    CreateAndJoinRoom ();
  }

  public override void OnJoinedRoom () {
    base.OnJoinedRoom ();
    // You're the first in the room
    if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
      ui_InformText.text = "joined to" + PhotonNetwork.CurrentRoom.Name + ". Waiting for other players...";
      // Second player, game is about to start
    } else {
      ui_InformText.text = "joined to" + PhotonNetwork.CurrentRoom.Name;
      // This removes the messaging UI as the game is about to start
      StartCoroutine (DeactivateAfterSeconds (ui_InformPanelGameObject, 2.0f));
    }
    Debug.Log ("joined to" + PhotonNetwork.CurrentRoom.Name);
  }

  public override void OnPlayerEnteredRoom (Player newPlayer) {
    base.OnPlayerEnteredRoom (newPlayer);
    ui_InformText.text = newPlayer.NickName + " joined to" + PhotonNetwork.CurrentRoom.Name + " - Player Count:" + PhotonNetwork.CurrentRoom.PlayerCount;

    // This removes the messaging UI as the game is about to start
    StartCoroutine (DeactivateAfterSeconds (ui_InformPanelGameObject, 2.0f));
  }

  public override void OnLeftRoom () {
    SceneSwitcher.Instance.LoadScene ("Scene_Lobby");
  }
  #endregion

  #region Private methods

  private void CreateAndJoinRoom () {
    string randomRoomName = "Room" + Random.Range (0, 10000);
    RoomOptions options = new RoomOptions ();

    options.MaxPlayers = 2;
    PhotonNetwork.CreateRoom (randomRoomName, options);
  }

  // This coroutine allows code to trigger within a given set of seconds
  IEnumerator DeactivateAfterSeconds (GameObject _gameObject, float _seconds) {
    yield return new WaitForSeconds (_seconds);
    _gameObject.SetActive (false);
  }
  #endregion

}
