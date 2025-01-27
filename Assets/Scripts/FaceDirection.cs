using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceDirection : MonoBehaviour
{
    [SerializeField] private Vector3 _faceDirection;
    
    private void Update()
    {
        transform.LookAt(transform.position + _faceDirection);
    }
}
