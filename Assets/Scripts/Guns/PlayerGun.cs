using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGun : MasterGun
{
    [Header("Player Components")]
    [SerializeField] private Camera _cam;
    [SerializeField] private GunType _baseGun;
    [SerializeField] private GunType _equipGun;


    [Header("UI")]
    [SerializeField] private Image _chargeBar;
    Color _chargeBarColDefault;

    private void OnValidate()
    {
        if (_equipGun != null)
            ApplyPropertiesFrom(_equipGun);
        else
            ApplyPropertiesFrom(_baseGun);
    }

    public Camera Cam
    {
        set { _cam = value; }
    }

    // Start is called before the first frame update
    new void Start()
    {
        _owner = GetComponent<PlayerMovement>();
        ApplyPropertiesFrom(_baseGun);
        _shotDelay = _shotDelayMax;
    }

    private void ApplyPropertiesFrom(GunType newGun)
    {
        _shotPower = newGun.Power;
        _shotSpeed = newGun.Speed;
        _shotSize = newGun.Size;
        _shotCount = newGun.Count;
        _shotDelayMax = newGun.Delay;
        _shotSpread = newGun.Spread;
        _color = newGun.Colour;
    }

    protected override Ray CalculateRay()
    {
        Ray _rayFromScreen = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        //Vector3 _dir = _rayFromScreen.direction;
        //Vector3 _rayStart = _cam.transform.position + transform.TransformDirection(Vector3.forward * Mathf.Abs(_camCon._zoom));
        //var _ray = new Ray(_rayStart, _dir); //);
        return _rayFromScreen;
    }

    override protected Vector3 CalculateDir(Ray ray)
    {
        Vector3 hitPoint = ray.GetPoint(100f);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            hitPoint = hit.point;
        }

        Vector3 _dir = (hitPoint - _bulletSpawn.position);
        _dir.Normalize();

        return _dir;
    }

    // Update is called once per frame
    void Update()
    {
        //UI update
        if (_shotDelay <= 0)
        {
            Shoot();
            _shotDelay = _shotDelayMax;
            return;
        }
        _shotDelay = Mathf.MoveTowards(_shotDelay, 0, Time.deltaTime);
        //_chargeBar.fillAmount = 1f - (_shotDelay / _shotDelayMax);
    }

    protected override void Shoot()
    {
        Ray ray = CalculateRay();
        CreateBullets(_shotCount, MyPlayer.GetDrift(), ray);
    }


    private void OnTriggerEnter(Collider other)
    {
            Debug.Log("Found trigger");

        if (other.TryGetComponent<Pickup>(out Pickup p))
        {
            Debug.Log("Found pickup");
            if (p.Collected)
            {
            Debug.Log("Pickup Collected");

                return;
            }

            ApplyPropertiesFrom(p.Gun);
            p.StartCoroutine("EndOfLife");
        }
    }
}