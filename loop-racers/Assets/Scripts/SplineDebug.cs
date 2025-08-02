using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace DefaultNamespace
{
    [ExecuteInEditMode]
    public class SplineDebug : MonoBehaviour
    {
        [SerializeField] private GameObject car;
        [SerializeField] private SplineContainer spline;

        private readonly List<GameObject> cars = new();

        private void OnEnable()
        {
            if (cars.Count > 0)
            {
                foreach (var car in cars)
                {
                    DestroyImmediate(car);
                }
            }

            cars.Clear();

            if (!car || !spline)
            {
                return;
            }

            for (var i = 0; i < 100; i++)
            {
                var newCar = Instantiate(car, transform);
                newCar.name = $"Car {i}";
                cars.Add(newCar);
            }
        }

        private void OnDisable()
        {
            if (cars.Count > 0)
            {
                foreach (var car in cars)
                {
                    DestroyImmediate(car);
                }
            }
        }

        private void Update()
        {
            if (cars.Count > 1)
            {
                for (var index = 0; index < cars.Count; index++)
                {
                    var t = index / (1.0f * (cars.Count - 1));
                    var c = cars[index];
                    c.transform.position = spline.EvaluatePosition(t);
                    var splineTan = spline.EvaluateTangent(t);
                    var tan = new Vector3(splineTan.x, splineTan.y, splineTan.z);
                    c.transform.LookAt(c.transform.position + tan, Vector3.up);
                }
            }
        }
    }
}