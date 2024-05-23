/******************************************************************************
 * GRADO EN DISE�O Y DESARROLLO DE VIDEOJUEGOS - ANIMACI�N 3D
 * Pr�ctica II-B�sica 2-ejercicio 1
 * 
 * Clase Node.cs: Define las propiedades de los nodos (masas puntuales en los 
 * extremos de cada muelle)
 *****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace P2.Ejercicio2
{
    public class Node : MonoBehaviour
    {
        public float mass = 5f; // Masa del nodo (kg)
        public bool fixedNode;  // Indica si es un nodo fijo (true) o si puede
                                // moverse (false)
        public Vector3 pos; // Posici�n 3D del nodo
        public Vector3 vel; // Velocidad 3D del nodo
        public Vector3 force; // Fuerza 3D que sufre el nodo

        public float dAbsolute = 0.1f; //Factor de amortiguamiento absoluto de la velocidad del nodo

        // Use this for initialization
        public void Start()
        {
            pos = transform.position; // Establecemos en el instante inicial el
                                      // valor de la posici�n "pos" a partir de la
                                      // transformada position del gameobject
        }

        // Update is called once per frame
        void Update()
        {
            // El valor de "pos" se calcula en el script PhysicsManager seg�n el
            // m�todo de integraci�n. Aqu� establecemos la transformada posici�n
            // del gameobject para que coincida con la posici�n calculada
            transform.position = pos;
        }
    }
}
