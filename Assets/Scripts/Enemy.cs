using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour //FlockAgent
{
    [SerializeField] private Transform _followTarget;
    [SerializeField] private Vector3[] _offsets;
    [SerializeField] private AnimationCurve _curve;
    private float _curveDelta;
    [SerializeField] private float _moveSpd = 5f, _turnSpd = 1f, _offsetSpd = 1f, _offsetXMod = 1f, _offsetYMod = 1f, _offsetZMod = 1f;
    [SerializeField] private bool _mapSpeedToCurve, _mapTurnToCurve;
    private Vector3 _dir, _anchor;
[SerializeField] private int _healthMax = 10;
private int _healthCurrent;

private Material _mat;
private float _dissolve;

private bool _dying;

     void Start()
    {
        //base.Start();
        _anchor = transform.position;
         _mat = GetComponentInChildren<MeshRenderer>().material;
        _curve.postWrapMode = WrapMode.PingPong;
        _curve.preWrapMode = WrapMode.PingPong;
        _followTarget = GameObject.Find("Player").transform;
        transform.forward = MathExt.FlattenVector3(MathExt.Direction(transform.position, _followTarget.position));
    }

    private void Update()
    {
        if (_dying)
return;
        _curveDelta += Time.deltaTime * _offsetSpd;

        Vector3 newDir = MathExt.Direction(transform.position, _followTarget.position);
        newDir = MathExt.FlattenVector3(newDir);
        float steering = _turnSpd * (_mapTurnToCurve ? CurveDeltaNormalised() : 1f);
        _dir = Vector3.MoveTowards(_dir, newDir, steering*Time.deltaTime);
        transform.forward = _dir;
        float spd = _moveSpd * (_mapSpeedToCurve ? CurveDeltaNormalised() : 1f);
        _anchor += _dir * spd * Time.deltaTime;
        transform.position = _anchor + CalculateOffset();
    }

    float CurveDeltaNormalised()
    {
        return Mathf.Max(0, Mathf.Abs(_curve.Evaluate(_curveDelta)));
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
        float mag = offset.magnitude;
        offset = transform.InverseTransformDirection(offset.normalized);
            offset *= mag;
        return offset;
    }
    
    public void TakeDamage(int dmg)
{
_healthCurrent -= dmg;
if (_healthCurrent <= 0)
StartCoroutine("EndOfLife");
}

IEnumerator EndOfLife()
{
_dying = true;
GetComponent<Collider>().enabled = false;
whie (_dissolve < 1)
{
_dissolve += Time.deltaTime;
_mat.SetFloat("_DissolveAmount",_dissolve);
yield return null;
}
Destroy(gameObject);
}

}
