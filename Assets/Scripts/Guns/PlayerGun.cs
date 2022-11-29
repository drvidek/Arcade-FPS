using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGun : MasterGun
{
    [Header("Player Components")]
    [SerializeField]
    private Camera _cam;

    [SerializeField] private GunType _baseGun;
    [SerializeField] private GunType _equipGun;
    [SerializeField] private float _timerMax = 10f;
    private float _timer;
    private bool _powerupActive;
    [Header("UI")] [SerializeField] private Image _chargeBar;

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
        _chargeBar.enabled = false;
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
        if (!GameManager.IsPlaying)
            return;

        ManageShotDelay();
        ManagePowerupTimer();
    }

    protected override void Shoot()
    {
        Ray ray = CalculateRay();
        CreateBullets(_shotCount, MyPlayer.GetDrift(), ray);
    }

    private void ManageShotDelay()
    {
        if (_shotDelay <= 0)
        {
            Shoot();
            _shotDelay = _shotDelayMax;
            return;
        }

        _shotDelay = Mathf.MoveTowards(_shotDelay, 0, Time.deltaTime);
    }

    private void ManagePowerupTimer()
    {
        if (!_powerupActive)
            return;

        if (_timer <= 0)
        {
            ApplyPropertiesFrom(_baseGun);
            _powerupActive = false;
            _chargeBar.enabled = false;

            return;
        }
        _timer = Mathf.MoveTowards(_timer, 0, Time.deltaTime);
        _chargeBar.material.SetFloat("_Health", _timer / _timerMax);
    }

    private void ResetTimer()
    {
        _timer = _timerMax;
    }

    private void SetBarColor(Color color)
    {
        _chargeBar.material.SetColor("_FullHealthColor", color);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Found trigger");

        if (other.TryGetComponent<Pickup>(out Pickup p))
        {
            if (p.Collected)
            {
                return;
            }
            GunType gun = p.Gun;
            ApplyPropertiesFrom(gun);

            SetBarColor(gun.Colour);

            ResetTimer();

            _powerupActive = true;
            _chargeBar.enabled = true;
            p.StartCoroutine("EndOfLife");
        }
    }
}