using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Joystick joystick;
    public float speed = 10;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      // Taking the joystick inputs
      float _xMovementInput = joystick.Horizontal;
      float _zMovementInput = joystick.Vertical;

      // Calculating velocity vectors
      Vector3 _MovementHorizontal = transform.right * _xMovementInput;
      Vector3 _MovementVertical = transform.forward * _zMovementInput;

      // Calculate the final movement velocity vector
      Vector3 _MovementVelocityVector = (_MovementHorizontal + _MovementVertical).normalized * speed;

      // Apply Movement
    }
}
