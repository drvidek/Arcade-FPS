using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform _followTarget;
    [SerializeField] private Vector3[] _offsets;
    [SerializeField] private AnimationCurve _curve;
    private float _curveDelta;
    [SerializeField] private float _moveSpd = 5f, _turnSpd = 1f, _offsetSpd = 1f, _offsetXMod = 1f, _offsetYMod = 1f, _offsetZMod = 1f;
    [SerializeField] private bool _mapSpeedToCurve, _mapTurnToCurve;
    private Vector3 _anchor,_dir;


    private void Start()
    {
        _anchor = transform.position;
        _curve.postWrapMode = WrapMode.PingPong;
        _curve.preWrapMode = WrapMode.PingPong;
        _followTarget = GameObject.Find("Player").transform;
        _dir = MathExt.Direction(transform.position, _followTarget.position);
    }

    private void Update()
    {
        _curveDelta += Time.deltaTime * _offsetSpd;

        Vector3 newDir = MathExt.Direction(transform.position, _followTarget.position);
        newDir = MathExt.FlattenVector3(newDir);
        Debug.Log(newDir);
        float steering = _turnSpd * (_mapTurnToCurve ? CurveDeltaNormalised() : 1f);
        _dir = Vector3.MoveTowards(_dir, newDir, steering*Time.deltaTime);
        transform.forward = _dir;
        float spd = _moveSpd * (_mapSpeedToCurve ? CurveDeltaNormalised() : 1f);
        _anchor += _dir * spd * Time.deltaTime;
        transform.position = _anchor + CalculateOffset();
    }

    float CurveDeltaNormalised()
    {
        return Mathf.Max(0, _curve.Evaluate(_curveDelta));
    }

    Vector3 CalculateOffset()
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
        return offset;
    }

}
