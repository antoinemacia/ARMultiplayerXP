using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LobbyManager : MonoBehaviourPunCallbacks {

  [Header ("Login UI")]
  public InputField playerNameInputField;
  public GameObject ui_LoginGameObject;

  [Header ("Lobby UI")]
  public GameObject ui_LobbyGameObject;
  // The Spinner in the lobby scene
  public GameObject ui_3DGameObject;

  [Header ("Connection Status UI")]
  public GameObject ui_ConnectionStatusGameObject;
  public Text connectionStatusText;
  public bool showConnectionStatus = false;

  #region Unity Methods
  // Start is called before the first frame update
  void Start () {
    // When app is loaded, Display the Login Screen (With nickname input)
    displayPlayerLogin ();
  }

  // Update is called once per frame
  void Update () {
    if (showConnectionStatus) {
      connectionStatusText.text = ("Connecting " + PhotonNetwork.NetworkClientState + " ...");
    }
  }
  #endregion

  #region UI Callback Methods

  public void OnClickQuickMatch () {
    // SceneManager.LoadScene ("Scene_Loading");
    SceneSwitcher.Instance.LoadScene ("Scene_PlayerSelection");
  }

  public void OnEnterGameButtonClicked () {

    string playerName = playerNameInputField.text;

    if (!string.IsNullOrEmpty (playerName)) {
      // Then, when player name is submitted, set loading panel

      showConnectionStatus = true;
      displayLoading ();
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
    // Finally, when player is connected, show lobby UI
    displayLobby ();
  }
  #endregion

  #region UI Callback Methods

  private void displayLobby () {
    ui_LobbyGameObject.SetActive (true);
    ui_3DGameObject.SetActive (true);
    ui_ConnectionStatusGameObject.SetActive (false);
    ui_LoginGameObject.SetActive (false);
  }

  private void displayLoading () {
    ui_LobbyGameObject.SetActive (false);
    ui_3DGameObject.SetActive (false);
    ui_ConnectionStatusGameObject.SetActive (true);
    ui_LoginGameObject.SetActive (false);
  }

  private void displayPlayerLogin () {
    ui_LobbyGameObject.SetActive (false);
    ui_3DGameObject.SetActive (false);
    ui_ConnectionStatusGameObject.SetActive (false);
    ui_LoginGameObject.SetActive (true);
  }

  #endregion
}
