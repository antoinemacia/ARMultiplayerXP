using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks {

  [Header ("Login UI")]
  public InputField playerNameInputField;

  #region Unity Methods
  // Start is called before the first frame update
  void Start () {

  }

  // Update is called once per frame
  void Update () {

  }
  #endregion

  #region UI Callback Methods
  public void OnEnterGameButtonClicked () {
    string playerName = playerNameInputField.text;

    if (!string.IsNullOrEmpty (playerName)) {
      if (!PhotonNetwork.IsConnected) {
        PhotonNetwork.LocalPlayer.NickName = playerName;
        PhotonNetwork.ConnectUsingSettings ();
      }
    } else {
      Debug.Log ("Player name is invalid or empty!");
    }
  }
  #endregion

  #region Photon Callback Methods
  public override void OnConnected () {
    Debug.Log ("Connected to internet");
  }

  public override void OnConnectedToMaster () {
    Debug.Log (PhotonNetwork.LocalPlayer.NickName + " is connected to Photon");
  }
  #endregion

}
