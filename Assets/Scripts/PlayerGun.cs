using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGun : MasterGun
{
    [Header("Player Components")]
    [SerializeField] private Camera _cam;

    [Header("UI")]
    [SerializeField] private Image _chargeBar;
    Color _chargeBarColDefault;

    public Camera Cam
    {
        set { _cam = value; }
    }

    // Start is called before the first frame update
    new void Start()
    {
        _owner = GetComponent<PlayerMovement>();
        //_chargeBarColDefault = _chargeBar.color;
        _shotDelay = _shotDelayMax;
    }

    public void DebugSliderUpdate()
    {
        //_shotSpeed = (int)_sliders[(int)GunStat.speed].value;
        //_shotSize = _sliders[(int)GunStat.size].value;
        //_shotCount = (int)_sliders[(int)GunStat.count].value;
        //_shotPower = (int)_sliders[(int)GunStat.power].value;
        //_shotDelayMax = _sliders[(int)GunStat.delay].value;
        //_shotSpread = _sliders[(int)GunStat.spread].value;
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
}