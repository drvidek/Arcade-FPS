using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour //FlockAgent
{
    [SerializeField] private float _healthMax = 10;
    [SerializeField] private Transform _followTarget;
    [SerializeField] private Vector3[] _offsets;
    [SerializeField] private AnimationCurve _curve;
    private float _curveDelta;
    [SerializeField] private float _yAnchorOffset;
    [SerializeField] private float _moveSpd = 5f, _turnSpd = 1f, _offsetSpd = 1f, _offsetXMod = 1f, _offsetYMod = 1f, _offsetZMod = 1f;
    [SerializeField] private bool _mapSpeedToCurve, _mapTurnToCurve;
    private Vector3 _dir, _anchor;
    private float _healthCurrent;
    [SerializeField] private GameObject _pickupPrefab;

    private Material _mat;
    private float _dissolve = 0;

    private bool _dying = false;

    private Vector3 Offset
    {
        get
        {
            Vector3 offset = Vector3.zero;
            foreach (Vector3 set in _offsets)
            {
                Vector3 pos = set;
                pos.x *= _curve.Evaluate(_curveDelta * _offsetXMod);
                pos.y *= _curve.Evaluate(_curveDelta * _offsetYMod);
                pos.z *= _curve.Evaluate(_curveDelta * _offsetZMod);
                offset += pos;
            }
            float mag = offset.magnitude;
            offset = transform.InverseTransformDirection(offset.normalized);
            offset *= mag;
            return offset;
        }
    }

    void Start()
    {
        //base.Start(); //need this for flocking behaviour
        _mat = GetComponentInChildren<MeshRenderer>().material;
        Reset();
        _curve.postWrapMode = WrapMode.PingPong;
        _curve.preWrapMode = WrapMode.PingPong;
        StartFollowPlayer();
    }

    private void StartFollowPlayer()
    {
        _anchor = transform.position;
        _anchor.y = _yAnchorOffset;
        _followTarget = GameObject.Find("Player").transform;
        transform.forward = MathExt.FlattenVector3(MathExt.Direction(transform.position, _followTarget.position));
    }

    private void Update()
    {
        if (_dying)
            return;
        _curveDelta += Time.deltaTime * _offsetSpd;

        UpdateFollowPlayer();
        _anchor.y = _yAnchorOffset;
        CalculateOffsetPositionFrom(_anchor);
    }

    private void UpdateFollowPlayer()
    {
        Vector3 newDir = MathExt.Direction(transform.position, _followTarget.position);
        newDir = MathExt.FlattenVector3(newDir);
        float steering = _turnSpd * (_mapTurnToCurve ? CurveDeltaCulled() : 1f);
        _dir = Vector3.MoveTowards(_dir, newDir, steering * Time.deltaTime);
        transform.forward = _dir;
        float spd = _moveSpd * (_mapSpeedToCurve ? CurveDeltaCulled() : 1f);
        _anchor += _dir * spd * Time.deltaTime;
    }

    float CurveDeltaCulled()
    {
        return Mathf.Max(0, Mathf.Abs(_curve.Evaluate(_curveDelta)));
    }

    private void CalculateOffsetPositionFrom(Vector3 anchor)
    {
        transform.position = anchor + Offset;
    }

    public void TakeDamage(float dmg)
    {
        _healthCurrent -= dmg;
        if (_healthCurrent <= 0)
            StartCoroutine("EndOfLife");
    }
    private void Reset()
    {
        ResetHealth();
        ResetModel();
        _dying = false;
    }

    private void ResetHealth()
    {
        _healthCurrent = _healthMax;
    }

    private void ResetModel()
    {
        GetComponent<Collider>().enabled = true;
        _dissolve = 0;
        _mat.SetFloat("_DissolveAmount", 0);
    }

    IEnumerator EndOfLife()
    {
        _dying = true;
        GetComponent<Collider>().enabled = false;
        _mat.SetColor("_DissolveOutline", GameObject.Find("Player").GetComponent<PlayerGun>().Colour);
        while (_dissolve < 1)
        {
            _dissolve += Time.deltaTime;
            _mat.SetFloat("_DissolveAmount", _dissolve);
            yield return null;
        }
        if (MathExt.Roll(4))
        {
            Instantiate(_pickupPrefab, _anchor, Quaternion.identity);
        }
        float arenaWidth = 100;
        _anchor = new Vector3(Random.Range(-arenaWidth/2, arenaWidth/2), 0, Random.Range(-arenaWidth/2, arenaWidth/2));
        Reset();
    }

}
