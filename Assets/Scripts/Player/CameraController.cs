using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private enum RotAxis { ver, hor }
    [SerializeField] private RotAxis rotAxis = RotAxis.hor;
    [SerializeField] private float _sensitivity = 40f;
    [SerializeField] private float _verticalClamp = 30f;
    private float _verRot;
    [SerializeField] private bool _inverted;

    private void Start()
    {
        _verRot = transform.localEulerAngles.x;
    }

    Vector3 GetInputDirection()
    {
        Vector3 dir = new Vector3();
        dir.x = Input.GetAxis("P2 Hori");
        dir.z = Input.GetAxis("P2 Verti");
        return dir;
    }

    private void Update()
    {
        Vector3 camDir = GetInputDirection();
        if (rotAxis == RotAxis.ver)
        {
            _verRot += camDir.z * _sensitivity * Time.deltaTime * (_inverted ? 1 : -1);
            _verRot = Mathf.Clamp(_verRot, -_verticalClamp, _verticalClamp);
            transform.localEulerAngles = new Vector3(_verRot, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
        else
        {
            float _horRot = camDir.x * _sensitivity * Time.deltaTime * (_inverted ? -1 : 1);
            transform.Rotate(0, _horRot, 0);
        }
    }
}
