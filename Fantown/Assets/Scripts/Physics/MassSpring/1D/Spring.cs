/******************************************************************************
 * GRADO EN DISE�O Y DESARROLLO DE VIDEOJUEGOS - ANIMACI�N 3D
 * Pr�ctica II-B�sica 2-ejercicio 1
 * 
 * Clase Spring.cs: Define las propiedades de los muelles el�sticos
 *****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace P2.Ejercicio2
{
    public class Spring : MonoBehaviour
    {
        public float k = 100f; // Constante de rigidez del muelle (N/m)
        public float length0; // Longitud natural del muelle (ah� la fuerza el�stica
                              // se anula)
        public float length; // Longitud del muelle en un momento dado
        public Vector3 pos; // Posici�n 3D del punto medio del muelle
        public Vector3 dir; // Vector unitario con la direcci�n del muelle que
                            // apunta de B a A
        public float defaultSize = 2f; // Longitud natural de los cilindros en
                                       // Unity (m)
        public Quaternion rotation; // Nos permitir� calcular la orientaci�n del
                                    // muelle  
        public Node nodeA; // Primer extremo del muelle
        public Node nodeB; // Segundo extremo del muelle

        public float dDeformation = 10f; //Amortiguamiento de la deformaci�n del muelle

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // El valor de "pos" se calcula en el script PhysicsManager seg�n el
            // m�todo de integraci�n. Aqu� establecemos la transformada posici�n
            // del gameobject para que coincida con la posici�n calculada 
            transform.position = pos;

            // Modificamos tambi�n transformada de escala local del gameobject
            // para que el cilindro que representa el muelle conecte siempre los
            // dos nodos. Las componentes x y z no se modifican
            transform.localScale = new Vector3(transform.localScale.x,
                                               length / defaultSize,
                                               transform.localScale.z);

            // Giramos el cilindro que representa el muelle seg�n la rotaci�n
            // calculada en PhysicsManager
            transform.rotation = rotation;
        }
    }
}
