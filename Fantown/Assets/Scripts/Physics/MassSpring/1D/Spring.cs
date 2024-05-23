/******************************************************************************
 * GRADO EN DISEÑO Y DESARROLLO DE VIDEOJUEGOS - ANIMACIÓN 3D
 * Práctica II-Básica 2-ejercicio 1
 * 
 * Clase Spring.cs: Define las propiedades de los muelles elásticos
 *****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace P2.Ejercicio2
{
    public class Spring : MonoBehaviour
    {
        public float k = 100f; // Constante de rigidez del muelle (N/m)
        public float length0; // Longitud natural del muelle (ahí la fuerza elástica
                              // se anula)
        public float length; // Longitud del muelle en un momento dado
        public Vector3 pos; // Posición 3D del punto medio del muelle
        public Vector3 dir; // Vector unitario con la dirección del muelle que
                            // apunta de B a A
        public float defaultSize = 2f; // Longitud natural de los cilindros en
                                       // Unity (m)
        public Quaternion rotation; // Nos permitirá calcular la orientación del
                                    // muelle  
        public Node nodeA; // Primer extremo del muelle
        public Node nodeB; // Segundo extremo del muelle

        public float dDeformation = 10f; //Amortiguamiento de la deformación del muelle

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // El valor de "pos" se calcula en el script PhysicsManager según el
            // método de integración. Aquí establecemos la transformada posición
            // del gameobject para que coincida con la posición calculada 
            transform.position = pos;

            // Modificamos también transformada de escala local del gameobject
            // para que el cilindro que representa el muelle conecte siempre los
            // dos nodos. Las componentes x y z no se modifican
            transform.localScale = new Vector3(transform.localScale.x,
                                               length / defaultSize,
                                               transform.localScale.z);

            // Giramos el cilindro que representa el muelle según la rotación
            // calculada en PhysicsManager
            transform.rotation = rotation;
        }
    }
}
