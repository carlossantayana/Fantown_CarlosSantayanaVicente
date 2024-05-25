/******************************************************************************
 * GRADO EN DISEÑO Y DESARROLLO DE VIDEOJUEGOS - ANIMACIÓN 3D
 * Práctica II-Básica 2-ejercicio 1
 * 
 * Clase PhysicsManager.cs: Es la que calcula las fuerzas sobre cada nodo libre
 * y halla su posición a lo largo del tiempo en función del método de 
 * integración elegido. Comunica estos cálculos a las clases Node y Spring
 * para que se dibujen en pantalla los objetos en su posición y orientación
 * correctas
 *****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MassSpring1D
{
    public class PhysicsManager : MonoBehaviour
    {
        public Vector3 g = new Vector3(0f, 9.8f, 0f); // Valor de la gravedad (m/s^2)

        // Lista enumerada con los tipos de integración posibles
        public enum Integration
        {
            ExplicitEuler = 0,   // Tiene problemas de divergencia
            SymplecticEuler = 1, // Método de integración recomendado
        }

        public Integration integrationMethod; // Este será el método de integración
                                              // con el que vamos a calcular la
                                              // animación
        public float h = 0.01f; // Paso de integración (es un tiempo)

        public List<Node> listOfNodes; // Lista que contiene todos los nodos
        public List<Spring> listOfSprings; // Lista que contiene todos los muelles

        private Wind wind; //Componente del viento y sus propiedades para aplicar la fuerza de este sobre la cuerda.

        // Use this for initialization
        void Start()
        {
            wind = GameObject.Find("Wind").GetComponent<Wind>(); //Se almacena el componente del viento del gameObject "Wind".

            // Recorremos la lista de nodos para conocer su posición inicial
            foreach (Node node in listOfNodes)
            {
                node.Start();
            }

            // Recorremos la lista de muelles para conocer su posición inicial, así
            // como la orientación y la longitud de reposo
            foreach (Spring spring in listOfSprings)
            {
                // Vector dirección en el instante inicial, apunta de B a A
                // con un tamaño igual a la longitud del muelle
                spring.dir = spring.nodeA.pos - spring.nodeB.pos;
                // Establecemos la longitud natural del muelle como la distancia
                // entre ambos nodos en el instante inicial
                spring.length0 = spring.dir.magnitude;
                // Y en ese instante inicial la longitud del muelle también
                // coincide con la longitud natural
                spring.length = spring.length0;
                // Normalizamos el vector que almacena la orientación del muelle
                spring.dir = Vector3.Normalize(spring.dir);
                // Posición del punto medio del muelle: media aritmética de las
                // posiciones de los dos nodos
                spring.pos = (spring.nodeA.pos + spring.nodeB.pos) / 2f;
                // Orientamos correctamente el muelle según el vector dir
                spring.rotation = Quaternion.FromToRotation(Vector3.up, spring.dir);
            }
        }

        private void FixedUpdate()
        {
            // Según el método de integración escogido, se invoca una función u otra
            switch (integrationMethod)
            {
                case Integration.ExplicitEuler:
                    integrateExplicitEuler();
                    break;

                case Integration.SymplecticEuler:
                    integrateSymplecticEuler();
                    break;
                default:
                    print("ERROR METODO INTEGRACION DESCONOCIDO");
                    break;
            }

            // Recorremos la lista de muelles para recalcularlos, una vez que hemos
            // calculado la nueva posición de los nodos con el método de integración
            foreach (Spring spring in listOfSprings)
            {
                // Vector dirección del muelle, apunta de B a A            
                spring.dir = spring.nodeA.pos - spring.nodeB.pos;
                // Nueva longitud del muelle 
                spring.length = spring.dir.magnitude;
                // Normalizamos el vector que almacena la orientación del muelle
                spring.dir = Vector3.Normalize(spring.dir);
                // Posición del punto medio del muelle: media aritmética de las
                // posiciones de los dos nodos
                spring.pos = (spring.nodeA.pos + spring.nodeB.pos) / 2f;
                // Orientamos correctamente el muelle según el vector dir
                spring.rotation = Quaternion.FromToRotation(Vector3.up, spring.dir);
            }
        }

        /// <summary>
        /// Método de integración de Euler Explícito
        /// </summary>
        void integrateExplicitEuler()
        {
            // Recorremos la lista de nodos para aplicar las fuerzas a cada uno de
            // ellos
            foreach (Node node in listOfNodes)
            {
                if (!node.fixedNode) // Si el nodo no es fijo
                {
                    // r_(n+1) = r_n + h * v_n
                    node.pos += h * node.vel;
                    node.force = -node.mass * g;

                    //Se agrega la fuerza del viento en función de su intensidad, de la máxima fuerza que puede alcanzar, y su dirección. También se agrega
                    //un cierto grado de aleatoriedad para cada nodo en cada frame, pues el viento nunca permanece completamente constante.
                    node.force += (wind.WindIntensity * wind.maxWindForce * Random.Range(0f, 1f)) * wind.WindDirection;

                    ApplyDampingNode(node);
                }
            }

            // Recorremos la lista de muelles para añadir a cada nodo la fuerza
            // elástica de cada muelle. Por la ley de acción y reacción, estas
            // fuerzas son iguales y de sentidos opuestos en los extremos de cada
            // muelle
            foreach (Spring spring in listOfSprings)
            {
                spring.nodeA.force += -spring.k * (spring.length - spring.length0)
                    * spring.dir;
                spring.nodeB.force += spring.k * (spring.length - spring.length0)
                    * spring.dir;
                ApplyDampingSpring(spring);
            }

            // Recorremos de nuevo la lista de nodos para calcular la nueva
            // velocidad, una vez que ya conocemos la fuerza total en cada nodo
            foreach (Node node in listOfNodes)
            {
                if (!node.fixedNode) // Si el nodo no es fijo
                {
                    // v_(n+1) = v_n + h F_n / m
                    node.vel += h * node.force / node.mass;
                }
            }
        }

        /// <summary>
        ///  Método de integración de Euler Simpléctico
        /// </summary>
        void integrateSymplecticEuler()
        {
            // Recorremos la lista de nodos para aplicar las fuerzas a cada uno de
            // ellos
            foreach (Node node in listOfNodes)
            {
                node.force = -node.mass * g;

                //Se agrega la fuerza del viento en función de su intensidad, de la máxima fuerza que puede alcanzar, y su dirección. También se agrega
                //un cierto grado de aleatoriedad para cada nodo en cada frame, pues el viento nunca permanece completamente constante.
                node.force += (wind.WindIntensity * wind.maxWindForce * Random.Range(0f, 1f)) * wind.WindDirection;

                ApplyDampingNode(node);
            }

            // Recorremos la lista de muelles para añadir a cada nodo la fuerza
            // elástica de cada muelle. Por la ley de acción y reacción, estas
            // fuerzas son iguales y de sentidos opuestos en los extremos de cada
            // muelle
            foreach (Spring spring in listOfSprings)
            {
                spring.nodeA.force += -spring.k * (spring.length - spring.length0)
                    * spring.dir;
                spring.nodeB.force += spring.k * (spring.length - spring.length0)
                    * spring.dir;
                ApplyDampingSpring(spring);
            }

            // Recorremos de nuevo la lista de nodos para calcular la nueva
            // velocidad y la nueva posición, una vez que ya conocemos la fuerza
            // total en cada nodo
            foreach (Node node in listOfNodes)
            {
                if (!node.fixedNode) // Si el nodo no es fijo
                {
                    // v_(n+1) = v_n + h F_n / m
                    node.vel += h * node.force / node.mass;
                    // r_(n+1) = r_n + h * v_(n+1)
                    node.pos += h * node.vel;
                }
            }
        }

        void ApplyDampingNode(Node node)
        {
            node.force += -node.dAbsolute * node.vel;
        }

        void ApplyDampingSpring(Spring spring)
        {
            spring.nodeA.force += -spring.dDeformation * Vector3.Dot((spring.nodeA.vel - spring.nodeB.vel), spring.dir) * spring.dir;
            spring.nodeB.force += spring.dDeformation * Vector3.Dot((spring.nodeA.vel - spring.nodeB.vel), spring.dir) * spring.dir;
        }
    }
}
