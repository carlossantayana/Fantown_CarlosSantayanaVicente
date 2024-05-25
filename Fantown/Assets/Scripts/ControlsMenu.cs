using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Componente que se encarga de gestionar la lógica de activación y desactivación de la pantalla de controles.
public class ControlsMenu : MonoBehaviour
{
    public GameObject canvas;
    bool active = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            active = !active;
            canvas.SetActive(active);
        }
    }
}
