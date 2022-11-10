using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpd, _maxDist, _power;
    [SerializeField] private LayerMask _hitLayer;
    private Vector3 _dir;
    private float _radius;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5f);
    }

    public void Initialise(Vector3 direction, Vector3 drift, float speed, float power, float scale, LayerMask mask)
    {
        _dir = direction + drift;
        _moveSpd = speed;
        _power = power;
        transform.localScale = Vector3.one * scale;
        transform.LookAt(transform.position + direction);
        _radius = GetComponent<SphereCollider>().radius * scale;
        _hitLayer = mask;
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckHit())
        {
            Debug.Log("Hit");
            EndOfLife();
            return;
        }

        transform.position += _dir * _moveSpd * Time.deltaTime;
    }


    bool CheckHit()
    {
        Ray ray = new Ray(transform.position, _dir);
        RaycastHit hit;
        if (Physics.SphereCast(ray, _radius, out hit, _moveSpd * Time.deltaTime, _hitLayer))
        {
            return true;
        }
        return false;
    }

    void EndOfLife()
    {
        Destroy(gameObject);
    }
}
