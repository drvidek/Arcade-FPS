using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class MasterGun : MonoBehaviour
{
    [SerializeField] protected CombatAgent _owner;

    [Header("Assets")]
    [SerializeField] protected GameObject _bulletPrefab;
    [SerializeField] protected Transform _bulletSpawn;

    [Header("Bullet Stats")]
    [SerializeField] protected int _shotCount = 1;
    [SerializeField] protected float _shotPower = 1;
    [SerializeField] protected float _shotSpeed = 40;
    [SerializeField] protected float _shotSize = 0.2f;
    protected float _shotDelay;
    [SerializeField] protected float _shotDelayMax = 0.3f;
    [SerializeField] protected float _shotSpread = 0.01f;
    [SerializeField] protected LayerMask _hitMask;
    [SerializeField] protected Color _color;
    public Color Colour { get => _color; }


    public PlayerMovement MyPlayer { get { return _owner as PlayerMovement; } }
    //public MasterEnemy Enemy { get { return _owner as MasterEnemy; } }


    virtual protected void Start()
    {
        _owner = GetComponent<CombatAgent>();
    }

    abstract protected Vector3 CalculateDir(Ray ray);

    abstract protected Ray CalculateRay();
    virtual protected void CreateBullets(int count, Vector3 drift, Ray _ray = new Ray())
    {
        Vector3 dir = CalculateDir(_ray);
        for (int i = 0; i < count; i++)
        {
            Bullet bullet = Instantiate(_bulletPrefab, _bulletSpawn.position, Quaternion.identity).GetComponent<Bullet>();
            ApplyBulletProperties(bullet, dir, drift, i);
            _shotDelay = _shotDelayMax;
        }
    }

    virtual public void ApplyBulletProperties(Bullet bullet, Vector3 dir, Vector3 drift, int count)
    {
        bullet.transform.LookAt(bullet.transform.position + dir);
        float _finalSpread = _shotSpread;
        Vector3 horScatter = bullet.transform.TransformDirection(Vector3.left) * Random.Range(-_finalSpread, _finalSpread) * (float)(count+1);   //scatter reduced to 0 for a scoped
        Vector3 verScatter = bullet.transform.TransformDirection(Vector3.up) * Random.Range(-_finalSpread, _finalSpread) * (float)(count+1);     //hitscan single shot
        Vector3 newDir = dir + horScatter + verScatter;
        bullet.Initialise(newDir, drift, _shotSpeed, _shotPower, _shotSize, _color, _hitMask);
    }


    abstract protected void Shoot();
}
