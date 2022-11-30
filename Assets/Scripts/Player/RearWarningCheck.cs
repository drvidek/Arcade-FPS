using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RearWarningCheck : MonoBehaviour
{
    [SerializeField] private GameObject _warning;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy e))
        {
            _warning.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy e))
        {
            _warning.SetActive(false);
        }
    }
}
