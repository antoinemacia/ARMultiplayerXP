using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour {
  public Joystick joystick;
  public float speed = 2f;
  public float maxVelocityChange = 10f;
  public float tiltAmount = 10f;
  private Rigidbody body;
  // Start is called before the first frame update
  void Start () {
    body = GetComponent<Rigidbody> ();
  }

  // Update is called once per frame
  void Update () {
    // Taking the joystick inputs

    float _xMovementInput = joystick.Horizontal;
    float _zMovementInput = joystick.Vertical;

    // Calculating velocity vectors
    Vector3 _MovementHorizontal = transform.right * _xMovementInput;
    Vector3 _MovementVertical = transform.forward * _zMovementInput;

    // Calculate the final movement velocity vector
    Vector3 _MovementVelocityVector = (_MovementHorizontal + _MovementVertical).normalized * speed;

    // Apply Movement
    Move (_MovementVelocityVector);

    // Apply Tilt/Rotation
    ApplyTiltEffect ();
  }

  private Vector3 velocityVector = Vector3.zero; // initial velocity

  void Move (Vector3 movementVelocityVector) {
    velocityVector = movementVelocityVector;
  }

  // FixedUpdate is called every fixed frame-rate frame

  // Use FixedUpdate when using Rigidbody.
  // Set a force to a Rigidbody and it applies each fixed frame.
  // FixedUpdate occurs at a measured time step that typically does not coincide with MonoBehaviour.Update.
  // https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html

  // Use FixedUpdate when
  private void FixedUpdate () {
    if (velocityVector != Vector3.zero) {
      // Get rigidbody current velocity
      Vector3 velocity = body.velocity;
      Vector3 velocityChange = velocityVector - velocity;

      // To avoid infinite velocity change, we "clamp" the value of x
      // (which basically restrict the x & z values between some min and max values)
      //  https://docs.unity3d.com/ScriptReference/Mathf.Clamp.html
      velocityChange.x = Mathf.Clamp (velocityChange.x, -maxVelocityChange, maxVelocityChange);
      velocityChange.z = Mathf.Clamp (velocityChange.z, -maxVelocityChange, maxVelocityChange);
      velocityChange.y = 0f;

      // Apply a force by the amount of velocity change to reach the target velocity
      body.AddForce (velocityChange, ForceMode.Acceleration);
    }
  }

  private float verticalTilt;
  private float horizontalTilt;
  // Quaternions are used to represent rotations.
  // https://docs.unity3d.com/ScriptReference/Quaternion.html
  // This line bellow adds a tilting rotation effect on the body when using the joystick
  private void ApplyTiltEffect () {
    verticalTilt = joystick.Vertical * speed * tiltAmount;
    horizontalTilt = -1 * joystick.Horizontal * speed * tiltAmount;
    transform.rotation = Quaternion.Euler (verticalTilt, 0, horizontalTilt);
  }
}
