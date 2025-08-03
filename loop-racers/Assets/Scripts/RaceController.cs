using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using UnityEngine.UI;

namespace Busta.LoopRacers
{
    public class RaceController : MonoBehaviour
    {
        [SerializeField] private GameConfigs gameConfigs;
        [SerializeField] private float acceleration = 50f;
        [SerializeField] private float drag = 25f;
        [SerializeField] private float maxSpeed = 50f;
        [SerializeField] private float dangerSpeed = 40f;

        [SerializeField] private int totalLaps = 15;


        [SerializeField] private float startOffset = 0.685f;

        [SerializeField] private PlayerUiGame playerUiTemplate;
        [SerializeField] private PlayerData[] players;

        [SerializeField] private Button quitButton;


        private void Awake()
        {
            for (var i = 0; i < players.Length; i++)
            {
                var player = players[i];
                var key = (KeyCode)PlayerPrefs.GetInt("player" + i, (int)KeyCode.None);
                if (key != KeyCode.None)
                {
                    player.Enabled = true;
                    player.car.gameObject.SetActive(true);
                    player.ParametricPosition = startOffset;
                    player.MeterToSplineUnit = 1.0f / player.spline.CalculateLength();
                    player.key = key;
                    player.Ui = Instantiate(playerUiTemplate, playerUiTemplate.transform.parent);
                    player.Ui.Init(i + 1, dangerSpeed / maxSpeed, key.ToString(), gameConfigs.PlayerColors[i]);
                    player.Ui.UpdateLaps(player.Laps, totalLaps);
                }
                else
                {
                    player.car.gameObject.SetActive(false);
                }
            }

            playerUiTemplate.gameObject.SetActive(false);

            quitButton.onClick.AddListener(() => { SceneManager.LoadScene("HomeScreen"); });
        }

        private void Update()
        {
            foreach (var player in players) player.OnUpdate(this);
            players = players.OrderByDescending(player => player.Laps + player.ParametricPosition).ToArray();
            for (var index = 0; index < players.Length; index++)
            {
                if (!players[index].Enabled)
                {
                    continue;
                }

                players[index].Ui.UpdatePosition(index);
            }
        }

        [Serializable]
        public class PlayerData
        {
            public GameObject car;
            public SplineContainer spline;
            public KeyCode key;
            public float offset;

            [NonSerialized] public bool Enabled;
            [NonSerialized] public float MeterToSplineUnit;
            [NonSerialized] public float ParametricPosition;
            [NonSerialized] public int Laps;
            [NonSerialized] public float Speed;
            [NonSerialized] public PlayerUiGame Ui;

            private bool _finished;

            public void OnUpdate(RaceController raceController)
            {
                if (!Enabled) return;

                if (Input.GetKey(key) && !_finished)
                {
                    Speed += raceController.acceleration * Time.deltaTime;
                    Ui.UpdateButton(true);
                }
                else
                {
                    Speed -= raceController.drag * Time.deltaTime;
                    Ui.UpdateButton(false);
                }

                Speed = Mathf.Clamp(Speed, 0, raceController.maxSpeed);

                Ui.UpdateSpeed(Speed / raceController.maxSpeed);

                ParametricPosition += MeterToSplineUnit * Speed * Time.deltaTime;

                if (ParametricPosition > 1.0f)
                {
                    Laps += 1;
                    ParametricPosition %= 1.0f;
                    Ui.UpdateLaps(Laps, raceController.totalLaps);
                    if (Laps == raceController.totalLaps)
                    {
                        _finished = true;
                    }
                }

                car.transform.position = spline.EvaluatePosition(ParametricPosition);
                var splineTan = spline.EvaluateTangent(ParametricPosition);
                var tan = new Vector3(splineTan.x, splineTan.y, splineTan.z);
                car.transform.LookAt(car.transform.position + tan, Vector3.up);
                car.transform.position += car.transform.right * offset;
            }
        }
    }
}