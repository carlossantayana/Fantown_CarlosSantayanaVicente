using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MassSpring2D;

//Componente que permite comprobar si un Nodo de un objeto masa-muelle se encuentra en el interior del Collider del gameObject al que está asociado.
//Se modificó respecto a las anteriores entregas para lograr que los nodos que "fije" sigan su movimiento, en lugar de quedarse inmóviles.
public class Fixer : MonoBehaviour
{
    private Collider fixerCollider;
    private MeshRenderer fixerMeshRenderer;
    private List<Node> fixedNodes = new List<Node>(); //Lista de Nodos que son fijados por el fixer.
    private List<Vector3> relativePositions = new List<Vector3>(); //Lista con las posiciones relativas de cada uno de los nodos fijados con respecto a la posición del fixer.

    private void Awake()
    {
        fixerCollider = gameObject.GetComponent<Collider>();
        fixerMeshRenderer = gameObject.GetComponent<MeshRenderer>();
        fixerMeshRenderer.enabled = false; //Desactivamos el renderizado de la malla del gameObject para que esta no sea visible durante la ejecución de la animación.
    }

    //Método que recibe un Nodo, devolviendo true si el punto se encuentra en el interior del collider y false en caso contrario.
    public bool CheckFixerContainsPoint(Node node)
    {
        bool contains = fixerCollider.bounds.Contains(node.pos); //Se utiliza el método contains del volumen envolvente del collider para comprobar si la posición del nodo está en su interior.

        if (contains) //Si el nodo está contenido en el fixer.
        {
            fixedNodes.Add(node); //Se agrega a la lista de nodos fijados por el fixer.
            Vector3 relativePos = node.pos - transform.position; //Se calcula la posición relativa del nodo respecto del fixer.
            relativePositions.Add(relativePos); //Se almacena la posición relativa.
        }

        return contains; //Se devuelve si el nodo estaba contenido en el fixer, pudiendo así evitar que se integre su posición, pues será fija a la posición relativa del fixer.
    }

    //Una vez realizados los cálculos del movimiento del personaje y de los nodos de los objetos masa-muelle, se actualiza la posición de los nodos "fijados" teniendo en cuenta la nueva
    //posición del fixer, la posición relativa de los nodos, y la rotación actual del fixer.
    private void LateUpdate()
    {
        Quaternion currentRotation = transform.rotation;

        for (int i = 0; i < fixedNodes.Count; i++)
        {
            fixedNodes[i].pos = transform.position + currentRotation * relativePositions[i];
        }
    }
}
