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
    private float _killTimerMax = 5f;
    private bool _dying;

    public void Initialise(Vector3 direction, Vector3 drift, float speed, float power, float scale, Color color, LayerMask mask)
    {
        _dir = direction + drift;
        _moveSpd = speed;
        _power = power;
        transform.localScale = Vector3.one * scale;
        transform.LookAt(transform.position + direction);
        _radius = GetComponent<SphereCollider>().radius * scale;
        _hitLayer = mask;
        _model.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
        var psys = _PSysTrail;
        var psysmain = psys.main;
        psysmain.startColor = color;
    }

    // Update is called once per frame
    void Update()
    {
        if (_dying)
            return;

        if (CheckHit(out RaycastHit hit))
        {
            Debug.Log("Hit");
            if (hit.gameObject.TryGetComponent<Enemy>(out Enemy e))
			{
				e.TakeDamage(_power);
			}
            _PSysHit.Play();
            StartCoroutine("EndOfLife");
            return;
        }

        _killTimer += Time.deltaTime;
        if (_killTimer >= 5f)
            StartCoroutine("EndOfLife");

        transform.position += _dir * _moveSpd * Time.deltaTime;
    }


    bool CheckHit(out RaycastHit hit)
    {
        Ray ray = new Ray(transform.position, _dir);
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
