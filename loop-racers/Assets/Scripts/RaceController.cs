using System;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Busta.LoopRacers
{
    public class RaceController : MonoBehaviour
    {
        [SerializeField] private GameConfigs gameConfigs;
        [SerializeField] private Rigidbody carRbPrefab;
        [SerializeField] private Button quitButton;
        [SerializeField] private Canvas loadingCanvas;
        [SerializeField] private TMP_Text midScreenText;


        [SerializeField] private float acceleration = 100f;
        [SerializeField] private float drag = 50f;
        [SerializeField] private float maxSpeed = 75f;
        [SerializeField] private float dangerSpeed = 70f;

        [SerializeField] private int totalLaps = 15;

        [SerializeField] private float startOffset = 0.685f;

        [SerializeField] private PlayerUiGame playerUiTemplate;
        [SerializeField] private PlayerData[] players;
        [SerializeField] private Vector2[] dangerZones;

        private bool _gameStarted = false;

        private void Start()
        {
            Debug.LogWarning("Will start!");
            for (var i = 0; i < players.Length; i++)
            {
                var player = players[i];
                var key = (KeyCode)StaticData.GetValue("player" + i, (int)KeyCode.None);
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

            quitButton.onClick.AddListener(() =>
            {
                loadingCanvas.gameObject.SetActive(true);
                SceneManager.LoadScene("HomeScreen");
            });

            loadingCanvas.gameObject.SetActive(false);

            Debug.LogWarning("Start!");
            // _gameStarted = true;
            // midScreenText.gameObject.SetActive(false);
            StartSequence();
        }

        private void StartSequence()
        {
            var sequence = DOTween.Sequence();

            sequence.AppendCallback(() =>
            {
                midScreenText.gameObject.SetActive(true);
                midScreenText.text = "";
            });

            sequence.AppendInterval(0.1f);

            sequence.AppendCallback(() =>
            {
                midScreenText.gameObject.SetActive(true);
                midScreenText.text = "3";
                midScreenText.transform.localScale = Vector3.one;
            });

            sequence.Append(midScreenText.transform.DOScale(1.2f, 1f));

            sequence.AppendCallback(() =>
            {
                midScreenText.text = "2";
                midScreenText.transform.localScale = Vector3.one;
            });

            sequence.Append(midScreenText.transform.DOScale(1.2f, 1f));

            sequence.AppendCallback(() =>
            {
                midScreenText.text = "1";
                midScreenText.transform.localScale = Vector3.one;
            });

            sequence.Append(midScreenText.transform.DOScale(1.2f, 1f));

            sequence.AppendCallback(() =>
            {
                midScreenText.text = "GO!";
                midScreenText.transform.localScale = Vector3.one;
                _gameStarted = true;
            });

            sequence.AppendInterval(0.2f);

            sequence.AppendCallback(() => { midScreenText.gameObject.SetActive(false); });
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

        private void OnDrawGizmosSelected()
        {
            foreach (var dangerZone in dangerZones)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(players[0].spline.EvaluatePosition(dangerZone.x), 0.5f);
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(players[0].spline.EvaluatePosition(dangerZone.y), 0.5f);
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
            private bool _detachedState;

            public void OnUpdate(RaceController raceController)
            {
                if (!Enabled || _detachedState) return;

                if (Input.GetKey(key) && !_finished && raceController._gameStarted)
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

                // if position in the spline is danger zone && speed is above danger speed
                if (Speed > raceController.dangerSpeed)
                {
                    foreach (var dangerZone in raceController.dangerZones)
                    {
                        if (ParametricPosition > dangerZone.x && ParametricPosition < dangerZone.y)
                        {
                            if (Random.Range(0f, 1f) < 0.1f) // 10% chance to detach
                            {
                                DetachedRoutine(raceController.carRbPrefab);
                                return;
                            }
                        }
                    }
                }

                if (ParametricPosition > 1.0f)
                {
                    Laps += 1;
                    ParametricPosition %= 1.0f;
                    Ui.UpdateLaps(Laps, raceController.totalLaps);
                    if (Laps == raceController.totalLaps)
                    {
                        _finished = true;
                        raceController.midScreenText.gameObject.SetActive(true);
                        raceController.midScreenText.text = "FINISHED!";
                        raceController._gameStarted = false;
                    }
                }

                car.transform.position = spline.EvaluatePosition(ParametricPosition);
                var splineTan = spline.EvaluateTangent(ParametricPosition);
                var tan = new Vector3(splineTan.x, splineTan.y, splineTan.z);
                car.transform.LookAt(car.transform.position + tan, Vector3.up);
                car.transform.position += car.transform.right * offset;
            }

            public void DetachedRoutine(Rigidbody rbPrefab)
            {
                _detachedState = true;
                Speed = 0;
                var rb = Instantiate(rbPrefab);
                rb.transform.position = car.transform.position;
                rb.transform.rotation = car.transform.rotation;
                car.transform.SetParent(rb.transform);
                rb.AddForce(rb.transform.TransformDirection(0, 5, 12), ForceMode.Impulse);
                rb.AddTorque(Vector3.up * 2, ForceMode.Impulse);

                DOVirtual.DelayedCall(1f, () =>
                {
                    car.transform.SetParent(null);
                    car.transform.position = spline.EvaluatePosition(ParametricPosition);
                    var splineTan = spline.EvaluateTangent(ParametricPosition);
                    var tan = new Vector3(splineTan.x, splineTan.y, splineTan.z);
                    car.transform.LookAt(car.transform.position + tan, Vector3.up);
                    car.transform.position += car.transform.right * offset;
                    Destroy(rb.gameObject);

                    _detachedState = false;
                });
            }
        }
    }
}