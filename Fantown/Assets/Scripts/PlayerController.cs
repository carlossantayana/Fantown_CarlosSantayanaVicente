using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Vector2 _movement;
    Transform _playerTransform;

    AudioSource _audioSource;

    Animator _playerAnimator;
    GameObject _playerSword;
    Animator _swordAnimator;

    float _walkSpeed = 1.25f;
    float _runSpeedMultiplier = 1.0f;
    float _rotSpeed = 270f;

    bool startedBlocking;
    bool blocking;
    bool isWalking;
    bool isRunning;
    bool isTalking;
    bool isPunching;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _playerTransform = transform;
        _playerAnimator = GetComponent<Animator>();

        _audioSource = GetComponent<AudioSource>();

        _playerSword = transform.Find("ESPADA").gameObject;
        _playerSword.SetActive(false);
        _swordAnimator = _playerSword.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !blocking && !isPunching)
        {
            isTalking = true;
            _playerAnimator.SetTrigger("talk");
            _audioSource.Play();
        }

        if (Input.GetKeyDown(KeyCode.F) && !blocking && !isTalking)
        {
            isPunching = true;
            _playerAnimator.SetTrigger("punch");
        }

        startedBlocking = Input.GetKeyDown(KeyCode.Mouse1) && !isTalking && !isPunching;
        blocking = Input.GetKey(KeyCode.Mouse1);

        isWalking = (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.W)) && !startedBlocking && !blocking && !isTalking && !isPunching;
        isRunning = Input.GetKey(KeyCode.LeftShift) && isWalking;

        if (startedBlocking)
        {
            _playerSword.SetActive(true);
        }

        if (isRunning)
        {
            _runSpeedMultiplier = 4.0f;
        }
        else
        {
            _runSpeedMultiplier = 1.0f;
        }

        _swordAnimator.SetBool("startedBlocking", startedBlocking);
        _playerAnimator.SetBool("startedBlocking", startedBlocking);
        _swordAnimator.SetBool("blocking", blocking);
        _playerAnimator.SetBool("blocking", blocking);
        _playerAnimator.SetBool("walking", isWalking);
        _playerAnimator.SetBool("running", isRunning);

        if (!blocking)
        {
            _playerSword.SetActive(false);
        }

        if (isTalking && CheckAnimationEnd("Talk"))
        {
            isTalking = false;
        }

        if (isPunching && CheckAnimationEnd("Punch"))
        {
            isPunching = false;
        }
    }

    private void FixedUpdate()
    {
        if (isWalking)
        {
            _playerTransform.Translate(Vector3.forward * (_movement.y * _walkSpeed * _runSpeedMultiplier * Time.fixedDeltaTime));
            _playerTransform.Rotate(Vector3.up * (_movement.x * _rotSpeed * Time.fixedDeltaTime));
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
    }

    bool CheckAnimationEnd(string anim)
    {
        AnimatorStateInfo stateInfo = _playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName(anim))
        {
            if (stateInfo.normalizedTime >= 0.99f)
            {
                return true;
            }
        }

        return false;
    }
}
