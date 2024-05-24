using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MassSpring2D;

//Componente que permite comprobar si un Vector3 se encuentra en el interior del Collider del gameObject al que está asociado.
public class Fixer : MonoBehaviour
{
    private Collider fixerCollider;
    private MeshRenderer fixerMeshRenderer;
    private List<Node> fixedNodes = new List<Node>();
    private List<Vector3> relativePositions = new List<Vector3>();

    private void Awake()
    {
        fixerCollider = gameObject.GetComponent<Collider>();
        fixerMeshRenderer = gameObject.GetComponent<MeshRenderer>();
        fixerMeshRenderer.enabled = false; //Desactivamos el renderizado de la malla del gameObject para que esta no sea visible durante la ejecución de la animación.
    }

    //Método que recibe un Vector3, devolviendo true si el punto se encuentra en el interior del collider y false en caso contrario.
    public bool CheckFixerContainsPoint(Node node)
    {
        bool contains = fixerCollider.bounds.Contains(node.pos);

        if (contains)
        {
            fixedNodes.Add(node);
            Vector3 relativePos = node.pos - transform.position;
            relativePositions.Add(relativePos);
        }

        return contains;
    }

    private void Update()
    {
        Quaternion currentRotation = transform.rotation;

        for (int i = 0; i < fixedNodes.Count; i++)
        {
            fixedNodes[i].pos = transform.position + currentRotation * relativePositions[i];
        }
    }
}
