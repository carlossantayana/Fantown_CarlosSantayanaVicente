/******************************************************************************
 * GRADO EN DISE�O Y DESARROLLO DE VIDEOJUEGOS - ANIMACI�N 3D
 * Pr�ctica II-B�sica 2-ejercicio 1
 * 
 * Clase PhysicsManager.cs: Es la que calcula las fuerzas sobre cada nodo libre
 * y halla su posici�n a lo largo del tiempo en funci�n del m�todo de 
 * integraci�n elegido. Comunica estos c�lculos a las clases Node y Spring
 * para que se dibujen en pantalla los objetos en su posici�n y orientaci�n
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

        // Lista enumerada con los tipos de integraci�n posibles
        public enum Integration
        {
            ExplicitEuler = 0,   // Tiene problemas de divergencia
            SymplecticEuler = 1, // M�todo de integraci�n recomendado
        }

        public Integration integrationMethod; // Este ser� el m�todo de integraci�n
                                              // con el que vamos a calcular la
                                              // animaci�n
        public float h = 0.01f; // Paso de integraci�n (es un tiempo)

        public List<Node> listOfNodes; // Lista que contiene todos los nodos
        public List<Spring> listOfSprings; // Lista que contiene todos los muelles

        private Wind wind; //Componente del viento y sus propiedades para aplicar la fuerza de este sobre la cuerda.

        // Use this for initialization
        void Start()
        {
            wind = GameObject.Find("Wind").GetComponent<Wind>(); //Se almacena el componente del viento del gameObject "Wind".

            // Recorremos la lista de nodos para conocer su posici�n inicial
            foreach (Node node in listOfNodes)
            {
                node.Start();
            }

            // Recorremos la lista de muelles para conocer su posici�n inicial, as�
            // como la orientaci�n y la longitud de reposo
            foreach (Spring spring in listOfSprings)
            {
                // Vector direcci�n en el instante inicial, apunta de B a A
                // con un tama�o igual a la longitud del muelle
                spring.dir = spring.nodeA.pos - spring.nodeB.pos;
                // Establecemos la longitud natural del muelle como la distancia
                // entre ambos nodos en el instante inicial
                spring.length0 = spring.dir.magnitude;
                // Y en ese instante inicial la longitud del muelle tambi�n
                // coincide con la longitud natural
                spring.length = spring.length0;
                // Normalizamos el vector que almacena la orientaci�n del muelle
                spring.dir = Vector3.Normalize(spring.dir);
                // Posici�n del punto medio del muelle: media aritm�tica de las
                // posiciones de los dos nodos
                spring.pos = (spring.nodeA.pos + spring.nodeB.pos) / 2f;
                // Orientamos correctamente el muelle seg�n el vector dir
                spring.rotation = Quaternion.FromToRotation(Vector3.up, spring.dir);
            }
        }

        private void FixedUpdate()
        {
            // Seg�n el m�todo de integraci�n escogido, se invoca una funci�n u otra
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
            // calculado la nueva posici�n de los nodos con el m�todo de integraci�n
            foreach (Spring spring in listOfSprings)
            {
                // Vector direcci�n del muelle, apunta de B a A            
                spring.dir = spring.nodeA.pos - spring.nodeB.pos;
                // Nueva longitud del muelle 
                spring.length = spring.dir.magnitude;
                // Normalizamos el vector que almacena la orientaci�n del muelle
                spring.dir = Vector3.Normalize(spring.dir);
                // Posici�n del punto medio del muelle: media aritm�tica de las
                // posiciones de los dos nodos
                spring.pos = (spring.nodeA.pos + spring.nodeB.pos) / 2f;
                // Orientamos correctamente el muelle seg�n el vector dir
                spring.rotation = Quaternion.FromToRotation(Vector3.up, spring.dir);
            }
        }

        /// <summary>
        /// M�todo de integraci�n de Euler Expl�cito
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

                    //Se agrega la fuerza del viento en funci�n de su intensidad, de la m�xima fuerza que puede alcanzar, y su direcci�n. Tambi�n se agrega
                    //un cierto grado de aleatoriedad para cada nodo en cada frame, pues el viento nunca permanece completamente constante.
                    node.force += (wind.WindIntensity * wind.maxWindForce * Random.Range(0f, 1f)) * wind.WindDirection;

                    ApplyDampingNode(node);
                }
            }

            // Recorremos la lista de muelles para a�adir a cada nodo la fuerza
            // el�stica de cada muelle. Por la ley de acci�n y reacci�n, estas
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
        ///  M�todo de integraci�n de Euler Simpl�ctico
        /// </summary>
        void integrateSymplecticEuler()
        {
            // Recorremos la lista de nodos para aplicar las fuerzas a cada uno de
            // ellos
            foreach (Node node in listOfNodes)
            {
                node.force = -node.mass * g;

                //Se agrega la fuerza del viento en funci�n de su intensidad, de la m�xima fuerza que puede alcanzar, y su direcci�n. Tambi�n se agrega
                //un cierto grado de aleatoriedad para cada nodo en cada frame, pues el viento nunca permanece completamente constante.
                node.force += (wind.WindIntensity * wind.maxWindForce * Random.Range(0f, 1f)) * wind.WindDirection;

                ApplyDampingNode(node);
            }

            // Recorremos la lista de muelles para a�adir a cada nodo la fuerza
            // el�stica de cada muelle. Por la ley de acci�n y reacci�n, estas
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
            // velocidad y la nueva posici�n, una vez que ya conocemos la fuerza
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
