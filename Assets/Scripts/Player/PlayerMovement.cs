using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CombatAgent
{
    [Header("Movement")]
    [SerializeField] private float _moveSpd;
    [SerializeField] PlayerGun _gun;


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
        Vector3 moveDir = transform.TransformDirection(inputDir); //Vector3.Normalize(_camProxy.right * inputDir.x + Vector3.Normalize(FlattenVector3(_camProxy.forward)) * inputDir.y);
        moveDir *= _moveSpd * Time.deltaTime;
        Vector3 newPosition = transform.position + moveDir;
        newPosition.x = Mathf.Clamp(newPosition.x, -49,49);
        newPosition.z = Mathf.Clamp(newPosition.z, -49,49);
        transform.position = newPosition;
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


}
