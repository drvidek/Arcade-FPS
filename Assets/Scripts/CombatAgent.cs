using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CombatAgent : MonoBehaviour
{
    [Header("Health + Damage")]
    [SerializeField] protected float _healthMax;
    [SerializeField] protected float _health;
    [SerializeField] protected Renderer _hitEffectMesh;
    [SerializeField] protected Material _hitEffectMat;
    [SerializeField] protected Material _homeMat;
    [SerializeField] protected Image _healthbar;

    virtual protected void Start()
    {
        _health = _healthMax;
        _hitEffectMesh = GetComponentInChildren<MeshRenderer>();
        _homeMat = _hitEffectMesh.material;
       
    }

    protected abstract void EndOfLife();

    public void TakeDamage(float hit)
    {
        _health -= hit;
        if (_healthbar != null)
            _healthbar.fillAmount = _health / _healthMax;
        if (_health <= 0)
        {
            EndOfLife();
        }
    }

}
