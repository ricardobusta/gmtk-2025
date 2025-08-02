using System;
using UnityEngine;
using UnityEngine.Splines;

public class RaceController : MonoBehaviour
{
    [SerializeField] private GameObject car;
    [SerializeField] private SplineContainer spline;
    [SerializeField] private float acceleration = 5.0f;
    [SerializeField] private float drag = 10.0f;
    [SerializeField] private float maxSpeed = 100;
    [SerializeField] private float offset = 0;


    private float _speed;
    private float _meterToSplineUnit;
    private float _parametricPosition;

    private void Start()
    {
        _meterToSplineUnit = 1.0f / spline.CalculateLength();
        _parametricPosition = 0;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _speed += acceleration * Time.deltaTime;
        }
        else
        {
            _speed -= drag * Time.deltaTime;
        }

        _speed = Mathf.Clamp(_speed, 0, maxSpeed);

        _parametricPosition += _meterToSplineUnit * _speed * Time.deltaTime;
        _parametricPosition %= 1.0f;

        car.transform.position = spline.EvaluatePosition(_parametricPosition);
        var splineTan = spline.EvaluateTangent(_parametricPosition);
        var tan = new Vector3(splineTan.x, splineTan.y, splineTan.z);
        car.transform.LookAt(car.transform.position + tan, Vector3.up);
        car.transform.position += car.transform.right * offset;
    }
}