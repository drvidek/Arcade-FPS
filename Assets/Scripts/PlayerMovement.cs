using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpd;
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] private float _timerMax;
    [SerializeField] Transform _bulletSpawn;
    [SerializeField] Camera _cam;
    private float _timer;


    void OnValidate()
    {
        Inititialise();
    }

    private void Start()
    {
        Inititialise();
    }

    private void Update()
    {
        Vector3 inputDir = GetInputDir();
        Move(inputDir);
        Shoot();
    }

    private void Inititialise()
    {

    }

    Vector3 GetInputDir()
    {
        Vector3 inputDir = Vector3.zero;
        inputDir.x = Input.GetAxis("P1 Hori");
        inputDir.z = Input.GetAxis("P1 Verti");
        return inputDir;
    }

    private void Move(Vector3 inputDir)
    {
        Vector3 moveDir = transform.TransformDirection(inputDir); //Vector3.Normalize(_camProxy.right * inputDir.x + Vector3.Normalize(FlattenVector3(_camProxy.forward)) * inputDir.y);
        moveDir *= _moveSpd * Time.deltaTime;
        transform.position += moveDir;
    }

    private void Shoot()
    {
        if (_timer <= 0)
        {
            Vector3 drift = GetInputDir() * Time.deltaTime * _moveSpd;
            drift.z = 0;
            drift.y = 0;

            drift = transform.TransformDirection(drift);
            
            Bullet bullet = Instantiate(_bulletPrefab, _bulletSpawn.position, Quaternion.identity).GetComponent<Bullet>();
            bullet.Initialise(_cam, transform, drift);

            _timer = _timerMax;
            return;
        }

        _timer = Mathf.MoveTowards(_timer, 0, Time.deltaTime);
    }

}
