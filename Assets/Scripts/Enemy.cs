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
    [SerializeField] private Vector3 _dir, _anchor;
    private float _healthCurrent;
    [SerializeField] private GameObject _pickupPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private AudioSource _sfxDeath;
    static int _dropChance;
    private int _dropChanceMax = 6;

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
        _mat = GetComponentInChildren<MeshRenderer>().material;
        Reset();
        _curve.postWrapMode = WrapMode.PingPong;
        _curve.preWrapMode = WrapMode.PingPong;
        _dropChance = _dropChanceMax;
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
        if (_dying || !GameManager.IsPlaying)
            return;

        _curveDelta += Time.deltaTime * _offsetSpd;

        UpdateFollowPlayer();
        _anchor.y = Mathf.MoveTowards(_anchor.y, _yAnchorOffset, 2* Time.deltaTime);
        if (_anchor.y == _yAnchorOffset)
            CalculateOffsetPositionFrom(_anchor);
        else
            transform.position = _anchor;
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
        _curveDelta = 0;
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
        ScoreKeeper.IncreaseScore(1);
        GetComponent<Collider>().enabled = false;
        _sfxDeath.Play();
        _mat.SetColor("_DissolveOutline", GameObject.Find("Player").GetComponent<PlayerGun>().Colour);
        while (_dissolve < 1)
        {
            _dissolve += Time.deltaTime;
            _mat.SetFloat("_DissolveAmount", _dissolve);
            yield return null;
        }
        if (MathExt.Roll(_dropChance))
        {
            Instantiate(_pickupPrefab, MathExt.FlattenVector3(_anchor), Quaternion.identity);
            _dropChance = _dropChanceMax;
        }
        else
            _dropChance--;

        int spawn = Random.Range(0, 4);
        _anchor = _spawnPoints[spawn].position;
        Reset();
    }

}
