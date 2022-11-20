using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpd, _maxDist, _power;
    [SerializeField] private LayerMask _hitLayer;
    private Vector3 _dir;
    private float _radius;
    [SerializeField] private GameObject _model;
    [SerializeField] private ParticleSystem _PSysHit;
    [SerializeField] private ParticleSystem _PSysTrail;
    private float _killTimer;
    private bool _dying;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5f);
    }

    public void Initialise(Vector3 direction, Vector3 drift, float speed, float power, float scale, Color color, LayerMask mask)
    {
        _dir = direction + drift;
        _moveSpd = speed;
        _power = power;
        transform.localScale = Vector3.one * scale;
        transform.LookAt(transform.position + direction);
        _radius = GetComponent<SphereCollider>().radius * scale;
        _hitLayer = mask;
        _model.GetComponent<MeshRenderer>().material.color = color;
        var psys = _PSysTrail;
        var psysmain = psys.main;
        psysmain.startColor = color;
        _PSysHit = psys;
    }

    // Update is called once per frame
    void Update()
    {
        if (_dying)
            return;

        if (CheckHit())
        {
            Debug.Log("Hit");
            _PSysHit.Play();
            StartCoroutine("EndOfLife");
            return;
        }

        _killTimer += Time.deltaTime;
        if (_killTimer >= 5f)
            StartCoroutine("EndOfLife");

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

    IEnumerator EndOfLife()
    {
        _dying = true;
        _PSysTrail.Stop();
        GetComponentInChildren<MeshRenderer>().enabled = false;
        while (_PSysHit.isPlaying)
            yield return null;
        Destroy(gameObject);
    }
}