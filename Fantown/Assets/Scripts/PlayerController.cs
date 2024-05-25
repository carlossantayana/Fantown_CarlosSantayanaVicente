using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Componente para gestionar el movimiento y las transiciones de las animaciones del personaje del jugador.
//Para la gesti�n del movimiento se utiliz� el paquete InputSystem.
public class PlayerController : MonoBehaviour
{
    Vector2 _movement; //Vector2 con el movimiento en "x" y en "y", utilizado por el InputSystem para trasladar y rotar al personaje.
    Transform _playerTransform; //Transformada del jugador para mover al personaje.

    AudioSource _audioSource; //Audio para la animaci�n de la frase.

    Animator _playerAnimator; //Controlador de animaciones del personaje del jugador.
    GameObject _playerSword; //Espada del jugador.
    Animator _swordAnimator; //Controlador de animaciones de la espada del jugador.

    float _walkSpeed = 1.25f; //Velocidad del jugador caminando.
    float _runSpeedMultiplier = 1.0f; //Multiplicador de velocidad al correr. Si no est� corriendo su valor es 1.
    float _rotSpeed = 270f; //Velocidad a la que rota el personaje en grados por segundo.

    //Booleanos de control de las animaciones.
    bool startedBlocking;
    bool blocking;
    bool isWalking;
    bool isRunning;
    bool isTalking;
    bool isPunching;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //Se captura el cursor.

        _playerTransform = transform;
        _playerAnimator = GetComponent<Animator>();

        _audioSource = GetComponent<AudioSource>();

        _playerSword = transform.Find("ESPADA").gameObject;
        _playerSword.SetActive(false); //Se desactiva la espada hasta que se vaya a realizar su animaci�n.
        _swordAnimator = _playerSword.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //GESTI�N DE LAS ANIMACIONES

        //FRASE
        if (Input.GetKeyDown(KeyCode.T) && !blocking && !isPunching)
        {
            isTalking = true;
            _playerAnimator.SetTrigger("talk");
            _audioSource.Play();
        }

        //MOCAP
        if (Input.GetKeyDown(KeyCode.F) && !blocking && !isTalking)
        {
            isPunching = true;
            _playerAnimator.SetTrigger("punch");
        }

        //ARMA
        startedBlocking = Input.GetKeyDown(KeyCode.Mouse1) && !isTalking && !isPunching;
        blocking = Input.GetKey(KeyCode.Mouse1);

        //ANDAR Y CORRER
        isWalking = (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.W)) && !startedBlocking && !blocking && !isTalking && !isPunching;
        isRunning = Input.GetKey(KeyCode.LeftShift) && isWalking;

        //Activaci�n de la espada en caso de comenzar a bloquear.
        if (startedBlocking)
        {
            _playerSword.SetActive(true);
        }

        //Modificaci�n del multiplicador de velocidad en caso de estar corriendo.
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

        //Desactivaci�n de la espada en caso de dejar de bloquear.
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
        //GESTI�N DEL MOVIMIENTO

        if (isWalking)
        {
            _playerTransform.Translate(Vector3.forward * (_movement.y * _walkSpeed * _runSpeedMultiplier * Time.fixedDeltaTime));
            _playerTransform.Rotate(Vector3.up * (_movement.x * _rotSpeed * Time.fixedDeltaTime));
        }

        if (!startedBlocking && !blocking && !isTalking && !isPunching && !isWalking)
        {
            _playerTransform.Rotate(Vector3.up * (_movement.x * _rotSpeed * Time.fixedDeltaTime));
        }
    }

    //Callback o manejador de eventos para el InputSystem.
    public void OnMove(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
    }

    //M�todo que se encarga de comprobar si una animaci�n en concreto se encuentra en curso y si est� a punto de terminar.
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
