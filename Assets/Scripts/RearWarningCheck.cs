using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RearWarningCheck : MonoBehaviour
{
    [SerializeField] private Transform _checkPos;
    [SerializeField] private float _radius = 0.2f;
    [SerializeField] private GameObject _warning;

    private void Update()
    {
        _warning.SetActive(!Physics.CheckSphere(_checkPos.position, _radius));
    }
}
