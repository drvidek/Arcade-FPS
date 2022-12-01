using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CombatAgent
{
    [Header("Movement")]
    [SerializeField] private float _moveSpd;
    [SerializeField] PlayerGun _gun;
    [SerializeField] AudioSource[] _sfxWalk;
    [SerializeField] float _sfxWalkDelayMax = 0.5f;
    [SerializeField] AudioSource _sfxDeath;
    float _sfxWalkDelay;
    int _sfxWalkIndex;

    void OnValidate()
    {
        Inititialise();
    }

    new void Start()
    {
        base.Start();
        Inititialise();
    }
    private void Inititialise()
    {

    }

    private void Update()
    {
        if (!GameManager.IsPlaying)
            return;

        Vector3 inputDir = GetInputDir();
        Move(inputDir);
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
        Vector3 moveDir = transform.TransformDirection(inputDir);
        moveDir *= _moveSpd * Time.deltaTime;
        Vector3 newPosition = transform.position + moveDir;
        newPosition.x = Mathf.Clamp(newPosition.x, -49,49);
        newPosition.z = Mathf.Clamp(newPosition.z, -49,49);
        transform.position = newPosition;
        if (inputDir.magnitude > 0)
            PlaySFXWalk();
        else
            _sfxWalkDelay = 0;
    }

    private void PlaySFXWalk()
    {
        if (_sfxWalkDelay <= 0)
        {
            _sfxWalk[_sfxWalkIndex].Play();
            _sfxWalkIndex++;
            if (_sfxWalkIndex > 1)
                _sfxWalkIndex = 0;
            _sfxWalkDelay = _sfxWalkDelayMax;
            return;
        }

        _sfxWalkDelay = Mathf.MoveTowards(_sfxWalkDelay, 0, Time.deltaTime);
    }

    public Vector3 GetDrift()
    {
        Vector3 drift = GetInputDir() * Time.deltaTime * _moveSpd;
        drift.z = 0;
        drift.y = 0;

        drift = transform.TransformDirection(drift);
        return drift;
    }

    protected override void EndOfLife()
    {
        throw new System.NotImplementedException();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!GameManager.IsPlaying)
            return;

        if (other.transform.TryGetComponent<Enemy>(out Enemy e))
        {
            GameManager.EndRound();
            _sfxDeath.Play();
        }
    }
}
