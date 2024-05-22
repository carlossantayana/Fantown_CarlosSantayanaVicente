using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Vector2 _movement;
    Transform _playerTransform;

    Animator _playerAnimator;

    float _walkSpeed = 1f;
    float _runSpeedMultiplier = 1.0f;
    float _rotSpeed = 270f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _playerTransform = transform;
        _playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _playerAnimator.SetBool("running", true);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _playerAnimator.SetBool("running", false);
        }
    }

    private void FixedUpdate()
    {
        _playerTransform.Translate(Vector3.forward * (_movement.y * _walkSpeed * _runSpeedMultiplier * Time.fixedDeltaTime));
        _playerTransform.Rotate(Vector3.up * (_movement.x * _rotSpeed * Time.fixedDeltaTime));
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
        if (_movement.y != 0)
        {
            _playerAnimator.SetBool("walking", true);
        }
        else
        {
            _playerAnimator.SetBool("walking", false);
        }
    }
}
