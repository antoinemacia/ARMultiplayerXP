using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleScript : MonoBehaviourPun {

  // TODO - Refactor to classes for spinner types
  // BUG - Ensure spinners dont hit each others on start
  // BUG - Ensure attackers do not each to much life
  // BUG - Spin speed of all players is back to full on re-match even know old player has little life

  public GameObject ui_3D_GameObject;
  public GameObject deathPanelUIPrefab;
  private GameObject deathPanelUIGameObject;

  Rigidbody deviceRigidBody;
  public Spinner spinnerScript;
  private float startSpinSpeed;
  private float currentSpinSpeed;

  public TextMeshProUGUI spinSpeedBar_Text;
  public Image spinSpeedBar_Image;

  public bool isAttacker = false;
  public bool isDefender = false;
  private bool isDead = false;

  private int defaultSpinSpeed = 3600;

  [Header ("Player Type Damage Coefficients")]
  public float common_Damage_Coefficient = 0.04f;

  public float doDamage_Coefficient_Attacker = 3f; // do more damage than defender - ADVANTAGE
  public float getDamaged_Coefficient_Attacker = 0.8f; // gets more damage - DISADVANTAGE

  public float doDamage_Coefficient_Defender = 0.75f; // do less damage - DISADVANTAGE
  public float getDamaged_Coefficient_Defender = 0.2f; // gets less damage - ADVANTAGE

  public List<GameObject> pooledObjects;
  public int amountToPool = 8;
  public GameObject CollisionEffectPrefab;

  // This is loaded before the script start (kinda like an initializer)
  private void Awake () {
    startSpinSpeed = spinnerScript.spinSpeed;
    currentSpinSpeed = spinnerScript.spinSpeed;

    spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
    deviceRigidBody = GetComponent<Rigidbody> ();
  }

  // Start is called before the first frame update
  void Start () {
    CheckPlayerType ();

    // Add all visual effects and sounds into a pool of objects and de-activate them
    // on start
    if (photonView.IsMine) {
      pooledObjects = new List<GameObject> ();
      for (int i = 0; i < amountToPool; i++) {
        GameObject obj = (GameObject) Instantiate (CollisionEffectPrefab, Vector3.zero, Quaternion.identity);
        obj.SetActive (false);
        pooledObjects.Add (obj);
      }
    }

  }

  public GameObject GetPooledObject () {

    for (int i = 0; i < pooledObjects.Count; i++) {
      if (!pooledObjects[i].activeInHierarchy) {
        return pooledObjects[i];
      }
    }

    return null;
  }

  IEnumerator DeactivateAfterSeconds (GameObject _gameObject, float _seconds) {
    yield return new WaitForSeconds (_seconds);
    _gameObject.SetActive (false);
  }

  private void CheckPlayerType () {
    if (gameObject.name.Contains ("Attacker")) {
      isAttacker = true;
      isDefender = false;
    } else if (gameObject.name.Contains ("Defender")) {
      isAttacker = false;
      isDefender = true;

      // Bump life of defender to 4400
      spinnerScript.spinSpeed = 4400;
      startSpinSpeed = spinnerScript.spinSpeed;
      currentSpinSpeed = spinnerScript.spinSpeed;

      spinSpeedBar_Text.text = currentSpinSpeed + "/" + startSpinSpeed;
    }
  }

  private void OnCollisionEnter (Collision collision) {
    if (collision.gameObject.CompareTag ("Player")) {

      if (photonView.IsMine) {
        Vector3 effectPosition = (gameObject.transform.position + collision.transform.position) / 2 + new Vector3 (0, 0.05f, 0);

        //Instantiate Collision Effect ParticleSystem
        // TODO - Check GetPooledObject and Pooling
        GameObject collisionEffectGameobject = GetPooledObject ();
        if (collisionEffectGameobject != null) {
          collisionEffectGameobject.transform.position = effectPosition;
          collisionEffectGameobject.SetActive (true);
          collisionEffectGameobject.GetComponentInChildren<ParticleSystem> ().Play ();

          //De-activate Collision Effect Particle System after some seconds.
          StartCoroutine (DeactivateAfterSeconds (collisionEffectGameobject, 0.5f));

        }
      }

      Rigidbody opponent = collision.collider.gameObject.GetComponent<Rigidbody> ();

      // Compare the speed of both players, the fastest makes the gamages
      // NOTE : Velocity magnitude equals speed of the object
      float mySpeed = deviceRigidBody.velocity.magnitude;
      float opponentSpeed = opponent.velocity.magnitude;

      if (mySpeed > opponentSpeed) {
        float damageAmount = gameObject.GetComponent<Rigidbody> ().velocity.magnitude * defaultSpinSpeed * common_Damage_Coefficient;

        if (isAttacker) {
          damageAmount *= doDamage_Coefficient_Attacker;
        } else if (isDefender) {
          damageAmount *= doDamage_Coefficient_Defender;
        }

        // Apply damage to the slower player
        if (collision.collider.gameObject.GetComponent<PhotonView> ().IsMine) {
          // Here we use RPC calls so thatn they're broadcasted to all users
          collision.collider.gameObject.GetComponent<PhotonView> ().RPC ("DoDamage", RpcTarget.AllBuffered, damageAmount);
        }
      }
    }
  }

  [PunRPC]
  public void DoDamage (float damageAmount) {
    if (!isDead) {
      if (isAttacker) {
        damageAmount *= getDamaged_Coefficient_Attacker;
      } else if (isDefender) {
        damageAmount *= getDamaged_Coefficient_Defender;
      }

      if (currentSpinSpeed < 100) {
        Die ();
      }

      spinnerScript.spinSpeed -= damageAmount;
      currentSpinSpeed = spinnerScript.spinSpeed;

      spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
      spinSpeedBar_Text.text = currentSpinSpeed.ToString ("F0") + "/" + startSpinSpeed;
    }
  }

  public void Die () {
    isDead = true;
    // Cancel movement, remove all spinner physics
    GetComponent<MovementController> ().enabled = false;
    deviceRigidBody.freezeRotation = false;
    deviceRigidBody.velocity = Vector3.zero;
    deviceRigidBody.angularVelocity = Vector3.zero;

    spinnerScript.spinSpeed = 0f;

    ui_3D_GameObject.SetActive (false);

    if (photonView.IsMine) {
      //countdown for respawn
      StartCoroutine (ReSpawn ());
    }
  }

  IEnumerator ReSpawn () {
    // This will set the death modal in the canvas scene
    GameObject canvasGameObject = GameObject.Find ("Canvas");
    if (deathPanelUIGameObject == null) {
      deathPanelUIGameObject = Instantiate (deathPanelUIPrefab, canvasGameObject.transform);
    } else {
      deathPanelUIGameObject.SetActive (true);
    }

    // Add a 8 seconds respawn time
    Text respawnTimeText = deathPanelUIGameObject.transform.Find ("RespawnTimeText").GetComponent<Text> ();
    float respawnTime = 8.0f;

    respawnTimeText.text = respawnTime.ToString (".00");

    while (respawnTime > 0) {
      yield return new WaitForSeconds (1.0f);
      respawnTime -= 1.0f;
      respawnTimeText.text = respawnTime.ToString (".00");

      GetComponent<MovementController> ().enabled = false;
    }

    deathPanelUIGameObject.SetActive (false);

    GetComponent<MovementController> ().enabled = true;

    photonView.RPC ("ReBorn", RpcTarget.AllBuffered);
  }

  [PunRPC]
  public void ReBorn () {
    spinnerScript.spinSpeed = startSpinSpeed;

    currentSpinSpeed = spinnerScript.spinSpeed;

    spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
    spinSpeedBar_Text.text = currentSpinSpeed.ToString () + "/" + startSpinSpeed;

    deviceRigidBody.freezeRotation = true;
    transform.rotation = Quaternion.Euler (Vector3.zero);

    ui_3D_GameObject.SetActive (true);

    isDead = false;
  }
}
