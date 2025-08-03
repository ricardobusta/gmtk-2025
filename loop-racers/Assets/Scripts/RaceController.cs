using System;
using UnityEngine;
using UnityEngine.Splines;

public class RaceController : MonoBehaviour
{
    [Serializable]
    public class PlayerData
    {
        public GameObject car;
        public SplineContainer spline;
        public KeyCode key;
        public float offset;

        [NonSerialized] public bool enabled;
        [NonSerialized] public float Speed;
        [NonSerialized] public float ParametricPosition;
        [NonSerialized] public float MeterToSplineUnit;

        public void OnUpdate(RaceController raceController)
        {
            if (!enabled)
            {
                return;
            }

            if (Input.GetKey(key))
            {
                Speed += raceController.acceleration * Time.deltaTime;
            }
            else
            {
                Speed -= raceController.drag * Time.deltaTime;
            }

            Speed = Mathf.Clamp(Speed, 0, raceController.maxSpeed);

            ParametricPosition += MeterToSplineUnit * Speed * Time.deltaTime;
            ParametricPosition %= 1.0f;

            car.transform.position = spline.EvaluatePosition(ParametricPosition);
            var splineTan = spline.EvaluateTangent(ParametricPosition);
            var tan = new Vector3(splineTan.x, splineTan.y, splineTan.z);
            car.transform.LookAt(car.transform.position + tan, Vector3.up);
            car.transform.position += car.transform.right * offset;
        }
    }

    [SerializeField] private float acceleration = 5.0f;
    [SerializeField] private float drag = 10.0f;
    [SerializeField] private float maxSpeed = 100;
    [SerializeField] private float startOffset = 0.685f;

    [SerializeField] private PlayerData[] players;

    private void Start()
    {
        for (var i = 0; i < players.Length; i++)
        {
            var player = players[i];
            var key = (KeyCode)PlayerPrefs.GetInt("player" + i, (int)KeyCode.None);
            if (key != KeyCode.None)
            {
                player.enabled = true;
                player.car.gameObject.SetActive(true);
                player.ParametricPosition = startOffset;
                player.MeterToSplineUnit = 1.0f / player.spline.CalculateLength();
                player.key = key;
            }
            else
            {
                player.car.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        foreach (var player in players)
        {
            player.OnUpdate(this);
        }
    }
}