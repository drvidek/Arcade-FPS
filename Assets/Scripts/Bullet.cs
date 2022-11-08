using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpd, _maxDist;
    [SerializeField] private Transform _owner;
    [SerializeField] private LayerMask _hitLayer;
    private Vector3 _dir;
    private float _radius;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Initialise(Camera cam, Transform owner, Vector3 drift)
    {
        Vector3 direction = cam.ViewportPointToRay(new Vector2(0.5f, 0.5f)).direction;
        _dir = direction + drift;
        transform.LookAt(transform.position + direction);
        _owner = owner;
        _radius = GetComponent<SphereCollider>().radius;
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

        if (Vector3.Distance(transform.position, _owner.position) > _maxDist)
        {
            EndOfLife();
        }
    }


    bool CheckHit()
    {
        Ray ray = new Ray(transform.position, _dir);
        RaycastHit hit;
        if (Physics.SphereCast(ray, _radius, out hit, _moveSpd * Time.deltaTime))
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
