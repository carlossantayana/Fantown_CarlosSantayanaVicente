using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntifallSystem : MonoBehaviour
{
    private Transform _playerTransform;
    private Vector3 _initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        _playerTransform = GameObject.Find("Player").transform;
        _initialPosition = _playerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            _playerTransform.position = _initialPosition;
        }
    }
}
