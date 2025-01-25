
using System;
using UnityEngine;

public class DevilController : MonoBehaviour
{
    [SerializeField]private GameObject _spriteContainer;
private Rigidbody _rigidbody ;

private void Awake() {
    _rigidbody = GetComponent<Rigidbody>();
}
}
