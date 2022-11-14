using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform _followTarget;
    [SerializeField] private Vector3[] _offsets;
    [SerializeField] private AnimationCurve _curve;
    private float _curveDelta;
    [SerializeField] private float _moveSpd = 5f, _offsetSpd = 0.5f, _offsetXMod = 0.5f, _offsetYMod = 1f;
    private Vector3 _anchor;

    private void Start()
    {
        _anchor = transform.position;
        _curve.postWrapMode = WrapMode.PingPong;
        _curve.preWrapMode = WrapMode.PingPong;
        _followTarget = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        Vector3 dir = MathExt.Direction(transform.position, _followTarget.position);
        dir = MathExt.FlattenVector3(dir);
        transform.forward = dir;
        _anchor += dir * _moveSpd * Time.deltaTime;
        transform.position = _anchor + CalculateOffset();
    }

    Vector3 CalculateOffset()
    {
        _curveDelta += Time.deltaTime * _offsetSpd;
        Vector3 offset = Vector3.zero;
        foreach (Vector3 set in _offsets)
        {
            Vector3 pos = set;
            pos.x *= _curve.Evaluate(_curveDelta * _offsetXMod);
            pos.y *= _curve.Evaluate(_curveDelta * _offsetYMod);
            offset += pos;
        }
        return offset;
    }

}
