using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MassSpring2D;

//Componente que permite comprobar si un Nodo de un objeto masa-muelle se encuentra en el interior del Collider del gameObject al que est� asociado.
//Se modific� respecto a las anteriores entregas para lograr que los nodos que "fije" sigan su movimiento, en lugar de quedarse inm�viles.
public class Fixer : MonoBehaviour
{
    private Collider fixerCollider;
    private MeshRenderer fixerMeshRenderer;
    private List<Node> fixedNodes = new List<Node>(); //Lista de Nodos que son fijados por el fixer.
    private List<Vector3> relativePositions = new List<Vector3>(); //Lista con las posiciones relativas de cada uno de los nodos fijados con respecto a la posici�n del fixer.

    private void Awake()
    {
        fixerCollider = gameObject.GetComponent<Collider>();
        fixerMeshRenderer = gameObject.GetComponent<MeshRenderer>();
        fixerMeshRenderer.enabled = false; //Desactivamos el renderizado de la malla del gameObject para que esta no sea visible durante la ejecuci�n de la animaci�n.
    }

    //M�todo que recibe un Nodo, devolviendo true si el punto se encuentra en el interior del collider y false en caso contrario.
    public bool CheckFixerContainsPoint(Node node)
    {
        bool contains = fixerCollider.bounds.Contains(node.pos); //Se utiliza el m�todo contains del volumen envolvente del collider para comprobar si la posici�n del nodo est� en su interior.

        if (contains) //Si el nodo est� contenido en el fixer.
        {
            fixedNodes.Add(node); //Se agrega a la lista de nodos fijados por el fixer.
            Vector3 relativePos = node.pos - transform.position; //Se calcula la posici�n relativa del nodo respecto del fixer.
            relativePositions.Add(relativePos); //Se almacena la posici�n relativa.
        }

        return contains; //Se devuelve si el nodo estaba contenido en el fixer, pudiendo as� evitar que se integre su posici�n, pues ser� fija a la posici�n relativa del fixer.
    }

    //Una vez realizados los c�lculos del movimiento del personaje y de los nodos de los objetos masa-muelle, se actualiza la posici�n de los nodos "fijados" teniendo en cuenta la nueva
    //posici�n del fixer, la posici�n relativa de los nodos, y la rotaci�n actual del fixer.
    private void LateUpdate()
    {
        Quaternion currentRotation = transform.rotation;

        for (int i = 0; i < fixedNodes.Count; i++)
        {
            fixedNodes[i].pos = transform.position + currentRotation * relativePositions[i];
        }
    }
}
