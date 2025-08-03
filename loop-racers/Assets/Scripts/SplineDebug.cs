using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace Busta.LoopRacers
{
    [ExecuteInEditMode]
    public class SplineDebug : MonoBehaviour
    {
        [SerializeField] private GameObject carPrefab;
        [SerializeField] private GameObject valuePrefab;
        [SerializeField] private SplineContainer spline;
        [SerializeField] private float offset;
        [SerializeField] private float value;


        private readonly List<GameObject> _cars = new();
        private GameObject _valueMarker;

        private void Update()
        {
            if (_cars.Count > 1)
                for (var index = 0; index < _cars.Count; index++)
                {
                    var t = index / (1.0f * (_cars.Count - 1));
                    var c = _cars[index];
                    c.transform.position = spline.EvaluatePosition(t);
                    var splineTan = spline.EvaluateTangent(t);
                    var tan = new Vector3(splineTan.x, splineTan.y, splineTan.z);
                    c.transform.LookAt(c.transform.position + tan, Vector3.up);
                    c.transform.position += c.transform.right * offset;
                }

            if (_valueMarker)
            {
                _valueMarker.transform.position = spline.EvaluatePosition(value);
            }
        }

        private void OnEnable()
        {
            if (_cars.Count > 0)
                foreach (var car in _cars)
                    DestroyImmediate(car);

            _cars.Clear();

            if (!carPrefab || !spline) return;

            for (var i = 0; i < 100; i++)
            {
                var newCar = Instantiate(carPrefab, transform);
                newCar.name = $"Car {i}";
                _cars.Add(newCar);
            }

            if (valuePrefab)
            {
                _valueMarker = Instantiate(valuePrefab, transform);
                _valueMarker.transform.localScale = Vector3.one * 2;
            }
        }

        private void OnDisable()
        {
            if (_cars.Count > 0)
                foreach (var car in _cars)
                    DestroyImmediate(car);

            if (_valueMarker)
            {
                DestroyImmediate(_valueMarker);
            }
        }
    }
}